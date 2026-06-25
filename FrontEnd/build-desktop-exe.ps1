$ErrorActionPreference = "Stop"

Write-Host "1) Building frontend..."
npm run build

$distPath = Join-Path $PSScriptRoot "dist"
$desktopProject = Join-Path $PSScriptRoot "StarAirDesktop"
$zipPath = Join-Path $desktopProject "web-assets.zip"

if (!(Test-Path $distPath)) {
    throw "dist folder was not generated."
}

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Write-Host "2) Packing frontend assets into web-assets.zip..."
Compress-Archive -Path (Join-Path $distPath "*") -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host "3) Publishing single-file exe..."
dotnet publish "$desktopProject\StarAirDesktop.csproj" -c Release /p:DebugType=None /p:DebugSymbols=false

Write-Host ""
$publishDir = Join-Path $desktopProject "bin\Release\net9.0-windows\win-x64\publish"
$releaseDir = Join-Path $PSScriptRoot "Desktop-Release"
$finalExe = Join-Path $releaseDir "STAR-Air-ADM.exe"

if (!(Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir | Out-Null
}
try {
    Copy-Item (Join-Path $publishDir "StarAirDesktop.exe") $finalExe -Force
} catch {
    $timestampExe = Join-Path $releaseDir ("STAR-Air-ADM-" + (Get-Date -Format "yyyyMMdd-HHmmss") + ".exe")
    Copy-Item (Join-Path $publishDir "StarAirDesktop.exe") $timestampExe -Force
    $finalExe = $timestampExe
}

Write-Host "Done."
Write-Host "Single EXE: $finalExe"
