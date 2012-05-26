namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageInstallerFactory
    {
        IPackageInstaller CreatePackageInstaller(string packageSourceId, string packageId, bool areArgumentsValid);
    }
}