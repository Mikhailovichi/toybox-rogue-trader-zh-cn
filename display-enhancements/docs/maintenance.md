# 显示增强维护

**ToyBox Display Enhancements 0.5.0** 适配 ToyBox **2.0.20 预发布版**和游戏 **1.6.1.514**。以下命令从仓库根目录执行并生成候选包。

## 构建

需要 Windows、.NET 8 SDK、.NET Framework 4.8.1、本机游戏、UMM 和官方 ToyBox 2.0.20 DLL。`-DotNetPath` 可指定本机 SDK，默认读取仓库 `reference/dotnet-sdk/dotnet.exe`。

```powershell
& ./display-enhancements/build.ps1 `
  -GameDirectory 'D:\SteamLibrary\steamapps\common\Warhammer 40,000 Rogue Trader' `
  -ModManagerDirectory (Resolve-Path './reference/stage-toybox-2.0.20').Path
```

暂存目录根部放入本机 UMM 的 `UnityModManager.dll` 与 `dnlib.dll`，并在 `0ToyBox0/` 放入官方 `ToyBox.dll`。真实程序集检查会从暂存目录读取这三份 DLL。省略 `-ModManagerDirectory` 时使用当前用户的游戏 UMM 目录。

输出 `release/toybox-display-enhancements-v0.5.zip`。`ToyBoxDisplayEnhancements/` 内的 11 个文件包括 DLL、Info、四份别名表、中英文 README、CHANGELOG、CREDITS 和 LICENSE。0.3／0.4 ZIP 对应 ToyBox 2.0.16，0.1／0.2 ZIP 对应 ToyBox 1.7.34。

## 新版入口

- `src/Main.cs` 检查 ToyBox 2.0.20 的清单和程序集版本，管理增强开关和技术详情。
- `src/Runtime.cs`：读取 `GeneralSettings.Settings` 的 `UILanguage`、`ToggleSearchDescriptions`、`ToggleBPsShowDisplayAndInternalName`，以及游戏 `LocalizationManager.Instance.CurrentPack.Locale`；通过游戏 `UIUtility.GetSoulMarkDirectionText`／`GetStatText` 取得名称，未知项保留原名。
- `src/UiText.cs`：集中维护中英完整句子模板。增强说明跟随 ToyBox，官方名称和对白跟随游戏；UMM 状态在绘制时生成，语言切换后同步更新。
- `src/Patches.cs`：13 个 Harmony 补丁。`DialogPreviewUtilities` 的动作、条件、信念及结果格式化六个入口；`BPHelper` 的名称、搜索、排序、描述四个入口；ToyBox 语言更新、ToyBox GUI 和游戏语言更新三个缓存刷新入口。
- `src/Semantic.cs`、`src/ConditionText.cs`：从完整 checker 读取 AND／OR、条件否定与嵌套，保持原摘要回退；丢失组合信息的数组仅逐项显示。
- `src/DialogueText.cs`：条件、动作和浏览器共用对白／回答文本及检定配置。支持回答、选项组、对白、检定、对话、叙事页、对白序列和出口。容器按声明顺序寻找首条有效文本，以访问集合跳过循环；不调用可选性检查或分支选择，不读取检定结果分支。缺失文本保留原名称，条件和动作引用可继续回退至 GUID。
- `src/Aliases.cs` 与 `feature-aliases.zh-CN.json`／`feature-aliases.en.json`：八个条目按 GUID 与内部名匹配，两表的 GUID 和内部名须一致。官方有效名称和描述优先；当前增强语言的补充描述经 `BPHelper.GetDescription` 返回。
- `src/StoryAliases.cs` 与 `story-aliases.zh-CN.json`／`story-aliases.en.json`：仅登记已核查的 Heresy，技术详情按 GUID 登记结果标明别名来源，英文名称与内部名相同时也保留来源，见 [依据](feature-evidence.md)。
- `src/DisplaySettings.cs`：默认关闭技术详情；用户偏好保存于安装目录 `display-settings.json`，ZIP 不带该文件。
- `tests/`：真实 Harmony 的托管夹具；`tools/Inspect/`：dnlib 只读元数据和 IL 核对。

移除旧 `ModKit.localize` 和泛型列表行描述 getter 替换。不开设旧版适配分支，不修改游戏蓝图属性或游戏自身提示。

