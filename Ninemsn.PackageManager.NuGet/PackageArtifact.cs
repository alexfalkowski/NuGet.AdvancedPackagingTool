namespace Ninemsn.PackageManager.NuGet
{
    using global::NuGet;

    public class PackageArtifact
    {
        public IPackageManager Manager { get; set; }

        public IPackage Package { get; set; }

        public bool IsUpdate { get; set; }
    }
}