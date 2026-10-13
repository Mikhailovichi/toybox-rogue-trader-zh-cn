# Nexus 发布准备

本文件为上传者使用的准备说明，不放入公开安装包。

## 页面设置

- 游戏：Warhammer 40,000: Rogue Trader。
- 名称：`ToyBox Simplified Chinese Localization — Revised`。
- 汉化文件版本：`1.0`；兼容的 ToyBox 版本：`1.7.34`。
- 类型／标签：按汉化附加包发布，使用 `Translation` 标记。依据当前制作过程，同时使用 `AI-Generated Content`；页面文案如沿用本项目的 AI 生成草稿，添加 `AI Media`。
- 关联原模组并填写依赖：[ToyBox](https://www.nexusmods.com/warhammer40kroguetrader/mods/1)。
- 页面文本：`nexus-description-zh.md`、`nexus-description-en.md`。
- 当前候选 ZIP：`../release/toybox-zh-cn-v1.0.zip`。

## 上传前的实际待办

1. 完成 `acceptance.md` 中的游戏界面检查。根据实测更新 README、CREDITS、CHANGELOG 和两份发布文案中的候选状态，然后重新生成 ZIP。
2. 人工复核词表与发布文案，特别是功能说明、富文本显示及专名；文案已如实披露 AI 辅助翻译和校订。
3. 核实原作者在 Nexus 上对翻译的许可要求，并保留明确允许公开上传的依据。
4. 手动上传 ZIP，添加原模组依赖与 Translation 标记。网站上传由 mitcheese 完成。

## 许可信息的差异

对应源码随附 MIT 许可。2026-10-13 通过公开页面文本读取到，原模组 Nexus 的“修改许可”要求作者许可。直接访问页面返回 403，公开文本可能有缓存；上传时应核实当前页面设置。源码许可与页面说明存在差异，本项目保留 MIT 声明并制作本地汉化，不推定作者已批准发布到 Nexus。

原页面：https://www.nexusmods.com/warhammer40kroguetrader/mods/1

上游源码许可：https://github.com/xADDBx/ToyBox-RogueTrader/blob/v1.7.34/LICENSE.txt

Nexus 文件提交规则要求翻译作为依赖原模组的附加包，关联原文件，并按原内容许可发布。AI 条款明确把 AI 生成的翻译列入 `AI-Generated Content`，AI 生成的页面文案列入 `AI Media`。`AI Assisted` 另要求足够的人类主导开发证据，本次不以它替代前两项。文案已披露 AI 辅助制作，上传前应由本人完成质量复核。

规则：https://help.nexusmods.com/article/28-file-submission-guidelines

网站发布由仓库维护者执行。
