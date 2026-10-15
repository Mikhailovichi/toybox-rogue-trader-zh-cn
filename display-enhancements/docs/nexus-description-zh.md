# ToyBox 显示增强：Nexus 中文发布文案

## 页面标题

ToyBox 显示增强 - ToyBox Display Enhancements

## 简介

为 ToyBox 2.0.16 的对话预览补充中英文说明：任务结果显示目标名，条件引用相关台词，招募要求说明利润因子门槛，并为八个内部特性提供名称或说明。增强文字随 ToyBox 语言切换，官方文本随游戏语言切换。独立 UMM 附加模组。

## 正文

### 对话提示，现在可以这样读

ToyBox 能预览对话的条件与结果。本附加模组为其中常见的提示补充中英文说明，让你更容易看懂商人、招募、任务目标和剧情状态相关内容。

例如，原来的 `Open vendor selecting window` 显示为“打开商人选择界面”；招募自定义队友的条件写成“利润因子达到招募自定义队友所需的门槛”。

当前版本为 **0.4 候选版**，UMM 中显示为 `0.4.0`。适用于 **ToyBox 2.0.16 预发布版**，对应游戏版本 **1.6.1.514**。本包调整显示文本，玩法和对话选择仍由游戏与 ToyBox 处理。

支持 ToyBox 简体中文（`zh-CN`）、英文（`en`）及英语区域代码。增强标签、别名和技术详情跟随 ToyBox，官方任务名、对白、信念与检定名称跟随游戏。两边都设为英文时显示完整英文；混合设置时保留各自语言，无需额外选择增强语言。其他 ToyBox 语言保留原显示；其他游戏语言仍优先使用其有效官方文本。

### 安装方法

先安装原版 [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) **2.0.16**，然后关闭游戏。游戏随附 Unity Mod Manager（UMM），无需另行安装。已有旧 `ToyBoxChineseDisplayEnhancements` 的用户，安装后按“更新与恢复”迁移设置并移除旧目录，再启动游戏。

**使用 Modfinder**

下载并打开 [Modfinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146)，将 `toybox-display-enhancements-v0.4.zip` 拖入窗口下方的 **“Drag zips here to install”** 区域，等待安装完成。当前候选包采用本地 ZIP 安装，不需要先在 Modfinder 目录中找到本模组。

**手动安装**

