$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot "src\VRCL.ClientPRO\VRCL.ClientPRO.csproj"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw ".NET SDK was not found. Install the .NET 10 SDK, then reopen PowerShell."
}

dotnet run --project $project
if ($LASTEXITCODE -ne 0) {
    throw "VRCL Client PRO exited with code $LASTEXITCODE."
}
