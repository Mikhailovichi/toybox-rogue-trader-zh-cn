param(
    [Parameter(Mandatory = $true)][string]$GameDirectory,
    [string]$ModManagerDirectory = (Join-Path $env:USERPROFILE 'AppData\LocalLow\Owlcat Games\Warhammer 40000 Rogue Trader\UnityModManager'),
    [string]$DotNetPath
)
$ErrorActionPreference = 'Stop'
$project = $PSScriptRoot
$repo = Split-Path $project -Parent
if (!$DotNetPath) { $DotNetPath = Join-Path $repo 'reference\dotnet-sdk\dotnet.exe' }
if (!(Test-Path -LiteralPath $DotNetPath)) { throw 'Provide -DotNetPath or install the private SDK as described in docs/maintenance.md.' }
$old = @{}
foreach ($key in @('GameManagedDir', 'ModManagerDir', 'DOTNET_CLI_HOME', 'DOTNET_CLI_TELEMETRY_OPTOUT', 'NUGET_PACKAGES')) {
    $old[$key] = [Environment]::GetEnvironmentVariable($key, 'Process')
}
try {
    # Environment variables avoid MSBuild interpreting the comma in "40,000" as a property separator.
    $env:GameManagedDir = Join-Path (Resolve-Path -LiteralPath $GameDirectory).Path 'WH40KRT_Data\Managed'
    $env:ModManagerDir = (Resolve-Path -LiteralPath $ModManagerDirectory).Path
    $env:DOTNET_CLI_HOME = Join-Path $repo 'reference\dotnet-home'
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $env:NUGET_PACKAGES = Join-Path $repo 'reference\nuget-packages'
    $version = Get-Content -LiteralPath (Join-Path $GameDirectory 'WH40KRT_Data\StreamingAssets\Version.info') -Raw
    if ($version -notmatch '\b1\.6\.1\.514\b') { throw 'This candidate build is checked against game 1.6.1.514.' }
    & $DotNetPath build (Join-Path $project 'tests\Tests.csproj') -c Release --nologo
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    & (Join-Path $project 'tests\bin\Release\net481\ToyBoxChineseDisplayEnhancements.Tests.exe') $project
    if ($LASTEXITCODE -ne 0) { throw 'Formatting/Harmony fixture tests failed.' }
    & $DotNetPath run --project (Join-Path $project 'tools\Inspect\Inspect.csproj') -c Release -- --verify $env:GameManagedDir $env:ModManagerDir
    if ($LASTEXITCODE -ne 0) { throw 'Actual-assembly compatibility checks failed.' }

    $info = Get-Content -LiteralPath (Join-Path $project 'Info.json') -Raw | ConvertFrom-Json
    $dll = Join-Path $project 'bin\Release\net481\ToyBoxChineseDisplayEnhancements.dll'
    if ($info.Id -ne 'ToyBoxChineseDisplayEnhancements' -or $info.Version -ne '0.2.0' -or
        $info.Requirements.Count -ne 1 -or $info.Requirements[0] -ne '0ToyBox0' -or
        [Reflection.AssemblyName]::GetAssemblyName($dll).Version -ne [version]'0.2.0.0') { throw 'Package metadata does not match the addon.' }
    $files = [ordered]@{
        'ToyBoxChineseDisplayEnhancements.dll' = $dll
        'Info.json' = Join-Path $project 'Info.json'
        'feature-aliases.zh-CN.json' = Join-Path $project 'feature-aliases.zh-CN.json'
        'story-aliases.zh-CN.json' = Join-Path $project 'story-aliases.zh-CN.json'
        'README.md' = Join-Path $project 'README.md'
        'README.zh-CN.md' = Join-Path $project 'README.zh-CN.md'
        'LICENSE.txt' = Join-Path $project 'LICENSE.txt'
        'CREDITS.md' = Join-Path $project 'CREDITS.md'
    }
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $release = Join-Path $repo 'release'
    New-Item -ItemType Directory -Path $release -Force | Out-Null
    $zipPath = Join-Path $release 'toybox-chinese-display-enhancements-v0.2.zip'
    $stream = [IO.File]::Open($zipPath, [IO.FileMode]::Create)
    $zip = [IO.Compression.ZipArchive]::new($stream, [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($name in $files.Keys) {
            [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $files[$name], "$($info.Id)/$name") | Out-Null
        }
    } finally { $zip.Dispose(); $stream.Dispose() }
    $zip = [IO.Compression.ZipFile]::OpenRead($zipPath)
    try {
        if ($zip.Entries.Count -ne $files.Count) { throw 'Unexpected package entries.' }
        foreach ($name in $files.Keys) {
            $entry = $zip.GetEntry("$($info.Id)/$name")
            if (!$entry) { throw "Missing package entry: $name" }
            $buffer = [IO.MemoryStream]::new()
            $input = $entry.Open()
            try { $input.CopyTo($buffer) } finally { $input.Dispose() }
            try {
                if ([Convert]::ToBase64String($buffer.ToArray()) -cne [Convert]::ToBase64String([IO.File]::ReadAllBytes($files[$name]))) {
                    throw "Package content mismatch: $name"
                }
            } finally { $buffer.Dispose() }
        }
    } finally { $zip.Dispose() }
    Write-Output "PASS: eight-file addon package verified: $zipPath"
} finally {
    foreach ($key in $old.Keys) { [Environment]::SetEnvironmentVariable($key, $old[$key], 'Process') }
}
