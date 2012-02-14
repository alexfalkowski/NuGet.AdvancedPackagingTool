namespace NuGet.Enterprise.Core
{
    using System;
    using System.IO;

    using Ionic.Zip;

    public class ZipPackageFile : IPackageFile
    {
        private readonly ZipEntry entry;

        public ZipPackageFile(ZipEntry entry)
        {
            this.entry = entry;
        }

        public Stream GetStream()
        {
            return this.entry.OpenReader();
        }

        public string Path
        {
            get
            {
                var replacedFileName = this.entry.FileName.Replace('/', '\\');
                var escapedFileName = Uri.UnescapeDataString(replacedFileName);
                return escapedFileName;
            }
        }
    }
}