<#
.SYNOPSIS
    Builds KakaoTalkAdBlockPlus, runs all tests and (optionally) copies the exe to dist\.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File build.ps1
    powershell -ExecutionPolicy Bypass -File build.ps1 -Configuration Release -Publish
#>
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug',
    [switch] $NoTest,
    [switch] $Publish
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

function Find-VsTool([string[]] $vswhereArgs) {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (-not (Test-Path $vswhere)) {
        throw 'vswhere.exe not found. Install Visual Studio 2022+ (or Build Tools) with MSBuild.'
    }
    $found = & $vswhere -latest -prerelease -products * @vswhereArgs | Select-Object -First 1
    if (-not $found) { throw "Tool not found: $vswhereArgs" }
    return $found
}

$msbuild = Find-VsTool @('-requires', 'Microsoft.Component.MSBuild', '-find', 'MSBuild\**\Bin\MSBuild.exe')

& $msbuild (Join-Path $root 'KakaoTalkAdBlockPlus.sln') -restore -nologo -v:minimal "-p:Configuration=$Configuration"
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }

if (-not $NoTest) {
    $vstest = Find-VsTool @('-find', '**\TestPlatform\vstest.console.exe')
    $testDll = Join-Path $root "tests\KakaoTalkAdBlockPlus.Tests\bin\$Configuration\KakaoTalkAdBlockPlus.Tests.dll"
    & $vstest $testDll
    if ($LASTEXITCODE -ne 0) { throw "Tests failed (exit code $LASTEXITCODE)." }
}

if ($Publish) {
    $dist = Join-Path $root 'dist'
    New-Item -ItemType Directory -Force -Path $dist | Out-Null
    $exe = Join-Path $root "src\KakaoTalkAdBlockPlus\bin\$Configuration\KakaoTalkAdBlockPlus.exe"
    Copy-Item $exe $dist -Force
    Write-Host "Published: $(Join-Path $dist 'KakaoTalkAdBlockPlus.exe')"
}
