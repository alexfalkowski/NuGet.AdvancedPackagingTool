namespace NuGet.Enterprise.Core
{
    using System;

    public class ZipPackageManager : IPackageManager
    {
        public ZipPackageManager(
            IPackageRepository localRepository,
            IPackageRepository sourceRepository,
            NuGet.IFileSystem fileSystem,
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

        public NuGet.IFileSystem FileSystem { get; set; }

        public IPackageRepository LocalRepository { get; private set; }

        public ILogger Logger { get; set; }

        public IPackageRepository SourceRepository { get; private set; }

        public IPackagePathResolver PathResolver { get; private set; }

        public void InstallPackage(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            var packageOperationEventArgs = new PackageOperationEventArgs(package, this.FileSystem, this.LocalRepository.Source, this.FileSystem.Root);
            this.OnPackageInstalling(packageOperationEventArgs);
            
            var zipPackage = (ZipPackage)package;

            zipPackage.ExtractContentsTo(this.FileSystem.Root);
            this.LocalRepository.AddPackage(zipPackage);

            this.OnPackageInstalled(packageOperationEventArgs);
        }

        public void InstallPackage(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("version");
            }

            this.SourceRepository.FindPackage(
                packageId, version, package => this.InstallPackage(package, ignoreDependencies, allowPrereleaseVersions));
        }

        public void UpdatePackage(IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
        {
            if (newPackage == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("newPackage");
            }

            this.LocalRepository.FindPackage(newPackage.Id, this.UninstallPackage);
            this.InstallPackage(newPackage, updateDependencies, allowPrereleaseVersions);
        }

        public void UpdatePackage(string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
        {
            this.SourceRepository.FindPackage(
                packageId, version, package => this.UpdatePackage(package, updateDependencies, allowPrereleaseVersions));
        }

        public void UpdatePackage(string packageId, IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
        {
            this.SourceRepository.FindPackage(
                packageId, versionSpec, package => this.UpdatePackage(package, updateDependencies, allowPrereleaseVersions));
        }

        public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            var installedPath = this.PathResolver.GetInstallFileName(package);

            using (var installedPackage = new ZipPackage(installedPath))
            {
                this.UninstallPackage(installedPackage);
            }
        }

        public void UninstallPackage(string packageId, SemanticVersion version, bool forceRemove, bool removeDependencies)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("version");
            }

            this.LocalRepository.FindPackage(packageId, version, this.UninstallPackage);
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

        private void UninstallPackage(IPackage package)
        {
            var packageOperationEventArgs = new PackageOperationEventArgs(package, this.FileSystem, this.LocalRepository.Source, this.FileSystem.Root);
            this.OnPackageUninstalling(packageOperationEventArgs);
            this.LocalRepository.RemovePackage(package);
            this.FileSystem.DeleteDirectory(".", true);
            this.OnPackageUninstalled(packageOperationEventArgs);
        }
    }
}