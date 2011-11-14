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

        public IEnumerable<string> InstallPackage(Version version = null)
        {
            var packageArtifact = this.GetPackageArtifact(version);
            var package = packageArtifact.Package;
            var packageManager = packageArtifact.Manager;
            var isPackageInstalled = packageManager.IsPackageInstalled(package);

            Directory.CreateDirectory(this.installationPath);

            if (!isPackageInstalled)
            {
                var initPackageFile = package.GetInitPackageFile();
                
                packageManager.ExecutePowerShell(initPackageFile);
            }

            packageManager.InstallPackage(package);

            if (!isPackageInstalled)
            {
                var installPackageFile = package.GetInstallPackageFile();
                packageManager.ExecutePowerShell(installPackageFile);
            }

            return packageManager.Logs;
        }

        public IEnumerable<string> UninstallPackage(Version version = null)
        {
            var packageArtifact = this.GetPackageArtifact(version);
            var package = packageArtifact.Package;
            var packageManager = packageArtifact.Manager;

            packageManager.UninstallPackage(package, true);

            Directory.Delete(this.installationPath);

            return packageManager.Logs;
        }

        private static void OnProjectManagerPackageReferenceRemoving(object sender, PackageOperationEventArgs e)
        {
            var projectManager = (IProjectManager)sender;
            var unistallPackageFile = e.Package.GetUninstallPackageFile();

            unistallPackageFile.ExecutePowerShell(projectManager.Logger);
        }

        private PackageArtifact GetPackageArtifact(Version version)
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var package = this.GetPackage(version);
            var destinationRepository = PackageRepositoryFactory.Default.CreateRepository(
                this.destinationRepositoryPath);
            var projectSystem = ProjectSystemFactory.CreateProjectSystem(package, this.installationPath);
            var logger = new PackageLogger();
            var packageManager = new PackageManager(sourceRepository, destinationRepository, projectSystem, logger);
            packageManager.ProjectManager.PackageReferenceRemoving += OnProjectManagerPackageReferenceRemoving;

            if (version != null)
            {
                return new PackageArtifact { Manager = packageManager, Package = package };
            }

            var updatePackage = packageManager.GetUpdate(package);
            var isUpdate = updatePackage != null;

            return new PackageArtifact
                {
                    Manager = packageManager, 
                    Package = isUpdate ? updatePackage : package
                };
        }

        private IPackage GetPackage(Version version)
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var packages = sourceRepository.GetPackages();
            var sourcePackage = packages.Find(this.packageName).FirstOrDefault();

            if (sourcePackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidPackage, this.packageName, this.source.Source);
            }

            if (version != null)
            {
                var versionSourcePackage = sourceRepository.FindPackage(sourcePackage.Id, version);

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