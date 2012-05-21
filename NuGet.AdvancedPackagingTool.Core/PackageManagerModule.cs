namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NuGet;

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

            this.packageSources = InitPackageSourceFile(this.sourceFile);
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

        private static ISet<PackageSource> InitPackageSourceFile(IPackagesSourceFile packageSourceFile)
        {
            return new HashSet<PackageSource>(packageSourceFile.ReadSources());
        }
    }
}