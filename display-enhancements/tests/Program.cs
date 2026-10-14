using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Quests;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Progression.Features;
using ToyBoxDisplayEnhancements;
using Preview = ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities;
using BP = ToyBox.Infrastructure.Blueprints.BPHelper;
using General = ToyBox.Infrastructure.GeneralSettings;
using Kingmaker.UnitLogic.Alignments;
using GameLocale = Kingmaker.Localization.Enums.Locale;
using GameLocalization = Kingmaker.Localization.LocalizationManager;

internal static class Program {
    private static int assertions;
    private static void Is(bool condition, string label) {
        assertions++;
        if (!condition) throw new Exception("FAIL: " + label);
    }
    private static void Main(string[] args) {
        try { Run(args); }
        catch (Exception ex) {
            Console.Error.WriteLine(ex.GetType().FullName + ": " + ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            if (ex.InnerException != null) Console.Error.WriteLine(ex.InnerException.GetType().FullName + ": " + ex.InnerException.Message);
            Environment.ExitCode = 1;
        }
    }
    private static void Run(string[] args) {
        Runtime.ToyBox = typeof(ToyBox.Main).Assembly;
        Runtime.Initialize();
        StoryAliases.Load(args[0]);
        ReadabilityTests();
        var objective = new BlueprintQuestObjective { Title = "返回着陆区域", Quest = new BlueprintQuest { Title = "终末黎明之光" } };
        Is(Semantic.Action(new GiveObjective { Objective = objective }) == "开启目标：返回着陆区域〔终末黎明之光〕", "give objective");
        foreach (var state in new[] { "Complete", "Fail" })
            Is(Semantic.Action(new SetObjectiveStatus { Objective = objective, Status = state }) != null, "supported action " + state);
        Is(Semantic.Action(new SetObjectiveStatus { Objective = objective, Status = "Start" }) == null, "unsupported action state");
        Is(Semantic.Action(new GiveObjective()) == null, "missing objective fallback");
        Is(Semantic.Action(new GiveObjective { Objective = new BlueprintQuestObjective { Title = "<null>" } }) == null, "invalid official title fallback");
        Is(Semantic.Action(new GameAction()) == null, "unknown action fallback");
        Is(Semantic.Action(new StartEtude { Evaluate = true }) == null, "dynamic evaluator fallback");
        Is(Semantic.Action(new StartEtude { Etude = new Ref { Value = new SimpleBlueprint { name = "StoryState" } }, StartImmediately = true }).Contains("立即启动"), "static etude");
        var condition = new ObjectiveStatus { QuestObjective = objective, State = "Completed", Not = true };
        var checker = new ConditionsChecker { Operation = "Or", Conditions = new Condition[] { condition, new Condition() } };
        var formatted = Semantic.Checker(checker);
        Is(formatted.StartsWith("满足以下任一组") && formatted.Contains("不是") && formatted.Contains("Unknown condition"), "OR/negation/unknown");
        Is(condition.Checks == 0, "formatting does not evaluate conditions");
        Is(Semantic.Condition(new QuestStatus { Quest = objective.Quest, State = "Failed" }).Contains("已失败"), "quest state");
        Is(Semantic.Condition(new ObjectiveStatus { QuestObjective = objective, State = "Unknown" }) == "Unknown condition", "unknown state retains caption");
        Is(Semantic.Action(new CompleteEtude { Etude = new Ref { Value = new SimpleBlueprint { name = "State" } } }).StartsWith("完成剧情状态"), "complete etude");
        var holder = new CheckConditionsHolder { ConditionsHolder = new Ref { Value = new Holder { Conditions = checker } } };
        Is(Semantic.Checker(new ConditionsChecker { Conditions = new Condition[] { holder } }).StartsWith("满足以下任一组"), "nested AND/OR");
        var cyclic = new ConditionsChecker();
        cyclic.Conditions = new Condition[] { new CheckConditionsHolder { ConditionsHolder = new Ref { Value = new Holder { Conditions = cyclic } } } };
        Is(Semantic.Checker(cyclic).Contains("循环条件引用"), "cycle fallback");
        Is(Semantic.Condition(new EtudeStatus { Etude = new SimpleBlueprint { name = "State" }, Started = true }).Contains("包括运行中或完成处理中"), "etude Started semantics");
        Is(Text.State("Postponed", false) == "已推迟", "objective postponed enum");
        Is(Text.State("Unknown", false) == null, "unknown condition state fallback");
        Is(Semantic.Checker(new ConditionsChecker()) == "", "empty checker preserves branch-selection behaviour");
        Is(Text.Layer(0, "内容").StartsWith("结果预览"), "depth zero");
        Is(Text.Layer(4, "内容").StartsWith("后续可能涉及"), "depth is not chronological step");
        Is(Text.Signed(-3) == "-3" && Text.Signed(3) == "+3", "signed changes");
        Is(Text.Shift("Faith", 3, false, null, key => "游戏官方名称").Contains("游戏官方名称"), "official conviction text takes priority");
        Is(Text.Shift("Hope", 3, false, null, key => key).Contains("自由主义") && Text.Shift("Corruption", -2, false, null, key => key).Contains("异端主义 -2"), "remaining conviction names");

        Aliases.Load(args[0]);
        ToyBoxDisplayEnhancements.Main.Enabled = true;
        var harmony = new Harmony("ToyBoxDisplayEnhancements.Tests");
        Is(ToyBoxDisplayEnhancements.Patches.Install(harmony) == 13, "all patches install on managed fixture");
        try {
            Is(Preview.FormatActionAsList(new GiveObjective { Objective = objective })[0].StartsWith("开启目标"), "action prefix installed");
            Is(Preview.FormatActionAsList(new GameAction())[0] == "Unknown action", "native action fallback installed");
            var branch = new Conditional {
                IfTrue = new ActionList { Actions = new GameAction[] { new GiveObjective { Objective = objective } } },
                IfFalse = new ActionList { Actions = new GameAction[] { new GameAction() } }
            };
            Is(Preview.FormatActionAsList(branch)[0].StartsWith("开启目标") && branch.ConditionsChecker.Checks == 1, "conditional branch selected/evaluated once by original fixture");
            branch.ConditionsChecker.TestResult = false;
            Is(Preview.FormatActionAsList(branch)[0] == "Unknown action" && branch.ConditionsChecker.Checks == 2, "opposite conditional branch preserved");
            Is(Preview.FormatConditions(Array.Empty<Condition>()) == "", "patched empty array");
            var heresy = new EtudeStatus { Etude = new SimpleBlueprint { AssetGuid = "572778222e3849848119cef86face0c7", name = "Heresy" }, Playing = true, Not = true };
            var readableAnswer = new Kingmaker.DialogSystem.Blueprints.BlueprintAnswer {
                ShowConditions = new ConditionsChecker { Conditions = new Condition[] { heresy } },
                SelectConditions = new ConditionsChecker { Conditions = new Condition[] { new HasEnoughMoneyForCustomCompanion() } },
                Actions = new GameAction[] { new CreateCustomCompanion() }
            };
            var conditionLines = Preview.FormatConditionsAsList(readableAnswer);
            Is(conditionLines[0] == "出现条件：剧情状态“异端”未在运行" && conditionLines[1].StartsWith("选择要求：利润因子"), "player-facing labels and single-condition sentence");
            ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
            var before = Preview.Traversals;
            var detailedPreview = Preview.GetAnswerResultText(readableAnswer);
            Is(detailedPreview.Contains("后续可能涉及：进入自定义队友创建流程") && detailedPreview.Contains("原始：Create custom companion") && detailedPreview.Contains("来源深度：4"), "technical mode keeps summary plus original caption/depth");
            Is(Preview.Traversals == before + 1 && heresy.Checks == 0, "technical mode adds no traversal or evaluation");
            before = branch.ConditionsChecker.Checks;
            Preview.FormatActionAsList(branch);
            Is(branch.ConditionsChecker.Checks == before + 1, "technical mode conditional still checked once");
            ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
            Is(Preview.FormatSoulmarkShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = 15 }, Preview.m_SoulMarkRequiredLocalizedText + "({0})").Contains("点数 ≥ 15"), "verified value requirement");
            Is(Preview.FormatSoulmarkShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = 15, CheckByValue = false }, Preview.m_SoulMarkRequiredLocalizedText + "({0})").StartsWith("信念要求("), "rank requirement native fallback");
            Is(Preview.FormatSoulmarkShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = 3 }, Preview.m_SoulMarkShiftLocalizedText + "({0})").Contains("+3"), "positive shift");
            Is(Preview.GetCueResultText(new Kingmaker.DialogSystem.Blueprints.BlueprintCue { SoulMarkShift = new SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = 3, Description = "说明" } }).Contains("教条主义 +3"), "cue summary");
            var answer = new Kingmaker.DialogSystem.Blueprints.BlueprintAnswer {
                Actions = new GameAction[] { new GiveObjective { Objective = objective } },
                Shift = new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = -3 }
            };
            var preview = Preview.GetAnswerResultText(answer);
            Is(preview.Contains("后续可能涉及") && !preview.Contains("来源深度") && !preview.Contains("后续结果可能取决于") && preview.Contains("教条主义 -3") , "preview transpiler");
            Is(Preview.Traversals == 2, "no extra preview traversal");
            var bp = new BlueprintFeature { AssetGuid = "175d1fd853b24f188a4078306ca066ad", name = "CorruptionSoulMark_Base" };
            Is(BP.GetTitle(bp).StartsWith("异端主义"), "Chinese alias");
            Is(BP.GetSearchKey(bp).Contains("异端主义") && BP.GetSearchKey(bp).Contains(bp.name.ToUpperInvariant()), "bilingual search");
            Is(BP.GetSearchKey(bp).Contains(bp.AssetGuid.ToUpperInvariant()), "GUID search");
            Is(BP.GetSortKey(bp).StartsWith("异端主义"), "alias sort");
            General.Settings.ToggleBPsShowDisplayAndInternalName = true; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetTitle(bp).Contains(bp.name), "show internal name");
            General.Settings.ToggleSearchDescriptions = true; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetSearchKey(bp).Contains("内部记录"), "description search");
            ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetDescription(bp).StartsWith("【汉化说明】"), "BPHelper description postfix");
            Is(bp.Description == null, "game description getter is unchanged");
            bp.OfficialDescription = "官方描述"; BP.ClearNameCaches();
            ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetDescription(bp) == "官方描述", "official description priority");
            bp.OfficialName = "官方名称";
            Is(Aliases.Title(bp, null) == null, "official name priority");
            bp.OfficialName = null; bp.OfficialDescription = null; BP.ClearNameCaches();
            General.Settings.UILanguage = "fr";
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            Is(Preview.FormatConditionsAsList(readableAnswer)[0] == "Native conditions", "unsupported language restores original condition presentation");
            Is(BP.GetTitle(bp) == bp.name, "unsupported language restores native title");
            Is(Preview.GetAnswerResultText(answer).Contains("[4:"), "unsupported language restores native preview");
            ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetDescription(bp) == null, "unsupported language restores native description");
            General.Settings.UILanguage = "zh-CN";
            ToyBoxDisplayEnhancements.Main.Enabled = false;
            Is(Preview.FormatActionAsList(new CreateCustomCompanion())[0] == "Create custom companion", "disabled restores native action");
            Is(Preview.FormatConditionsAsList(readableAnswer)[0] == "Native conditions", "disabled restores native condition presentation");
            Is(BP.GetTitle(bp) == bp.name, "disabled restores native title");
            ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(BP.GetDescription(bp) == null, "disabled restores native row description");
            Is(!BP.GetSearchKey(bp).Contains("内部记录") && !BP.GetSearchKey(bp).Contains("异端主义"), "disabled clears enriched cached search");
            Is(BP.GetSortKey(bp).StartsWith(bp.name), "disabled restores cached sort");
            ToyBoxDisplayEnhancements.Main.Enabled = true;
            Runtime.RefreshCaches();
            Is(BP.GetSearchKey(bp).Contains("内部记录"), "reenable restores description search");
            var cleared = BP.Clears;
            General.Settings.UILanguage = "fr";
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            Is(BP.Clears > cleared && ToyBox.Main.BrowserInvalidations == BP.Clears, "locale change clears names and browser caches");
            Is(!BP.GetSearchKey(bp).Contains("内部记录") && BP.GetDescription(bp) == null && BP.GetSortKey(bp).StartsWith(bp.name), "locale restores search sort description together");
            General.Settings.UILanguage = "zh-CN";
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            General.Settings.ToggleSearchDescriptions = false;
            ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
            Is(!BP.GetSearchKey(bp).Contains("内部记录") && BP.GetSearchKey(bp).Contains("异端主义"), "description search follows new setting");
            readableAnswer.HasShowCheck = true;
            Is(Preview.FormatConditionsAsList(readableAnswer)[0] == "出现条件：检定：劝说，难度 10", "show check uses game stat text");
            readableAnswer.ShowCheck.Type = Kingmaker.EntitySystem.Stats.Base.StatType.Unknown;
            Is(Preview.FormatConditionsAsList(readableAnswer)[0].Contains("Unknown"), "unknown check remains identifiable");
            Is(Text.Check("SkillAwareness", 12, true, Runtime.Localize).Contains("警觉，难度 12（隐藏检定）"), "localized hidden check format");
            var unknownShift = new SoulMarkShift { Direction = SoulMarkDirection.Unknown, Value = -2 };
            Is(Preview.FormatSoulmarkShift(unknownShift, Preview.m_SoulMarkShiftLocalizedText + "({0})").Contains("Unknown -2"), "unknown conviction remains identifiable");
            Is(Preview.FormatSoulmarkShift(unknownShift, "Other({0})").StartsWith("Other("), "unknown shift format untouched");
            foreach (var item in Aliases.Entries) {
                var feature = new BlueprintFeature { AssetGuid = item.Key, name = item.Value.InternalName };
                Is(BP.GetTitle(feature).Contains(item.Value.Name) && BP.GetDescription(feature).StartsWith("【汉化说明】"), "all eight aliases " + item.Key);
                feature.OfficialName = "官方名称"; feature.OfficialDescription = "官方描述"; BP.ClearNameCaches();
                Is(BP.GetTitle(feature) == "官方名称" && BP.GetDescription(feature) == "官方描述", "all eight official texts preferred " + item.Key);
            }
            var cueAction = new OpenVendorSelectingWindow();
            var cue = new Kingmaker.DialogSystem.Blueprints.BlueprintCue { OnShow = new ActionList { Actions = new GameAction[] { cueAction } }, SoulMarkShift = new SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = -3 } };
            Is(Preview.GetCueResultText(cue).Contains("打开商人") && !Preview.GetCueResultText(cue).Contains("-3") && cueAction.Executions == 0, "cue preserves upstream positive-only shift collection and never executes actions");
            Is(Preview.GetCueResultText(new Kingmaker.DialogSystem.Blueprints.BlueprintCue()) == "", "empty cue remains empty");
            BilingualTests();
        } finally { harmony.UnpatchAll(harmony.Id); }
        Console.WriteLine("PASS: " + assertions + " assertions; managed Harmony fixture, not a game UI test.");
    }
    private static void BilingualTests() {
        var chineseAliases = new Dictionary<string, Alias>(Aliases.Entries);
        var objective = new BlueprintQuestObjective {
            TitleEn = "Return to the landing zone", TitleZh = "返回着陆区域",
            Quest = new BlueprintQuest { TitleEn = "At the dawn of doom", TitleZh = "终末黎明之光" }
        };
        var official = new BlueprintFeature {
            AssetGuid = "175d1fd853b24f188a4078306ca066ad", name = "CorruptionSoulMark_Base",
            NameEn = "Official English name", NameZh = "官方中文名称",
            DescriptionEn = "Official English description", DescriptionZh = "官方中文描述"
        };
        var heresyBlueprint = new SimpleBlueprint { AssetGuid = "572778222e3849848119cef86face0c7", name = "Heresy" };
        var heresy = new EtudeStatus { Etude = heresyBlueprint, Playing = true, Not = true };
        var money = new HasEnoughMoneyForCustomCompanion();
        var vendor = new OpenVendorSelectingWindow();
        var answer = new Kingmaker.DialogSystem.Blueprints.BlueprintAnswer {
            HasShowCheck = true,
            ShowConditions = new ConditionsChecker { Conditions = new Condition[] { heresy } },
            SelectConditions = new ConditionsChecker { Conditions = new Condition[] { money } },
            Actions = new GameAction[] { vendor },
            Shift = new SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = -3 }
        };
        var cue = new Kingmaker.DialogSystem.Blueprints.BlueprintCue {
            name = "Cue_English", AssetGuid = "cue-english",
            TextEn = "<b>You are not the Lord Inquisitor's only servant.</b>", TextZh = "<b>你并不是大审判官唯一的侍仆。</b>"
        };
        foreach (var ui in new[] { "zh-CN", "en" }) {
            General.Settings.UILanguage = ui;
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            foreach (var locale in new[] { GameLocale.zhCN, GameLocale.enGB }) {
                BP.GetTitle(official); BP.GetDescription(official); BP.GetSearchKey(official); BP.GetSortKey(official);
                var cleared = BP.Clears;
                var changes = GameLocalization.Instance.Changes;
                var previousLocale = Runtime.GameCulture;
                GameLocalization.Instance.OnLocaleProviderValueChanged(locale);
                Is(GameLocalization.Instance.Changes == changes + 1, "language postfix calls original once");
                Is(Runtime.GameCulture == locale.ToString(), "current game pack read after change");
                Is(previousLocale == locale.ToString() || BP.Clears == cleared + 1, "game locale changes clear caches immediately");
                var gameChinese = locale == GameLocale.zhCN;
                var expectedName = gameChinese ? "官方中文名称" : "Official English name";
                var expectedDescription = gameChinese ? "官方中文描述" : "Official English description";
                Is(Runtime.Active, "supported mixed-language combination stays active");
                Is(BP.GetTitle(official) == expectedName && BP.GetDescription(official) == expectedDescription, "official feature text follows game locale");
                Is(BP.GetSearchKey(official).Contains(expectedName.ToUpperInvariant()) && BP.GetSortKey(official).StartsWith(expectedName), "game locale refreshes official search and sort");
                var text = Semantic.Action(new GiveObjective { Objective = objective });
                Is(text.StartsWith(ui == "en" ? "Start objective: " : "开启目标：") &&
                    text.Contains(gameChinese ? "返回着陆区域" : "Return to the landing zone") &&
                    text.Contains(gameChinese ? "终末黎明之光" : "At the dawn of doom"), "objective wrapper follows ToyBox; titles follow game");
                var seen = Semantic.Condition(new CueSeen { Cue = cue });
                Is(seen.StartsWith(ui == "en" ? "Dialogue shown previously: " : "此前已显示过对白") &&
                    seen.Contains(gameChinese ? "你并不是" : "You are not") && !seen.Contains("<b>"), "dialogue wrapper and official quote use independent languages");
                var lines = Preview.FormatConditionsAsList(answer);
                Is(lines[0] == (ui == "en" ? "Availability conditions: Check: " : "出现条件：检定：") +
                    (gameChinese ? "劝说" : "Persuasion") + (ui == "en" ? ", difficulty 10" : "，难度 10"), "check label follows ToyBox and stat name follows game");
                Is(lines[1] == (ui == "en" ? "Availability conditions: Story state \"Heresy\" is not running" : "出现条件：剧情状态“异端”未在运行"), "registered story alias follows ToyBox");
                var traversals = Preview.Traversals;
                var preview = Preview.GetAnswerResultText(answer);
                Is(preview.Contains(ui == "en" ? "Possible later results: Open vendor selection" : "后续可能涉及：打开商人选择界面") &&
                    preview.Contains((gameChinese ? "教条主义" : "Dogmatic") + " -3"), "preview layer and official conviction across language matrix");
                Is(Preview.Traversals == traversals + 1 && vendor.Executions == 0 && money.Checks == 0 && heresy.Checks == 0, "mixed languages add no traversal actions or checks");
                Is(UiText.Get("TechnicalSetting") == (ui == "en" ? "Show technical details" : "显示技术详情") &&
                    ToyBoxDisplayEnhancements.Main.StatusText == (ui == "en" ? "Not loaded yet" : "尚未加载"), "UMM labels and status resolve at render time");
            }
        }
        General.Settings.UILanguage = "en";
        ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        Is(Semantic.Action(new CreateCustomCompanion()) == "Begin custom companion creation", "English recruitment flow");
        Is(Semantic.Condition(money).StartsWith("Profit Factor meets"), "English recruitment requirement");
        money.Not = true;
        Is(Semantic.Condition(money).StartsWith("Profit Factor does not meet"), "English negative recruitment requirement");
        money.Not = false;
        Is(Semantic.Action(new SetObjectiveStatus { Objective = objective, Status = "Complete", StartObjectiveIfNone = true }).Contains("Complete objective:") &&
            Semantic.Action(new SetObjectiveStatus { Objective = objective, Status = "Fail" }).StartsWith("Fail objective:"), "English objective actions");
        Is(Semantic.Action(new StartEtude { Etude = new Ref { Value = heresyBlueprint }, StartImmediately = true }) == "Start story state: Heresy (start immediately)" &&
            Semantic.Action(new CompleteEtude { Etude = new Ref { Value = heresyBlueprint } }) == "Complete story state: Heresy", "English story actions");
        foreach (var state in new[] { "None", "Started", "Completed", "Updated", "Postponed", "Failed" })
            Is(Semantic.Condition(new QuestStatus { Quest = objective.Quest, State = state, Not = true }).Contains("is not \""), "English quest state " + state);
        Is(Text.State("Updated", false) == null && Text.State("Unknown", true) == null, "unsupported English states retain fallback");
        Is(Semantic.Condition(new ObjectiveStatus { QuestObjective = objective, State = "Completed" }).Contains("[At the dawn of doom] is \"completed\""), "English objective condition includes quest");
        Is(Semantic.Condition(new EtudeStatus { Etude = heresyBlueprint, Started = true }).Contains("including running or completion in progress"), "English Started includes transient states");
        Is(Semantic.Condition(new EtudeStatus { Etude = heresyBlueprint, Playing = true, Completed = true, Not = true }).Contains("is in none of these states: running / completed"), "English multiple-state negation");
        var group = new OrAndLogic { ConditionsChecker = new ConditionsChecker { Conditions = new Condition[] { heresy, money } } };
        var checker = new ConditionsChecker { Operation = "Or", Conditions = new Condition[] { group, new Condition { Not = true } } };
        var grouped = Semantic.Checker(checker);
        Is(grouped.StartsWith("At least one of the following must be true:\n") && grouped.Contains("1. All of the following must be true") &&
            grouped.Contains("     1.") && grouped.Contains("Not (Unknown condition)"), "English nested groups preserve indentation and unknown negation");
        group.Not = true;
        Is(Semantic.Condition(group).StartsWith("The following group must not be true:\n  All"), "English NOT AND");
        group.ConditionsChecker.Operation = "Or";
        Is(Semantic.Condition(group).StartsWith("The following group must not be true:\n  At least"), "English NOT OR");
        Is(Semantic.Condition(new OrAndLogic()).Contains("And[0] [Empty condition group") && Semantic.Checker(new ConditionsChecker()) == "", "English empty groups preserve native availability");
        var cycle = new OrAndLogic();
        cycle.ConditionsChecker.Conditions = new Condition[] { cycle };
        Is(Semantic.Condition(cycle).Contains("And[1] [Cyclic condition reference"), "English cycle fallback");
        var unknownGroup = new OrAndLogic { ConditionsChecker = new ConditionsChecker { Operation = "Unknown", Conditions = new Condition[] { money } } };
        Is(Semantic.Condition(unknownGroup).Contains("Unknown[1] [Unknown condition grouping"), "English unknown grouping fallback");
        Is(Semantic.Condition(new Condition { Not = true }) == "Not (Unknown condition)" && Semantic.Action(new GameAction()) == null, "English unknown types retain native captions");
        var seenCondition = new CueSeen { Cue = cue, CurrentDialog = true, Not = true };
        Is(Semantic.Condition(seenCondition).StartsWith("Dialogue not shown in this conversation:"), "English current dialogue negation");
        seenCondition.CurrentDialog = false;
        Is(Semantic.Condition(seenCondition).StartsWith("Dialogue not shown previously:"), "English history negation");
        var longQuote = string.Concat(System.Linq.Enumerable.Repeat("word ", 23)) + "unfinished";
        Is(Text.Excerpt(longQuote) == longQuote.Substring(0, 114) + "...", "English 120-character excerpt stops before partial word");
        Is(Text.Excerpt(new string('x', 120)) == new string('x', 120) && Text.Excerpt(new string('x', 121)) == new string('x', 120) + "...", "English excerpt length boundaries");
        cue.Text = "<null>";
        Is(Semantic.Condition(seenCondition).Contains("Cue_English"), "English missing dialogue uses internal identity");
        cue.name = null;
        Is(Semantic.Condition(seenCondition).Contains("cue-english"), "English missing dialogue and name uses GUID");
        seenCondition.Cue = null;
        Is(Semantic.Condition(seenCondition) == "Not Cue Seen ()", "English missing cue uses native negation");
        Is(Text.Layer(0, "test") == "Result preview: test" && Text.Layer(4, "test") == "Possible later results: test", "English depth labels");
        foreach (var direction in new[] { SoulMarkDirection.Faith, SoulMarkDirection.Hope, SoulMarkDirection.Corruption }) {
            var name = direction == SoulMarkDirection.Faith ? "Dogmatic" : direction == SoulMarkDirection.Hope ? "Iconoclast" : "Heretical";
            var shift = new SoulMarkShift { Direction = direction, Value = 15 };
            Is(Preview.FormatSoulmarkShift(shift, Preview.m_SoulMarkRequiredLocalizedText + "({0})") == "Conviction requirement: " + name + " points >= 15", "English conviction points " + name);
            shift.Value = -2;
            Is(Preview.FormatSoulmarkShift(shift, Preview.m_SoulMarkShiftLocalizedText + "({0})") == "Conviction change: " + name + " -2", "English signed shift " + name);
            shift.CheckByValue = false;
            Is(Preview.FormatSoulmarkShift(shift, Preview.m_SoulMarkRequiredLocalizedText + "({0})").StartsWith("SoulMarkRequired("), "English rank requirement native fallback");
        }
        Is(Text.Check("SkillAwareness", 12, true, Runtime.Localize) == "Check: Awareness, difficulty 12 (hidden check)" &&
            Text.Check("Unknown", 8, false, Runtime.Localize) == "Check: Unknown, difficulty 8", "English checks and unknown names");
        Is(Text.Shift("Unknown", -2, false, null, Runtime.Localize) == "Conviction change: Unknown -2", "English unknown conviction");
        Kingmaker.UI.Common.UIUtility.MissingText = true;
        Is(Text.Conviction("Faith", Runtime.Localize) == "Dogmatic" && Runtime.Localize("SkillAwareness") == "Awareness", "English missing official text fallback");
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.zhCN);
        Is(Text.Conviction("Hope", Runtime.Localize) == "自由主义" && Runtime.Localize("SkillPersuasion") == "劝说", "mixed-language fallback follows game");
        Kingmaker.UI.Common.UIUtility.MissingText = false;
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.frFR);
        Is(Text.Shift("Faith", 2, false, null, Runtime.Localize) == "Conviction change: Doctrine +2" &&
            Text.Check("SkillPersuasion", 10, false, Runtime.Localize).Contains("Persuasion (fr)"), "third game locale official text is retained");
        Kingmaker.UI.Common.UIUtility.MissingText = true;
        Is(Text.Conviction("Faith", Runtime.Localize) == "Faith" && Runtime.Localize("SkillPersuasion") == "SkillPersuasion", "unregistered locale missing text retains enum names");
        Kingmaker.UI.Common.UIUtility.MissingText = false;
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.enGB);
        General.Settings.ToggleSearchDescriptions = true;
        Runtime.RefreshCaches();
        foreach (var item in Aliases.Entries) {
            var feature = new BlueprintFeature { AssetGuid = item.Key, name = item.Value.InternalName };
            Is(BP.GetTitle(feature).Contains(item.Value.Name) && BP.GetDescription(feature) == "[Addon note] " + item.Value.Description, "English alias and note " + item.Key);
            var search = BP.GetSearchKey(feature);
            Is(search.Contains(item.Value.Name.ToUpperInvariant()) && search.Contains(chineseAliases[item.Key].Name.ToUpperInvariant()) &&
                search.Contains(feature.name.ToUpperInvariant()) && search.Contains(feature.AssetGuid.ToUpperInvariant()), "dual alias internal and GUID search " + item.Key);
            Is(search.Contains(item.Value.Description.ToUpperInvariant()) && !search.Contains(chineseAliases[item.Key].Description.ToUpperInvariant()), "only English addon description searched " + item.Key);
            Is(BP.GetSortKey(feature).StartsWith(item.Value.Name) && feature.Description == null, "English sorting without blueprint changes " + item.Key);
            feature.OfficialName = "Valid official name"; feature.OfficialDescription = "Valid official description"; BP.ClearNameCaches();
            Is(BP.GetTitle(feature) == feature.OfficialName && BP.GetDescription(feature) == feature.OfficialDescription, "English official text priority " + item.Key);
        }
        var aliasFeature = new BlueprintFeature { AssetGuid = official.AssetGuid, name = official.name };
        General.Settings.ToggleSearchDescriptions = false; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
        Is(!BP.GetSearchKey(aliasFeature).Contains("AN INTERNAL RECORD") && BP.GetSearchKey(aliasFeature).Contains("HERETICAL") &&
            BP.GetSearchKey(aliasFeature).Contains("异端主义"), "description switch keeps both alias names");
        General.Settings.ToggleBPsShowDisplayAndInternalName = false; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
        Is(!BP.GetTitle(aliasFeature).Contains(aliasFeature.name), "English internal name setting off");
        General.Settings.ToggleBPsShowDisplayAndInternalName = true; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
        Is(BP.GetTitle(aliasFeature).Contains(aliasFeature.name), "English internal name setting on");
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
        var detailed = Semantic.Condition(heresy);
        Is(detailed.StartsWith("Story state \"Heresy\" is not running") && detailed.Contains("name is a registered addon alias") &&
            detailed.Contains(heresyBlueprint.AssetGuid) && detailed.Contains("Original:"), "English registered alias provenance even when equal to internal name");
        Is(Semantic.Action(vendor).Contains("Original: Open vendor selecting window") && Text.Layer(4, "test").Contains("Source depth: 4"), "English technical action and depth");
        var traversalsBefore = Preview.Traversals;
        Preview.GetAnswerResultText(answer);
        Is(Preview.Traversals == traversalsBefore + 1 && heresy.Checks == 0 && vendor.Executions == 0, "English technical mode adds no behavior");
        Is(Preview.GetCueResultText(new Kingmaker.DialogSystem.Blueprints.BlueprintCue {
            OnShow = new ActionList { Actions = new GameAction[] { vendor } },
            SoulMarkShift = new SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = 3 }
        }).Contains("Conviction change: Dogmatic +3"), "English cue conviction format");
        var negativeCue = new Kingmaker.DialogSystem.Blueprints.BlueprintCue { SoulMarkShift = new SoulMarkShift { Direction = SoulMarkDirection.Faith, Value = -3 } };
        Is(Preview.GetCueResultText(negativeCue) == "", "English cue retains positive-only upstream collection");
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
        var conditional = new Conditional {
            IfTrue = new ActionList { Actions = new GameAction[] { vendor } },
            IfFalse = new ActionList { Actions = new GameAction[] { new CreateCustomCompanion() } }
        };
        Is(Preview.FormatActionAsList(conditional)[0] == "Open vendor selection" && conditional.ConditionsChecker.Checks == 1, "English true branch checked once");
        conditional.ConditionsChecker.TestResult = false;
        Is(Preview.FormatActionAsList(conditional)[0] == "Begin custom companion creation" && conditional.ConditionsChecker.Checks == 2, "English false branch checked once");
        foreach (var locale in new[] { "en-GB", "en-US", "en" }) {
            General.Settings.UILanguage = locale;
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            Is(Runtime.Active && Semantic.Action(vendor) == "Open vendor selection", "English regional code " + locale);
        }
        foreach (var locale in new[] { "fr", "zh-TW", "invalid_locale", "" }) {
            General.Settings.UILanguage = locale; ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            Is(!Runtime.Active && BP.GetTitle(aliasFeature) == aliasFeature.name && BP.GetDescription(aliasFeature) == null &&
                !BP.GetSearchKey(aliasFeature).Contains("HERETICAL") && BP.GetSortKey(aliasFeature).StartsWith(aliasFeature.name), "unsupported locale restores all BP displays " + locale);
            Is(Preview.FormatConditionsAsList(answer)[0] == "Native conditions" && Preview.FormatActionAsList(vendor)[0] == vendor.GetCaption(), "unsupported locale native preview " + locale);
        }
        General.Settings.UILanguage = "en"; ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        Is(BP.GetTitle(aliasFeature).StartsWith("Heretical") && BP.GetDescription(aliasFeature).StartsWith("[Addon note]"), "English reenables after unsupported language");
        ToyBoxDisplayEnhancements.Main.Enabled = false; Runtime.RefreshCaches();
        Is(BP.GetTitle(aliasFeature) == aliasFeature.name && BP.GetDescription(aliasFeature) == null &&
            !BP.GetSearchKey(aliasFeature).Contains("HERETICAL") && BP.GetSortKey(aliasFeature).StartsWith(aliasFeature.name), "English disabling restores all BP displays");
        Is(Preview.FormatConditionsAsList(answer)[0] == "Native conditions" && Preview.GetAnswerResultText(answer).Contains("[4:"), "English disabling restores upstream formatting");
        ToyBoxDisplayEnhancements.Main.Enabled = true; Runtime.RefreshCaches();
        Is(BP.GetTitle(aliasFeature).StartsWith("Heretical"), "English addon reenables");
        General.Settings.UILanguage = "zh-CN"; ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        Is(BP.GetTitle(aliasFeature).StartsWith("异端主义") && BP.GetDescription(aliasFeature).StartsWith("【汉化说明】") &&
            BP.GetSearchKey(aliasFeature).Contains("HERETICAL") && BP.GetSortKey(aliasFeature).StartsWith("异端主义"), "back to Chinese refreshes all displays with dual search");
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.zhCN);
    }
    private static void ReadabilityTests() {
        var heresy = new SimpleBlueprint { AssetGuid = "572778222e3849848119cef86face0c7", name = "Heresy" };
        var notPlaying = new EtudeStatus { Etude = heresy, Playing = true, Not = true };
        Is(Semantic.Condition(notPlaying) == "剧情状态“异端”未在运行", "not running is not never started");
        Is(!Semantic.Condition(notPlaying).Contains("未触发") && !Semantic.Condition(notPlaying).Contains("未启动"), "no invented story progression");
        Is(StoryAliases.Name(new SimpleBlueprint { AssetGuid = "other", name = "Heresy" }) == "Heresy", "same name different GUID does not translate");
        Is(StoryAliases.Name(new SimpleBlueprint { AssetGuid = heresy.AssetGuid, name = "Changed" }) == "Changed", "renamed GUID falls back");
        var money = new HasEnoughMoneyForCustomCompanion();
        Is(Semantic.Condition(money) == "利润因子达到招募自定义队友所需的门槛", "profit factor rule without cost or balance evaluation");
        var and = new OrAndLogic { ConditionsChecker = new ConditionsChecker { Conditions = new Condition[] { notPlaying, money } } };
        var or = new ConditionsChecker { Operation = "Or", Conditions = new Condition[] { and, new OrAndLogic { ConditionsChecker = new ConditionsChecker { Conditions = new Condition[] { money, notPlaying, new Condition() } } } } };
        var summary = Semantic.Checker(or);
        Is(summary.StartsWith("满足以下任一组：\n") && summary.Contains("1. 以下条件都成立") && summary.Contains("2. 以下条件都成立") && !summary.Contains("And["), "And[2]/And[3] expand from real structure");
        Is(summary.Contains("     1.") && summary.Contains("Unknown condition"), "nested indentation and unknown leaf retained");
        and.Not = true;
        Is(Semantic.Condition(and).StartsWith("以下组合不成立：\n  以下条件都成立") && !Semantic.Condition(and).Contains("全部不满足"), "NOT AND preserves whole group");
        and.ConditionsChecker.Operation = "Or";
        Is(Semantic.Condition(and).StartsWith("以下组合不成立：\n  满足以下任一组"), "NOT OR preserves whole group");
        Is(Semantic.Condition(new OrAndLogic()).Contains("And[0]〔空条件组"), "nested empty group remains identifiable");
        var nestedUnknown = new OrAndLogic { ConditionsChecker = new ConditionsChecker { Operation = "Unknown", Conditions = new Condition[] { money } } };
        Is(Semantic.Condition(nestedUnknown).Contains("Unknown[1]") && Semantic.Condition(nestedUnknown).Contains("未知条件组合"), "unknown group preserves native identity");
        var cycle = new OrAndLogic();
        cycle.ConditionsChecker.Conditions = new Condition[] { cycle };
        Is(Semantic.Condition(cycle).Contains("And[1]〔循环条件引用"), "OrAnd cycle keeps original caption");
        Is(Semantic.Condition(new Condition { Not = true }) == "Not (Unknown condition)", "unknown native negation not duplicated");
        var seen = new CueSeen { Cue = new Kingmaker.DialogSystem.Blueprints.BlueprintCue { name = "Cue_0019", AssetGuid = "cue-guid", Text = "<b>你并不是大审判官唯一的侍仆。</b>" } };
        Is(Semantic.Condition(seen) == "此前已显示过对白：“你并不是大审判官唯一的侍仆。”", "official dialogue text with markup removed");
        seen.CurrentDialog = true; seen.Not = true;
        Is(Semantic.Condition(seen).StartsWith("本次对话中未显示过对白"), "local cue scope and negation");
        seen.Cue.Text = "<b>" + new string('甲', 41) + "</b>";
        Is(Semantic.Condition(seen).Contains(new string('甲', 40) + "…") && !Semantic.Condition(seen).Contains(new string('甲', 41)), "40-character excerpt");
        seen.Cue.Text = "<null>";
        Is(Semantic.Condition(seen).Contains("Cue_0019"), "missing official text retains internal cue identity");
        seen.Cue = null;
        Is(Semantic.Condition(seen) == "Not Cue Seen ()", "missing cue native fallback");
        var vendor = new OpenVendorSelectingWindow(); var recruit = new CreateCustomCompanion();
        Is(Semantic.Action(vendor) == "打开商人选择界面" && Semantic.Action(recruit) == "进入自定义队友创建流程", "supported UI actions");
        Is(vendor.Executions == 0 && recruit.Executions == 0 && money.Checks == 0 && seen.Checks == 0 && notPlaying.Checks == 0 && and.Checks == 0 && and.ConditionsChecker.Checks == 0, "rendering never executes actions or checks");
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
        var detailed = Semantic.Condition(notPlaying);
        Is(detailed.Contains("Heresy") && detailed.Contains(heresy.AssetGuid) && detailed.Contains("本包别名") && detailed.StartsWith("剧情状态“异端”未在运行"), "technical alias provenance and identifiers");
        Is(Semantic.Action(vendor).Contains("原始：Open vendor selecting window"), "technical action caption");
        Is(Text.Layer(4, "内容").Contains("来源深度：4"), "depth only in technical mode");
        Is(Semantic.Checker(new ConditionsChecker()) == "", "technical mode empty root stays empty");
        ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
        Is(!Semantic.Condition(notPlaying).Contains("Heresy") && !Text.Layer(4, "内容").Contains("深度"), "compact mode hides known identifiers and depth");
        var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings-test");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "display-settings.json");
        if (File.Exists(path)) File.Delete(path);
        Is(!DisplaySettings.Load(directory).ShowTechnicalDetails, "missing settings defaults off");
        new DisplaySettings { ShowTechnicalDetails = true }.Save(directory);
        Is(DisplaySettings.Load(directory).ShowTechnicalDetails, "technical setting persists enabled");
        new DisplaySettings().Save(directory);
        Is(!DisplaySettings.Load(directory).ShowTechnicalDetails, "technical setting persists disabled");
        File.Delete(path);
    }
}
