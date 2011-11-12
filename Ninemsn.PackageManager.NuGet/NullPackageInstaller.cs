namespace Ninemsn.PackageManager.NuGet
{
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

        public void InstallPackage()
        {
        }

        public bool IsPackageInstalled()
        {
            return false;
        }

        public void UninstallPackage()
        {
        }
    }
}