namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageInstallerFactory
    {
        IPackageInstaller CreatePackageInstaller(string installationPath, string configurationPath);
    }
}