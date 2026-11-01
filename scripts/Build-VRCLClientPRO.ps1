$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\VRCL.ClientPRO\VRCL.ClientPRO.csproj"
$output = Join-Path $repoRoot "dist\win-x64"

Write-Host ""
Write-Host "VRCL Client PRO - Windows build" -ForegroundColor Magenta
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

New-Item -ItemType Directory -Force -Path $output | Out-Null

dotnet publish $project --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true --output $output

if ($LASTEXITCODE -ne 0) {
    throw "The desktop build failed with exit code $LASTEXITCODE."
}

Write-Host ""
Write-Host "Build complete." -ForegroundColor Green
Write-Host "Output: $output" -ForegroundColor Cyan
Write-Host "Executable: $(Join-Path $output 'VRCL Client PRO.exe')" -ForegroundColor Cyan
