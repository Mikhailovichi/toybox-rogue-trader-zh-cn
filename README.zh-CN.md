# ToyBox 简体中文汉化修订

[English](README.md) | [简体中文](README.zh-CN.md)

《战锤 40,000：行商浪人》ToyBox 简体中文汉化修订。

## 兼容版本

- 汉化包版本：**1.0**
- 对应 ToyBox：**1.7.34**
- 当前状态：**待游戏界面验收**
- 类型：纯汉化词表附加包，需要先安装原版 ToyBox。

保留原词表全部 680 个键，修订 345 条原译文，新增 276 个键，共 956 条。新增键包括界面短语、有限枚举选项和富文本产生的精确匹配键，并不代表新增了 276 个独立功能。

## 主要改进

- 统一“职业”“剧情状态”“重置加点”“搜索与添加”等核心界面用语。
- 按游戏官方简体中文统一技能、信念、障壁恶化、合法灵能者、派系及虚空舰相关术语。
- 修正把《行商浪人》写成“正义之怒”、把社交属性译成“魅力”等问题。
- 补译灵能异象设置、蓝图补丁管理器、角色编辑器、联网与更新设置。
- 补充富文本精确匹配键，保留原键的大小写、空格、换行及格式参数。

## 安装

1. 先安装 [ToyBox for Rogue Trader](https://www.nexusmods.com/warhammer40kroguetrader/mods/1)，确认版本为 1.7.34，然后关闭游戏。
2. 找到现有 ToyBox 的模组目录。默认路径为：

   ```text
   %USERPROFILE%\AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager\0ToyBox0
   ```

3. 将该目录原有的 `Localization\zh-CN.json` 复制到自己指定的备份位置。
4. 将本项目的 `Localization\zh-CN.json` 覆盖到该目录的同名位置。使用 ZIP 安装包时，解压目标为 **`0ToyBox0` 模组目录**，不要再嵌套一层 `Localization`。
5. 启动游戏，在 ToyBox 的“设置”页选择简体中文（`zh-CN`）。若当前已为简体中文，重新启动后即可载入新词表。

本包不包含 ToyBox DLL，不适合作为独立模组导入 Unity Mod Manager。安装只需覆盖一个 JSON 文件。

## 恢复与更新

关闭游戏，将备份的 `zh-CN.json` 复制回原位置即可恢复旧汉化。未保留备份时，可从原版 ToyBox 1.7.34 安装包提取其 `Localization\zh-CN.json`。

ToyBox 自动更新或重新安装可能覆盖本汉化。更新后先确认汉化与新版本兼容，再重新覆盖词表。不要将本包的汉化版本 1.0 与 ToyBox 的版本 1.7.34 混淆。

## 保留英文的范围

ToyBox 中未调用翻译接口的文本仍可能显示英文，例如部分对话与角色页开关、升级说明、补丁编辑器的操作状态。纯词表汉化包不能改变这些写死的文本。

蓝图内部名称、字段名、类型名、GUID、路径、异常信息及第三方 Unity Mod Manager 界面不在本包修改范围内。ToyBox、Unity、GitHub 等品牌名，Ctrl、Shift、F11 等按键名，以及坐标轴 X/Y/Z 保留原样。游戏对象的名称和描述仍由游戏自身的语言设置决定。

## 署名与许可

ToyBox：Narria、ADDB，以及上游贡献者。上游 MIT 声明还保留 Hsinyu 署名。

原简体中文贡献者：ohZbs、Kirie、lhr7324925。汉化修订：mitcheese。

采用 MIT 许可，详见 [LICENSE.txt](LICENSE.txt) 和 [CREDITS.md](CREDITS.md)。
