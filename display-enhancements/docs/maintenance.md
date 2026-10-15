# 显示增强维护

目标固定为 ToyBox **2.0.16 预发布版**、游戏 **1.6.1.514**。增强版本 **0.4.0**，公开名称为 **ToyBox Display Enhancements**。UMM ID、DLL、安装目录、命名空间和项目名统一为 `ToyBoxDisplayEnhancements`，设置格式和文件名仍为 `display-settings.json`。以下命令在仓库根目录 PowerShell 中执行，构建脚本生成候选包，不部署文件。

## 构建

需要 Windows、.NET 8 SDK、.NET Framework 4.8.1，以及本机游戏、UMM 和 ToyBox 2.0.16。SDK、NuGet 缓存置于已忽略的根 `reference/`；可复用现有 SDK，通过 `-DotNetPath` 指定 `dotnet.exe`。编译使用 net481 参考程序集，不重建上游 ToyBox。

```powershell
& ./display-enhancements/build.ps1 `
  -GameDirectory 'D:\SteamLibrary\steamapps\common\Warhammer 40,000 Rogue Trader'
```

默认 UMM 为当前用户 `AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager`。其他位置用 `-ModManagerDirectory` 指定。

输出 `release/toybox-display-enhancements-v0.4.zip`。包内 11 个文件位于 `ToyBoxDisplayEnhancements/`：DLL、Info、特性与剧情四份中英文 JSON、中英文 README、CHANGELOG、CREDITS、LICENSE。游戏、UMM、Harmony、测试程序集、用户设置与旧截图不入包；旧 0.3 ZIP 保留供 ToyBox 2.0.16 使用，0.1／0.2 ZIP 保留供 ToyBox 1.7.34 使用。

## 新版入口

- `src/Main.cs`：独立 UMM ID `ToyBoxDisplayEnhancements`、2.0.16 版本检查、加载失败回退、增强开关与技术详情保存。`Info.json` 的 DLL 和入口、程序集名、Harmony ID 与项目引用同步使用新标识。
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

## 行为约束与验证

新版 `GetAnswerResultText` 唯一的二参数 `string.Format` 使用 `"[{0}: {1}]"`。仅替换该调用，深度 0 标“结果预览”，其余标“后续可能涉及”；保留原收集顺序、分支和条件求值次数。`GetCueResultText` 的三参数格式化同样只替换显示，保留上游仅收集正值信念变化的规则。

`FormatConditionsAsList` 读取完整 `ShowConditions`／`SelectConditions`；`FormatConditions(Condition[])` 不推测整体组合关系。空条件保持空字符串，避免改变上游以摘要长度判断的分支。格式化不调用动作、条件检查或自定义队友费用方法。技术详情复用同一读取结果。

ToyBox `zh-CN` 启用中文增强，`en` 及有效英语区域代码启用英文增强，其他 ToyBox 语言恢复原显示。游戏语言独立生效：四种中英组合分别显示同语言或混合语言文本，游戏使用其他语言时仍优先保留其官方文本。缺失的已核实名称使用对应语言后备，未知项保留内部名或原摘要。

对白引用去除富文本，中文上限 40 字，英文上限 120 字符，英文截断尽量保留完整单词。标题、排序和补充描述跟随 ToyBox；名称搜索同时支持中英文别名、内部名和 GUID，描述搜索仅添加当前增强语言的补充说明，并遵循上游开关。

回答与选项组的浏览器显示复用四个 `BPHelper` 后置补丁，不增加补丁入口或固定别名表。名称使用摘录并遵循原内部名显示开关；排序只使用简洁名称。描述保留已有官方描述并追加完整回答，内容相同时不重复添加。名称搜索保留上游内部名和 GUID，并增加当前显示名称；完整回答和官方描述仅在描述搜索开启时加入搜索键。选项组的描述与搜索复用同一条代表回答。

缓存刷新调用新版 `BPHelper.ClearNameCaches`，由上游清空四类缓存并使已注册浏览器失效。增强开关、ToyBox 与游戏语言、描述搜索和内部名显示变化时刷新；名称、搜索、排序及描述同步恢复。游戏 `LocalizationManager.OnLocaleProviderValueChanged(Locale)` 已确认在返回前设置新本地化包；后置补丁只检测语言并刷新缓存，不改变切换流程。

构建脚本完成编译、780 项夹具断言、182 项真实程序集检查与 11 文件 ZIP 字节核对。夹具覆盖四种中英组合、不支持的 ToyBox 语言、八个特性、双语搜索、缓存恢复及行为计数。游戏内验收见 [acceptance.md](acceptance.md)。测试专用 Newtonsoft.Json 仅在测试输出中使用；运行模组引用游戏版本，不发布替代 DLL。

## 修订与交接

UMM 版本 `0.4.0`、程序集 `0.4.0.0`。新标识不自动迁移旧安装：用户关闭游戏后安装到 `ToyBoxDisplayEnhancements`，将旧 `ToyBoxChineseDisplayEnhancements/display-settings.json` 复制到新目录，移除旧目录并在 UMM 重新启用；旧目录备份放在 UMM 之外，防止不同 ID 的两套补丁同时加载。设置格式不变，不增加运行时迁移层。

调整别名时同步中英文表与 `feature-evidence.md`；文案与布局反馈直接修改主文件，再构建同一候选包。当前待用户安装验收全英文、混合语言、长对白和分组换行，再补新版截图。保留旧 ZIP，只安装一套显示增强。SDK、源码参考、游戏资产、缓存、ZIP 均不进入 Git；不自动提交、推送或安装。
