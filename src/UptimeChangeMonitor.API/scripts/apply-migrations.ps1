# Script PowerShell para aplicar migrations localmente

Write-Host "Applying database migrations..." -ForegroundColor Green

$projectPath = Join-Path $PSScriptRoot "..\UptimeChangeMonitor.API"
$infrastructurePath = Join-Path $PSScriptRoot "..\..\UptimeChangeMonitor.Infrastructure"

Set-Location $projectPath

dotnet ef database update --project $infrastructurePath --startup-project .

Write-Host "Migrations applied successfully!" -ForegroundColor Green
