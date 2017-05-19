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

Task("Restore")
    .IsDependentOn("Folder.Restore")
    .IsDependentOn("Nuget.Restore")
    .IsDependentOn("VMLab.Restore");

Task("Clean")
    .IsDependentOn("VMLab.Clean");

Task("Build")
    .IsDependentOn("VMLab.Build");

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
Folder structure
*****************************************************************************************************/
Task("Folder.Restore")
    .Does(() => {
        CreateDirectory(ReleaseFolder);
        CreateDirectory(BuildFolder);
    });

/*****************************************************************************************************
Global Nuget Tasks
*****************************************************************************************************/
Task("Nuget.Restore")
    .Does(() => NuGetRestore(SolutionFile));

/*****************************************************************************************************
VMLab
*****************************************************************************************************/
Task("VMLab.Clean")
    .IsDependentOn("VMLab.Clean.Main");

Task("VMLab.Restore")
    .IsDependentOn("VMLab.Restore.AssemblyInfo");

Task("VMLab.Build")
    .IsDependentOn("VMLab.Build.Compile")
    .IsDependentOn("VMLab.Build.Plugins");

Task("VMLab.Build.Plugins")
    .IsDependentOn("VMLab.Build.Plugins.VMwareWorkstation");

Task("VMLab.Clean.Main")
    .Does(() => {
        CleanDirectory(BuildFolder + "/VMLab");
    });

Task("VMLab.Restore.AssemblyInfo")
    .Does(() => {
            CreateDirectory(SourceFolder + "/VMLab/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab" }); // Don't bother setting versions, gitversion overwrites them.
    });

Task("VMLab.Build.Compile")
    .IsDependentOn("VMLab.Clean.Main")
    .Does(() => {
        MSBuild(SolutionFile, config =>
            config.SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithTarget("VMLab")
            .WithProperty("OutDir", BuildFolder + "/VMLab")
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetPlatformTarget(PlatformTarget.MSIL));
        });

Task("VMLab.Build.Plugins.VMwareWorkstation")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Build")
    .Does(() => {
        var files = GetFiles(BuildFolder + "/VMLab.Hypervisor.VMwareWorkstation/*.*");
        CopyFiles(files, BuildFolder + "/VMLab");
    });

/*****************************************************************************************************
VMLab.Hypervisor.VMwareWorkstation
*****************************************************************************************************/
Task("VMLab_Hypervisor_VMwareWorkstation.Clean")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Clean.Main");

Task("VMLab_Hypervisor_VMwareWorkstation.Restore")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Restore.AssemblyInfo");

Task("VMLab_Hypervisor_VMwareWorkstation.Build")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Build.Compile");

Task("VMLab_Hypervisor_VMwareWorkstation.Clean.Main")
    .Does(() => {
        CleanDirectory(BuildFolder + "/VMLab.Hypervisor.VMwareWorkstation");
    });

Task("VMLab_Hypervisor_VMwareWorkstation.Restore.AssemblyInfo")
    .Does(() => {
            CreateDirectory(SourceFolder + "/VMLab.Hypervisor.VMwareWorkstation/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab.Hypervisor.VMwareWorkstation/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab.Hypervisor.VMwareWorkstation" }); // Don't bother setting versions, gitversion overwrites them.
    });

Task("VMLab_Hypervisor_VMwareWorkstation.Build.Compile")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Clean.Main")
    .Does(() => {
        MSBuild(SolutionFile, config =>
            config.SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithTarget("VMLab_Hypervisor_VMwareWorkstation")
            .WithProperty("OutDir", BuildFolder + "/VMLab.Hypervisor.VMwareWorkstation")
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetPlatformTarget(PlatformTarget.MSIL));
        });

/*****************************************************************************************************
End of script
*****************************************************************************************************/
RunTarget(target);