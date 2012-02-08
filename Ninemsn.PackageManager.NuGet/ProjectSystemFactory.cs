namespace Ninemsn.PackageManager.NuGet
{
    using global::NuGet;

    public static class ProjectSystemFactory
    {
        public static IProjectSystem CreateProjectSystem(IPackageMetadata package, bool install)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            if (!package.IsValid())
            {
                return new NullProjectSystem();
            }

            var localPath = package.ProjectUrl.LocalPath;

            return new DefaultProjectSystem(localPath, install);
        }
    }
}