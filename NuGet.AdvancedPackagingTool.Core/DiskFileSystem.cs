namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DiskFileSystem : PhysicalFileSystem, IFileSystem
    {
        public DiskFileSystem(string root)
            : base(root)
        {
        }

        public override IEnumerable<string> GetFiles(string path, string filter)
        {
            var fullPath = this.GetFullPath(path);

            try
            {
                return !Directory.Exists(fullPath)
                           ? Enumerable.Empty<string>()
                           : from file in Directory.EnumerateFiles(fullPath, filter, SearchOption.AllDirectories)
                             orderby file descending
                             select file;
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
            Directory.CreateDirectory(this.GetFullPath(path));
        }
    }
}