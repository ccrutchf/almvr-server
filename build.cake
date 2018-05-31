#tool nuget:?package=Nerdbank.GitVersioning&version=2.1.23

#addin "Cake.Incubator"
#addin "Cake.Docker"
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./build") + Directory(configuration);
var artifactsDir = Directory("./artifacts");

var slnFile = File("./src/AlmVR.Server.sln");

dynamic version;
string dockerTag;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(artifactsDir);
});

Task("Git-Versioning")
	.Does(() =>
{
	version = StartPowershellFile("./tools/Nerdbank.GitVersioning.2.1.23/tools/Get-Version.ps1")[1].BaseObject;

	Information($"Version number: \"{version.AssemblyInformationalVersion}\".");

	var script = @"
if (Get-Command ""Update-AppveyorBuild"" -errorAction SilentlyContinue)
{{
    Update-AppveyorBuild -Version {0}
}}";

	StartPowershellScript(string.Format(script, version.AssemblyInformationalVersion));
});

Task("Build-Server")
    .IsDependentOn("Clean")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings
	{
		Configuration = configuration
	};
	
	DotNetCoreBuild(slnFile, dotNetCoreSettings);
});

Task("Publish-Server")
	.IsDependentOn("Build-Server")
	.Does(() =>
{
     var settings = new DotNetCorePublishSettings
     {
         Configuration = configuration,
         OutputDirectory = artifactsDir
     };

     DotNetCorePublish(slnFile, settings);
});

Task("Copy-Plugins")
	.IsDependentOn("Publish-Server")
	.Does(() =>
{
	var artifactsPluginsDir = artifactsDir + Directory("Plugins");

	CreateDirectory(artifactsPluginsDir);
	CopyFiles($"./build/{configuration}/Plugins/*", artifactsPluginsDir);
});

Task("Docker-Build-Server")
	.IsDependentOn("Git-Versioning")
	.IsDependentOn("Copy-Plugins")
	.Does(() =>
{
	dockerTag = $"almvr:{version.AssemblyFileVersion}";

	Information($"Docker image tag: \"{dockerTag}\".");

	var dockerImageBuildSettings = new DockerImageBuildSettings
	{
		Tag = new string[] { dockerTag }
	};

	DockerBuild(dockerImageBuildSettings, ".");
});

Task("Docker-Push-Server")
	.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("DOCKER_USER")))
	.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("DOCKER_PASSWORD")))
	.IsDependentOn("Git-Versioning")
	.IsDependentOn("Docker-Build-Server")
	.Does(() =>
{
	var user = EnvironmentVariable("DOCKER_USER");
	var password = EnvironmentVariable("DOCKER_PASSWORD");

	DockerLogin(user, password);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Docker-Build-Server");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
