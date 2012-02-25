namespace NuGet.Enterprise.Core
{
    using System.Collections.Generic;

    using NuGet;

    public class PackageInstaller : IPackageInstaller
    {
        private readonly PackageSource packageSource;

        private readonly string packageName;

        private readonly PackageLogger logger;

        private readonly IPackageManager manager;

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
            this.packageName = packageName;
            this.logger = new PackageLogger();
            var sourceRepository = new DiskPackageRepository(this.packageSource.Source);
            var localRepository = new DiskPackageRepository(localRepositoryPath);
            var defaultPackagePathResolver = new DefaultPackagePathResolver(localRepositoryPath);

            this.manager = new ZipPackageManager(localRepository, sourceRepository, defaultPackagePathResolver)
                {
                    Logger = this.logger 
                };

            this.manager.PackageInstalling += this.OnManagerPackageInstalling;
            this.manager.PackageInstalled += this.OnManagerPackageInstalled;
            this.manager.PackageUninstalling += this.OnManagerPackageUninstalling;
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
                this.manager.UpdatePackage(this.packageName, version, false, false);
            }
            finally
            {
                this.installCalled = false;
            }
        }

        public void UninstallPackage(SemanticVersion version)
        {
            this.manager.UninstallPackage(this.packageName, version, true, false);
        }

        private void OnManagerPackageUninstalling(object sender, PackageOperationEventArgs e)
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

        private void OnManagerPackageInstalled(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var installPackageFile = package.GetInstallPackageFile();

            installPackageFile.ExecutePowerShell(package, this.logger);
        }

        private void OnManagerPackageInstalling(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var initPackageFile = package.GetSetupPackageFile();

            initPackageFile.ExecutePowerShell(package, this.logger);
        }
    }
}