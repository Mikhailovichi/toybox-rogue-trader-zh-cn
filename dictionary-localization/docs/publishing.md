# 词表与显示增强的 Nexus 发布信息

这份说明用于填写上传页面。面向玩家的标题、简介、正文、下载说明和图片标题分别见：

- [词表汉化发布文案](nexus-description-zh.md)
- [显示增强中文发布文案](../../display-enhancements/docs/nexus-description-zh.md)
- [显示增强英文发布文案](../../display-enhancements/docs/nexus-description-en.md)

显示增强 0.5 的说明跟随 ToyBox，官方名称和对白跟随游戏语言。词表包提供简体中文菜单与设置说明。

## 分别创建两个页面

游戏选择 **Warhammer 40,000: Rogue Trader**，两个页面分别上传各自的安装包。

| 填写项 | 词表汉化 | 显示增强 |
| --- | --- | --- |
| 名称 | ToyBox 简体中文词表修订 - Simplified Chinese Localization | ToyBox Display Enhancements |
| 文件版本 | 2.1 候选版 | 0.5.0 候选版 |
| 下载包 | [toybox-zh-cn-v2.1.zip](../../release/toybox-zh-cn-v2.1.zip) | [toybox-display-enhancements-v0.5.zip](../../release/toybox-display-enhancements-v0.5.zip) |
| 原模组依赖 | ToyBox 2.0.20 预发布版 | ToyBox 2.0.20 预发布版及游戏随附的 UMM |
| 另一附加包 | 可选搭配 | 可选搭配 |

两个页面均填写原 [ToyBox](https://www.nexusmods.com/warhammer40kroguetrader/mods/1) 依赖，并在正文保留原模组链接。词表包按汉化附加包发布，使用页面提供的 `Translation` 标记；显示增强按独立显示附加模组介绍。

将各文案的“页面标题”和“简介”填入对应字段，“正文”一节用于详细描述。“下载文件信息”和“图片说明”分别用于文件与图片字段。正文可按页面编辑器提供的标题、加粗和列表工具排版。

## 两个页面互相推荐

两份正文已包含搭配说明。两个页面创建后：

1. 在词表页面，把显示增强的推荐链接指向 ToyBox Display Enhancements 的 Nexus 页面。
2. 在显示增强页面，把“ToyBox 简体中文词表修订”链接到词表的 Nexus 页面。
3. 将对方列为可选推荐，原 ToyBox 保持为必需依赖。

页面标题保留英文产品名称便于搜索。显示增强下载文件使用「0.5 候选版」，安装目录和模组 ID 均为 `ToyBoxDisplayEnhancements`。从旧标识版本升级时，将 `display-settings.json` 复制到新目录，再把旧 `ToyBoxChineseDisplayEnhancements` 目录移出 UMM。

## 图片与文件

当前两包已在 ToyBox **2.0.20 预发布版**和游戏 **1.6.1.514** 上完成本机样本验收。新版配图使用对应版本的设置、搜索或对话界面。

词表 2.0 与显示增强 0.3／0.4 对应 ToyBox 2.0.16。词表 1.0 与显示增强 0.1／0.2 对应 ToyBox 1.7.34。两个新版 ZIP 分别上传。

## 许可与制作标签

### 上游许可

上游源码随附 MIT 许可；本项目的安装包已保留许可与署名。此前保存的原 ToyBox 页面信息中，修改许可要求取得作者许可。2026 年 10 月 14 日直接读取原页面返回 403，本次未能核实它是否已经变化。

上传前在浏览器打开原页面的“Permissions and credits”，核对其当前要求；如仍要求作者许可，取得适用于对应附加包的许可后再发布。当前记录没有作者已授权 Nexus 发布的依据。

Nexus 的文件提交规则要求翻译依赖原模组、链接原页面，并遵守原内容的许可要求。

- [原 ToyBox 页面](https://www.nexusmods.com/warhammer40kroguetrader/mods/1)
- [对应版本的 MIT 许可](https://github.com/xADDBx/ToyBox-RogueTrader/blob/v2.0.20/LICENSE.txt)
- [Nexus 文件提交规则](https://help.nexusmods.com/article/28-file-submission-guidelines)

### AI 使用标签

按 2026 年 10 月 14 日读取到的 Nexus 规则：

- AI 生成的翻译和代码属于 `AI-Generated Content` 的标记范围。词表修订及显示增强应按实际生成内容设置该标签。
- 本次发布文案由 AI 协助撰写，沿用时设置 `AI Media`。该标签也适用于模组页面描述。
- `AI Assisted` 适用于开发者主导、AI 有限参与的工作，规则要求有相应开发记录和知识依据。根据实际制作过程选择，不能只因人工复核就将它作为前两项的替代。

旧配图来自玩家提供的实际游戏截图，仅做裁剪。新版图片尚待补充。制作标签通过页面字段填写；玩家正文保留使用说明和功能介绍。
