var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var packPath            = Directory("./src/unleash");
var buildArtifacts      = Directory("./artifacts/packages");

var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;
var version             = "1.0.1";

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration,
        ArgumentCustomization = args => args.Append("/p:SemVer=" + version)
    };

    // libraries
	var projects = GetFiles("./src/**/*.csproj");
	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }

    // tests
	projects = GetFiles("./test/**/*.csproj");
	foreach(var project in projects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration
    };

    var projects = GetFiles("./test/**/*.csproj");

    foreach(var project in projects)
	{
        DotNetCoreTest(project.FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Pack
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        ArgumentCustomization = args => args.Append("--include-symbols").Append(" /p:SemVer=" + version)
    };

    DotNetCorePack(packPath, settings);
});


Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

RunTarget(target);