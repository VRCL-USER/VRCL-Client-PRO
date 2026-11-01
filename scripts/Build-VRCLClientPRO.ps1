$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\VRCL.ClientPRO\VRCL.ClientPRO.csproj"
$output = Join-Path $repoRoot "dist\win-x64"
$icon = Join-Path $repoRoot "src\VRCL.ClientPRO\Assets\vrcl-pro.ico"
$versionFile = Join-Path $repoRoot "VERSION"

Write-Host ""
Write-Host "VRCL PRO $(Get-Content $versionFile -Raw | ForEach-Object { $_.Trim() }) - Windows build" -ForegroundColor Magenta
Write-Host "--------------------------------" -ForegroundColor DarkGray

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK was not found. Install the .NET 10 SDK, then reopen PowerShell."
}

$sdks = dotnet --list-sdks
if (-not ($sdks | Select-String '^10\.')) {
    throw ".NET 10 SDK is required. Installed SDKs: $($sdks -join ', ')"
}

if (-not (Test-Path $project)) {
    throw "Project file not found: $project"
}

if (-not (Test-Path $icon)) {
    throw "Application icon not found: $icon"
}

New-Item -ItemType Directory -Force -Path $output | Out-Null

dotnet publish $project --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true --output $output

if ($LASTEXITCODE -ne 0) {
    throw "The desktop build failed with exit code $LASTEXITCODE."
}

Write-Host ""
Write-Host "Build complete." -ForegroundColor Green
Write-Host "Version: $(Get-Content $versionFile -Raw | ForEach-Object { $_.Trim() })" -ForegroundColor Cyan
Write-Host "Output: $output" -ForegroundColor Cyan
Write-Host "Executable: $(Join-Path $output 'VRCL PRO.exe')" -ForegroundColor Cyan
