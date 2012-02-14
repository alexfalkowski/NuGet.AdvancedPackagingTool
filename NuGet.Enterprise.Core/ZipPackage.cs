namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ionic.Zip;
    using NuGet.Enterprise.Core.Properties;

    public class ZipPackage : IPackage, IDisposable
    {
        private readonly string fileName;

        private readonly ZipFile file;

        public ZipPackage(string fileName)
        {
            this.fileName = fileName;
            this.file = ZipFile.Read(fileName);
            this.ParseManifest();
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

        public IEnumerable<IPackageAssemblyReference> AssemblyReferences
        {
            get
            {
                return from entry in this.file.EntriesSorted
                       let startsWithLib = entry.FileName.StartsWith("lib/", StringComparison.CurrentCultureIgnoreCase)
                       let endsWithDll = entry.FileName.StartsWith("dll", StringComparison.CurrentCultureIgnoreCase)
                       where startsWithLib && endsWithDll
                       select new ZipPackageAssemblyReference(entry);
            }
        }

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

        private void ParseManifest()
        {
            var specFile =
                this.file.EntriesSorted.Where(
                    entry => entry.FileName.EndsWith(".nuspec", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (specFile == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(Resources.SpecFileNotFound);
            }

            var manifest = Manifest.ReadFrom(specFile.OpenReader());
            IPackageMetadata metadata = manifest.Metadata;

            this.Id = metadata.Id;
            this.Version = metadata.Version;
            this.Title = metadata.Title;
            this.Authors = metadata.Authors;
            this.Owners = metadata.Owners;
            this.IconUrl = metadata.IconUrl;
            this.LicenseUrl = metadata.LicenseUrl;
            this.ProjectUrl = metadata.ProjectUrl;
            this.RequireLicenseAcceptance = metadata.RequireLicenseAcceptance;
            this.Description = metadata.Description;
            this.Summary = metadata.Summary;
            this.ReleaseNotes = metadata.ReleaseNotes;
            this.Language = metadata.Language;
            this.Tags = metadata.Tags;
            this.Copyright = metadata.Copyright;
            this.FrameworkAssemblies = metadata.FrameworkAssemblies;
            this.Dependencies = metadata.Dependencies;
            this.ReportAbuseUrl = null;
            this.DownloadCount = 0;
            this.IsAbsoluteLatestVersion = true;
            this.IsLatestVersion = metadata.IsReleaseVersion();
            this.Listed = true;
            this.Published = null;
        }
    }
}