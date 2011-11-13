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

        public IEnumerable<string> InstallPackage()
        {
            var managerAndPackage = this.GetManagerAndPackage();

            Directory.CreateDirectory(this.installationPath);

            var package = managerAndPackage.Item2;
            var initPackageFile = package.GetInitPackageFile();
            var packageManager = managerAndPackage.Item1;
            packageManager.ExecutePowerShell(initPackageFile);

            packageManager.InstallPackage(package);

            var installPackageFile = package.GetInstallPackageFile();
            packageManager.ExecutePowerShell(installPackageFile);

            return packageManager.Logs;
        }

        public bool IsPackageInstalled()
        {
            var managerAndPackage = this.GetManagerAndPackage();

            return managerAndPackage.Item1.IsPackageInstalled(managerAndPackage.Item2);
        }

        public IEnumerable<string> UninstallPackage()
        {
            var managerAndPackage = this.GetManagerAndPackage();

            var package = managerAndPackage.Item2;
            var unistallPackageFile = package.GetUninstallPackageFile();
            var packageManager = managerAndPackage.Item1;
            packageManager.ExecutePowerShell(unistallPackageFile);

            packageManager.UninstallPackage(package, true);

            Directory.Delete(this.installationPath);

            return packageManager.Logs;
        }

        private Tuple<PackageManager, IPackage> GetManagerAndPackage()
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(this.source.Source);
            var package = this.GetPackage();

            var destinationRepository = PackageRepositoryFactory.Default.CreateRepository(this.destinationRepositoryPath);
            var projectSystem = ProjectSystemFactory.CreateProjectSystem(package, this.installationPath);

            var logger = new PackageLogger();

            var packageManager = new PackageManager(sourceRepository, destinationRepository, projectSystem, logger);

            return new Tuple<PackageManager, IPackage>(packageManager, package);
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