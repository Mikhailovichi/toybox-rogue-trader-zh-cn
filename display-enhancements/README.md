# ToyBox 显示增强 - ToyBox Display Enhancements

[English instructions](README.en.md)

为《战锤 40,000：行商浪人》的 ToyBox 提供中英文显示增强：任务结果显示目标名称，对话条件引用相关台词，招募要求说明利润因子门槛。角色特性列表中的八个内部条目也有了中英文名称或说明，方便辨认它们的用途。

## 版本与依赖

- 附加模组：**0.4 候选版**，UMM 中显示为 `0.4.0`。
- 适用版本：**ToyBox 2.0.16 预发布版**，游戏 **1.6.1.514**。
- 需要原版 [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1)。游戏随附 Unity Mod Manager（UMM），无需另行安装 UMM。
- 可与 **ToyBox 简体中文词表修订 2.0** 一起使用，两者分别安装。

增强支持 ToyBox 简体中文（`zh-CN`）、英文（`en`）及英语区域代码，例如 `en-GB`、`en-US`。增强说明、别名和技术详情跟随 ToyBox；官方任务名、对白、信念与检定名称跟随游戏语言，不需要另设增强语言。

| ToyBox 语言 | 游戏语言 | 显示效果 |
| --- | --- | --- |
| 简体中文 | 简体中文 | 中文增强说明与官方中文文本 |
| 英文 | 英文 | 英文增强说明与官方英文文本 |
| 英文 | 简体中文 | 英文增强说明，官方名称与对白为中文 |
| 简体中文 | 英文 | 中文增强说明，官方名称与对白为英文 |

其他 ToyBox 语言不启用增强。游戏使用其他语言时保留其有效官方文本；缺失且没有已确认的后备名称时保留内部名称或原摘要。本版未增加其他增强语言。

## 安装

先安装原版 ToyBox **2.0.16**，然后关闭游戏。选择以下任一种方式安装显示增强。已安装旧 `ToyBoxChineseDisplayEnhancements` 的用户，请按下方“更新与恢复”迁移设置并移除旧目录后再启动游戏。

### 使用 Modfinder

