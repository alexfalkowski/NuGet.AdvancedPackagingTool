namespace NuGet.Enterprise.Core
{
    using System;

    using NuGet.Enterprise.Core.Properties;

    public class ZipPackageManager : IPackageManager
    {
        public ZipPackageManager(
            IPackageRepository localRepository,
            IPackageRepository sourceRepository,
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

            if (pathResolver == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("pathResolver");
            }

            this.LocalRepository = localRepository;
            this.SourceRepository = sourceRepository;
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

            if (!package.IsValid())
            {
                throw ExceptionFactory.CreateInvalidOperationException(Resources.InvalidInstallationFolder, package.Id);
            }

            var fileSystem = this.CreateFileSystem(package);
            var logger = this.CreateLogger();
            var installedFileName = this.PathResolver.GetInstallFileName(package);

            if (fileSystem.FileExists(installedFileName))
            {
                logger.Log(MessageLevel.Error, Resources.AlreadyInstalledErrorMessage, package.Id, package.Version);
                return;
            }

            var packageOperationEventArgs = new PackageOperationEventArgs(package, fileSystem, this.LocalRepository.Source, fileSystem.Root);
            this.OnPackageInstalling(packageOperationEventArgs);
            
            var zipPackage = (ZipPackage)package;
            zipPackage.ExtractContentsTo(fileSystem.Root);
            this.LocalRepository.AddPackage(zipPackage);

            this.OnPackageInstalled(packageOperationEventArgs);
            logger.Log(MessageLevel.Info, Resources.InstallSuccessMessage, package.Id, package.Version, fileSystem.Root);
        }

        public void InstallPackage(
            string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                this.SourceRepository.FindPackage(
                    packageId, package => this.InstallPackage(package, ignoreDependencies, allowPrereleaseVersions));
            }
            else
            {
                this.SourceRepository.FindPackage(
                    packageId,
                    version,
                    package => this.InstallPackage(package, ignoreDependencies, allowPrereleaseVersions));
            }
        }

        public void UpdatePackage(IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
        {
            if (newPackage == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("newPackage");
            }

            if (!newPackage.IsValid())
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidInstallationFolder, newPackage.Id);
            }

            this.LocalRepository.FindPackage(
                newPackage.Id,
                foundPackage =>
                    {
                        if (foundPackage != null)
                        {
                            this.UninstallPackage(foundPackage);
                        }

                        this.InstallPackage(newPackage, updateDependencies, allowPrereleaseVersions);
                    });
        }

        public void UpdatePackage(
            string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
        {
            if (version == null)
            {
                this.SourceRepository.FindPackage(
                    packageId, package => this.UpdatePackage(package, updateDependencies, allowPrereleaseVersions));
            }
            else
            {
                this.SourceRepository.FindPackage(
                    packageId,
                    version,
                    package => this.UpdatePackage(package, updateDependencies, allowPrereleaseVersions));
            }
        }

        public void UpdatePackage(
            string packageId, IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
        {
            this.SourceRepository.FindPackage(
                packageId,
                versionSpec,
                package => this.UpdatePackage(package, updateDependencies, allowPrereleaseVersions));
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

        public void UninstallPackage(
            string packageId, SemanticVersion version, bool forceRemove, bool removeDependencies)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                this.LocalRepository.FindPackage(
                    packageId,
                    foundPackage => this.UnisntallPackage(packageId, foundPackage));
            }
            else
            {
                this.LocalRepository.FindPackage(
                    packageId,
                    version,
                    foundPackage => this.UnisntallPackage(packageId, foundPackage));
            }
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

        private void UnisntallPackage(string packageId, IPackage foundPackage)
        {
            if (foundPackage == null)
            {
                var logger = this.CreateLogger();
                logger.Log(MessageLevel.Error, Resources.PackageNotInstalledErrorMessage, packageId);
                return;
            }

            this.UninstallPackage(foundPackage);
        }

        private void UninstallPackage(IPackage package)
        {
            var logger = this.CreateLogger();
            var fileSystem = this.CreateFileSystem(package);
            var packageOperationEventArgs = new PackageOperationEventArgs(
                package, fileSystem, this.LocalRepository.Source, fileSystem.Root);

            this.OnPackageUninstalling(packageOperationEventArgs);

            this.LocalRepository.RemovePackage(package);
            fileSystem.DeleteDirectory(string.Empty, true);

            this.OnPackageUninstalled(packageOperationEventArgs);
            logger.Log(
                MessageLevel.Info, Resources.UninstallSuccessMessage, package.Id, package.Version, fileSystem.Root);
        }

        private NuGet.IFileSystem CreateFileSystem(IPackageMetadata package)
        {
            var fileSystem = this.FileSystem;

            return fileSystem ?? new DiskFileSystem(package.ProjectUrl.LocalPath);
        }

        private ILogger CreateLogger()
        {
            var logger = this.Logger;

            return logger ?? new PackageLogger();
        }
    }
}