namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageInstallerFactory
    {
        IPackageInstaller CreatePackageInstaller(
            bool areArgumentsValid, string installationPath, string configurationPath);
    }
}