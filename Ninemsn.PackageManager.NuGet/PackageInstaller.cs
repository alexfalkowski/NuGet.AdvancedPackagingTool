namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public class PackageInstaller : IPackageInstaller
    {
        private readonly PackageSource source;

        private readonly string localRepositoryPath;

        private readonly string packageName;

        private readonly string installationPath;

        private readonly PackageLogger logger;

        private readonly IPackageRepository sourceRepository;

        private readonly IPackageRepository localRepository;

        private IProjectSystem projectSystem;

        private IProjectManager projectManager;

        private bool isPackageInstalled;

        public PackageInstaller(
            PackageSource source, 
            string localRepositoryPath, 
            string packageName,
            string installationPath)
        {
            if (source == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("source");
            }

            if (string.IsNullOrWhiteSpace(localRepositoryPath))
            {
                throw ExceptionFactory.CreateArgumentNullException("localRepositoryPath");
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageName");
            }

            if (string.IsNullOrWhiteSpace(installationPath))
            {
                throw ExceptionFactory.CreateArgumentNullException("installationPath");
            }

            this.source = source;
            this.localRepositoryPath = localRepositoryPath;
            this.packageName = packageName;
            this.installationPath = installationPath;
            this.logger = new PackageLogger();
            this.sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            this.localRepository = PackageRepositoryFactory.Default.CreateRepository(this.localRepositoryPath);
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logger.Logs;
            }
        }

        public void InstallPackage(Version version = null)
        {
            var package = this.GetPackage(version);
            this.isPackageInstalled = this.projectManager.IsPackageInstalled(package);

            Directory.CreateDirectory(this.installationPath);

            this.projectManager.InstallPackage(package);
        }

        public void UninstallPackage(Version version = null)
        {
            var package = this.GetPackage(version);

            this.projectManager.UninstallPackage(package, true);

            Directory.Delete(this.installationPath);
        }

        private static void OnProjectManagerPackageReferenceRemoving(object sender, PackageOperationEventArgs e)
        {
            var projectManager = (IProjectManager)sender;
            var unistallPackageFile = e.Package.GetUninstallPackageFile();

            unistallPackageFile.ExecutePowerShell(projectManager.Logger);
        }

        private void OnProjectManagerPackageReferenceAdding(object sender, PackageOperationEventArgs e)
        {
            if (this.isPackageInstalled)
            {
                return;
            }

            var initPackageFile = e.Package.GetInitPackageFile();

            initPackageFile.ExecutePowerShell(this.projectManager.Logger);
        }

        private void OnProjectManagerPackageReferenceAdded(object sender, PackageOperationEventArgs e)
        {
            if (this.isPackageInstalled)
            {
                return;
            }

            var installPackageFile = e.Package.GetInstallPackageFile();

            installPackageFile.ExecutePowerShell(this.projectManager.Logger);
        }

        private IPackage GetPackage(Version version)
        {
            var package = this.FindPackage(version);
            this.projectSystem = ProjectSystemFactory.CreateProjectSystem(package, this.installationPath);

            var pathResolver = new DefaultPackagePathResolver(this.localRepository.Source);
            this.projectManager = new ProjectManager(
                this.sourceRepository, pathResolver, this.projectSystem, this.localRepository)
                {
                    Logger = this.logger
                };
            this.projectManager.PackageReferenceRemoving += OnProjectManagerPackageReferenceRemoving;
            this.projectManager.PackageReferenceAdding += this.OnProjectManagerPackageReferenceAdding;
            this.projectManager.PackageReferenceAdded += this.OnProjectManagerPackageReferenceAdded;

            if (version != null)
            {
                return package;
            }

            var updatePackage = this.projectManager.GetUpdate(package);
            var isUpdate = updatePackage != null;

            return isUpdate ? updatePackage : package;
        }

        private IPackage FindPackage(Version version)
        {
            var packages = this.sourceRepository.GetPackages();
            var sourcePackage = packages.Find(this.packageName).FirstOrDefault();

            if (sourcePackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidPackage, this.packageName, this.source.Source);
            }

            if (version != null)
            {
                var versionSourcePackage = this.sourceRepository.FindPackage(sourcePackage.Id, version);

                if (versionSourcePackage == null)
                {
                    throw ExceptionFactory.CreateInvalidOperationException(
                        Resources.InvalidPackageWithVersion, this.packageName, version, this.source.Source);
                }

                return versionSourcePackage;
            }

            return sourcePackage;
        }
    }
}