using System;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Progression.Features;
using ToyBoxDisplayEnhancements;
using Eval = Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using BP = ToyBox.Infrastructure.Blueprints.BPHelper;
using General = ToyBox.Infrastructure.GeneralSettings;
using Preview = ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities;
using GameLocale = Kingmaker.Localization.Enums.Locale;
using GameLocalization = Kingmaker.Localization.LocalizationManager;

internal static partial class Program {
    private static void DialogueLanguages(string ui, GameLocale game) {
        General.Settings.UILanguage = ui;
        General.Settings.ToggleBPsShowDisplayAndInternalName = false;
        General.Settings.ToggleSearchDescriptions = false;
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
        GameLocalization.Instance.OnLocaleProviderValueChanged(game);
        ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
    }

    private static void DialogueDisplayTests() {
        var cue = new BlueprintCue {
            name = "Cue_001", AssetGuid = "cue-guid",
            TextZh = "<b>" + new string('\u7532', 160) + "</b>\nZhCueTail",
            TextEn = "<b>First official line</b> " + string.Concat(Enumerable.Repeat("word ", 40)) + "\nEnCueTail",
            Description = "Official cue note."
        };
        var answer = new BlueprintAnswer { name = "Answer_Second", AssetGuid = "second-answer-guid", Text = "Answer branch text" };
        var modifier = new DCModifier { Mod = 99 };
        var check = new BlueprintCheck { name = "Check_001", AssetGuid = "check-guid", DC = 18, Hidden = true, DCModifiers = new[] { modifier } };
        check.m_Success = Link(cue); check.m_Fail = Link(answer);
        var page = new BlueprintBookPage { name = "BookPage_001", AssetGuid = "page-guid" };
        page.Cues.AddRange(new[] { Link(page), null, Link(check), Link(cue) });
        page.Answers.Add(Link(answer));
        var exit = new BlueprintSequenceExit { name = "SequenceExit_001", AssetGuid = "exit-guid" };
        exit.Continue.Cues.AddRange(new[] { Link(exit), Link(check), Link(cue) });
        exit.Answers.Add(Link(answer));
        var sequence = new BlueprintCueSequence { name = "CueSequence_001", AssetGuid = "sequence-guid", m_Exit = Link(exit) };
        sequence.Cues.AddRange(new[] { Link(sequence), Link(page) });
        var dialog = new BlueprintDialog { name = "Scene_dialog", AssetGuid = "dialog-guid", Description = "Official dialog note." };
        dialog.FirstCue.Cues.AddRange(new[] { Link(check), Link(sequence) });
        var textBlueprints = new SimpleBlueprint[] { cue, page, exit, sequence, dialog };
        var passed = new CheckPassed { m_Check = Link(check) };
        var failed = new CheckFailed { m_Check = Link(check) };

        foreach (var ui in new[] { "zh-CN", "en" }) {
            foreach (var game in new[] { GameLocale.zhCN, GameLocale.enGB }) {
                DialogueLanguages(ui, game);
                var english = ui == "en";
                var marker = game == GameLocale.zhCN ? new string('\u7532', 8) : "First official line";
                var tail = game == GameLocale.zhCN ? "ZhCueTail" : "EnCueTail";
                foreach (var bp in textBlueprints) {
                    var title = BP.GetTitle(bp);
                    Is(title != bp.name && title.Contains(marker) && !title.Contains(tail) && !title.Contains("<b>"),
                        "localized representative title " + ui + "/" + game + "/" + bp.name);
                    Is(BP.GetSortKey(bp) == title && !BP.GetSortKey(bp).Contains(bp.name), "clean enhanced sort " + bp.name);
                    var search = BP.GetSearchKey(bp);
                    Is(search.Contains(marker.ToUpperInvariant()) && search.Contains(bp.name.ToUpperInvariant()) &&
                        search.Contains(bp.AssetGuid.ToUpperInvariant()) && !search.Contains(tail.ToUpperInvariant()),
                        "excerpt and native identifiers searchable without description search " + bp.name);
                    var description = BP.GetDescription(bp);
                    Is(description.Contains(tail) && !description.Contains("<b>") && !description.Contains(answer.Text),
                        "full first dialogue text only " + bp.name);
                    Is(BP.GetDescription(bp) == description, "descriptions do not grow on repeated reads " + bp.name);
                    Is(title.Length < (english ? 230 : 90), "bounded container excerpt " + bp.name);
                }
                Is(BP.GetTitle(cue).StartsWith(english ? "Dialogue line: " : "\u5bf9\u767d\uff1a"), "cue label follows ToyBox language");
                Is(BP.GetDescription(cue).StartsWith("Official cue note.\n") &&
                    BP.GetDescription(dialog).StartsWith("Official dialog note.\n"), "official descriptions retained");
                var stat = game == GameLocale.zhCN ? "\u8b66\u89c9" : "Awareness";
                var checkTitle = BP.GetTitle(check);
                Is(checkTitle.Contains(stat) && checkTitle.Contains("18") && !checkTitle.Contains("99") &&
                    checkTitle.Contains(english ? "hidden check" : "\u9690\u85cf\u68c0\u5b9a") &&
                    checkTitle.Contains(english ? "conditional modifiers" : "\u6761\u4ef6\u4fee\u6b63"),
                    "check names show base DC, hidden flag and unapplied conditional modifiers");
                Is(BP.GetDescription(check).Contains(checkTitle) && BP.GetSortKey(check) == checkTitle &&
                    BP.GetSearchKey(check).Contains(stat.ToUpperInvariant()), "check browser entry points enhanced");
                foreach (var condition in new CheckPassed[] { passed, failed }) {
                    condition.Not = false;
                    var positive = Preview.FormatConditions(new Condition[] { condition });
                    Is(positive.Contains(stat) && positive.Contains(english ? "in this conversation" : "\u672c\u6b21\u5bf9\u8bdd"),
                        "check records are local to this conversation");
                    condition.Not = true;
                    var negative = Preview.FormatConditions(new Condition[] { condition });
                    Is(negative.Contains(english ? "No record" : "\u6ca1\u6709") &&
                        negative.Contains(condition == passed ? (english ? "passing" : "\u6210\u529f") : (english ? "failing" : "\u5931\u8d25")),
                        "negated success and failure records remain distinct");
                }
                General.Settings.ToggleBPsShowDisplayAndInternalName = true;
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                foreach (var bp in textBlueprints.Concat(new[] { check })) {
                    Is(BP.GetTitle(bp).Contains(bp.name) && !BP.GetSortKey(bp).Contains(bp.name) &&
                        !BP.GetSortKey(bp).Contains("<color"), "internal names do not affect sorting " + bp.name);
                }
                General.Settings.ToggleSearchDescriptions = true;
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                foreach (var bp in textBlueprints)
                    Is(BP.GetSearchKey(bp).Contains(tail.ToUpperInvariant()), "full text search follows description setting " + bp.name);
                ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
                Is(Semantic.Condition(passed).Contains(check.name) && Semantic.Condition(passed).Contains(check.AssetGuid) &&
                    Semantic.Condition(passed).Contains("Check Passed"), "technical check details identify referenced check");
                Is(modifier.Conditions.Checks == 0 && cue.Conditions.Checks == 0 && page.Conditions.Checks == 0 &&
                    sequence.Conditions.Checks == 0 && dialog.Conditions.Checks == 0 && passed.Checks == 0 && failed.Checks == 0,
                    "all new text reads leave condition counts unchanged");
            }
        }

        DialogueLanguages("en", GameLocale.enGB);
        foreach (var difficulty in new[] { "Easy", "Normal", "Hard", "Impossible" }) {
            check.Difficulty = difficulty;
            BP.ClearNameCaches();
            Is(BP.GetTitle(check).Contains(difficulty.ToLowerInvariant()) && !BP.GetTitle(check).Contains("18") &&
                !BP.GetTitle(check).Contains("99"), "preset difficulty is not the fixed DC field " + difficulty);
        }
        check.Difficulty = "Unrecognized"; BP.ClearNameCaches();
        Is(BP.GetTitle(check) == check.name, "unknown check configuration retains native name");
        check.Difficulty = "Custom"; check.DCModifiers = Array.Empty<DCModifier>(); check.Hidden = false; BP.ClearNameCaches();
        Is(BP.GetTitle(check) == "Check: Awareness, base DC 18", "simple custom check config");
        var nestedAnswers = new BlueprintAnswersList { name = "List" };
        nestedAnswers.Answers.AddRange(new[] { Link(nestedAnswers), Link(answer) });
        page.Cues.Clear(); page.Answers.Clear(); page.Answers.Add(Link(nestedAnswers));
        Is(DialogueText.FullText(page) == answer.Text && BP.GetTitle(page).Contains("containing answer"),
            "book page falls back to first nested answer without inventing dialogue");
        sequence.Cues.Clear(); exit.Continue.Cues.Clear();
        Is(DialogueText.FullText(sequence) == answer.Text && DialogueText.FullText(exit) == answer.Text,
            "sequence falls back through exit to declared answers");
        var emptyCue = new BlueprintCue { name = "EmptyCue", AssetGuid = "empty-cue-guid", Text = "<b> </b>" };
        emptyCue.Continue.Cues.Add(Link(cue)); emptyCue.Answers.Add(Link(answer));
        Is(DialogueText.FullText(emptyCue) == null, "empty cue does not follow continuation or answers");
        var emptyDialog = new BlueprintDialog { name = "EmptyDialog" };
        emptyDialog.FirstCue.Cues.AddRange(new[] { Link(emptyDialog), Link(check), Link(emptyCue) });
        var emptyPage = new BlueprintBookPage { name = "EmptyPage" }; emptyPage.Cues.Add(Link(emptyPage));
        var emptySequence = new BlueprintCueSequence { name = "EmptySequence" }; emptySequence.Cues.Add(Link(emptySequence));
        var emptyExit = new BlueprintSequenceExit { name = "EmptyExit" }; emptyExit.Continue.Cues.Add(Link(emptyExit));
        foreach (var bp in new SimpleBlueprint[] { emptyCue, emptyDialog, emptyPage, emptySequence, emptyExit }) {
            Is(BP.GetTitle(bp) == bp.name && BP.GetDescription(bp) == null && BP.GetSortKey(bp) == bp.name &&
                BP.GetSearchKey(bp).Contains(bp.name.ToUpperInvariant()), "unreadable or cyclic container keeps original presentation " + bp.name);
        }
        var after = new BlueprintCue { name = "Second", Text = "Later cue must not replace first" };
        cue.Continue.Cues.Add(Link(after));
        var firstText = DialogueText.FullText(cue);
        Is(!firstText.Contains(after.Text), "read only the selected representative node's own text");
        DialogueSemanticTests(dialog, check, answer);

        foreach (var ui in new[] { "fr", "en" }) {
            General.Settings.UILanguage = ui;
            foreach (var active in new[] { false, true }) {
                ToyBoxDisplayEnhancements.Main.Enabled = active;
                ToyBox.Infrastructure.Localization.LocalizationManager.Update();
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                foreach (var bp in textBlueprints.Concat(new[] { check })) {
                    if (active && ui == "en") Is(BP.GetTitle(bp) != bp.name, "supported UI restores enhancement " + bp.name);
                    else Is(BP.GetTitle(bp) == bp.name && BP.GetDescription(bp) == null &&
                        BP.GetSortKey(bp) == bp.name && !BP.GetSearchKey(bp).Contains("ENCUETAIL"),
                        "disabled or unsupported UI restores all native entry points " + bp.name);
                }
                if (!active || ui == "fr")
                    Is(Preview.FormatConditions(new Condition[] { passed }) == passed.GetCaption(), "disabled check formatter restores native caption");
            }
        }
        DialogueLanguages("zh-CN", GameLocale.zhCN);
    }

