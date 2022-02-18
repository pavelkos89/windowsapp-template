$ErrorActionPreference = 'Stop'; # stop on all errors

if($packageName -eq $null) {
	$packageName= '${{ values.component_id }}'
}

if($deployType -eq $null) {
	$deployType = 'servicecore'
}

$serviceName = "Elements.${{ values.component_id }}" #Name of the service to (try to) start / stop when installing / uninstalling

$rootFolder = Join-Path $env:ProgramFiles $packageName

$projectBinPath = '${{ values.component_id }}\bin\Release\PublishOutput\Windows'
$nameOfExecutable = '${{ values.component_id }}.exe'

$overrideSettings = @{
	'ConfigServer.BaseUrl' = ''
	'ConfigServer.Password' = ''
	'ConfigServer.Scopes' = ''
	'ConfigServer.LocalOverride' = 'false'
}
