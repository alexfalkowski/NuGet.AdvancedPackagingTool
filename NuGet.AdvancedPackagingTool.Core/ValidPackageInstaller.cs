namespace NuGet.AdvancedPackagingTool.Core
{
    using System.Collections.Generic;

    using NuGet;

    public class ValidPackageInstaller : IPackageInstaller
    {
        private readonly PackageSource packageSource;

        private readonly string localRepositoryPath;

        private readonly string packageName;

        private readonly PackageLogger logger;

        private readonly IPackageManager packageManager;

        private readonly LocalPackageRepository localRepository;

        private bool installCalled;

        public ValidPackageInstaller(
            PackageSource packageSource, 
            string localRepositoryPath, 
            string packageName)
        {
            if (packageSource == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("packageSource");
            }

            if (string.IsNullOrWhiteSpace(localRepositoryPath))
            {
                throw ExceptionFactory.CreateArgumentNullException("localRepositoryPath");
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageName");
            }

            this.packageSource = packageSource;
            this.localRepositoryPath = localRepositoryPath;
            this.packageName = packageName;
            this.logger = new PackageLogger();
            this.localRepository = new LocalPackageRepository(this.localRepositoryPath);
            this.packageManager = this.CreatePackageManager();
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
            try
            {
                this.installCalled = true;

                if (this.localRepository.Exists(this.packageName, version))
                {
                    this.packageManager.InstallPackage(this.packageName, version, false, false);
                    return;
                }

                if (this.localRepository.Exists(this.packageName))
                {
                    this.packageManager.UpdatePackage(this.packageName, version, false, false);
                    return;
                }

                this.packageManager.InstallPackage(this.packageName, version, false, false);
            }
            finally
            {
                this.installCalled = false;
            }
        }

        public void UninstallPackage(SemanticVersion version)
        {
            this.packageManager.UninstallPackage(this.packageName, version, true, false);
        }

        private void OnPackageManagerPackageUninstalling(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var unistallPackageFile = package.GetUninstallPackageFile();
            var teardownPackageFile = package.GetTeardownPackageFile();

            unistallPackageFile.ExecutePowerShell(package, this.logger);

            if (!this.installCalled)
            {
                teardownPackageFile.ExecutePowerShell(package, this.logger);
            }
        }

        private void OnPackageManagerPackageInstalled(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var installPackageFile = package.GetInstallPackageFile();

            installPackageFile.ExecutePowerShell(package, this.logger);
        }

        private void OnPackageManagerPackageInstalling(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var initPackageFile = package.GetSetupPackageFile();

            initPackageFile.ExecutePowerShell(package, this.logger);
        }

        private IPackageManager CreatePackageManager()
        {
            var sourceRepository = new LocalPackageRepository(this.packageSource.Source);
            var packagePathResolver = new DefaultPackagePathResolver(this.localRepositoryPath);
            var fileSystem = new PhysicalFileSystem(this.localRepositoryPath) { Logger = this.logger };

            var manager = new PackageManager(sourceRepository, packagePathResolver, fileSystem, this.localRepository)
                {
                    Logger = this.logger
                };

            manager.PackageInstalling += this.OnPackageManagerPackageInstalling;
            manager.PackageInstalled += this.OnPackageManagerPackageInstalled;
            manager.PackageUninstalling += this.OnPackageManagerPackageUninstalling;

            return manager;
        }
    }
}