/////////////////////////////////////////////////////////////////
//						ARGUMENTS							   //
////////////////////////////////////////////////////////////////
var target = Argument<string>("Target", "Default");
var configuration = Argument<string>("Configuration", "Release");
var outputDirectory = Argument<string>("OutputDirectory", "artifacts");

//Project
var solution = Argument<string>("Solution", "Frankenstein.sln");

// Versioning
var major = Argument<int>("Major", 1);
var minor = Argument<int>("Minor", 0);
var patch = Argument<int>("Patch", 0);
var runNumber = Argument<int>("RunNumber", 0);

var paths = new
{
	Src = "./src",
	BuildPropsFile = "./Directory.Build.props",
	Artifacts = $"./{outputDirectory}"
};

var versioning = new 
{
	AssemblyVersion = $"{major}.{minor}.{patch}.{runNumber}",
	InformationalVersion = $"{major}.{minor}.{patch}.{runNumber}+{EnvironmentVariable<string>("Build.SourceVersion",$"{Guid.NewGuid()}")}",
	FileVersion = $"{major}.{minor}.{patch}.{runNumber}",
	PackageVersion = EnvironmentVariable<string>("Build.SourceBranch","") == "ref/heads/master" ? $"{major}.{minor}.{patch}" : $"{major}.{minor}.{patch}.{runNumber}"
};

/////////////////////////////////////////////////////////////////
//						TASKS							  	   //
////////////////////////////////////////////////////////////////

Information($"Running target {target} in configuration {configuration}");
// Deletes the contents of the artifacts folder if it contains anything from a previous build.
Task("Clean")
    .Does(() =>
    {
		if(DirectoryExists(paths.Artifacts))
		{
			DeleteDirectory(paths.Artifacts, new DeleteDirectorySettings {
				Recursive = true,
				Force = true
			});
		}

        DotNetCoreClean(solution);
    });

// Run dotnet restore to restore all package references.
Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

// Versions files and assemblies (excl nuget package version)
Task("Version")
	.Does(() => {
		Information($"Versioning files");
		Information($"Version: {versioning.AssemblyVersion}");
		Information($"Informational version: {versioning.InformationalVersion}");
		Information($"Assembly version: {versioning.AssemblyVersion}");
		Information($"File version: {versioning.FileVersion}");

		XmlPoke(paths.BuildPropsFile, "//Version", versioning.AssemblyVersion);
		XmlPoke(paths.BuildPropsFile, "//InformationalVersion", versioning.InformationalVersion);
		XmlPoke(paths.BuildPropsFile, "//AssemblyVersion", versioning.AssemblyVersion);
		XmlPoke(paths.BuildPropsFile, "//FileVersion", versioning.FileVersion);
	});

// Build using the build configuration specified as an argument.
 Task("Build")
    .Does(() =>
    {
        DotNetCoreBuild(solution, new DotNetCoreBuildSettings()
        {
			Configuration = configuration,
			NoRestore = true
        });
    });

// Pack relevant projects into Nuget packages and output to artifacts/packages directory
Task("Pack")
    .Does(() =>
    {
		DotNetCorePack(solution, new DotNetCorePackSettings
        {
            Configuration = configuration,
            NoRestore = true,
            IncludeSymbols = true,
            OutputDirectory = Directory(paths.Artifacts + "/packages"),
            MSBuildSettings = new DotNetCoreMSBuildSettings()
                .WithProperty("PackageVersion", versioning.PackageVersion)
        });
    });

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
	.IsDependentOn("Version")
    .IsDependentOn("Build")
	.IsDependentOn("Pack");

// Executes the task specified in the target argument.
RunTarget(target);