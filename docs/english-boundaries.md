# 英文保留范围与源码依据

依据：ToyBox `v1.7.34`，提交 `f044d0ccc90c4800aef0142fdd5147abaf7f5a4e`。以下行号相对于 `reference/source/ToyBox/`。

## 已覆盖的动态显示名称

| 来源 | 处理 |
|---|---|
| `Classes/MainUI/Main.cs:299` | 主标签页名称；Patch Tool 等已补译 |
| `Classes/MainUI/Browser/SearchAndPick.cs:235` | NamedTypeFilter 的有限显示名称已覆盖 |
| `Classes/MainUI/ColonyEditor.cs:97` | Contentment、Efficiency、Security 已覆盖 |
| `Classes/MainUI/PartyEditor/StatsEditor.cs:362` | 外观、骨骼、装备尺寸显示键，连同后续四处调用已覆盖 |
| `Classes/MainUI/PartyEditor/StatsEditor.cs:525` | Size 显示枚举已覆盖 |
| `Classes/MainUI/PartyEditor/StatsEditor.cs:588` | SoulMarkDirection 信念方向已覆盖 |
| `Classes/MainUI/PartyEditor/StatsEditor.cs:631` | 本机 StatType 显示名称已覆盖 |
| `Classes/ModKit/UI/KeyBindings/UI+KeyBindings.cs:128` | 快捷键按钮的有限标题已覆盖，内部绑定 ID 保留 |
| `Classes/ModKit/UI/UI+Pickers.cs:120` | 掷骰对象、日志等级与富文本稀有度名称已覆盖 |
| `Classes/ModKit/UI/UI+Pickers.cs:222` | RogueCheats 的派系选择器已覆盖 |
| `Classes/MainUI/EnhancedUI/PhatLoot.cs:158` | Containers / Units 已覆盖 |

源码中的 Enchantment 主页签位于 `#if false` 分支，当前主界面不启用；原有翻译保留，并保留其中静态键、物品枚举等对应译文。强化类别由蓝图类型生成的内部类别名称保留。没有把这个关闭分支的覆盖计数当作游戏可见功能数。

## 词表无法处理的写死界面文本

这些位置没有调用 `.localize()`，即使词表中有相同英文也不会使用译文。

| 源码入口 | 原文例子 | 原因 |
|---|---|---|
| `Classes/MainUI/BagOfTricks.cs:270` | Remove / Add | 身份覆盖区域中的按钮直接使用英文 |
| `Classes/MainUI/Browser/BlueprintListUI.cs:65` | Parameter | 参数标签未接入词表 |
| `Classes/MainUI/Dialog+NPCs.cs:57` | Show Inactive Conditions | 同页的 Show other versions of NPCs、Reveal All On Map 等也未接入 |
| `Classes/MainUI/DialogEditor.cs:153` | Check / Show / Select | 部分检查区标签直接使用英文 |
| `Classes/MainUI/EnhancedUI/GameSaves.cs:76` | Level: | 与角色等级动态拼接，未接入词表 |
| `Classes/MainUI/EnhancedUI/PhatLoot.cs:38` | Export / Import / Replace | 部分清单导入导出按钮 |
| `Classes/MainUI/LevelUp.cs:13` | This area is under construction | 页首和部分重新选职业说明未接入 |
| `Classes/MainUI/Main.cs:281` | Blueprints loading: | 此处完整标签直接拼接，不是词表里的旧说明键 |

补丁字段编辑器还会显示运行时字段名、类型名和操作状态，属于技术编辑界面。完整自动候选列表为 `reference/extracted-keys.json` 的 `hardcoded_candidates`；其中还含数字、游戏已翻译的名称及调试／关闭分支，不能把所有候选都当成待译界面。

## 技术标识和品牌

保留 ToyBox、Sh0dan、Unity、Unity Mod Manager、GitHub、Owlcat、Discord、Mono；保留 Ctrl、Shift、F11、X/Y/Z、GUID、ID、PNG，以及示例文件 `Weapon.json` 和示例内部标记 `[PuzzlePiece Green3x1]`。

船体枚举如 `Cruiser_2x4` 保留键原样，译文只翻译名称并保留规格 `2x4`。工具读取蓝图时显示的内部英文名称、字段、类名、GUID 和异常消息不进行词表替换。

## 富文本匹配

静态提取覆盖源码原色、GUI sRGB 与线性色彩转换后的键。稀有度 `Color` 重载生成的键含双 `#` 时，查找键仍精确保留，译文使用单 `#` 的有效颜色标记。

本次覆盖检查是对已整理的源码键和有限显示枚举进行精确查找。运行时颜色转换和动态内容的实际显示仍需通过游戏验收；如出现未命中键，可从 ToyBox 的导出词表或截图继续定位。