    private static void DialogueSemanticTests(BlueprintDialog dialog, BlueprintCheck check, BlueprintAnswer answer) {
        var missing = new BlueprintAnswer { name = "Answer_Empty", AssetGuid = "missing-guid" };
        var mark = new MarkAnswersSelected { m_Answers = new[] { Link(answer), Link(missing) } };
        var unmark = new UnmarkAnswersSelected { m_Answers = mark.m_Answers };
        Is(Preview.FormatActionAsList(mark)[0] == "Mark answers selected: \"Answer branch text\" / \"Answer_Empty\"" &&
            Preview.FormatActionAsList(unmark)[0].StartsWith("Clear answer selection records: "), "record actions use ordered answer text and fallback");
        missing.name = null;
        Is(Semantic.Action(mark).Contains("missing-guid"), "missing answer text and name falls back to GUID in action");
        missing.name = "Answer_Empty";
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
        var detailed = Semantic.Action(mark);
        Is(detailed.Contains(answer.name) && detailed.Contains(answer.AssetGuid) && detailed.Contains(missing.name) &&
            detailed.Contains(missing.AssetGuid) && detailed.Contains("Mark Answers Selected"), "technical multi-answer action includes every identity");
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
        Is(Semantic.Action(new MarkAnswersSelected()) == null &&
            Semantic.Action(new MarkAnswersSelected { m_Answers = new Ref[] { null } }) == null, "empty or broken answer action falls back");
        var dynamicTarget = new Eval.DynamicEvaluator { Caption = "Current speaker expression" };
        var start = new StartDialog { m_Dialogue = Link(dialog), DialogEvaluator = dynamicTarget };
        Is(Preview.FormatActionAsList(start)[0].StartsWith("Start dialog: Dialog: containing answer"), "start dialog uses direct reference");
        start.m_Dialogue = null;
        Is(Semantic.Action(start) == "Start dialog: Current speaker expression", "dynamic dialog retains evaluator caption");
        start.DialogEvaluator = null; start.DialogueOwner = dynamicTarget;
        Is(Semantic.Action(start).EndsWith(dynamicTarget.Caption), "dynamic dialogue owner caption retained");
        var unit = new BlueprintUnit { name = "Companion_Internal", AssetGuid = "unit-guid", NameEn = "Abelard", NameZh = "\u963f\u8d1d\u62c9\u5fb7" };
        var companion = new CompanionInParty { m_companion = Link(unit), MatchWhenActive = true };
        var companionText = Semantic.Condition(companion);
        Is(companionText.Contains("Abelard") && !companionText.Contains(unit.name) &&
            companionText.Contains("excluding states: remote / detached / ex-companion") &&
            companionText.Contains("excluding dead characters"), "companion condition preserves exact excluded states");
        foreach (var member in new[] { "MatchWhenActive", "MatchWhenRemote", "MatchWhenDetached", "MatchWhenEx" }) {
            companion.GetType().GetField(member).SetValue(companion, true);
        }
        companion.IncludeDead = true;
        Is(Semantic.Condition(companion) == "Character roster contains \"Abelard\" (including dead characters)",
            "all states allowed does not imply active party membership");
        foreach (var pair in new[] { new[] { "Active", "in party" }, new[] { "Remote", "remote" },
            new[] { "Detached", "detached" }, new[] { "Ex", "ex-companion" } }) {
            var field = companion.GetType().GetField("MatchWhen" + pair[0]);
            field.SetValue(companion, false);
            Is(Semantic.Condition(companion) == "Character roster contains \"Abelard\", excluding states: " + pair[1] + " (including dead characters)",
                "independent companion state exclusion " + pair[0]);
            field.SetValue(companion, true);
        }
        companion.Not = true;
        Is(Semantic.Condition(companion).StartsWith("Not satisfied (Character roster contains"), "companion negation applies to complete filter");
        unit.NameEn = "-unit name not set-";
        Is(Semantic.Condition(companion).Contains(unit.name), "invalid unit display name falls back to identifier");
        unit.NameEn = "Abelard";
        var item = new BlueprintItem { name = "Item_Internal", AssetGuid = "item-guid", OfficialName = "<b>Medikit</b>" };
        var items = new ItemsEnough { m_ItemToCheck = Link(item), Quantity = 3 };
        Is(Semantic.Condition(items) == "At least 3 of item \"Medikit\"", "item official name stripped and quantity preserved");
        items.Not = true;
        Is(Semantic.Condition(items) == "Fewer than 3 of item \"Medikit\"", "item quantity negation");
        items.Money = true;
        Is(Semantic.Condition(items) == "Money < 3", "money setting overrides item reference");
        items.Money = false; items.Not = false;
        item.ComponentsArray = new object[] { new Kingmaker.Blueprints.Items.Components.MoneyReplacement() };
        Is(Semantic.Condition(items) == "Money >= 3", "money replacement component follows actual money branch");
        var fact = new BlueprintFeature { name = "Fact_Internal", AssetGuid = "fact-guid", OfficialName = "<i>Officer</i>" };
        var player = new Eval.PlayerCharacter();
        var knownCompanion = new Eval.CompanionInParty { m_Companion = Link(unit), Caption = "Companion (Companion_Internal) #2", Index = 2 };
        var hasFact = new HasFact { Unit = player, m_Fact = Link(fact) };
        var hasBuff = new HasBuff { Target = dynamicTarget, m_Buff = Link(fact) };
        Is(Semantic.Condition(hasFact) == "Player character has ability or fact \"Officer\"", "known player target identity");
        hasFact.Unit = knownCompanion;
        Is(Semantic.Condition(hasFact).Contains("Abelard") && Semantic.Condition(hasFact).Contains("#2"), "static companion identity retains evaluator qualifiers");
        hasFact.Not = true;
        Is(Semantic.Condition(hasFact).Contains("does not have ability or fact"), "fact negation");
        Is(Semantic.Condition(hasBuff) == "Current speaker expression has effect \"Officer\"", "unknown target is not assumed to be player");
        hasBuff.Not = true;
        Is(Semantic.Condition(hasBuff).Contains("does not have effect"), "buff negation");
        var flag = new BlueprintUnlockableFlag { name = "StoryFlag_Internal", AssetGuid = "flag-guid" };
        var unlocked = new FlagUnlocked { m_ConditionFlag = Link(flag) };
        Is(Semantic.Condition(unlocked) == "Story flag \"StoryFlag_Internal\" is unlocked", "flag name retained without inventing translation");
        unlocked.SpecifiedValues.AddRange(new[] { 1, 3, -2 });
        Is(Semantic.Condition(unlocked).EndsWith("with value in {1, 3, -2}"), "flag specified values preserve order");
        unlocked.ExceptSpecifiedValues = true;
        Is(Semantic.Condition(unlocked).Contains("is unlocked, with value outside {1, 3, -2}"), "flag excluded values still require unlock");
        unlocked.Not = true;
        Is(Semantic.Condition(unlocked).StartsWith("Not satisfied (Story flag") &&
            Semantic.Condition(unlocked).Contains("is unlocked, with value outside"), "negation retains unlocked prerequisite and whole predicate");
        var range = new FlagInRange { m_Flag = Link(flag), MinValue = -4, MaxValue = 7, Not = true };
        Is(Semantic.Condition(range) == "Not satisfied (Story flag \"StoryFlag_Internal\" is unlocked, with value in inclusive range [-4, 7])",
            "negated inclusive range includes locked state");
        var unlock = new UnlockFlag { m_flag = Link(flag), flagValue = 8 };
        var locked = new LockFlag { m_Flag = Link(flag) };
        var constant = new Eval.IntConstant { Value = -3 };
        var increment = new IncrementFlagValue { m_Flag = Link(flag), Value = constant };
        Is(Semantic.Action(unlock) == "Unlock story flag \"StoryFlag_Internal\" and set its value to 8" &&
            Semantic.Action(locked) == "Lock story flag \"StoryFlag_Internal\"", "unlock set-value and lock actions");
        Is(Semantic.Action(increment) == "Adjust story flag \"StoryFlag_Internal\" by -3 (only if unlocked)", "constant increment only for unlocked flag");
        increment.UnlockIfNot = true; constant.Value = 4;
        Is(Semantic.Action(increment).EndsWith("by +4 (unlock first if locked)"), "positive increment and unlock-first option");
        increment.Value = dynamicTarget;
        Is(Semantic.Action(increment).Contains("by expression: Current speaker expression") &&
            !Semantic.Action(increment).Contains("+4"), "dynamic increment uses expression rather than stale value");
        var conditions = new Condition[] { companion, items, hasFact, hasBuff, unlocked, range, new CheckPassed { m_Check = Link(check) }, new CheckFailed { m_Check = Link(check) } };
        var or = new OrAndLogic { Not = true, ConditionsChecker = new ConditionsChecker { Operation = "Or", Conditions = conditions } };
        var and = new ConditionsChecker { Conditions = new Condition[] { or, conditions[6] } };
        var grouped = Semantic.Checker(and);
        Is(grouped.StartsWith("All of the following must be true:") && grouped.Contains("The following group must not be true:") &&
            grouped.Contains("At least one of the following must be true:") && grouped.Contains("inclusive range [-4, 7]"),
            "new leaf predicates retain nested AND OR and whole-group negation");
        foreach (var condition in conditions) {
            ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
            Is(Semantic.Condition(condition).Contains("GUID:"), "new condition technical identity " + condition.GetType().Name);
            ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
            Is(condition.Checks == 0, "new condition never evaluated " + condition.GetType().Name);
        }
        var previewAnswer = new BlueprintAnswer { Actions = new GameAction[] { mark, unmark, start, unlock, locked, increment } };
        var traversals = Preview.Traversals;
        var preview = Preview.GetAnswerResultText(previewAnswer);
        Is(Preview.Traversals == traversals + 1 && preview.Contains("Mark answers selected") &&
            preview.Contains("by expression"), "new actions integrate with a single upstream preview traversal");
        Is(previewAnswer.Actions.All(a => a.Executions == 0) && player.Evaluations == 0 &&
            dynamicTarget.Evaluations == 0 && constant.Evaluations == 0 && knownCompanion.Evaluations == 0 &&
            and.Checks == 0 && or.ConditionsChecker.Checks == 0, "display never executes actions or evaluates dynamic targets, values or conditions");
        foreach (var condition in new Condition[] { new CheckPassed(), new CheckFailed(), new CompanionInParty(), new ItemsEnough(),
            new HasFact { Unit = player }, new HasBuff { Target = dynamicTarget }, new FlagUnlocked(), new FlagInRange() }) {
            condition.Not = true;
            Is(Semantic.Condition(condition) == condition.GetCaption(), "missing static reference retains already negated caption " + condition.GetType().Name);
        }
        foreach (var action in new GameAction[] { new StartDialog(), new UnlockFlag(), new LockFlag(), new IncrementFlagValue { m_Flag = Link(flag) } })
            Is(Semantic.Action(action) == null && Preview.FormatActionAsList(action)[0] == action.GetCaption(), "missing action configuration retains upstream caption " + action.GetType().Name);

        foreach (var ui in new[] { "zh-CN", "en" }) {
            foreach (var game in new[] { GameLocale.zhCN, GameLocale.enGB }) {
                DialogueLanguages(ui, game);
                var name = game == GameLocale.zhCN ? "\u963f\u8d1d\u62c9\u5fb7" : "Abelard";
                Is(Preview.FormatConditions(new Condition[] { companion, hasFact }).Contains(name), "character names follow game language " + ui + "/" + game);
                foreach (var action in previewAnswer.Actions)
                    Is(Preview.FormatActionAsList(action)[0] == Semantic.Action(action), "all action types use installed bilingual patch " + ui + "/" + game + "/" + action.GetType().Name);
                foreach (var condition in conditions)
                    Is(Preview.FormatConditions(new[] { condition }) == Semantic.Condition(condition), "all condition types use installed bilingual patch " + ui + "/" + game + "/" + condition.GetType().Name);
            }
        }
        DialogueLanguages("en", GameLocale.enGB);
    }
}
