namespace NuGet.Enterprise.Core
{
    public class ZipPackagePathResolver : IPackagePathResolver
    {
        public string GetInstallPath(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public string GetPackageDirectory(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public string GetPackageDirectory(string packageId, SemanticVersion version)
        {
            throw new System.NotImplementedException();
        }

        public string GetPackageFileName(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public string GetPackageFileName(string packageId, SemanticVersion version)
        {
            throw new System.NotImplementedException();
        }
    }
}