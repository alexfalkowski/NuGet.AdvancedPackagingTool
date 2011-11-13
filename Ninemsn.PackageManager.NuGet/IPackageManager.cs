namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    using global::NuGet;

    public interface IPackageManager
    {
        IEnumerable<string> Logs { get; }

        IPackage GetUpdate(IPackage package);

        void InstallPackage(IPackage package);

        bool IsPackageInstalled(IPackage package);

        void UninstallPackage(IPackage package, bool removeDependencies);

        void UpdatePackage(IPackage package);

        void ExecutePowerShell(IPackageFile file);
    }
}