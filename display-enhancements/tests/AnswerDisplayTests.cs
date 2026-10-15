using System;
using System.Linq;
using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using ToyBoxDisplayEnhancements;
using BP = ToyBox.Infrastructure.Blueprints.BPHelper;
using General = ToyBox.Infrastructure.GeneralSettings;
using Preview = ToyBox.Features.BagOfTricks.Preview.DialogPreviewUtilities;
using GameLocale = Kingmaker.Localization.Enums.Locale;
using GameLocalization = Kingmaker.Localization.LocalizationManager;

internal static partial class Program {
    private static Ref Link(object value) => new Ref { Value = value };

    private static void AnswerDisplayTests() {
        var answer = new BlueprintAnswer {
            name = "Answer_00101", AssetGuid = "answer-00101-guid",
            TextZh = "<b>" + new string('\u7532', 145) + "</b>\nZhTail",
            TextEn = "<b>Return to the ship</b>\n" + string.Concat(Enumerable.Repeat("word ", 28)) + "EnTail"
        };
        var missing = new BlueprintAnswer { name = "Answer_Empty", AssetGuid = "empty-guid", Text = "<null>" };
        var root = new BlueprintAnswersList { name = "AnswersList_001", AssetGuid = "list-001-guid" };
        var nested = new BlueprintAnswersList { name = "NestedAnswers" };
        root.Answers.AddRange(new[] { Link(root), null, Link(missing), Link(nested) });
        nested.Answers.AddRange(new[] { Link(root), Link(new SimpleBlueprint { name = "Unrelated" }), Link(answer),
            Link(new BlueprintAnswer { name = "Answer_Second", Text = "Second answer must not identify the list." }) });
        var selected = new AnswerSelected { m_Answer = Link(answer) };
        var shown = new AnswerListShown { m_AnswersList = Link(root) };
        var previewAnswer = new BlueprintAnswer {
            ShowConditions = new ConditionsChecker { Conditions = new Condition[] { selected } },
            SelectConditions = new ConditionsChecker { Conditions = new Condition[] { shown } }
        };

        foreach (var ui in new[] { "zh-CN", "en" }) {
            General.Settings.UILanguage = ui;
            ToyBox.Infrastructure.Localization.LocalizationManager.Update();
            foreach (var locale in new[] { GameLocale.zhCN, GameLocale.enGB }) {
                General.Settings.ToggleBPsShowDisplayAndInternalName = false;
                General.Settings.ToggleSearchDescriptions = false;
                ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
                GameLocalization.Instance.OnLocaleProviderValueChanged(locale);
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                var english = ui == "en";
                var gameChinese = locale == GameLocale.zhCN;
                var marker = gameChinese ? new string('\u7532', 10) : "Return to the ship";
                var tail = gameChinese ? "ZhTail" : "EnTail";
                foreach (var current in new[] { false, true }) {
                    foreach (var not in new[] { false, true }) {
                        selected.CurrentDialog = shown.CurrentDialog = current;
                        selected.Not = shown.Not = not;
                        var selectedText = Semantic.Condition(selected);
                        var shownText = Semantic.Condition(shown);
                        var scope = english ? (current ? "in this conversation" : "previously")
                            : (current ? "\u672c\u6b21\u5bf9\u8bdd\u4e2d" : "\u6b64\u524d");
                        var selection = english ? (not ? "Answer not selected" : "Answer selected")
                            : (not ? "\u672a\u9009\u62e9\u8fc7\u56de\u7b54" : "\u5df2\u9009\u62e9\u8fc7\u56de\u7b54");
                        var list = english ? (not ? "Answer list not shown" : "Answer list shown")
                            : (not ? "\u672a\u663e\u793a\u8fc7\u9009\u9879\u7ec4" : "\u5df2\u663e\u793a\u8fc7\u9009\u9879\u7ec4");
                        Is(selectedText.Contains(scope) && selectedText.Contains(selection) && selectedText.Contains(marker),
                            "selected answer scope, negation and game text " + ui + "/" + locale + "/" + current + "/" + not);
                        Is(shownText.Contains(scope) && shownText.Contains(list) && shownText.Contains(marker) &&
                            !shownText.Contains(english ? "selected" : "\u9009\u62e9"),
                            "list shown does not imply an answer was selected " + ui + "/" + locale + "/" + current + "/" + not);
                        var lines = Preview.FormatConditionsAsList(previewAnswer);
                        Is(lines.Count == 2 && lines[0].EndsWith(selectedText) && lines[1].EndsWith(shownText),
                            "answer conditions use installed show/select formatter");
                    }
                }
                var title = BP.GetTitle(answer);
                var groupTitle = BP.GetTitle(root);
                Is(title.StartsWith(english ? "Answer: \"" : "\u56de\u7b54\uff1a\u201c") &&
                    title.Contains(marker) && !title.Contains(answer.name) && !title.Contains(tail) && !title.Contains("<b>"),
                    "browser answer title is a clean localized excerpt " + ui + "/" + locale + ": " + title);
                Is(groupTitle.StartsWith(english ? "Answer list containing" : "\u9009\u9879\u7ec4\uff1a\u5305\u542b\u56de\u7b54") &&
                    groupTitle.Contains(marker) && !groupTitle.Contains(root.name) && !groupTitle.Contains("Second answer"),
                    "browser list uses first readable nested answer in declared order");
                Is(BP.GetTitle(answer, s => "formatted(" + s + ")") == "formatted(" + title + ")",
                    "browser title formatter applied once");
                Is(!BP.GetSearchKey(answer).Contains(tail.ToUpperInvariant()) &&
                    BP.GetSearchKey(answer).Contains(marker.ToUpperInvariant()) &&
                    BP.GetSearchKey(answer).Contains(answer.name.ToUpperInvariant()) &&
                    BP.GetSearchKey(answer).Contains(answer.AssetGuid.ToUpperInvariant()),
                    "compact search retains excerpt, internal name and GUID without full-text search");
                Is(BP.GetSearchKey(root).Contains(marker.ToUpperInvariant()) &&
                    BP.GetSearchKey(root).Contains(root.name.ToUpperInvariant()) &&
                    BP.GetSearchKey(root).Contains(root.AssetGuid.ToUpperInvariant()),
                    "list search retains identity and includes representative answer");
                Is(BP.GetDescription(answer).Contains(tail) && !BP.GetDescription(answer).Contains("<b>") &&
                    BP.GetDescription(root).Contains(tail) && !BP.GetDescription(root).Contains("Second answer"),
                    "descriptions expose full representative text without enumerating all branches");
                Is(BP.GetSortKey(answer) == title && BP.GetSortKey(root) == groupTitle,
                    "browser sort uses displayed plain names");
                var cleared = BP.Clears;
                General.Settings.ToggleBPsShowDisplayAndInternalName = true;
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                Is(BP.Clears == cleared + 1 && BP.GetTitle(answer).Contains(answer.name) && BP.GetTitle(root).Contains(root.name),
                    "internal-name setting refreshes both titles");
                Is(BP.GetSortKey(answer) == title && BP.GetSortKey(root) == groupTitle,
                    "internal-name decoration does not affect answer sorting");
                General.Settings.ToggleSearchDescriptions = true;
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                Is(BP.GetSearchKey(answer).Contains(tail.ToUpperInvariant()) && BP.GetSearchKey(root).Contains(tail.ToUpperInvariant()),
                    "description setting enables searching beyond excerpt in answer and list");
                ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
                var detailed = Semantic.Condition(selected);
                var groupDetailed = Semantic.Condition(shown);
                Is(detailed.Contains("Answer Selected") && detailed.Contains(answer.name) && detailed.Contains(answer.AssetGuid),
                    "answer technical details retain original caption and identity");
                Is(groupDetailed.Contains("Answer List Shown") && groupDetailed.Contains(root.name) && groupDetailed.Contains(root.AssetGuid),
                    "list technical details identify the list rather than its representative child");
                ToyBoxDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
                Is(!Semantic.Condition(selected).Contains(answer.name) && !Semantic.Condition(shown).Contains(root.name),
                    "compact conditions hide known identities independently of browser internal-name setting");
                Is(selected.Checks == 0 && shown.Checks == 0 && root.Conditions.Checks == 0 && nested.Conditions.Checks == 0 &&
                    previewAnswer.ShowConditions.Checks == 0 && previewAnswer.SelectConditions.Checks == 0 &&
                    answer.ShowConditions.Checks == 0 && answer.SelectConditions.Checks == 0,
                    "formatting and browser reads never evaluate answer or list availability");
            }
        }

        General.Settings.UILanguage = "en";
        ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.enGB);
        General.Settings.ToggleBPsShowDisplayAndInternalName = false;
        General.Settings.ToggleSearchDescriptions = false;
        ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
        selected.CurrentDialog = shown.CurrentDialog = false;
        selected.Not = false; shown.Not = true;
        var or = new OrAndLogic { ConditionsChecker = new ConditionsChecker { Operation = "Or", Conditions = new Condition[] { selected, shown } } };
        var and = new ConditionsChecker { Conditions = new Condition[] { or, selected } };
        var grouped = Semantic.Checker(and);
        Is(grouped.StartsWith("All of the following must be true:") && grouped.Contains("At least one of the following must be true:") &&
            grouped.Contains("Answer list not shown previously") && !grouped.Contains("Or["),
            "answer conditions preserve nested AND/OR and leaf negation");
        or.Not = true;
        Is(Semantic.Condition(or).StartsWith("The following group must not be true:"),
            "answer group negation remains whole-group negation");
        answer.Description = "Official author note.";
        BP.ClearNameCaches();
        var description = BP.GetDescription(answer);
        Is(description.StartsWith("Official author note.\nAnswer text: ") && description.Contains("EnTail") &&
            BP.GetDescription(answer) == description, "official answer description preserved without repeated additions");
        General.Settings.ToggleSearchDescriptions = true; ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
        Is(BP.GetSearchKey(answer).Contains("OFFICIAL AUTHOR NOTE."), "official answer description participates in description search");
        answer.Description = DialogueText.FullText(answer); BP.ClearNameCaches();
        Is(BP.GetDescription(answer) == answer.Description, "identical official description not duplicated");
        answer.Description = null;
        foreach (var absent in new[] { "", " ", "<null>", "[unknown key: missing]", "<b> </b>" }) {
            missing.Text = absent; BP.ClearNameCaches();
            Is(BP.GetTitle(missing) == missing.name && BP.GetDescription(missing) == null &&
                BP.GetSortKey(missing) == missing.name, "missing answer text retains native browser presentation: " + absent);
        }
        var emptyList = new BlueprintAnswersList { name = "EmptyList", AssetGuid = "empty-list-guid" };
        emptyList.Answers.AddRange(new[] { Link(emptyList), Link(missing) });
        Is(BP.GetTitle(emptyList) == emptyList.name && BP.GetDescription(emptyList) == null,
            "list with no readable answer retains native browser identity");
        selected.m_Answer = Link(missing); selected.Not = true;
        Is(Semantic.Condition(selected) == "Answer not selected previously: \"Answer_Empty\"",
            "missing official answer text falls back to internal identity with one negation");
        missing.name = null;
        Is(Semantic.Condition(selected).Contains("empty-guid"), "answer identity falls back to GUID");
        selected.m_Answer = null;
        Is(Semantic.Condition(selected) == "Not Answer Selected ()", "missing answer reference preserves native caption");
        shown.m_AnswersList = Link(emptyList);
        Is(Semantic.Condition(shown) == "Answer list not shown previously: EmptyList", "empty list identity is not presented as a quoted answer");
        emptyList.name = null;
        Is(Semantic.Condition(shown).Contains(emptyList.AssetGuid), "empty list identity falls back to GUID");
        shown.m_AnswersList = null;
        Is(Semantic.Condition(shown) == "Not Answer List Shown ()", "missing list reference preserves native caption");
        selected.m_Answer = Link(answer); shown.m_AnswersList = Link(root);
        Is(Text.Excerpt(DialogueText.FullText(answer)).Length <= 123 &&
            !Text.Excerpt(DialogueText.FullText(answer)).Contains("\n"), "English answer excerpt is bounded and single-line");
        General.Settings.UILanguage = "zh-CN"; ToyBox.Infrastructure.Localization.LocalizationManager.Update();
        GameLocalization.Instance.OnLocaleProviderValueChanged(GameLocale.zhCN);
        Is(Text.Excerpt(DialogueText.FullText(answer)) == new string('\u7532', 40) + "\u2026", "Chinese answer excerpt retains 40-character limit");

