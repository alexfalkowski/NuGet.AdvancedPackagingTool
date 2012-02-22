namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DefaultFileSystem : PhysicalFileSystem, IFileSystem
    {
        public DefaultFileSystem(string root)
            : base(root)
        {
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