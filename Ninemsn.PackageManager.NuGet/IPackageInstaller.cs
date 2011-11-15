namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;

    public interface IPackageInstaller
    {
        IEnumerable<string> Logs { get; }

        void InstallPackage(Version version = null);

        void UninstallPackage(Version version = null);
    }
}