文本遍历顺序固定：对话读 `FirstCue.Cues`，叙事页先 `Cues` 后 `Answers`，对白序列先 `Cues` 后 `Exit`，序列出口先 `Continue.Cues` 后 `Answers`。对白、回答只读自身 `Text`；选项组按 `Answers` 声明顺序递归。代表文本用于辨认容器，不预测可用分支。预设检定显示难度等级，自定义检定显示基础 DC；存在条件修正时仅附说明，不求实际 DC。

新增语义覆盖 `CheckPassed`／`CheckFailed`、`CompanionInParty`、`ItemsEnough`、`HasFact`／`HasBuff`、`FlagUnlocked`／`FlagInRange`，以及回答标记／清除、开始对话、解锁／锁定／调整标志动作。队友条件按四个 `MatchWhen*` 标记列出排除状态；`IncludeDead` 独立说明。物品存在 `MoneyReplacement` 组件时走资金提示。标志值条件保留已解锁前提，整体否定由 `ConditionText` 处理。只识别静态 `IntConstant.Value`；其他数值和目标使用原表达式，不调用 `GetValue()`。

`BlueprintUnit.CharacterName` 提供角色正式名，未设置时回退内部名或 GUID。主角求值器显示目标类型，带直接队友蓝图的求值器附正式名及原目标表达式；不查询实际角色。新增目标与文本不登记到固定别名表，八个特性和 Heresy 的表保持不变。

`ShowCheck` 复用浏览器的检定配置说明。`DialogSeen` 从对话的声明顺序取得代表文本，历史条件的否定沿用 `ConditionText`。获得资金按货币替代物价值乘配置数量显示。扣除优先显示全部移除，否则显示固定数量和持有量比例。殖民地特质读取指定蓝图或显示当前殖民地。

## 行为约束与验证

新版 `GetAnswerResultText` 唯一的二参数 `string.Format` 使用 `"[{0}: {1}]"`。仅替换该调用，深度 0 标“结果预览”，其余标“后续可能涉及”；保留原收集顺序、分支和条件求值次数。`GetCueResultText` 的三参数格式化同样只替换显示，保留上游仅收集正值信念变化的规则。

`FormatConditionsAsList` 读取完整 `ShowConditions`／`SelectConditions`；`FormatConditions(Condition[])` 不推测整体组合关系。空条件保持空字符串，避免改变上游以摘要长度判断的分支。格式化不调用动作、条件检查或自定义队友费用方法。技术详情复用同一读取结果。

ToyBox `zh-CN` 启用中文增强，`en` 及有效英语区域代码启用英文增强，其他 ToyBox 语言恢复原显示。游戏语言独立生效：四种中英组合分别显示同语言或混合语言文本，游戏使用其他语言时仍优先保留其官方文本。缺失的已核实名称使用对应语言后备，未知项保留内部名或原摘要。

对白引用去除富文本，中文上限 40 字，英文上限 120 字符，英文截断尽量保留完整单词。标题、排序和补充描述跟随 ToyBox；名称搜索同时支持中英文别名、内部名和 GUID，描述搜索仅添加当前增强语言的补充说明，并遵循上游开关。

回答与选项组的浏览器显示复用四个 `BPHelper` 后置补丁，不增加补丁入口或固定别名表。名称使用摘录并遵循原内部名显示开关；排序只使用简洁名称。描述保留已有官方描述并追加完整回答，内容相同时不重复添加。名称搜索保留上游内部名和 GUID，并增加当前显示名称；完整回答和官方描述仅在描述搜索开启时加入搜索键。选项组的描述与搜索复用同一条代表回答。

缓存刷新调用新版 `BPHelper.ClearNameCaches`，由上游清空四类缓存并使已注册浏览器失效。增强开关、ToyBox 与游戏语言、描述搜索和内部名显示变化时刷新；名称、搜索、排序及描述同步恢复。游戏 `LocalizationManager.OnLocaleProviderValueChanged(Locale)` 已确认在返回前设置新本地化包；后置补丁只检测语言并刷新缓存，不改变切换流程。

构建脚本依次完成编译、格式化与 Harmony 夹具、真实程序集检查、11 文件 ZIP 核对。游戏内复查见 [验收记录](acceptance.md)。

## 修订与交接

UMM 版本 `0.5.0`，程序集版本使用对应的四段格式。关闭游戏后，备份 `0ToyBox0` 和 `ToyBoxDisplayEnhancements` 到 UMM 之外。新版沿用 `display-settings.json`，恢复整组版本时将两个备份目录一起还原。

调整别名时同步中英文表与 `feature-evidence.md`。根据复现反馈更新主文件并重建候选包。SDK、参考资料、缓存和 ZIP 保存在 Git 忽略目录。
