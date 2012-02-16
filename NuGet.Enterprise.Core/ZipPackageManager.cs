namespace NuGet.Enterprise.Core
{
    using System;

    public class ZipPackageManager : IPackageManager
    {
        public ZipPackageManager(
            IPackageRepository localRepository,
            IPackageRepository sourceRepository,
            IFileSystem fileSystem,
            IPackagePathResolver pathResolver)
        {
            if (localRepository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("localRepository");
            }

            if (sourceRepository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("sourceRepository");
            }

            if (fileSystem == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("fileSystem");
            }

            if (pathResolver == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("pathResolver");
            }

            this.LocalRepository = localRepository;
            this.SourceRepository = sourceRepository;
            this.FileSystem = fileSystem;
            this.PathResolver = pathResolver;
        }

        public event EventHandler<PackageOperationEventArgs> PackageInstalled;

        public event EventHandler<PackageOperationEventArgs> PackageInstalling;

        public event EventHandler<PackageOperationEventArgs> PackageUninstalled;

        public event EventHandler<PackageOperationEventArgs> PackageUninstalling;

        public IFileSystem FileSystem { get; set; }

        public IPackageRepository LocalRepository { get; private set; }

        public ILogger Logger { get; set; }

        public IPackageRepository SourceRepository { get; private set; }

        public IPackagePathResolver PathResolver { get; private set; }

        public void InstallPackage(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            var packageOperationEventArgs = new PackageOperationEventArgs(package, this.FileSystem, this.LocalRepository.Source, this.FileSystem.Root);
            this.OnPackageInstalling(packageOperationEventArgs);
            this.OnPackageInstalled(packageOperationEventArgs);

            var zipPackage = (ZipPackage)package;

            zipPackage.ExtractContentsTo(this.FileSystem.Root);
        }

        public void InstallPackage(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            throw new NotImplementedException();
        }

        public void UpdatePackage(IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
        {
            throw new NotImplementedException();
        }

        public void UpdatePackage(string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
        {
            throw new NotImplementedException();
        }

        public void UpdatePackage(string packageId, IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
        {
            throw new NotImplementedException();
        }

        public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
        {
            throw new NotImplementedException();
        }

        public void UninstallPackage(string packageId, SemanticVersion version, bool forceRemove, bool removeDependencies)
        {
            throw new NotImplementedException();
        }

        protected void OnPackageInstalled(PackageOperationEventArgs e)
        {
            var handler = this.PackageInstalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnPackageInstalling(PackageOperationEventArgs e)
        {
            var handler = this.PackageInstalling;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnPackageUninstalled(PackageOperationEventArgs e)
        {
            var handler = this.PackageUninstalled;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void OnPackageUninstalling(PackageOperationEventArgs e)
        {
            var handler = this.PackageUninstalling;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}