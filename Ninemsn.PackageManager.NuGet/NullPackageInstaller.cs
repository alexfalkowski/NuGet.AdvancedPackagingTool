namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NullPackageInstaller : IPackageInstaller
    {
        public IEnumerable<string> Logs
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        public void InstallPackage(Version version = null)
        {
        }

        public void UninstallPackage(Version version = null)
        {
        }
    }
}