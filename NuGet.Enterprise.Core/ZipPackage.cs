namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Ionic.Zip;
    using NuGet.Enterprise.Core.Properties;

    public class ZipPackage : IPackage, IDisposable
    {
        private readonly string filePath;

        private readonly ZipFile file;

        private bool markForDeletion;

        public ZipPackage(string filePath)
        {
            this.filePath = filePath;
            this.file = ZipFile.Read(filePath);
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
                var query = from entry in this.file.EntriesSorted
                            let startsWithLib =
                                entry.FileName.StartsWith(
                                    Constants.LibDirectory, StringComparison.CurrentCultureIgnoreCase)
                            where startsWithLib && IsAssembly(entry.FileName)
                            select new ZipPackageAssemblyReference(entry);
                return query;
            }
        }

        public IEnumerable<IPackageFile> GetFiles()
        {
            var query = from entry in this.file.EntriesSorted
                        let startsWithContent =
                            entry.FileName.StartsWith(
                                Constants.ContentDirectory, StringComparison.CurrentCultureIgnoreCase)
                        let startsWithTools =
                            entry.FileName.StartsWith(
                                Constants.ToolsDirectory, StringComparison.CurrentCultureIgnoreCase)
                        where startsWithContent || startsWithTools
                        select new ZipPackageFile(entry);

            return query;
        }

        public Stream GetStream()
        {
            return File.OpenRead(this.filePath);
        }

        public void CopyTo(string path)
        {
            File.Copy(this.filePath, path);
        }

        public void ExtractContentsTo(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(this.filePath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.FileNameErrorMessage, this.filePath));
            }

            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            Directory.CreateDirectory(tempPath);
            this.file.ExtractAll(tempPath,  ExtractExistingFileAction.OverwriteSilently);

            var directory = new DirectoryInfo(Path.Combine(tempPath, "content"));
            directory.Copy(new DirectoryInfo(path));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);

            if (this.markForDeletion)
            {
                File.Delete(this.filePath);
            }
        }

        public void Delete()
        {
            this.markForDeletion = true;
        }

        protected virtual void Dispose(bool disposable)
        {
            if (disposable)
            {
                this.file.Dispose();
            }
        }

        private static bool IsAssembly(string path)
        {
            return path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".winmd", StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
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