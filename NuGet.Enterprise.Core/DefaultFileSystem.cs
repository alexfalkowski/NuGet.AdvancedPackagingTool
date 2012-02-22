namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DefaultFileSystem : PhysicalFileSystem, IFileSystem
    {
        private readonly bool install;

        public DefaultFileSystem(string installationPath, bool install)
            : base(installationPath)
        {
            if (string.IsNullOrEmpty(installationPath))
            {
                throw ExceptionFactory.CreateArgumentNullException("installationPath");
            }

            this.install = install;
        }

        public override void AddFile(string path, Stream stream)
        {
            var fileName = Path.GetFileName(path);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                base.AddFile(path, stream);
            }
        }

        public override bool FileExists(string path)
        {
            return !this.install && base.FileExists(path);
        }

        public override IEnumerable<string> GetFiles(string path, string filter)
        {
            var fullPath = GetFullPath(path);

            try
            {
                return !Directory.Exists(fullPath)
                           ? Enumerable.Empty<string>()
                           : Directory.EnumerateFiles(fullPath, filter, SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
            catch (DirectoryNotFoundException)
            {
                return Enumerable.Empty<string>();
            }
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(GetFullPath(path));
        }
    }
}