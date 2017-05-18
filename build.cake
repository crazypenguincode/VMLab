#tool "nuget:?package=GitVersion.CommandLine"

//Folder Variables
var RepoRootFolder = MakeAbsolute(Directory(".")); 
var ReleaseFolder = RepoRootFolder + "/Release";
var BuildFolder = RepoRootFolder + "/Build";
var SourceFolder = RepoRootFolder +"/Src";
var SolutionFile = SourceFolder + "/VMLab.sln";

var target = Argument("target", "Default");
var version = GitVersion(new GitVersionSettings{UpdateAssemblyInfo = true}); //This updates all AssemblyInfo files automatically.


Task("Default")
    .IsDependentOn("Package");

Task("Restore");

Task("Clean");

Task("Build");

Task("Test");

Task("Package");

Task("Deploy");

Task("Version")
    .Does(() => {
        Information("Assembly Version: " + version.AssemblySemVer);
        Information("SemVer: " + version.SemVer);
        Information("Branch: " + version.BranchName);
        Information("Commit Date: " + version.CommitDate);
        Information("Build Metadata: " + version.BuildMetaData);
        Information("PreReleaseLabel: " + version.PreReleaseLabel);
    });

/*****************************************************************************************************
End of script
*****************************************************************************************************/
RunTarget(target);