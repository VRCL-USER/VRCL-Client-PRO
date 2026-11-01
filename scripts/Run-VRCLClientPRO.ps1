$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\VRCL.ClientPRO\VRCL.ClientPRO.csproj"
$icon = Join-Path $repoRoot "src\VRCL.ClientPRO\Assets\vrcl-pro.ico"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK was not found. Install the .NET 10 SDK, then reopen PowerShell."
}

if (-not (Test-Path $icon)) {
    throw "Application icon not found: $icon"
}

dotnet run --project $project
if ($LASTEXITCODE -ne 0) {
    throw "VRCL PRO exited with code $LASTEXITCODE."
}
