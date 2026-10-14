# 截图条目的名称与描述核查

## 范围与依据

针对游戏 **1.6.1.514**、ToyBox **2.0.16** 的八个英文条目及对话截图。只读取本机 `Bundles/cheatdata.json`、`Bundles/blueprints-pack.bbp`、官方 `enGB.json`／`zhCN.json` 和程序集元数据；未运行动作或修改蓝图。0.3.0 已重新定位八个 GUID 的本机蓝图记录并核对名称与描述引用，沿用下述补充说明。

使用蓝图包索引中的 GUID 与记录边界定位条目，核对名称、类型、组件及关联 GUID；在各记录内核对官方本地化键引用。八个记录均未发现可直接用于条目名称／描述的官方文本关联。这不表示整个游戏没有相关的机制介绍，也不把内部名称的直译当作官方译名。运行时仍优先保留游戏实际返回的有效官方名称与描述。

0.4.0 沿用这些蓝图结构依据，增加英文别名与说明。两份主文件为 `../feature-aliases.zh-CN.json` 与 `../feature-aliases.en.json`，保持相同八个 GUID 和内部名。补充说明随 ToyBox 显示为“【汉化说明】”或 `[Addon note]`；有效官方名称与描述始终优先。

## 八个条目

| 内部名 | GUID | 类型与已确认结构 | 显示决定 |
| --- | --- | --- | --- |
| `CorruptionSoulMark_Base` | `175d1fd853b24f188a4078306ca066ad` | `BlueprintSoulMark`；五个 `RankAddedTrigger`，含 `HasFactGetter` 条件结构 | 异端主义〔内部信念条目〕；说明为信念内部记录，列表数值是内部等级，不当作普通天赋 |
| `HopeSoulMark_Base` | `676567cf7bb8459abded7ee617d1625e` | `BlueprintSoulMark`；五个 `RankAddedTrigger`，含 `HasFactGetter` | 自由主义〔内部信念条目〕；同上 |
| `FaithSoulMark_Base` | `1aa7cb5ae17c4ed19aa2596b6bcca9d3` | `BlueprintSoulMark`；五个 `RankAddedTrigger`，含 `HasFactGetter` | 教条主义〔内部信念条目〕；同上 |
| `BaseCharacterFeatures` | `f7cb6da6f8424aacb7414aa35cb06824` | `BlueprintFeature`；`AddFacts`、两个 `AddFeaturesToLevelUp`，另有动作点消耗覆盖组件；`AddFacts` 包含下列装填特性的关联 | 基础角色特性〔内部条目〕；说明是多个关联项的组合，不枚举未追踪的全部效果 |
| `ReloadAfterCombat_Feature` | `314d7be4a98249438db05b517213e810` | `BlueprintFeature`；`CombatStateTrigger` 关联 `ContextActionCastSpell`，施放 `ReloadAfterCombatAbility` | 战斗后装填〔内部条目〕；仅介绍已核实的触发结构与装填能力，不承诺弹药消耗、所有武器适用性或特殊状态行为 |
| `IsPlayer_Feature` | `9fd7e5142622424d9686b7f27e2e05d4` | `BlueprintFeature`；`AddFacts` | 保留内部名；只说明关联状态项，完整作用未确认，不推断为“玩家身份开关” |
| `CustomCharacterChargenPath` | `45181a40472441a8904a5282f83693f4` | `BlueprintOriginPath`；六个 `AddFeaturesToLevelUp` 及多项关联蓝图 | 自定义角色创建路径〔内部条目〕；说明为内部创建路径，数值不是独立天赋等级 |
| `FeatProgressionTest` | `25a98d3901c0426f87d2e85930fc1cd7` | `Kingmaker.UnitLogic.Levelup.Obsolete.Blueprints.BlueprintProgression`；`AddFacts` | 保留内部名；说明旧版进度类型，不由名称中的 Test 推断无用或可删除 |

三个信念名称沿用官方机制术语。`SoulMarkShiftExtension` 的方向映射与 `BlueprintSoulMark` 类型支持对应关系；它们的补充说明并非官方条目描述。

