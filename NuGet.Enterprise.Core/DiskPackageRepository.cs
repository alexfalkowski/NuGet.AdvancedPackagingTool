namespace NuGet.Enterprise.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    public class DiskPackageRepository : IPackageRepository
    {
        public DiskPackageRepository(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw ExceptionFactory.CreateArgumentNullException("source");
            }

            this.Source = new Uri(source).LocalPath;
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
            if (!Directory.Exists(this.Source))
            {
                return Enumerable.Empty<IPackage>().AsQueryable();
            }

            var query = from file in Directory.EnumerateFiles(this.Source, "*.nupkg")
                        orderby file descending
                        select (IPackage)new ZipPackage(file);

            return query.AsQueryable();
        }

        public void AddPackage(IPackage package)
        {
            Directory.CreateDirectory(this.Source);

            var zipPackage = (ZipPackage)package;
            zipPackage.CopyTo(this.Source);
        }

        public void RemovePackage(IPackage package)
        {
            var zipPackage = (ZipPackage)package;
            zipPackage.Delete();
        }
    }
}