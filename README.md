# AlmVR (Server)
[![Build status](https://ci.appveyor.com/api/projects/status/9s2ln2oi41tpxhcl/branch/master?svg=true)](https://ci.appveyor.com/project/ccrutchf/almvr-server/branch/master)

A virtual reality (VR) application life cycle (ALM) management utility written for the Google Daydream and targeting Trello.

## Demo
[![AlmVR Demo](http://img.youtube.com/vi/dSCv77CD3rA/0.jpg)](http://www.youtube.com/watch?v=dSCv77CD3rA)

## What?
This repository contains the server component of AlmVR. The server is responsible for managing the connection to the ALM products that AlmVR uses as its backend (ie Trello).

## How?
The server component is written in `C#` using `.NET Core`, `ASP.NET Core`, and `SignalR`. The server is written with a plugin architecture that allows the ALM platforms (i.e. Trello) to be replaced without having to modify any of the clients.

### Plugins
Plugins are a `.NET Standard` or `.NET Core` assembly that contain a class which implements `AlmVR.Server.Core.IPlugin`. The implementations of `IPlugin` then register their own implementations of the core providers, which reside in the `AlmVR.Server.Core.Providers` namespace, with `Autofac`. An example plugin for Trello can be found in the `AlmVR.Server.Providers.Trello` project.

Plugins are located by looking for a `Plugins` directory in the same directory as the executable for the server.  An implementation of this strategy can be seen below.

```csharp
private static IEnumerable<Assembly> GetAssemblies()
{
    var exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    var pluginLocation = Path.Combine(exeLocation, "Plugins");

    return Directory.GetFiles(pluginLocation, "*.dll", SearchOption.AllDirectories)
      .Select(path => AssemblyLoadContext.Default.LoadFromAssemblyPath(path))
      .Where(x => x != null)
      .Select(x => x.GetTypes())
      .SelectMany(x => x)
      .Where(x => x.GetInterfaces().Contains(typeof(IPlugin)))
      .Select(x => x.Assembly)
      .Distinct()
      .ToArray();
}
```

### Communication with the client
The server uses web sockets implemented by `SignalR` for real time communication with each client. This allows the server to be able to push updates to the client without the clients polling the server for updates, thus increasing efficiency. The hubs for `SignalR` reside in the `AlmVR.Server.Hubs` project.

### Communication with Trello
The Trello plugin requests data from Trello using its `REST API`. It also sets up a `ASP.NET Core MVC` controller to receive responses from Trello using webhooks. Webhooks are registered with Trello upon the first request of a Trello card. In order for this to function correctly, Trello must be able to hit the controller with a `HEAD` request. This requires the server to be public facing. When Trello calls a webhook, the server responds by raising an event on the clients using `SignalR`.

### Hosting
The build for the server generates a `Docker` container and pushes to a public facing repository.

### Builds
The server (as well as the remainder of AlmVR) is built using `Cake Build` run in `AppVeyor`. The build executes the following on every commit that is pushed to GitHub:
1. A clean of all of the build files.
2. Generate the version number using [Nerdbank.GitVersioning](https://github.com/AArnott/Nerdbank.GitVersioning).
3. Build the server using the dotnet CLI.
4. Publish the server to the local file system using the dotnet CLI to prepare it to be packaged for consumption within a Docker container.
5. Copies the plugins from step 4 into the published server directory.
6. Builds a Docker container based on Alpine Linux.
7. Pushes the Docker container to a public repository for consumption by the virtual private server (VPS) that hosts the container.
8. Auto deploys to a VPS.

## Why?
The choice to create a `Client-Server` architecture instead of a `Peer-to-Peer` architecture was made in order to ensure that the plugins only had to be installed and configured in a single location.

Another motivation for the Client-Server architecture is to support Trello's webhooks, since they need to be a web service that responds to `HEAD` and `PUT`.

The choice to implement a plugin architecture was made so that the project could easily be adopted to use other ALM platforms (ie Azure DevOps, Jira, etc).

The continuous integration build script was put together to facilitate deployments of the server binaries without connecting to the VPS manually.

## Struggles
* Beta software - During development, we started with beta versions of `.NET Core`, `ASP.NET Core`, `SignalR`. These dependencies saw their APIs change during development.
* Public facing server for Trello webhooks - It was necessary for Trello to be able to hit our server, so it needed to be public facing. This prevented us from being able run/deploy the server from the lab necessitating significant build efforts.
* Initially `AppVeyor` did not support Linux builds, so it was not possible to build the project and deploy to our VPS using `AppVeyor`. We ended up setting up `TravisCI` and then migrating back `AppVeyor`.

## How to build
The following software is required to build this project:
* Visual Studio 2017
* .NET Core 2.1 SDK
* PowerShell 5.1/PowerShell 6 Core/Bash (minimum one)
* Docker

Execute the `build.ps1` or `build.sh` file found in the root of the repository.  This builds the `Docker` container which can then be run.