### 英文别名

| 内部名 | 英文后备名称 |
| --- | --- |
| `CorruptionSoulMark_Base` | Heretical [internal conviction entry] |
| `HopeSoulMark_Base` | Iconoclast [internal conviction entry] |
| `FaithSoulMark_Base` | Dogmatic [internal conviction entry] |
| `BaseCharacterFeatures` | Base character features [internal entry] |
| `ReloadAfterCombat_Feature` | Reload after combat [internal entry] |
| `IsPlayer_Feature` | IsPlayer_Feature |
| `CustomCharacterChargenPath` | Custom character creation path [internal entry] |
| `FeatProgressionTest` | FeatProgressionTest |

英文说明保留中文说明中的已核实作用范围与内部条目性质，不将关联结构扩写成完整机制。标题、排序与补充描述使用 ToyBox 当前语言；名称搜索同时纳入中英文别名、内部名和 GUID，描述搜索仅纳入当前增强语言的补充说明，并遵循上游开关。

### 装填关联链

`ReloadAfterCombat_Feature` → `CombatStateTrigger` → `ContextActionCastSpell` → `ReloadAfterCombatAbility`（`1cedb5f0cf104f57a88f91168e4c0df8`）→ `AbilityEffectRunAction` → `WarhammerContextActionReloadWeapon`。

程序集确认 `CombatStateTrigger` 的 `CombatStartActions` 与 `CombatEndActions` 字段分别由加入战斗／离开战斗事件执行；后者受 `UnitMustBeConscious` 配置约束。蓝图记录包含空的开始动作列表和含一次施放的结束动作列表。说明仅表述这条关联链，不宣称任何额外的武器或弹药规则。

## 对话截图中的官方名称

| 内部名 | 蓝图 GUID | 官方中文标题 | 所属任务 |
| --- | --- | --- | --- |
| `Obj6_GoToLandingZone` | `1bc9e3eedc7848c484bd63e3c8644f18` | 返回着陆区域 | 终末黎明之光 |
| `Obj7_TalkToGovernor` | `61af509e98d94340a9ae295633f6564d` | 与总督谈谈 | 终末黎明之光 |
| `1_ExploreTheStation` | `4c30288d5e5c4db5b612a4170cd77a1f` | 探寻星站，寻找答案 | 导航宗族的秘密 |

运行时从目标的 `Title` 与 `Quest.Title` 读取本地化文本，不按截图或内部名进行字符串替换。

## 新版对话条件与动作依据

核查 `Code.dll` 与 `RogueTrader.GameCore.dll` 中的类型、字段和相关方法 IL；以下记录描述读取依据，不代表执行过游戏逻辑。

| 类型或数据 | 已确认依据 | 显示决定 |
| --- | --- | --- |
| `OrAndLogic` | 持有 `ConditionsChecker`；原摘要只显示组合操作与数量；实际检查委托给此 checker | 递归读取 `Operation`、`Conditions` 和各条件的 `Not`，展开 `And[2]`／`And[3]`；不调用检查方法 |
| `Heresy`，GUID `572778222e3849848119cef86face0c7` | 已定位的剧情状态蓝图；中文名不是官方玩家向标题 | 中英文表分别登记“异端”与 Heresy；仅匹配此 GUID 与内部名，技术详情按 GUID 登记结果说明来源，不依赖别名是否与内部名不同 |
| `EtudeStatus` | `Playing` 表示运行中；`Started` 的检查包含已启动、运行中及完成处理中；`Not` 否定整体条件 | 截图中 `Not + Playing` 显示“剧情状态‘异端’未在运行”，不写“未触发”或推断信念路线 |
| `OpenVendorSelectingWindow` | `RunAction` 进入商人选择流程 | “打开商人选择界面”；不运行商人求值或动作 |
| `CreateCustomCompanion` | `RunAction` 在非免费情形检查门槛后调用 `Player.CreateCustomCompanion`；后者打开角色创建界面 | “进入自定义队友创建流程”，不宣称立即获得队友或必然扣费 |
| `HasEnoughMoneyForCustomCompanion` | `CheckCondition` 比较 `GetCustomCompanionCost()` 与 `Player.ProfitFactor.Total`；前者读取 `BlueprintRoot.CustomCompanionBaseCost` | 仅描述“利润因子达到招募自定义队友所需的门槛”；不读取实时数值、不调用费用方法或求值 |
| `CueSeen` | `Cue` 关联 `BlueprintCueBase`；`CurrentDialog` 为真检查 `LocalShownCues`，否则检查玩家对话记录 `ShownCuesContains`；`BlueprintCue.Text` 提供官方对白 | 区分本次对话与此前记录，使用“已显示过／未显示过”或对应英文，不写“已选择”；只读取文本、去富文本，中文上限 40 字、英文上限 120 字符且尽量保留完整单词，缺失时保留内部名或 GUID |