        foreach (var active in new[] { false, true }) {
            ToyBoxDisplayEnhancements.Main.Enabled = active;
            foreach (var locale in new[] { "fr", "en" }) {
                General.Settings.UILanguage = locale;
                ToyBox.Infrastructure.Localization.LocalizationManager.Update();
                ToyBox.Main.OnGUI(ToyBox.Main.ModEntry);
                if (active && locale == "en") {
                    Is(BP.GetTitle(answer).StartsWith("Answer: ") && BP.GetDescription(answer).Contains("ZhTail") &&
                        BP.GetSearchKey(answer).Contains("ZHTAIL") && BP.GetSortKey(root).StartsWith("Answer list containing"),
                        "reenabling supported UI restores all dynamic answer displays");
                } else {
                    Is(BP.GetTitle(answer) == answer.name && BP.GetTitle(root) == root.name &&
                        BP.GetDescription(answer) == null && BP.GetDescription(root) == null &&
                        !BP.GetSearchKey(answer).Contains("ZHTAIL") && BP.GetSortKey(root) == root.name,
                        "disabled or unsupported UI restores title, description, search and sort");
                    Is(Preview.FormatConditionsAsList(previewAnswer)[0] == "Native conditions" &&
                        Preview.FormatConditions(new Condition[] { selected }) == selected.GetCaption(),
                        "disabled or unsupported UI restores condition formatters");
                }
            }
        }
        Is(selected.Checks == 0 && shown.Checks == 0 && root.Conditions.Checks == 0 && nested.Conditions.Checks == 0,
            "all answer display paths leave condition evaluation counts unchanged");
        General.Settings.UILanguage = "zh-CN";
        General.Settings.ToggleSearchDescriptions = false;
        ToyBox.Infrastructure.Localization.LocalizationManager.Update();
    }
}
