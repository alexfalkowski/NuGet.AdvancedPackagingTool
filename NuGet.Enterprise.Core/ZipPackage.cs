namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ionic.Zip;

    public class ZipPackage : IPackage, IDisposable
    {
        private readonly string fileName;

        private readonly ZipFile file;

        public ZipPackage(string fileName)
        {
            this.fileName = fileName;
            this.file = ZipFile.Read(fileName);
            this.Id = null;
            this.Version = null;
            this.Title = null;
            this.Authors = null;
            this.Owners = null;
            this.IconUrl = null;
            this.LicenseUrl = null;
            this.ProjectUrl = null;
            this.RequireLicenseAcceptance = false;
            this.Description = null;
            this.Summary = null;
            this.ReleaseNotes = null;
            this.Language = null;
            this.Tags = null;
            this.Copyright = null;
            this.FrameworkAssemblies = null;
            this.Dependencies = null;
            this.ReportAbuseUrl = null;
            this.DownloadCount = 0;
            this.IsAbsoluteLatestVersion = false;
            this.IsLatestVersion = false;
            this.Listed = false;
            this.Published = null;
            this.AssemblyReferences = null;
        }

        public string Id { get; private set; }

        public SemanticVersion Version { get; private set; }

        public string Title { get; private set; }

        public IEnumerable<string> Authors { get; private set; }

        public IEnumerable<string> Owners { get; private set; }

        public Uri IconUrl { get; private set; }

        public Uri LicenseUrl { get; private set; }

        public Uri ProjectUrl { get; private set; }

        public bool RequireLicenseAcceptance { get; private set; }

        public string Description { get; private set; }

        public string Summary { get; private set; }

        public string ReleaseNotes { get; private set; }

        public string Language { get; private set; }

        public string Tags { get; private set; }

        public string Copyright { get; private set; }

        public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies { get; private set; }

        public IEnumerable<PackageDependency> Dependencies { get; private set; }

        public Uri ReportAbuseUrl { get; private set; }

        public int DownloadCount { get; private set; }

        public bool IsAbsoluteLatestVersion { get; private set; }

        public bool IsLatestVersion { get; private set; }

        public bool Listed { get; private set; }

        public DateTimeOffset? Published { get; private set; }

        public IEnumerable<IPackageAssemblyReference> AssemblyReferences { get; private set; }

        public IEnumerable<IPackageFile> GetFiles()
        {
            return from entry in this.file.EntriesSorted
                   let startsWithContent =
                       entry.FileName.StartsWith("content/", StringComparison.CurrentCultureIgnoreCase)
                   let startsWithTools = entry.FileName.StartsWith("tools/", StringComparison.CurrentCultureIgnoreCase)
                   where startsWithContent || startsWithTools
                   select new ZipPackageFile(entry);
        }

        public Stream GetStream()
        {
            return File.OpenRead(this.fileName);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposable)
        {
            if (disposable)
            {
                this.file.Dispose();
            }
        }
    }
}