namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using NuGet;

    public class InvalidPackageInstaller : IPackageInstaller
    {
        public IEnumerable<string> Logs
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        public void InstallPackage(string packageId, SemanticVersion version)
        {
        }

        public void UninstallPackage(string packageId, SemanticVersion version)
        {
        }
    }
}