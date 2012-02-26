namespace NuGet.Enterprise.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using NuGet;

    public class PackageInstaller : IPackageInstaller
    {
        private readonly PackageSource packageSource;

        private readonly string localRepositoryPath;

        private readonly string packageName;

        private readonly PackageLogger logger;

        private readonly IPackageManager packageManager;

        private bool installCalled;

        public PackageInstaller(
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
                this.packageManager.UpdatePackage(this.packageName, version, false, false);
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

        private void OnManagerPackageUninstalled(object sender, PackageOperationEventArgs e)
        {
            var fileSystem = new DefaultFileSystem(this.localRepositoryPath);

            if (!fileSystem.GetDirectories(string.Empty).Any())
            {
                fileSystem.DeleteDirectory(string.Empty, true);
            }
        }

        private IPackageManager CreatePackageManager()
        {
            var sourceRepository = new DiskPackageRepository(this.packageSource.Source);
            var localRepository = new DiskPackageRepository(this.localRepositoryPath);
            var packagePathResolver = new DefaultPackagePathResolver(this.localRepositoryPath);

            var manager = new ZipPackageManager(localRepository, sourceRepository, packagePathResolver)
                {
                    Logger = this.logger
                };

            manager.PackageInstalling += this.OnPackageManagerPackageInstalling;
            manager.PackageInstalled += this.OnPackageManagerPackageInstalled;
            manager.PackageUninstalling += this.OnPackageManagerPackageUninstalling;
            manager.PackageUninstalled += this.OnManagerPackageUninstalled;

            return manager;
        }
    }
}