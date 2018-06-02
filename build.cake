#addin "Cake.Incubator"
#addin "Cake.Docker"
#addin nuget:https://www.myget.org/F/alm-vr/api/v2?package=Cake.GitVersioning&prerelease

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
	version = GitVersioningGetVersion();

	Information($"Version number: \"{version.AssemblyInformationalVersion}\".");
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

	Information("Logging into Docker");
	DockerLogin(user, password);

	Information("Tagging Docker Image");
	DockerTag(dockerTag, $"https://hub.docker.com/r/ccrutchf/ucsd-research/{dockerTag}");

	Information("Pushing Docker Image");
	DockerPush($"https://hub.docker.com/r/ccrutchf/ucsd-research/{dockerTag}");
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Docker-Push-Server");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
