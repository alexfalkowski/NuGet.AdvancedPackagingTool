namespace Ninemsn.PackageManager.NuGet
{
    using global::NuGet;

    public sealed class UninstallPackageFile : PowerShellPackageFileBase
    {
        public UninstallPackageFile(IPackageFile file)
            : base(file)
        {
        }
    }
}