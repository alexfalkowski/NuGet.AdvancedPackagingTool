namespace NuGet.Enterprise.Core
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class DiskPackageRepository : IPackageRepository
    {
        private readonly IFileSystem fileSystem;

        private readonly IPackagePathResolver packagePathResolver;

        public DiskPackageRepository(string source)
            : this(new DiskFileSystem(source), new DefaultPackagePathResolver(source))
        {
            if (source == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("source");
            }

            this.Source = source;
        }

        public DiskPackageRepository(IFileSystem fileSystem, IPackagePathResolver packagePathResolver)
        {
            if (fileSystem == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("fileSystem");
            }

            if (packagePathResolver == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("packagePathResolver");
            }

            this.fileSystem = fileSystem;
            this.packagePathResolver = packagePathResolver;
        }

        public string Source { get; private set; }

        public bool SupportsPrereleasePackages
        {
            get
            {
                return true;
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Will be disposed later.")]
        public IQueryable<IPackage> GetPackages()
        {
            if (!this.fileSystem.DirectoryExists(string.Empty))
            {
                return Enumerable.Empty<IPackage>().AsQueryable();
            }

            var query = from file in this.fileSystem.GetFiles(string.Empty, "*.nupkg")
                        orderby file descending
                        select (IPackage)new ZipPackage(file);

            return query.AsQueryable();
        }

        public void AddPackage(IPackage package)
        {
            var packageDirectory = this.packagePathResolver.GetPackageDirectory(package);
            this.fileSystem.CreateDirectory(packageDirectory);

            var installedPath = this.packagePathResolver.GetInstallFileName(package);

            var zipPackage = (ZipPackage)package;
            zipPackage.CopyTo(installedPath);
        }

        public void RemovePackage(IPackage package)
        {
            var zipPackage = (ZipPackage)package;
            zipPackage.Delete();
        }
    }
}