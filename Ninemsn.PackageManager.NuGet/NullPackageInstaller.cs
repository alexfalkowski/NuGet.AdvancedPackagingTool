namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.Linq;

    public class NullPackageInstaller : IPackageInstaller
    {
        public IEnumerable<string> InstallPackage()
        {
            return Enumerable.Empty<string>();
        }

        public bool IsPackageInstalled()
        {
            return false;
        }

        public IEnumerable<string> UninstallPackage()
        {
            return Enumerable.Empty<string>();
        }
    }
}