﻿namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.Linq;

    using global::NuGet;

    public class NullPackageInstaller : IPackageInstaller
    {
        public IEnumerable<string> Logs
        {
            get
            {
                return Enumerable.Empty<string>();
            }
        }

        public void InstallPackage(SemanticVersion version)
        {
        }

        public void UninstallPackage(SemanticVersion version)
        {
        }
    }
}