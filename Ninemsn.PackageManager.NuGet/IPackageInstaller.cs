namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    public interface IPackageInstaller
    {
        IEnumerable<string> Logs { get; }

        void InstallPackage();

        bool IsPackageInstalled();

        void UninstallPackage();
    }
}