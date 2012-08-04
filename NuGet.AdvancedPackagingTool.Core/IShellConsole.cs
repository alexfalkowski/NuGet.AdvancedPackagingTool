namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IShellConsole
    {
        ProcessExitInfo Start(string installationPath, string configurationPath);
    }
}