条件读取和渲染分离：组合否定保留整个分组，未知条件沿用已带否定的原摘要，避免再次否定。顶层空条件仍返回空字符串；嵌套空组与循环则保留可辨认的原摘要及原因。

新版 `DialogPreviewUtilities.GetAnswerResultText(answer)` 的深度分组使用 `"[{0}: {1}]"`；0.4.0 继续仅替换唯一的二参数格式化调用，不改上游收集方法。各组依原顺序保留，当前层标为“结果预览”／`Result preview`、后续层标为“后续可能涉及”／`Possible later results`，不去重，不改写为连续步骤。技术详情附原始摘要、关联标识及原数字深度，不重复遍历或判断条件。Heresy 的否定运行状态英文为 `Story state "Heresy" is not running`，不推断从未触发。

## 语言与缓存依据

增强说明读取 ToyBox `GeneralSettings.Settings.UILanguage`：`zh-CN` 为中文，`en` 及有效英语区域代码为英文；其他语言使用上游原显示。官方名称与对白读取游戏当前本地化文本，因此四种中英设置组合可分别得到同语言或混合语言显示，其他游戏语言也保留其有效官方文本。

真实程序集确认 `LocalizationManager.Instance.CurrentPack.Locale` 提供当前游戏语言，`enGB` 与 `zhCN` 分别为英文与简体中文。`OnLocaleProviderValueChanged(Locale)` 返回前完成本地化包替换；新增后置补丁只刷新 `BPHelper` 与浏览器缓存。缓存状态同时比较 ToyBox 与游戏语言、增强开关、描述搜索和内部名显示，沿用 ToyBox 自身刷新入口，总计 13 个补丁。

信念与检定通过游戏 `UIUtility.GetSoulMarkDirectionText`／`GetStatText` 读取；已核实英文机制名称为 Dogmatic、Iconoclast、Heretical、Profit Factor、Persuasion、Awareness。缺失时使用已核实的对应语言后备，未知项仍保留内部名或原摘要。中英模板集中在 `src/UiText.cs`，UMM 状态按绘制时的语言生成。

0.4.0 已通过 279 项托管 Harmony／格式化断言与 81 项真实程序集检查，覆盖四种中英组合、八个特性、双语搜索、否定、对白、缓存恢复和行为计数。此记录用于说明实现依据；新版游戏内排版仍按 [验收记录](acceptance.md) 由用户安装后确认。

## 不推测的内容

- `IsPlayer_Feature` 与 `FeatProgressionTest` 的完整影响尚未确认；不提供删改建议。
- 剧情状态没有通用的官方玩家向标题；除已登记的“异端”别名外保留内部名，仅翻译动作和状态语义。
- 信念点数要求由实际 `CheckShiftAtLeast` 比较确认是“至少”；按信念阶级判断的要求不套用点数说明，保留原摘要。
- 特性补充描述通过新版 `BPHelper.GetDescription` 显示；不再替换旧列表行 getter，不修改蓝图属性或游戏自身提示。
- 信念与检定名称优先读取游戏 `UIUtility.GetSoulMarkDirectionText`／`GetStatText`；未知项保留原名称。
- `GetCueResultText` 保留新版上游仅收集正值信念变化的规则，增强不补执行或重新求值。
