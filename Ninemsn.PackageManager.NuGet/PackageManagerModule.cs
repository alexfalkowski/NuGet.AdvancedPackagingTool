namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public class PackageManagerModule
    {
        private readonly IPackagesSourceFile sourceFile;

        private readonly ISet<PackageSource> packageSources;

        public PackageManagerModule(IPackagesSourceFile sourceFile)
        {
            if (sourceFile == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("sourceFile");
            }

            this.sourceFile = sourceFile;

            if (!this.sourceFile.Exists())
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.PackagesSourceFileDoesNotExixst, this.sourceFile.ToString());
            }

            InitPackageSourceFile(this.sourceFile, out this.packageSources);
        }

        public PackageSource ActiveSource
        {
            get
            {
                return this.packageSources.First();
            }
        }

        public IEnumerable<PackageSource> PackageSources
        {
            get
            {
                return this.packageSources;
            }
        }

        public PackageSource GetSource(string sourceName)
        {
            return GetSource(this.PackageSources, sourceName);
        }

        private static PackageSource GetSource(IEnumerable<PackageSource> packageSourcesSet, string sourceName)
        {
            Func<PackageSource, bool> predicate =
                source => source.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase);
            return packageSourcesSet.Where(predicate).FirstOrDefault();
        }

        private static void InitPackageSourceFile(
            IPackagesSourceFile packageSourceFile, out ISet<PackageSource> packageSourcesSet)
        {
            packageSourcesSet = new HashSet<PackageSource>(packageSourceFile.ReadSources());
        }
    }
}