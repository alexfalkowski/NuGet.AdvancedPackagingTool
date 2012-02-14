namespace NuGet.Enterprise.Core
{
    using System.IO;

    using NuGet;

    public class PowerShellPackageFile : IPackageFile
    {
        private readonly IPackageFile file;

        public PowerShellPackageFile(IPackageFile file)
        {
            this.file = file;
        }

        public string Path
        {
            get
            {
                return this.file.Path;
            }
        }

        public Stream GetStream()
        {
            return this.file.GetStream();
        }
    }
}