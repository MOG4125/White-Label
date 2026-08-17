$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$msbuild = Get-Command msbuild.exe -ErrorAction SilentlyContinue
if (-not $msbuild) { throw 'MSBuild was not found. Install Visual Studio Build Tools with .NET Framework 4.6.2 targeting pack.' }
& $msbuild.Source (Join-Path $root 'WhiteLabelLauncher.csproj') /p:Configuration=Release /m
$out = Join-Path $root 'bin\Release'
$bundle = Join-Path $root 'WhiteLabelLauncher-bundle'
Remove-Item $bundle -Recurse -Force -ErrorAction SilentlyContinue
New-Item $bundle -ItemType Directory | Out-Null
Copy-Item (Join-Path $out 'WhiteLabelLauncher.exe') $bundle
Copy-Item (Join-Path $root 'WhiteLabelLauncher.ini') $bundle
Compress-Archive -Path (Join-Path $bundle '*') -DestinationPath (Join-Path $root 'WhiteLabelLauncher-bundle.zip') -Force
Write-Host "Built: $out\WhiteLabelLauncher.exe"
Write-Host 'Copy WhiteLabelLauncher.exe and WhiteLabelLauncher.ini beside Playnite.exe, then run WhiteLabelLauncher.exe.'
