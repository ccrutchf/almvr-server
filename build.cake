#addin "Cake.Incubator"
#addin "Cake.Docker"

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

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
	CleanDirectory(artifactsDir);
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
         Configuration = "Release",
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
	.IsDependentOn("Copy-Plugins")
	.Does(() =>
{
	var dockerImageBuildSettings = new DockerImageBuildSettings
	{
		Tag = new string[] { "almvr" }
	};

	DockerBuild(dockerImageBuildSettings, ".");
});

Task("Build")
	.IsDependentOn("Docker-Build-Server");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
