﻿namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NullPackageInstaller : IPackageInstaller
    {
        public IEnumerable<string> InstallPackage(Version version = null)
        {
            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> UninstallPackage(Version version = null)
        {
            return Enumerable.Empty<string>();
        }
    }
}