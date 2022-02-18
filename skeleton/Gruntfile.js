module.exports = function (grunt) {
	const solutionPath = "Source/${{ values.component_id }}.sln";
	const appPath = "Source/${{ values.component_id }}/${{ values.component_id }}.csproj";
	const packageName = "${{ values.component_id }}";
	const coberturaReportFiles = grunt.file.expand({ filter: "isFile" }, ["Source/*.Tests/TestResults/coverage.cobertura.xml"]).join(';');
	const getBuildContainerParams = () => {
		return {
			options: { stdout: true },
			command: `powershell -NonInteractive -ExecutionPolicy Bypass Source\\Docker\\container-local.ps1 -operation "build" < NUL`
		}
	}

	const getPushContainerParams = () => {
		return {
			options: { stdout: true },
			command: `powershell -NonInteractive -ExecutionPolicy Bypass Source\\Docker\\container-local.ps1 -operation "push" < NUL`
		}
	}

	grunt.initConfig({
		pkg: grunt.file.readJSON("package.json"),
		cleanfiles: {
			basic: [
				"**/TestResults/**",
				"CoverageReport/**",
				["Source/**/bin/**/*.*", "!Source/**/bin/**/*vshost*"],  // sometimes VS opens and locks some vshost files, so we don't try to delete them
				"Source/**/obj/**",
				"Source/Docker/install/choco/**",
				"Source/Chocolatey/Artifacts/**",
				"Source/Chocolatey/Temp/**",
				"dist/**"
			],
			options: {
				folders: true
			}
		},
		shell: {
			test: {
				command: `dotnet test ${solutionPath} --logger:trx;LogFileName=TestResultVSTest.trx;verbosity=minimal /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/`
			},
			buildDebug: {
				command: `dotnet build ${solutionPath} --configuration Debug`
			},
			buildRelease: {
				command: `dotnet build ${solutionPath} --configuration Release`
			},
			containerBuild: {
				command:
					"powershell -ExecutionPolicy Bypass Source/Docker/container-local.ps1 build"
			},
			publishWindows: {
				command: `dotnet publish ${appPath} --configuration Release --output Source/${packageName}/bin/Release/PublishOutput/Windows/ -p:PublishSingleFile=false -p:PublishTrimmed=true --runtime win-x64 --self-contained=true`
			},
			publishLinux: {
				command: `dotnet publish ${appPath} --configuration Release --output Source/${packageName}/bin/Release/PublishOutput/Linux/ -p:PackageAsSingleFile=false -p:PublishTrimmed=true --runtime debian.10-x64`
			},
			chocoBuild: {
				options: { stdout: true },
				command:
					`powershell -ExecutionPolicy Bypass Source/Shared.Chocolatey/CreatePackage.ps1  -PackageName Elements.${packageName} -DeployType 'servicecore' < NUL`
			},
			buildContainer: getBuildContainerParams(),
			pushContainer: getPushContainerParams(),
			generateCoverageReport: {
				command:
					`reportgenerator.exe -reports:${coberturaReportFiles} -targetdir:CoverageReport`
			}
		},
		nucheck: {
			all: {
				src: solutionPath
			}
		},
		copy: {
			dist: {
				files: [
					{
						expand: true,
						cwd: "Source/Chocolatey/Artifacts",
						src: ["**"],
						dest: "dist/Chocolatey/"
					},
					{
						expand: true,
						cwd: "Source/Chocolatey/InstallationHelpers",
						src: ["**"],
						dest: "dist/Chocolatey/"
					},
					{
						expand: true,
						cwd: "Documentation",
						src: ["**"],
						dest: "dist/"
					},
					{
						expand: true,
						cwd: "Source/Docker",
						src: ["Dockerfile"],
						dest: "dist/"
					},
					{
						expand: true,
						cwd: `Source/${packageName}/bin/Release/PublishOutput/Linux`,
						src: ["**/*"],
						dest: "dist/PublishOutput/Linux"
					}
				]
			}
		}
	});

	// Every package defined in package.json, also have to be loaded here
	grunt.loadNpmTasks("grunt-nucheck");
	grunt.loadNpmTasks("grunt-shell");
	grunt.loadNpmTasks("grunt-contrib-copy");
	grunt.loadNpmTasks("grunt-contrib-clean");

	grunt.renameTask("clean", "cleanfiles"); //Rename so that task-name "clean" can be used as "main" clean

	// Default task(s).
	grunt.registerTask("clean", ["cleanfiles"]);
	grunt.registerTask("test", ["clean", "nucheck", "shell:test"]);
	grunt.registerTask("chocoBuild", ["shell:chocoBuild"]);
	grunt.registerTask("buildContainer", ["shell:buildContainer"]);
	grunt.registerTask("pushContainer", ["shell:pushContainer"]);
	grunt.registerTask("publish", ["shell:publishLinux", "shell:publishWindows", "chocoBuild"]);
	grunt.registerTask("default", ["clean", "test", "publish", "copy:dist"]);
	grunt.registerTask("coverage", ["test", "shell:generateCoverageReport"]);
};
