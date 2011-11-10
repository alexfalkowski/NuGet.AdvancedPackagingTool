namespace Ninemsn.PackageManager.NuGet
{
    using global::NuGet;

    public sealed class InitPackageFile : PowerShellPackageFileBase
    {
        public InitPackageFile(IPackageFile file)
            : base(file)
        {
        }
    }
}