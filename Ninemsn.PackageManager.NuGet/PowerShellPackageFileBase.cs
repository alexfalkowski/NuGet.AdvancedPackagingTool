namespace Ninemsn.PackageManager.NuGet
{
    using System.IO;

    using global::NuGet;

    public abstract class PowerShellPackageFileBase : IPackageFile
    {
        private readonly IPackageFile file;

        protected PowerShellPackageFileBase(IPackageFile file)
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