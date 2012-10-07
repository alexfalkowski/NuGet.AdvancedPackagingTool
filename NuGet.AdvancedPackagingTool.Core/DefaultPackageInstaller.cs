namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;

    using NuGet;

    public class DefaultPackageInstaller : IPackageInstaller
    {
        private readonly PackageLogger logger;

        private readonly IPackageManager packageManager;

        private readonly IShellPackageFile shellPackageFile;

        private bool installCalled;

        public DefaultPackageInstaller(
            IPackageManager packageManager, IShellPackageFile shellPackageFile, PackageLogger logger)
        {
            this.shellPackageFile = shellPackageFile;
            if (packageManager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("packageManager");
            }

            this.packageManager = packageManager;
            this.packageManager.PackageInstalling += this.OnPackageManagerPackageInstalling;
            this.packageManager.PackageInstalled += this.OnPackageManagerPackageInstalled;
            this.packageManager.PackageUninstalling += this.OnPackageManagerPackageUninstalling;
            this.logger = logger;
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.logger.Logs;
            }
        }

        public void InstallPackage(string packageId, SemanticVersion version)
        {
            try
            {
                this.installCalled = true;

                if (this.packageManager.LocalRepository.Exists(packageId, version))
                {
                    this.packageManager.InstallPackage(packageId, version, false, false);
                    return;
                }

                if (this.packageManager.LocalRepository.Exists(packageId))
                {
                    this.packageManager.UpdatePackage(packageId, version, false, false);
                    return;
                }

                this.packageManager.InstallPackage(packageId, version, false, false);
            }
            finally
            {
                this.installCalled = false;
            }
        }

        public void UninstallPackage(string packageId, SemanticVersion version)
        {
            try
            {
                this.packageManager.UninstallPackage(packageId, version, true, false);
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
            this.shellPackageFile.Execute(unistallPackageFile, package, this.logger);

            if (this.installCalled)
            {
                return;
            }

            var teardownPackageFile = package.GetTeardownPackageFile();
            this.shellPackageFile.Execute(teardownPackageFile, package, this.logger);
        }

        private void OnPackageManagerPackageInstalled(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var installPackageFile = package.GetInstallPackageFile();
            this.shellPackageFile.Execute(installPackageFile, package, this.logger);
        }

        private void OnPackageManagerPackageInstalling(object sender, PackageOperationEventArgs e)
        {
            var package = e.Package;
            var initPackageFile = package.GetSetupPackageFile();
            this.shellPackageFile.Execute(initPackageFile, package, this.logger);
        }
    }
}