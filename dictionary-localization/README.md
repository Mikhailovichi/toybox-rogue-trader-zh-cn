# ToyBox 简体中文词表修订

为《战锤 40,000：行商浪人》ToyBox **2.0.16 预发布版**校订简体中文菜单与说明。汉化包 **2.0 候选版**审阅新版全部 938 个字段，修订 216 条，其中补齐 86 条缺译；四条空原文保留原用途。

## 版本与依赖

需要原版 [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) **2.0.16**。本机术语依据为游戏 **1.6.1.514**。可独立安装，也可搭配 **ToyBox 中文显示增强 0.3**；不包含 ToyBox 本体或游戏文件。

## 安装

1. 确认 ToyBox 为 2.0.16，然后关闭游戏。
2. 找到实际使用的 `0ToyBox0` 目录。Windows 默认位置：

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
   ```

3. 将 `Localization\zh-CN_lang.json` 复制到其他位置备份。
4. 将 `toybox-zh-cn-v2.0.zip` 解压到 **`0ToyBox0` 文件夹**，覆盖同名词表。最终路径：

   ```text
   0ToyBox0\Localization\zh-CN_lang.json
   ```

   从仓库安装时，只复制本子项目的 `Localization\zh-CN_lang.json` 到同一位置。
5. 启动游戏，在 ToyBox“设置”页选择简体中文（`zh-CN`）。游戏语言也选择简体中文后，游戏内容使用官方名称与描述。

新版读取的是 `zh-CN_lang.json`，不是旧版的 `zh-CN.json`。不要把词表包当作独立 UMM 模组安装。

## 主要内容

- 覆盖 938 个字段，补齐 86 条缺译，提供菜单、按钮和设置的简体中文说明。
- 统一职业、剧情状态、利润因子、导航者洞察和灵能术语，便于对照游戏内容。
- 完善蓝图补丁、搜索、存档与重置加点说明，写清操作用途及相关设置的影响。
- 区分视野角度与战争迷雾揭示范围，说明对话的出现条件、选择要求、信念要求与信念变化。

部分未接入新版词表的按钮、游戏生成摘要和内部标识仍可能为英文。对话语义与内部特性说明可搭配独立显示增强包。旧截图暂不展示，新版排版待用户验收。

## 更新与恢复

ToyBox 更新或重装后，先确认适配版本，再关闭游戏并重新覆盖词表。恢复原译文时，将备份的 `zh-CN_lang.json` 复制回 `Localization`，也可从官方 **2.0.16** 包提取原文件。

旧词表 **1.0** ZIP 保留供 **ToyBox 1.7.34** 使用，不能用作新版的恢复包。

## 署名与许可

ToyBox：Narria、ADDB 及上游贡献者。原中文贡献者包括 ohZbs、Kirie、lhr7324925、icls1337、devilmandare。词表修订：**mitcheese**。贡献与版本信息维护在文档中，不写入新版词表接口。

采用 MIT 许可，见 [LICENSE.txt](LICENSE.txt)、[CREDITS.md](CREDITS.md)。修订概要见 [CHANGELOG.md](CHANGELOG.md)。源码仓库的 `docs/` 中提供界面验收、英文范围及维护说明。
