namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;

    public interface IPackageInstaller
    {
        IEnumerable<string> InstallPackage(Version version = null);

        bool IsPackageInstalled(Version version = null);

        IEnumerable<string> UninstallPackage(Version version = null);
    }
}