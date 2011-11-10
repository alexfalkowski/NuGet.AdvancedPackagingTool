namespace Ninemsn.PackageManager.NuGet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text;

    using global::NuGet;

    public class ProjectManager
    {
        private readonly IPackageRepository sourceRepository;

        private readonly IPackageRepository localRepository;

        private readonly IProjectManager projectManager;

        public ProjectManager(
            IPackageRepository sourceRepository, IPackageRepository localRepository, IProjectSystem project)
        {
            this.sourceRepository = sourceRepository;
            this.localRepository = localRepository;
            var pathResolver = new DefaultPackagePathResolver(localRepository.Source);
            this.projectManager = new global::NuGet.ProjectManager(
                sourceRepository, pathResolver, project, localRepository);
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

        public IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            return this.GetPackageDependencies(package).Where(p => p.RequireLicenseAcceptance);
        }

        public IQueryable<IPackage> GetPackages(IPackageRepository repository, string searchTerm)
        {
            return this.GetPackages(repository.GetPackages(), searchTerm);
        }

        public IQueryable<IPackage> GetInstalledPackages(string searchTerms)
        {
            return this.GetPackages(this.localRepository, searchTerms);
        }

        public IQueryable<IPackage> GetPackagesWithUpdates(string searchTerms)
        {
            return this.GetPackages(this.sourceRepository.GetUpdates(this.localRepository.GetPackages()).AsQueryable(), searchTerms);
        }

        public IQueryable<IPackage> GetRemotePackages(string searchTerms)
        {
            return this.GetPackages(this.sourceRepository, searchTerms);
        }

        public IPackage GetUpdate(IPackage package)
        {
            return this.sourceRepository.GetUpdates(new[] { package }).SingleOrDefault();
        }

        public void InstallPackage(IPackage package, ILogger logger)
        {
            this.projectManager.Logger = logger;

            try
            {
                this.projectManager.AddPackageReference(package.Id, package.Version, false);
            }
            finally
            {
                this.projectManager.Logger = null;
            }
        }

        public bool IsPackageInstalled(IPackage package)
        {
            return this.localRepository.Exists(package);
        }

        public void UninstallPackage(IPackage package, bool removeDependencies)
        {
            this.projectManager.RemovePackageReference(package.Id, false, removeDependencies);
        }

        public void UpdatePackage(IPackage package)
        {
            this.projectManager.UpdatePackageReference(package.Id, package.Version, true);
        }

        public void ExecutePowerShell(IPackageFile file, ILogger logger)
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

                var executePowerShell = stringBuilder.ToString();

                logger.Log(MessageLevel.Warning, executePowerShell);
            }
        }

        private IEnumerable<IPackage> GetPackageDependencies(IPackage package)
        {
            var instance = NullLogger.Instance;
            var installWalker = new InstallWalker(this.localRepository, this.sourceRepository, instance, false);

            var packageDependencies = from packageOperation in installWalker.ResolveOperations(package)
                                      where packageOperation.Action == PackageAction.Install
                                      select packageOperation.Package;

            return packageDependencies;
        }
    }
}