1. 下载并打开 [Modfinder](https://www.nexusmods.com/warhammer40kroguetrader/mods/146)。
2. 将 `toybox-display-enhancements-v0.4.zip` 拖入窗口下方的 **“Drag zips here to install”** 区域，等待安装完成。
3. 确认附加模组安装在 UMM 目录内，与 `0ToyBox0` 并列。

当前候选包采用本地 ZIP 安装，不需要先在 Modfinder 目录中找到本模组。

### 手动安装

将安装包解压到现有 UMM 模组目录，使 **`ToyBoxDisplayEnhancements` 与 `0ToyBox0` 并列**。Windows 默认位置为：

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\
   ├── 0ToyBox0\
   └── ToyBoxDisplayEnhancements\
       ├── Info.json
       ├── ToyBoxDisplayEnhancements.dll
       ├── feature-aliases.zh-CN.json
       ├── feature-aliases.en.json
       ├── story-aliases.zh-CN.json
       └── story-aliases.en.json
   ```

### 启用与设置

1. 启动游戏，按 `Ctrl + F10` 打开 UMM，在其中启用 **ToyBox Display Enhancements**。安装目录和模组 ID 为 `ToyBoxDisplayEnhancements`，DLL 为 `ToyBoxDisplayEnhancements.dll`。
2. 将 ToyBox 界面设为简体中文或英文，在“百宝袋”（Bag of Tricks）的“预览”（Preview）一栏开启“对话结果”（Dialog Results）和“对话条件”（Dialog Conditions）。游戏语言也选为英文时得到完整英文显示；两边都选简体中文时得到完整中文显示。
3. 默认显示简短说明。需要查看原摘要、内部名称、GUID 和来源层级时，在本附加模组的 UMM 面板开启“显示技术详情”（Show technical details）。选择会自动保存，面板文字随 ToyBox 语言切换更新。

## 相对原版的改进

### 对话结果：直接说明将打开什么、改变哪个目标

原来显示 `Open vendor selecting window` 的地方，改为“打开商人选择界面”；`Create custom companion` 改为“进入自定义队友创建流程”。提示直接说明动作的用途。

关键提示示例：

> 结果预览：打开商人选择界面
>
> 选择要求：利润因子达到招募自定义队友所需的门槛
>
> 后续可能涉及：进入自定义队友创建流程

任务提示也会使用目标与所属任务的名称。例如，`Obj6_GoToLandingZone` 对应“返回着陆区域”，并附上任务名“终末黎明之光”；动作则区分“开启目标”“完成目标”和“目标失败”。

提示用 **“结果预览”** 标出当前层的结果，用 **“后续可能涉及”** 标出后续内容。后续提示帮助查看可能涉及的结果，实际走向取决于所选分支与检定。信念变化会显示“教条主义”“自由主义”“异端主义”和带正负号的数值，出现检定会显示游戏本地化技能名称和难度。

ToyBox 与游戏都设为英文时，对应示例为：

> Result preview: Open vendor selection
>
> Selection requirements: Profit Factor meets the requirement for recruiting a custom companion
>
> Possible later results: Begin custom companion creation

英文信念名称采用游戏官方的 Dogmatic、Iconoclast、Heretical。点数要求写明至少需要多少点，按信念阶级判断的要求保留原摘要。

### 对话条件：看相关台词和要求，而不是猜内部编号

**“出现条件”** 说明选项何时会出现在对话中，**“选择要求”** 说明选择它需要具备什么条件。例如，`Has enough money for custom companion` 现在说明的是招募所需的利润因子门槛。

单条条件直接写成一句话，多条条件按“以下条件都成立”或“满足以下任一组”分行显示。原来的 `And[2]`、`And[3]` 会展开具体条件，方便看清它们之间的关系。

例如，单条条件显示“出现条件：剧情状态‘异端’未在运行”；第二条列出此前显示过的对白和剧情状态要求。“异端”是本包为 `Heresy` 提供的中文别名，“未在运行”描述的是该状态当前的运行情况。

`Cue Seen [Cue_0019]` 这类提示会引用关联对白的开头，玩家可以直接辨认是哪段话。提示还会区分本次对话与此前记录。按 ToyBox 语言，中文摘录上限为 40 字，英文为 120 字符，英文截断尽量保留完整单词。

例如，“此前已显示过对白”引用了“这是帝皇赐予我的祝福……”这段台词；剧情状态仍以“正在运行”等中文说明显示。保留的内部名称可以配合技术详情用于定位。

英文使用 Availability conditions 和 Selection requirements 区分出现与选择要求。AND／OR、嵌套和整体否定保持原关系；对应剧情条件显示为 `Story state "Heresy" is not running`，不推断此前是否发生过。

### 特性说明与搜索：辨认内部条目的用途

角色的特性列表中，部分条目原来只有内部名称。现在名称和描述区域提供以下说明：

| 原名称 | 中文显示或说明 |
| --- | --- |
| `CorruptionSoulMark_Base`、`HopeSoulMark_Base`、`FaithSoulMark_Base` | 异端主义、自由主义、教条主义的内部信念条目，说明列表数值记录的是内部等级。 |
| `BaseCharacterFeatures` | 基础角色特性，说明它包含多个关联特性。 |
| `ReloadAfterCombat_Feature` | 战斗后装填，说明它关联的战斗结束装填逻辑。 |
| `CustomCharacterChargenPath` | 自定义角色创建路径，说明它记录角色创建时的关联特性。 |
| `IsPlayer_Feature`、`FeatProgressionTest` | 保留名称，在描述区域补充关联状态项或旧版职业进度类型的说明。 |

游戏已有的有效名称和描述优先显示。中文补充内容标为 **“汉化说明”**，英文标为 **Addon note**；例如基础角色特性的英文别名为 `Base character features [internal entry]`。搜索支持中英文别名、原内部名和 GUID；开启“搜索描述”后，还可以搜索当前增强语言的补充说明。“显示内部名称”可在别名旁保留原标识，便于对照查找。

当前覆盖任务目标、剧情状态、对白记录、商人及自定义队友等提示，以及上表中的八个特性条目；其余类型沿用 ToyBox 原摘要。

## 更新与恢复

模组 ID、DLL 和安装目录统一为 `ToyBoxDisplayEnhancements`。从旧 `ToyBoxChineseDisplayEnhancements` 升级，包括此前使用旧标识的 0.4 候选包，不能直接覆盖旧目录：

1. 确认 ToyBox 为 **2.0.16 预发布版**，关闭游戏，安装新版到 `ToyBoxDisplayEnhancements`。
2. 若旧目录有 `display-settings.json`，将它复制到新目录，保留技术详情偏好；没有该文件则使用默认设置。
3. 移除 UMM 目录中的旧 `ToyBoxChineseDisplayEnhancements` 文件夹。需要备份时将其保存到 UMM 目录之外；不要保留两套模组。
4. 启动游戏，在 UMM 中启用 **ToyBox Display Enhancements**。UMM 将其视为新模组，需要重新启用。

后续同为 `ToyBoxDisplayEnhancements` 的版本可关闭游戏后覆盖同一目录。安装包不包含 `display-settings.json`，不会覆盖该目录中的设置文件。

切换 ToyBox 或游戏语言时，会刷新名称、搜索、排序、描述及浏览器缓存。关闭增强或将 ToyBox 切换到不支持的语言时恢复原显示。对话提示随界面刷新；必要时重新打开相关界面。卸载时关闭游戏并移除本附加模组文件夹。

保留 0.3 中文包，适用于 **ToyBox 2.0.16**；旧 0.1／0.2 ZIP 仅供 **ToyBox 1.7.34** 使用。同一模组只安装一个版本。更新 ToyBox 后应选择对应适配包。

当前候选包通过 279 项托管格式化／Harmony 断言与 81 项本机真实程序集检查；新版游戏内加载、排版及语言切换仍待用户验收。已有图片来自旧版，本版先用文字示例，验收后补新版截图。

## 署名与许可

附加模组作者：**mitcheese**。ToyBox 由 Narria、ADDB 及上游贡献者开发，完整署名见 [CREDITS.md](CREDITS.md)。

采用 MIT 许可，详见 [LICENSE.txt](LICENSE.txt)。
