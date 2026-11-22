# 词表维护

以下命令在仓库根目录执行。词表 **2.1** 适配 ToyBox **2.0.20**，使用仓库内数据和 Python 标准库重建。

## 主文件

- `reference/upstream-en-v2.0.20.json`、`reference/upstream-zh-CN-v2.0.20.json` 保存官方发布包内 940 个字段的原文和译文。对照上游 ZIP 的 `Localization` 目录核对字段内容。
- `tools/revisions.json`：按字段 ID 保存 `Translated` 与修订原因，是修订主文件。
- `docs/translation-review.json`：全部字段的逐项审阅记录，包括保留、修订与空原文用途，进入 Git。
- `Localization/zh-CN_lang.json`：可安装的生成词表，仅含 `{字段ID: {Original, Translated}}`。
- `tools/build.py`、`tools/validate.py`、`tools/test_dictionary.py`、`tools/package.py`：重建、接口校验、聚焦测试及 ZIP 打包。
- `docs/glossary.md`、`docs/acceptance.md` 记录术语与游戏验收。构建工具生成本地静态检查报告 `docs/validation.json`。

退出旧的英文原文精确键提取与有限枚举扩展流程。旧工具、旧安装 JSON 和四份构建参考由 Git 历史保留，1.0 ZIP 留在根目录 `release/`，不并入新版包。

## 重建与打包

```powershell
python -X utf8 dictionary-localization/tools/build.py
python -X utf8 dictionary-localization/tools/test_dictionary.py
python -X utf8 dictionary-localization/tools/validate.py
python -X utf8 dictionary-localization/tools/package.py
```

输出 `release/toybox-zh-cn-v2.1.zip`。包内的词表、README、CHANGELOG、CREDITS、LICENSE 逐项与仓库文件核对。

校验拒绝重复键、字段丢失／新增、元数据混入、`Original` 改动、缺译、格式参数或 CR／LF 数量变化、错误富文本与重建不一致。四个空原文字段保持基线内容。

## 修订方式

按截图或字段 ID 找到基线，在 `tools/revisions.json` 修改译文和原因，同时更新对应审阅记录。不要修改 `Original`，不要用旧译文批量覆盖新版，也不根据相同英文断定功能语境相同。先看新版实际功能，再结合游戏术语。

游戏反馈只修显示与文案。安装由用户关闭游戏后完成，维护工具不部署、不修改存档、不提交或推送。依据和阶段状态记录在 `acceptance.md`；网站上传说明见 `publishing.md`。

只有官方中英文基线允许进入本子项目 `reference/` 的 Git 数据。上游源码、游戏词表、SDK、NuGet 缓存及本地资产记录存放在根目录已忽略的 `reference/`，生成 ZIP 与缓存均不进入 Git。
