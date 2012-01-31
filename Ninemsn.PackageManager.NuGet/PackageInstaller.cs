namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public class PackageInstaller : IPackageInstaller
    {
        private readonly PackageSource source;

        private readonly string packageName;

        private readonly PackageLogger logger;

        private readonly IPackageRepository sourceRepository;

        private readonly IPackageRepository localRepository;

        private IProjectSystem projectSystem;

        private IProjectManager projectManager;

        private bool isPackageInstalled;

        private bool installCalled;

        public PackageInstaller(
            PackageSource source, 
            string localRepositoryPath, 
            string packageName)
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

            this.source = source;
            this.packageName = packageName;
            this.logger = new PackageLogger();
            this.sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            this.localRepository = PackageRepositoryFactory.Default.CreateRepository(localRepositoryPath);
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logger.Logs;
            }
        }

        public void InstallPackage(SemanticVersion version)
        {
            this.installCalled = true;

            try
            {
                var package = this.GetValidPackage(version);
                this.isPackageInstalled = this.projectManager.IsPackageInstalled(package);

                Directory.CreateDirectory(package.ProjectUrl.LocalPath);

                this.projectManager.InstallPackage(package);
            }
            finally
            {
                this.installCalled = false;
            }
        }

        public void UninstallPackage(SemanticVersion version)
        {
            var package = this.GetValidPackage(version);
            this.projectManager.UninstallPackage(package, true);

            var localPath = package.ProjectUrl.LocalPath;

            PathHelper.SafeDelete(localPath);
        }

        private void OnProjectManagerPackageReferenceRemoving(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var unistallPackageFile = package.GetUninstallPackageFile();
            var teardownPackageFile = package.GetTeardownPackageFile();

            unistallPackageFile.ExecutePowerShell(package, this.projectManager.Logger);

            if (!this.installCalled)
            {
                teardownPackageFile.ExecutePowerShell(package, this.projectManager.Logger);
            }
        }

        private void OnProjectManagerPackageReferenceAdding(object sender, PackageOperationEventArgs e)
        {
            if (this.isPackageInstalled)
            {
                return;
            }

            var package = e.Package;
            var initPackageFile = package.GetSetupPackageFile();

            initPackageFile.ExecutePowerShell(package, this.projectManager.Logger);
        }

        private void OnProjectManagerPackageReferenceAdded(object sender, PackageOperationEventArgs e)
        {
            if (this.isPackageInstalled)
            {
                return;
            }

            var package = e.Package;
            var installPackageFile = package.GetInstallPackageFile();

            installPackageFile.ExecutePowerShell(package, this.projectManager.Logger);
        }

        private IPackage GetValidPackage(SemanticVersion version)
        {
            var package = this.GetPackage(version);

            if (!package.IsValid())
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidInstallationFolder, this.packageName);
            }

            return package;
        }

        private IPackage GetPackage(SemanticVersion version)
        {
            var package = this.FindPackage(version);
            this.projectSystem = ProjectSystemFactory.CreateProjectSystem(package);

            var pathResolver = new DefaultPackagePathResolver(this.localRepository.Source);
            this.projectManager = new ProjectManager(
                this.sourceRepository, pathResolver, this.projectSystem, this.localRepository)
                {
                    Logger = this.logger
                };
            this.projectManager.PackageReferenceRemoving += this.OnProjectManagerPackageReferenceRemoving;
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

        private IPackage FindPackage(SemanticVersion version)
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