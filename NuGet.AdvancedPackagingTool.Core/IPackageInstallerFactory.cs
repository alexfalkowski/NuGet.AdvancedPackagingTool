namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageInstallerFactory
    {
        IPackageInstaller CreatePackageInstaller(string packageId, bool areArgumentsValid);
    }
}