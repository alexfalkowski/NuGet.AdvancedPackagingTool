namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;

    using NuGet;

    public class ValidPackageInstaller : IPackageInstaller
    {
        private readonly string packageName;

        private readonly PackageLogger logger;

        private readonly IPackageManager packageManager;

        private readonly IPackageRepository destinationRepository;

        private bool installCalled;

        public ValidPackageInstaller(
            IPackageRepository destinationRepository,
            IPackageManager packageManager,
            PackageLogger logger,
            string packageName)
        {
            if (destinationRepository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("localRepositoryPath");
            }

            if (packageManager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("packageManager");
            }

            if (logger == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("logger");
            }

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageName");
            }

            this.packageManager = packageManager;
            this.packageManager.PackageInstalling += this.OnPackageManagerPackageInstalling;
            this.packageManager.PackageInstalled += this.OnPackageManagerPackageInstalled;
            this.packageManager.PackageUninstalling += this.OnPackageManagerPackageUninstalling;
            this.logger = logger;
            this.packageName = packageName;
            this.destinationRepository = destinationRepository;
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

                if (this.destinationRepository.Exists(this.packageName, version))
                {
                    this.packageManager.InstallPackage(this.packageName, version, false, false);
                    return;
                }

                if (this.destinationRepository.Exists(this.packageName))
                {
                    this.packageManager.UpdatePackage(this.packageName, version, false, false);
                    return;
                }

                this.packageManager.InstallPackage(this.packageName, version, false, false);
            }
            catch (InvalidOperationException e)
            {
                this.logger.Log(MessageLevel.Error, e.Message);
            }
            finally
            {
                this.installCalled = false;
            }
        }

        public void UninstallPackage(SemanticVersion version)
        {
            try
            {
                this.packageManager.UninstallPackage(this.packageName, version, true, false);
            }
            catch (InvalidOperationException e)
            {
                this.logger.Log(MessageLevel.Error, e.Message);
            }
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
    }
}