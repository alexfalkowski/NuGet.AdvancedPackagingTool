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

        private readonly string destinationRepositoryPath;

        private readonly string packageName;

        private readonly string installationPath;

        public PackageInstaller(
            PackageSource source, 
            string destinationRepositoryPath, 
            string packageName, 
            string installationPath)
        {
            if (source == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("source");
            }

            if (string.IsNullOrWhiteSpace(destinationRepositoryPath))
            {
                throw ExceptionFactory.CreateArgumentNullException("destinationRepositoryPath");
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
            this.destinationRepositoryPath = destinationRepositoryPath;
            this.packageName = packageName;
            this.installationPath = installationPath;
        }

        public IEnumerable<string> InstallPackage()
        {
            var packageArtifact = this.GetPackageArtifact();

            Directory.CreateDirectory(this.installationPath);

            var package = packageArtifact.Package;
            var initPackageFile = package.GetInitPackageFile();
            var packageManager = packageArtifact.Manager;
            packageManager.ExecutePowerShell(initPackageFile);

            packageManager.InstallPackage(package);

            var installPackageFile = package.GetInstallPackageFile();
            packageManager.ExecutePowerShell(installPackageFile);

            return packageManager.Logs;
        }

        public bool IsPackageInstalled()
        {
            var packageArtifact = this.GetPackageArtifact();

            return packageArtifact.Manager.IsPackageInstalled(packageArtifact.Package);
        }

        public IEnumerable<string> UninstallPackage()
        {
            var packageArtifact = this.GetPackageArtifact();

            var package = packageArtifact.Package;
            var unistallPackageFile = package.GetUninstallPackageFile();
            var packageManager = packageArtifact.Manager;
            packageManager.ExecutePowerShell(unistallPackageFile);

            packageManager.UninstallPackage(package, true);

            Directory.Delete(this.installationPath);

            return packageManager.Logs;
        }

        private PackageArtifact GetPackageArtifact()
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var package = this.GetPackage();

            var destinationRepository = PackageRepositoryFactory.Default.CreateRepository(
                this.destinationRepositoryPath);
            var projectSystem = ProjectSystemFactory.CreateProjectSystem(package, this.installationPath);

            var logger = new PackageLogger();

            var packageManager = new PackageManager(sourceRepository, destinationRepository, projectSystem, logger);

            var updatePackage = packageManager.GetUpdate(package);
            var isUpdate = updatePackage != null;

            return new PackageArtifact
                {
                    IsUpdate = isUpdate, 
                    Manager = packageManager, 
                    Package = isUpdate ? updatePackage : package
                };
        }

        private IPackage GetPackage()
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var packages = sourceRepository.GetPackages();
            var sourcePackage = packages.Find(this.packageName).FirstOrDefault();

            if (sourcePackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidPackage, this.source.Source, this.packageName);
            }

            return sourcePackage;
        }
    }
}