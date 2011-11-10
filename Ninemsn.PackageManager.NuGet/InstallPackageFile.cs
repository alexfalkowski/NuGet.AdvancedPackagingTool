namespace Ninemsn.PackageManager.NuGet
{
    using global::NuGet;

    public sealed class InstallPackageFile : PowerShellPackageFileBase
    {
        public InstallPackageFile(IPackageFile file)
            : base(file)
        {
        }
    }
}