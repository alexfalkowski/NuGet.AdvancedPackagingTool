namespace NuGet.AdvancedPackagingTool.Core
{
    public interface IPackageInstallerFactory
    {
        IPackageInstaller CreatePackageInstaller(bool areArgumentsValid);
    }
}