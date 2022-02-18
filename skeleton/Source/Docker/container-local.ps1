param(
	[Parameter(Mandatory=$true)]
	[ValidateSet('build','push')]
	[String]$operation
)

$ErrorActionPreference = 'Stop'; # stop on all errors

$imageName = '${{ values.component_id | lower }}';
$installSourceFolder = "dist\PublishOutput\Linux\*"
$initFiles = "init.sh,300-UpdateAppsettings.sh,901-ConfigserverListener.sh"
$dockerFileFolder = "Source\Docker"

. Source\Docker.ContainerHelpers\helpers.ps1
$registry, $registryUsername, $namespace, $imageTag, $initSourceFolder, $registryPassword = GetDefaultParameters

$initSourceFolder = "Source\Docker.ContainerHelpers\content\init"

Source\Docker.ContainerHelpers\container.ps1 -operation $operation `
											-registry $registry `
											-registryUsername $registryUsername `
											-registryPassword $registryPassword `
											-namespace $namespace `
											-imageName $imageName `
											-imageTag $imageTag `
											-installSourceFolder $installSourceFolder `
											-initSourceFolder $initSourceFolder `
											-dockerFileFolder $dockerFileFolder `
											-initFiles $initFiles
