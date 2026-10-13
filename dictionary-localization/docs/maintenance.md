# 维护入口

文件清单中的路径相对于 `dictionary-localization/`，命令在仓库根目录执行。常规构建不需要游戏安装文件或上游源码；本地安装备份、游戏词表和上游源码不进入 Git。显示增强独立维护于 `display-enhancements/`。

## 文件

- `Localization/zh-CN.json`：可安装、可发布的完整汉化主文件。
- `tools/build.py`：对原 680 条的校订表；编号对应不可改动的基线条目顺序。
- `tools/additions.json`：新增普通文本与有限显示名称的校订表。
- `tools/extract.py`：通过 C# AST 解析转义、拼接和富文本，生成精确键。
- `tools/finite.py`：补充显示用枚举、主标签页、快捷键标题、骨骼名称和稀有度键。
- `tools/validate.py`：接口、精确键覆盖、占位符、换行、富文本和英文保留检查。
- `tools/test_extract.py`：提取器与格式验证的针对性回归测试。
- `reference/baseline-zh-CN.json`：原词表基线；保留原始内容和条目顺序，不在此文件修订翻译。
- `reference/extracted-keys.json`、`reference/finite-keys.json`：已提取的精确键参考，供常规构建和校验使用。
- `reference/game-enums.json`：从游戏 1.6.1.514 程序集元数据提取的有限枚举参考；仅记录名称，不含程序集。
- `docs/translation-review.json`：构建时生成的逐条修订对照，不进入 Git。
- `docs/glossary.md`：术语依据与备选建议。
- `docs/validation.json`：校验时生成的检查结果，不进入 Git。
- `docs/acceptance.md`：验收状态与游戏检查项目。
- `docs/publishing.md`：上传时的依赖、Translation 和 AI 标签、许可待核实事项。

`.gitignore` 只允许本子项目上述四份 `reference` JSON 进入 Git；其余参考资料、SDK、快照、安装备份和上游源码保留在仓库根目录的 `reference/`，不进入 Git。根目录 `release/`、依赖目录、生成报告和个人安装记录也不进入 Git。

## 构建与检查

使用 Python 3.12，在仓库根目录执行。构建、校验和打包仅使用标准库；提取器及其测试需要 `dictionary-localization/tools/requirements.txt` 中固定版本的两个解析依赖：

```powershell
python -m venv .venv
.\.venv\Scripts\python.exe -m pip install -r dictionary-localization/tools/requirements.txt
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/build.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/test_extract.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/validate.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/package.py
```

打包脚本会先校验，再创建 `release/toybox-zh-cn-v1.0.zip`。生成的 ZIP 用作 GitHub Release 附件或 Nexus 下载文件，不提交到 Git 历史。

## 可选：重新提取源码键

仅在需要复核提取结果时执行。源码固定为 `v1.7.34`，提交 `f044d0ccc90c4800aef0142fdd5147abaf7f5a4e`。若本地尚无 `reference/source`：

```powershell
git clone --branch v1.7.34 --depth 1 https://github.com/xADDBx/ToyBox-RogueTrader.git reference/source
git -C reference/source rev-parse HEAD
```

确认提交与上述基线一致，再执行：

```powershell
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/extract.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/finite.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/build.py
.\.venv\Scripts\python.exe -X utf8 dictionary-localization/tools/validate.py
```

不要用上游主分支的键覆盖当前兼容版本。参考源码读取仓库根目录 `reference/source`，四份构建参考 JSON 则位于 `dictionary-localization/reference/`。`finite.py` 使用已保存的 `game-enums.json`，不会自动读取游戏程序集。缺少参考源码时，提取脚本会明确报错，并保留已有键文件。

## 修订译文

主 JSON 可直接编辑供临时试译；要保证重建后保留修订，应同步修改 `build.py` 或 `additions.json`。英文键不可用中文术语替换，也不可自动修剪空白。

## 游戏反馈的处理

确认游戏关闭，依据截图／词表导出定位实际键，调整译文并运行构建与静态检查。若是未调用翻译接口的写死英文，将具体位置记入 `english-boundaries.md`，保持本次纯词表范围。随后覆盖用户数据目录中的汉化文件、再次验收并重新打包。

现有包版本为 1.0，游戏界面验收仍待完成。验收结果维护在 `docs/acceptance.md`，公开发布前的待办见 `docs/publishing.md`。