按 `Win + R`，粘贴下面的路径并打开，将本包解压到这里：

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager
   ```

如果 UMM 模组目录在其他位置，将本包解压到实际使用的模组目录，保持两个文件夹并列即可。

两种方式安装后，`ToyBoxDisplayEnhancements` 和 `0ToyBox0` 应并列，附加模组 DLL 与 `Info.json` 应位于 `ToyBoxDisplayEnhancements` 内。

**启用与设置**

1. 启动游戏，按 `Ctrl + F10` 打开 UMM，在其中启用 **ToyBox Display Enhancements**。模组 ID 和目录为 `ToyBoxDisplayEnhancements`，DLL 为 `ToyBoxDisplayEnhancements.dll`。
2. 将 ToyBox 界面设为简体中文或英文，在“百宝袋”（Bag of Tricks）的“预览”（Preview）一栏开启“对话结果”（Dialog Results）和“对话条件”（Dialog Conditions）。游戏语言也选为同一种语言后，官方名称和对白与增强说明一致。

### 相对原版的改进

**对话结果：说明动作与目标**

商人选择、自定义队友创建和任务目标等结果会显示当前增强语言。任务预览还会附上所属任务名，例如游戏为简体中文时，把 `Obj6_GoToLandingZone` 显示为“返回着陆区域”，并标出任务“终末黎明之光”。

- **结果预览**：当前层的结果，例如“打开商人选择界面”。
- **后续可能涉及**：后续对话中的结果，例如“进入自定义队友创建流程”。

后续提示帮助查看可能涉及的结果，实际走向取决于所选分支与检定。信念变化显示游戏本地化名称和带正负号的数值，出现检定使用游戏本地化技能名称与难度。

英文示例为 `Result preview: Open vendor selection` 和 `Possible later results: Begin custom companion creation`；英文游戏的信念名称为 Dogmatic、Iconoclast、Heretical。

**对话条件：引用台词，展开要求**

- **出现条件**：这个选项在什么情况下出现。
- **选择要求**：选择这个选项需要具备什么条件。

原来的 `And[2]`、`And[3]` 会展开为具体条件，多条要求按“以下条件都成立”或“满足以下任一组”分行显示。

英文使用 Availability conditions 和 Selection requirements 对应这两类要求。AND／OR、嵌套及单项或整体否定均保持原关系，不增加条件求值。

`Cue Seen [Cue_0019]` 这类提示会引用相关台词的开头，说明对白是否曾显示过，并区分本次对话与此前记录。中文摘录上限 40 字，英文 120 字符，英文截断尽量保留完整单词。

`Answer Selected (Answer_00101)` 会引用正式回答，说明是否曾选择过它；`Answer List Shown` 则说明是否显示过某组选项，并附代表回答帮助辨认。两者均区分本次对话、此前记录和否定条件。

蓝图浏览器中的回答与选项组也使用回答摘录作为名称，描述区域提供完整回答。搜索支持摘录、内部名和 GUID，开启“搜索描述”后支持完整回答文本；“显示内部名称”可在名称旁保留原标识，排序按简洁名称进行。文本随游戏语言切换，缺失时保留原名称。

对白、对话、叙事页、对白序列和出口也会引用首条可读取的正式台词或回答，描述可查看该条全文。检定蓝图显示技能、基础 DC 或预设难度，并注明隐藏检定和条件修正；提示不计算实际检定结果。

检定条件分别说明本次对话的成功与失败记录；队友、物品、能力和效果条件列出状态筛选、数量门槛或目标角色。剧情标志提示保留解锁前提及数值要求，结果提示补充标记／清除回答记录、开始对话和标志数值调整。动态目标与增量保留表达式，缺少正式名称的标志保留内部名。

`Heresy` 使用本包提供的中文别名“异端”。对应条件显示为“剧情状态‘异端’未在运行”，描述该状态当前的运行情况。

英文对应 `Story state "Heresy" is not running`，不推断从未触发。未知类型保留可辨认的原摘要。

**特性说明与搜索：辨认内部条目**

为八个内部条目补充中英文名称或说明；中文标为“汉化说明”，英文标为 Addon note。例如：

- `BaseCharacterFeatures` 显示为“基础角色特性〔内部条目〕”。
- `ReloadAfterCombat_Feature` 显示为“战斗后装填〔内部条目〕”，说明它关联的自动装填逻辑。
- 三个信念条目分别显示为“异端主义”“自由主义”“教条主义”，并注明内部信念条目。

游戏已有的有效名称和描述优先显示。搜索同时支持中英文别名、原内部名和 GUID；开启“搜索描述”后，也可以搜索当前增强语言的补充说明。`IsPlayer_Feature` 与 `FeatProgressionTest` 的完整作用尚未确认，保留内部名，不根据名称推荐增删。

默认使用简短提示。需要对照原摘要和内部标识时，可在本附加模组的 UMM 面板开启“显示技术详情”。

目前覆盖任务目标、剧情状态与标志、对白与回答记录、检定记录、常用角色与物品条件、商人及自定义队友等提示，对话相关蓝图的名称与搜索，以及八个内部特性；其余类型沿用 ToyBox 原摘要。

### 搭配词表汉化

菜单和设置说明可以搭配 **ToyBox 简体中文词表修订 2.0** 阅读。它统一“职业”“剧情状态”“搜索与选取”等译名，并补充重置加点、灵能和地图选项的中文说明。

两个包可以单独安装，也可以一起使用：词表汉化替换 `0ToyBox0` 中的中文 JSON，本包安装在独立 UMM 文件夹。

### 更新与恢复

模组 ID、DLL 和目录统一为 `ToyBoxDisplayEnhancements`。从旧 `ToyBoxChineseDisplayEnhancements` 升级，包括此前使用旧标识的 0.4 候选包，先确认 ToyBox 为 2.0.16，再关闭游戏并安装新版到新目录。若旧目录有 `display-settings.json`，将它复制到新目录，然后移除 UMM 中的旧 `ToyBoxChineseDisplayEnhancements` 文件夹；备份应放在 UMM 目录之外，避免重复加载。启动游戏后在 UMM 中重新启用新版。

后续同标识版本可覆盖 `ToyBoxDisplayEnhancements` 目录。包内不包含 `display-settings.json`，不会覆盖该目录中的技术详情偏好。

切换 ToyBox 或游戏语言会刷新名称、搜索、排序、描述及浏览器缓存。关闭附加模组或 ToyBox 使用不支持的语言时恢复原显示；已打开的对话提示必要时重新打开。卸载时关闭游戏并移除本附加模组文件夹。

保留的 0.3 中文包适用于 ToyBox 2.0.16；旧 0.1／0.2 ZIP 只适用于 ToyBox 1.7.34，同一模组只安装一个版本。当前候选包通过 780 项托管断言、182 项真实程序集检查，游戏内布局和语言切换待验收后再确定公开发布。

### 署名与许可

显示增强作者：**mitcheese**。ToyBox：Narria、ADDB 及上游贡献者。

采用 MIT 许可，完整声明和署名见安装包内的 `LICENSE.txt` 与 `CREDITS.md`。

## 下载文件信息

- 文件名称：ToyBox Display Enhancements 0.4 候选版
- 文件版本：`0.4.0`
- 上传文件：`toybox-display-enhancements-v0.4.zip`
- 文件说明：适用于 ToyBox 2.0.16，游戏版本 1.6.1.514。使用 Modfinder 拖入 ZIP 或解压到 UMM 模组目录；启用增强，将 ToyBox 设为英文或简体中文，并开启对话结果与条件预览。增强说明跟随 ToyBox，官方文本跟随游戏语言。
- 保留文件：`toybox-chinese-display-enhancements-v0.3.zip`，仅中文增强，适用于 ToyBox 2.0.16。
- 旧版文件：`toybox-chinese-display-enhancements-v0.1.zip`、`toybox-chinese-display-enhancements-v0.2.zip`，明确标注仅适用于 ToyBox 1.7.34。

## 图片说明

已有四张图来自旧版，不作为 2.0.16 效果展示。本版先使用商人、招募、任务目标、对白条件和特性条目的文字例子，待新版界面验收后再补图。
