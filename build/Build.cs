using System;
using System.Diagnostics;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.BuildPythonPackage);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    readonly AbsolutePath CsDirectory = RootDirectory / "Library";

    readonly AbsolutePath PyPackageDirectory = RootDirectory / "python_interface";

    readonly AbsolutePath PyPackageLibraryDirectory = RootDirectory / "python_interface" / "src" / "python_interface" / "lib";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            CsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            PyPackageLibraryDirectory.CreateOrCleanDirectory();
            (PyPackageDirectory / "dist").DeleteDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(s => s
                .SetProjectFile(CsDirectory / "Library.csproj")
            );
        });

    Target PublishDotnet => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetTasks.DotNetPublish(s => s
                .SetProject(CsDirectory / "Library.csproj")
                .SetConfiguration(Configuration)
                .SetOutput(PyPackageLibraryDirectory)
                .EnableNoRestore()
            );
        });

    Target BuildPythonPackage => _ => _
        .DependsOn(PublishDotnet)
        .Executes(() =>
        {
            EnsurePythonAvailable();
            ProcessTasks.StartProcess("uv", "build", workingDirectory: PyPackageDirectory).AssertZeroExitCode();
        });

    public static bool EnsurePythonAvailable()
    {
        try
        {
            ProcessTasks.StartProcess("uv", "--version").AssertZeroExitCode();
            return true;
        }
        catch (Exception ex)
        {
            Log.Warning("Python is not available", ex);
            return false;
        }
    }
}
