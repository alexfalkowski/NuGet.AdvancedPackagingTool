namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;

    public interface IPackageInstaller
    {
        IEnumerable<string> InstallPackage();

        bool IsPackageInstalled();

        IEnumerable<string> UninstallPackage();
    }
}