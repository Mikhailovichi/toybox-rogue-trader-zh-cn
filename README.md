# ToyBox 行商浪人词表与显示增强

两个独立包为《战锤 40,000：行商浪人》的 ToyBox 提供中文词表和中英文显示说明。当前候选包适配 **ToyBox 2.0.20 预发布版**，本机游戏版本为 **1.6.1.514**。

## 选择安装包

| 安装包 | 版本 | 主要内容 | 安装位置 |
| --- | --- | --- | --- |
| [简体中文词表修订](dictionary-localization/README.md) | 2.1 候选版 | 覆盖 940 个字段，保留 216 项译文修订，包含新版特性等级设置的译文。 | 替换 `0ToyBox0/Localization/zh-CN_lang.json`。 |
| [显示增强](display-enhancements/README.md) | 0.5 候选版 | 中英文对话和蓝图显示增强，补齐选项检定、对话历史、物品与资金增减和殖民地特质预览。 | 独立 UMM 模组，与 `0ToyBox0` 并列。 |

关闭游戏后安装官方 [ToyBox 2.0.20 发布包](https://github.com/xADDBx/ToyBox-RogueTrader/releases/tag/v2.0.20)。两个附加包可独立安装，具体步骤见各自的 README。

## 显示示例

- 重置加点说明从 0、15、35 级开始时分别可重选哪些职业。
- `Open vendor selecting window` 显示为“结果预览：打开商人选择界面”。
- 招募条件说明“利润因子达到招募自定义队友所需的门槛”。
- 已登记的 `Heresy` 条件显示“剧情状态‘异端’未在运行”，不是“从未触发”。
- 任务目标附所属任务名，例如“返回着陆区域〔终末黎明之光〕”。
- 英文界面显示 `Result preview: Open vendor selection`、`Story state "Heresy" is not running`，信念名称使用 Dogmatic、Iconoclast、Heretical。

现有图片记录 ToyBox 1.7.34 的界面。

## 使用与版本

中文词表在 ToyBox“设置”页选择简体中文（`zh-CN`）后使用。显示增强支持 ToyBox 简体中文、英文（`en`）及英语区域代码。增强说明跟随 ToyBox，官方任务名、对白、信念与检定名称跟随游戏；两边都选英文时得到完整英文显示，也可混合设置。对话预览需在“百宝袋”（Bag of Tricks）的“预览”（Preview）分组开启“对话结果”（Dialog Results）和“对话条件”（Dialog Conditions）。

游戏随附 Unity Mod Manager（UMM）。显示增强可通过 Modfinder 拖入 ZIP 或手动解压安装，步骤见其 README，并提供 [English instructions](display-enhancements/README.en.md)。公开名称为 ToyBox Display Enhancements，安装目录与模组 ID 统一为 `ToyBoxDisplayEnhancements`。从旧标识版本升级时，将 `display-settings.json` 复制到新目录，再移除旧 `ToyBoxChineseDisplayEnhancements` 目录，避免重复加载。切换 ToyBox 或游戏语言时刷新名称与浏览器缓存；关闭增强或 ToyBox 使用不支持的语言时恢复原显示。

`release/` 下的候选包为 `toybox-zh-cn-v2.1.zip` 和 `toybox-display-enhancements-v0.5.zip`。ToyBox 2.0.16 对应词表 2.0 和显示增强 0.3／0.4，ToyBox 1.7.34 对应词表 1.0 和增强 0.1／0.2。

## 署名与许可

ToyBox：Narria、ADDB 及上游贡献者。原中文贡献者包括 ohZbs、Kirie、lhr7324925、icls1337、devilmandare。词表修订与显示增强：**mitcheese**。

采用 MIT 许可，见 [LICENSE.txt](LICENSE.txt)、[CREDITS.md](CREDITS.md)。源码与维护入口分别位于两个子项目；安装包不自动部署，网站上传由用户完成。

## 维护与验收入口

- 词表：[维护说明](dictionary-localization/docs/maintenance.md)、[术语表](dictionary-localization/docs/glossary.md)、[界面验收](dictionary-localization/docs/acceptance.md)。
- 显示增强：[维护说明](display-enhancements/docs/maintenance.md)、[核查依据](display-enhancements/docs/feature-evidence.md)、[界面验收](display-enhancements/docs/acceptance.md)。
- 发布：[词表文案](dictionary-localization/docs/nexus-description-zh.md)、[显示增强中文文案](display-enhancements/docs/nexus-description-zh.md)、[显示增强英文文案](display-enhancements/docs/nexus-description-en.md)、[上传说明](dictionary-localization/docs/publishing.md)。
