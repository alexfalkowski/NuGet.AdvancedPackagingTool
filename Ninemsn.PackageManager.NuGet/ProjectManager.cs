namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;

    using global::NuGet;

    public class ProjectManager
    {
        private readonly IProjectManager projectManager;

        public ProjectManager(string remoteSource, string localSource, IProjectSystem project)
        {
            var sourceRepository = PackageRepositoryFactory.Default.CreateRepository(remoteSource);
            var pathResolver = new DefaultPackagePathResolver(localSource);
            var localRepository = PackageRepositoryFactory.Default.CreateRepository(localSource);
            this.projectManager = new global::NuGet.ProjectManager(sourceRepository, pathResolver, project, localRepository);
        }

        public IPackageRepository LocalRepository
        {
            get
            {
                return this.projectManager.LocalRepository;
            }
        }

        public IPackageRepository SourceRepository
        {
            get
            {
                return this.projectManager.SourceRepository;
            }
        }

        public IQueryable<IPackage> GetPackages(IQueryable<IPackage> packages, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                packages = packages.Find(searchTerm.Split(new char[0])[0]);
            }

            return packages;
        }

        public IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            return
                GetPackageDependencies(package, localRepository, sourceRepository).Where(
                    p => p.RequireLicenseAcceptance);
        }

        public IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return this.GetPackages(repository.GetPackages(), searchTerm);
        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return this.GetPackages(this.LocalRepository, searchTerms);
        }

        public IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            var localRepository = this.LocalRepository;
            var sourceRepository = this.SourceRepository;
            return this.GetPackagesRequiringLicenseAcceptance(package, localRepository, sourceRepository);
        }

        public IQueryable<IPackage> GetPackagesWithUpdates(string searchTerms)
        {
            return this.GetPackages(this.SourceRepository.GetUpdates(this.LocalRepository.GetPackages()).AsQueryable(), searchTerms);
        }

        public IQueryable<IPackage> GetRemotePackages(string searchTerms)
        {
            return this.GetPackages(this.SourceRepository, searchTerms);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return this.SourceRepository.GetUpdates(new[] { package }).SingleOrDefault();
        }

        public IEnumerable<string> InstallPackage(IPackage package)
        {
            return this.PerformLoggedAction(
                () => this.projectManager.AddPackageReference(package.Id, package.Version, false));
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return this.LocalRepository.Exists(package);
        }

        public IEnumerable<string> UninstallPackage(IPackage package, bool removeDependencies)
        {
            return this.PerformLoggedAction(
                () => this.projectManager.RemovePackageReference(package.Id, false, removeDependencies));
        }

        public IEnumerable<string> UpdatePackage(IPackage package)
        {
            return
                this.PerformLoggedAction(
                    () => this.projectManager.UpdatePackageReference(package.Id, package.Version, true));
        }

        public IEnumerable<IPackageFile> GetToolsFiles(IPackage package)
        {
            return package.GetFiles().Where(packageFile => packageFile.Path.StartsWith("tools"));
        }

        public string ExecutePowerShell(IPackageFile file)
        {
            using (var powerShell = PowerShell.Create())
            {
                var scriptContents = file.GetStream().ReadToEnd();
                powerShell.AddScript(scriptContents);

                var stringBuilder = new StringBuilder();

                foreach (var result in powerShell.Invoke())
                {
                    stringBuilder.AppendLine(result.ToString());
                }

                return stringBuilder.ToString();
            }
        }

        private static IEnumerable<IPackage> GetPackageDependencies(IPackage package, IPackageRepository localRepository, IPackageRepository sourceRepository)
        {
            var instance = NullLogger.Instance;
            var installWalker = new InstallWalker(localRepository, sourceRepository, instance, false);

            var packageDependencies = from packageOperation in installWalker.ResolveOperations(package)
                                      where packageOperation.Action == PackageAction.Install
                                      select packageOperation.Package;

            return packageDependencies;
        }

        private IEnumerable<string> PerformLoggedAction(Action action)
        {
            var logger = new ErrorLogger();
            this.projectManager.Logger = logger;
            try
            {
                action();
            }
            finally
            {
                this.projectManager.Logger = null;
            }

            return logger.Errors;
        }
    }
}