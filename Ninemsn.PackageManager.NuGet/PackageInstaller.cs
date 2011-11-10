namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninemsn.PackageManager.NuGet.Properties;

    using global::NuGet;

    public class PackageInstaller
    {
        private readonly PackageSource source;

        private readonly PackageSource destination;

        private readonly ProjectManager projectManager;

        private readonly IPackage package;

        public PackageInstaller(
            PackageSource source, PackageSource destination, string packageName, string installationPath)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }

            this.source = source;
            this.destination = destination;

            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            this.package = this.GetPackage(packageName);

            var destinationRepository = PackageRepositoryFactory.Default.CreateRepository(this.destination.Source);
            var projectSystem = ProjectSystemFactory.CreateProjectSystem(
                this.package, this.destination.Source, installationPath);

            var logger = new PackageLogger();

            this.projectManager = new ProjectManager(sourceRepository, destinationRepository, projectSystem, logger);
        }

        public IPackage Package
        {
            get
            {
                return this.package;
            }
        }

        public IEnumerable<string> Logs
        {
            get
            {
                return this.projectManager.Logs;
            }
        }

        public void InstallPackage()
        {
            var initPackageFile = this.package.GetInitPackageFile();
            this.ExecutePowerShell(initPackageFile);

            this.projectManager.InstallPackage(this.package);

            var installPackageFile = this.package.GetInstallPackageFile();
            this.ExecutePowerShell(installPackageFile);
        }

        public void ExecutePowerShell(IPackageFile file)
        {
            this.projectManager.ExecutePowerShell(file);
        }

        public bool IsPackageInstalled()
        {
            return this.projectManager.IsPackageInstalled(this.package);
        }

        public void UninstallPackage()
        {
            this.projectManager.UninstallPackage(this.package, true);
        }

        private IPackage GetPackage(string packageName)
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var sourcePackage = sourceRepository.GetPackages().Find(packageName).FirstOrDefault();

            if (sourcePackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    Resources.InvalidPackage, this.source.Source, packageName);
            }

            return sourcePackage;
        }
    }
}