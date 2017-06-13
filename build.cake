#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=WiX.Toolset"

//Folder Variables
var RepoRootFolder = MakeAbsolute(Directory(".")); 
var InstallSourceFolder = RepoRootFolder + "/Installer";
var ReleaseFolder = RepoRootFolder + "/Release";
var BuildFolder = RepoRootFolder + "/Build";
var SourceFolder = RepoRootFolder +"/Src";
var SolutionFile = SourceFolder + "/VMLab.sln";
var ToolsFolder = RepoRootFolder + "/Tools";

var target = Argument("target", "Default");
var version = GitVersion(new GitVersionSettings{UpdateAssemblyInfo = true}); //This updates all AssemblyInfo files automatically.
var RunningInCI = EnvironmentVariable("CI");


Task("Default")
    .IsDependentOn("Package");

Task("Restore")
    .IsDependentOn("Folder.Restore")
    .IsDependentOn("Nuget.Restore")
    .IsDependentOn("Appveyor.Restore")
    .IsDependentOn("VMLab.Restore")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Restore");

Task("Clean")
    .IsDependentOn("VMLab.Clean");

Task("Build")
    .IsDependentOn("VMLab.Build");

Task("Test");

Task("Package")
    .IsDependentOn("VMLab.Package.Link");

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
Appveyor Tasks
*****************************************************************************************************/
Task("Appveyor.Restore")
    .IsDependentOn("Appveyor.Restore.GitVersion");

Task("Appveyor.Restore.GitVersion")
    .WithCriteria(RunningInCI == "True")
    .Does(() => StartProcess(ToolsFolder + "/GitVersion.CommandLine/tools/GitVersion.exe", 
        "/l console /output buildserver /updateAssemblyInfo"));

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
    .IsDependentOn("VMLab.Build.Plugins.Null")
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

            CreateDirectory(SourceFolder + "/VMLab.Contract/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab.Contract/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab.Contract" }); // Don't bother setting versions, gitversion overwrites them.

            CreateDirectory(SourceFolder + "/VMLab.Core/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab.Core/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab.Core" }); // Don't bother setting versions, gitversion overwrites them.

            CreateDirectory(SourceFolder + "/VMLab.UnitTest/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab.UnitTest/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab.UnitTest" }); // Don't bother setting versions, gitversion overwrites them.
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

Task("VMLab.Build.Plugins.Null")
    .IsDependentOn("VMLab_Hypervisor_Null.Build")
    .Does(() => {
        var files = GetFiles(BuildFolder + "/VMLab.Hypervisor.Null/*.*");
        CopyFiles(files, BuildFolder + "/VMLab");
    });

Task("VMLab.Build.Plugins.VMwareWorkstation")
    .IsDependentOn("VMLab_Hypervisor_VMwareWorkstation.Build")
    .Does(() => {
        var files = GetFiles(BuildFolder + "/VMLab.Hypervisor.VMwareWorkstation/*.*");
        CopyFiles(files, BuildFolder + "/VMLab");
    }); 

Task("VMLab.Package.Clean")
    .Does(() => CleanDirectory(BuildFolder + "/VMLab.Package"))
    .Does(() => CleanDirectory(BuildFolder + "/VMLab.msi"));

Task("VMLab.Package.Harvest")
    .IsDependentOn("VMLab.Build")
    .IsDependentOn("VMLab.Package.Clean")
    .Does(() =>
        WiXHeat(BuildFolder + "/VMLab", BuildFolder + "/VMLab.Package/VMLabFiles.wxs", WiXHarvestType.Dir, 
            new HeatSettings { 
                NoLogo = true, 
                SuppressRegistry = true,
                AutogeneratedGuid = true,
                ComponentGroupName = "vmlabFiles",
                SuppressRootDirectory = true,
                DirectoryId	 = "APPLICATIONROOTDIRECTORY",
                DirectoryReferenceId = "APPLICATIONROOTDIRECTORY"
            })
    )
    .Does(() => CopyDirectory(BuildFolder + "/VMLab", BuildFolder + "/VMLab.Package/SourceDir"))
    .Does(() => CopyFile(InstallSourceFolder + "/main.wxs", BuildFolder + "/VMLab.Package/main.wxs"));

Task("VMLab.Package.Build")
    .IsDependentOn("VMLab.Package.Harvest")
    .Does(() => {
        var files = GetFiles(BuildFolder + "/VMLab.Package/*.wxs");
        var settings = new CandleSettings {
            Architecture = Architecture.X64,
            Verbose = true,
            NoLogo = true,
            OutputDirectory = BuildFolder + "/VMLab.Package",
            Defines = new Dictionary<string,string> {
                {"version", version.AssemblySemVer}
            }            
        };

        WiXCandle(files, settings);
    });

Task("VMLab.Package.Link")
    .IsDependentOn("VMLab.Package.Build")
    .Does(() => WiXLight(BuildFolder + "/VMLab.Package/*.wixobj", new LightSettings {
        NoLogo = true,
        OutputFile = BuildFolder + "/VMLab.msi/vmlab - v" + version.SemVer + ".msi"
    }));


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
VMLab.Hypervisor.Null
*****************************************************************************************************/
Task("VMLab_Hypervisor_Null.Clean")
    .IsDependentOn("VMLab_Hypervisor_Null.Clean.Main");

Task("VMLab_Hypervisor_Null.Restore")
    .IsDependentOn("VMLab_Hypervisor_Null.Restore.AssemblyInfo");

Task("VMLab_Hypervisor_Null.Build")
    .IsDependentOn("VMLab_Hypervisor_Null.Build.Compile");

Task("VMLab_Hypervisor_Null.Clean.Main")
    .Does(() => {
        CleanDirectory(BuildFolder + "/VMLab.Hypervisor.Null");
    });

Task("VMLab_Hypervisor_Null.Restore.AssemblyInfo")
    .Does(() => {
            CreateDirectory(SourceFolder + "/VMLab.Hypervisor.Null/Properties");

            CreateAssemblyInfo(SourceFolder + "/VMLab.Hypervisor.Null/Properties/AssemblyInfo.cs", 
                new AssemblyInfoSettings { Product = "VMLab.Hypervisor.Null" }); // Don't bother setting versions, gitversion overwrites them.
    });

Task("VMLab_Hypervisor_Null.Build.Compile")
    .IsDependentOn("VMLab_Hypervisor_Null.Clean.Main")
    .Does(() => {
        MSBuild(SolutionFile, config =>
            config.SetVerbosity(Verbosity.Minimal)
            .UseToolVersion(MSBuildToolVersion.VS2017)
            .WithTarget("VMLab_Hypervisor_Null")
            .WithProperty("OutDir", BuildFolder + "/VMLab.Hypervisor.Null")
            .SetMSBuildPlatform(MSBuildPlatform.x64)
            .SetPlatformTarget(PlatformTarget.MSIL));
        });

/*****************************************************************************************************
End of script
*****************************************************************************************************/
RunTarget(target);