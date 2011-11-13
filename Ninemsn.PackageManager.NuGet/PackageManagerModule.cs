namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

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

        public bool AddPackageSource(PackageSource packageSource)
        {
            return AddPackageSource(this.sourceFile, this.packageSources, packageSource);
        }

        public bool AddPackageSource(string source, string name)
        {
            var nameOfSource = string.IsNullOrEmpty(name) ? source : name;
            var packageSource = new PackageSource(source, nameOfSource);
            return AddPackageSource(this.sourceFile, this.packageSources, packageSource);
        }

        public bool AddPackageSource(
            IPackagesSourceFile packageSourceFile, 
            ISet<PackageSource> packageSourcesSet, 
            PackageSource packageSource)
        {
            if (GetSource(packageSourcesSet, packageSource.Name) != null)
            {
                return false;
            }

            packageSourcesSet.Add(packageSource);
            packageSourceFile.WriteSources(packageSourcesSet);

            return true;
        }

        public PackageSource GetSource(string sourceName)
        {
            return GetSource(this.PackageSources, sourceName);
        }

        public PackageSource GetSource(IEnumerable<PackageSource> packageSourcesSet, string sourceName)
        {
            Func<PackageSource, bool> predicate =
                source => source.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase);
            return packageSourcesSet.Where(predicate).FirstOrDefault();
        }

        public void RemovePackageSource(string sourceName)
        {
            RemovePackageSource(this.sourceFile, this.packageSources, sourceName);
        }

        public void RemovePackageSource(
            IPackagesSourceFile packageSourceFile, 
            ISet<PackageSource> packageSourcesSet, 
            string name)
        {
            var item = GetSource(packageSourcesSet, name);

            if (item != null)
            {
                packageSourcesSet.Remove(item);
                packageSourceFile.WriteSources(packageSourcesSet);
            }
        }

        private static void InitPackageSourceFile(
            IPackagesSourceFile packageSourceFile, out ISet<PackageSource> packageSourcesSet)
        {
            packageSourcesSet = new HashSet<PackageSource>(packageSourceFile.ReadSources());
        }
    }
}