namespace NuGet.Enterprise.Core
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DefaultFileSystem : PhysicalFileSystem
    {
        private readonly bool install;

        public DefaultFileSystem(string installationPath, bool install)
            : base(installationPath)
        {
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
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateFiles(path, filter);
        }

        public override IEnumerable<string> GetDirectories(string path)
        {
            return !Directory.Exists(path) ? Enumerable.Empty<string>() : Directory.EnumerateDirectories(path);
        }
    }
}