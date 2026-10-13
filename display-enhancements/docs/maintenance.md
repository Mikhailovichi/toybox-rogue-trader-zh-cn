# 显示增强附加模组维护

以下命令在仓库根目录的 PowerShell 中执行。此附加模组单独构建，不修改 `Localization/zh-CN.json` 或纯词表 1.0 ZIP。源码、缓存与安装包均在本仓库内；构建脚本不部署任何文件。

## 构建环境

需要 Windows、.NET 8 SDK、可运行测试的 .NET Framework 4.8.1，以及本机游戏 **1.6.1.514**、UMM 和已安装的 ToyBox **1.7.34**。通过 NuGet 的 `Microsoft.NETFramework.ReferenceAssemblies` 使用 net481 参考程序集，无需重建上游 ToyBox。

没有 SDK 时，将 SDK 安装到已被 Git 排除的项目目录：

```powershell
New-Item -ItemType Directory -Path reference -Force | Out-Null
Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile reference/dotnet-install.ps1
& ./reference/dotnet-install.ps1 -Version 8.0.425 -InstallDir "$PWD/reference/dotnet-sdk" -NoPath
```

构建、测试、核对本机程序集并生成 ZIP：

```powershell
& ./display-enhancements/build.ps1 `
  -GameDirectory 'D:\SteamLibrary\steamapps\common\Warhammer 40,000 Rogue Trader'
```

默认 UMM 路径为当前用户 `AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager`；非默认安装时用 `-ModManagerDirectory` 指定。可用 `-DotNetPath` 指定现有 SDK 的 `dotnet.exe`。

构建输出：`release/toybox-chinese-display-enhancements-v0.2.zip`，不覆盖原有 0.1 ZIP。只允许十一个安装文件进入包：附加模组 DLL、Info、特性与剧情状态两份中文对照 JSON、中文 `README.md`、许可、署名及 README 使用的四张截图。游戏／UMM／Harmony／测试程序集及用户设置不进入安装包。

## 代码入口

- `src/Main.cs`：UMM 入口、严格 ToyBox 版本检查、加载失败回退与开关。
- `src/Patches.cs`：11 个显示补丁；只替换预览中的格式化调用，不修改 `CollateAnswerData` 或条件分支执行。
- `src/Semantic.cs`：按动作／条件对象读取真实参数，递归读取 `OrAndLogic.ConditionsChecker` 与条件容器；未知类型使用游戏原 caption。
- `src/ConditionText.cs`：渲染读取后的条件结构，区分 AND／OR 与整体否定；单条件省略分组标题。
- `src/Text.cs`：中文格式与已核实的枚举状态。
- `src/DisplaySettings.cs`：技术详情偏好的加载与保存。默认关闭；UMM 面板切换时保存到安装目录的 `display-settings.json`，升级安装包不覆盖此文件。
- `src/Aliases.cs`：GUID 与内部名双重匹配，官方文本优先，行内附加描述。
- `feature-aliases.zh-CN.json`：八个条目的显示主文件；更改前同步 [核查依据](feature-evidence.md)。
- `src/StoryAliases.cs`、`story-aliases.zh-CN.json`：独立维护剧情状态别名，按 GUID 与内部名匹配。目前仅登记 `Heresy`，不从英文名称批量推测译名。
- `tests`：纯文本与真实 Harmony 的托管夹具。伪 ToyBox 仅用于测试，不发布。
- `tools/Inspect`：dnlib 只读元数据检查与指定方法 IL 查看；不加载游戏代码执行。

## 聚焦验证

构建脚本依次验证格式化、Harmony 补丁安装和回退、本机版本／入口／枚举、ZIP 白名单与文件内容。

测试尤其保留空条件的空字符串：ToyBox 的一个对话可选性分支会检查摘要长度，不能把空条件改成非空提示。对话动作格式化不调用 `Check` 或 `RunAction`；原 ToyBox 的 `Conditional` 路径与求值次数保持原样。

0.2 聚焦验证单条件、AND／OR 嵌套及组合否定，嵌套空组、循环与未知类型的原摘要回退；“未在运行”不得变成“从未触发”。对白只读关联蓝图的 `Text`，清除富文本并截取 40 个字符；招募条件只描述利润因子门槛，不调用 `GetCustomCompanionCost()`。技术详情模式沿用同一遍历结果，设置保存测试仅写入已忽略的测试输出目录。

泛型列表行补丁定位 `IUIDataProvider.get_Description` 的单个调用，不全局修改 getter。托管测试通过反射调用闭合泛型方法，实际游戏的 Mono 共享泛型体与菜单显示仍由 [界面验收](acceptance.md) 确认。

测试目录通过 NuGet 使用签名有效的 Newtonsoft.Json，因为游戏随附版本无法在 Windows CLR 测试进程中通过强名称验证。此依赖仅用于测试输出；附加模组编译和运行仍引用游戏版本，安装包不带替代 DLL。

## 版本与发布

0.2 的 UMM／程序集版本为 `0.2.0`／`0.2.0.0`。0.1 ZIP 保留用于回退，不同时安装两份同 ID 模组。ToyBox 更新后先核对显示方法和数据结构，再调整兼容检查与测试，不直接取消版本限制。

SDK、NuGet 缓存在 `reference` 下；所有 `bin`／`obj` 和 ZIP 均被 Git 排除。不提交游戏资产或程序集，不自动安装、不提交或推送 Git。游戏反馈与验证结果维护于 `acceptance.md`。
