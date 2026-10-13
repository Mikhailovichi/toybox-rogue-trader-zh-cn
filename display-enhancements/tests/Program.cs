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
using ToyBoxChineseDisplayEnhancements;

internal static class Program {
    private static int assertions;
    private static void Is(bool condition, string label) {
        assertions++;
        if (!condition) throw new Exception("FAIL: " + label);
    }
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void DrawRow(BlueprintFeature bp) => typeof(ToyBox.FactsEditor).GetMethod("BlueprintRowGUI").MakeGenericMethod(typeof(Kingmaker.UnitLogic.Feature), typeof(BlueprintFeature)).Invoke(null, new object[] { null, null, bp, null, null });
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
        Is(Text.Shift("Faith", 3, false, null, key => "信仰").Contains("教条主义"), "official conviction name independent of old dictionary wording");
        Is(Text.Shift("Hope", 3, false, null, key => key).Contains("自由主义") && Text.Shift("Corruption", -2, false, null, key => key).Contains("异端主义 -2"), "remaining conviction names");

        Runtime.ToyBox = typeof(ToyBox.Main).Assembly;
        Runtime.ToyBoxMain = typeof(ToyBox.Main);
        Runtime.Mod = typeof(ModKit.Mod);
        Runtime.LocalizeMethod = typeof(ModKit.LocalizationManager).GetMethod("localize");
        Aliases.Load(args[0]);
        ToyBoxChineseDisplayEnhancements.Main.Enabled = true;
        var harmony = new Harmony("ToyBoxChineseDisplayEnhancements.Tests");
        Is(ToyBoxChineseDisplayEnhancements.Patches.Install(harmony) == 11, "all patches install on managed fixture");
        try {
            Is(ToyBox.PreviewUtilities.FormatActionAsList(new GiveObjective { Objective = objective })[0].StartsWith("开启目标"), "action prefix installed");
            Is(ToyBox.PreviewUtilities.FormatActionAsList(new GameAction())[0] == "Unknown action", "native action fallback installed");
            var branch = new Conditional {
                IfTrue = new ActionList { Actions = new GameAction[] { new GiveObjective { Objective = objective } } },
                IfFalse = new ActionList { Actions = new GameAction[] { new GameAction() } }
            };
            Is(ToyBox.PreviewUtilities.FormatActionAsList(branch)[0].StartsWith("开启目标") && branch.ConditionsChecker.Checks == 1, "conditional branch selected/evaluated once by original fixture");
            branch.ConditionsChecker.TestResult = false;
            Is(ToyBox.PreviewUtilities.FormatActionAsList(branch)[0] == "Unknown action" && branch.ConditionsChecker.Checks == 2, "opposite conditional branch preserved");
            Is(ToyBox.PreviewUtilities.FormatConditions(checker).StartsWith("满足以下任一组"), "checker patch installed");
            Is(ToyBox.PreviewUtilities.FormatConditions(new ConditionsChecker()) == "", "patched empty checker");
            Is(ToyBox.PreviewUtilities.FormatConditions(Array.Empty<Condition>()) == "", "patched empty array");
            var heresy = new EtudeStatus { Etude = new SimpleBlueprint { AssetGuid = "572778222e3849848119cef86face0c7", name = "Heresy" }, Playing = true, Not = true };
            var readableAnswer = new Kingmaker.DialogSystem.Blueprints.BlueprintAnswer {
                ShowConditions = new ConditionsChecker { Conditions = new Condition[] { heresy } },
                SelectConditions = new ConditionsChecker { Conditions = new Condition[] { new HasEnoughMoneyForCustomCompanion() } },
                Actions = new GameAction[] { new CreateCustomCompanion() }
            };
            var conditionLines = ToyBox.PreviewUtilities.FormatConditionsAsList(readableAnswer);
            Is(conditionLines[0] == "出现条件：剧情状态“异端”未在运行" && conditionLines[1].StartsWith("选择要求：利润因子"), "player-facing labels and single-condition sentence");
            ToyBoxChineseDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
            var before = ToyBox.PreviewManager.Traversals;
            var detailedPreview = ToyBox.PreviewManager.ResultsText(readableAnswer);
            Is(detailedPreview.Contains("后续可能涉及：进入自定义队友创建流程") && detailedPreview.Contains("原始：Create custom companion") && detailedPreview.Contains("来源深度：4"), "technical mode keeps summary plus original caption/depth");
            Is(ToyBox.PreviewManager.Traversals == before + 1 && heresy.Checks == 0, "technical mode adds no traversal or evaluation");
            before = branch.ConditionsChecker.Checks;
            ToyBox.PreviewUtilities.FormatActionAsList(branch);
            Is(branch.ConditionsChecker.Checks == before + 1, "technical mode conditional still checked once");
            ToyBoxChineseDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
            Is(ToyBox.PreviewManager.FormatShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = "Faith", Value = 15 }, "SoulMarkRequired({0})").Contains("点数 ≥ 15"), "verified value requirement");
            Is(ToyBox.PreviewManager.FormatShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = "Faith", Value = 15, CheckByValue = false }, "SoulMarkRequired({0})").StartsWith("SoulMarkRequired("), "rank requirement native fallback");
            Is(ToyBox.PreviewManager.FormatShift(new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = "Faith", Value = 3 }, "SoulMarkShift({0})").Contains("+3"), "positive shift");
            Is(ToyBox.PreviewManager.ResultsText(new Kingmaker.DialogSystem.Blueprints.BlueprintCue()).Contains("教条主义 +3"), "cue summary");
            var answer = new Kingmaker.DialogSystem.Blueprints.BlueprintAnswer {
                Actions = new GameAction[] { new GiveObjective { Objective = objective } },
                Shift = new Kingmaker.UnitLogic.Alignments.SoulMarkShift { Direction = "Faith", Value = -3 }
            };
            var preview = ToyBox.PreviewManager.ResultsText(answer);
            Is(preview.Contains("后续可能涉及") && !preview.Contains("来源深度") && !preview.Contains("后续结果可能取决于") && preview.Contains("教条主义 -3") && preview.Contains("劝说") && preview.Contains("隐藏检定"), "preview transpiler");
            Is(ToyBox.PreviewManager.Traversals == 2, "no extra preview traversal");
            var bp = new BlueprintFeature { AssetGuid = "175d1fd853b24f188a4078306ca066ad", name = "CorruptionSoulMark_Base" };
            Is(ToyBox.BlueprintExtensions.GetTitle(bp).StartsWith("异端主义"), "Chinese alias");
            Is(ToyBox.BlueprintExtensions.GetSearchKey(bp).Contains("异端主义") && ToyBox.BlueprintExtensions.GetSearchKey(bp).Contains(bp.name), "bilingual search");
            Is(ToyBox.BlueprintExtensions.GetSearchKey(bp).Contains(bp.AssetGuid), "GUID search");
            Is(ToyBox.BlueprintExtensions.GetSortKey(bp).StartsWith("异端主义"), "alias sort");
            ToyBox.Main.Settings.showDisplayAndInternalNames = true;
            Is(ToyBox.BlueprintExtensions.GetTitle(bp).Contains(bp.name), "show internal name");
            ToyBox.Main.Settings.searchDescriptions = true;
            Is(ToyBox.BlueprintExtensions.GetSearchKey(bp).Contains("内部记录"), "description search");
            DrawRow(bp);
            Is(ToyBox.FactsEditor.LastDescription.StartsWith("【汉化说明】"), "generic row description transpiler");
            Is(bp.Description == null, "game description getter is unchanged outside row");
            bp.OfficialDescription = "官方描述";
            DrawRow(bp);
            Is(ToyBox.FactsEditor.LastDescription == "官方描述", "official description priority");
            bp.OfficialName = "官方名称";
            Is(Aliases.Title(bp, null) == null, "official name priority");
            bp.OfficialName = null; bp.OfficialDescription = null;
            ModKit.Mod.ModKitSettings.uiCultureCode = "en";
            Is(ToyBox.PreviewUtilities.FormatConditionsAsList(readableAnswer)[0] == "Native conditions", "English restores original condition presentation");
            Is(ToyBox.BlueprintExtensions.GetTitle(bp) == bp.name, "English language restores native title");
            Is(ToyBox.PreviewManager.ResultsText(answer).Contains("Check("), "English language restores native preview");
            DrawRow(bp);
            Is(ToyBox.FactsEditor.LastDescription == null, "English restores native description");
            ModKit.Mod.ModKitSettings.uiCultureCode = "zh-CN";
            ToyBoxChineseDisplayEnhancements.Main.Enabled = false;
            Is(ToyBox.PreviewUtilities.FormatActionAsList(new CreateCustomCompanion())[0] == "Create custom companion", "disabled restores native action");
            Is(ToyBox.PreviewUtilities.FormatConditionsAsList(readableAnswer)[0] == "Native conditions", "disabled restores native condition presentation");
            Is(ToyBox.BlueprintExtensions.GetTitle(bp) == bp.name, "disabled restores native title");
            DrawRow(bp);
            Is(ToyBox.FactsEditor.LastDescription == null, "disabled restores native row description");
        } finally { harmony.UnpatchAll(harmony.Id); }
        Console.WriteLine("PASS: " + assertions + " assertions; managed Harmony fixture, not a game UI test.");
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
        ToyBoxChineseDisplayEnhancements.Main.Settings.ShowTechnicalDetails = true;
        var detailed = Semantic.Condition(notPlaying);
        Is(detailed.Contains("Heresy") && detailed.Contains(heresy.AssetGuid) && detailed.Contains("本包别名") && detailed.StartsWith("剧情状态“异端”未在运行"), "technical alias provenance and identifiers");
        Is(Semantic.Action(vendor).Contains("原始：Open vendor selecting window"), "technical action caption");
        Is(Text.Layer(4, "内容").Contains("来源深度：4"), "depth only in technical mode");
        Is(Semantic.Checker(new ConditionsChecker()) == "", "technical mode empty root stays empty");
        ToyBoxChineseDisplayEnhancements.Main.Settings.ShowTechnicalDetails = false;
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
