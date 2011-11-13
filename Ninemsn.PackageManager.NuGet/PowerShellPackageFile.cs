namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    using global::NuGet;

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