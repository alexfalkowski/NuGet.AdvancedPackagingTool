namespace NuGet.Enterprise.Core
{
    using NuGet;

    public class PackageArtifact
    {
        public IPackageManager Manager { get; set; }

        public IPackage Package { get; set; }
    }
}