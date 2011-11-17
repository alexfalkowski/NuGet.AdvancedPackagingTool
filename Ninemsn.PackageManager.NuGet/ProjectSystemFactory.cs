namespace Ninemsn.PackageManager.NuGet
{
    using System.Linq;

    using global::NuGet;

    public static class ProjectSystemFactory
    {
        public static IProjectSystem CreateProjectSystem(IPackage package)
        {
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            if (!package.IsValid())
            {
                return new NullProjectSystem();
            }

            var packageFileQuery = package.GetContentFiles().Where(file => file.Path.Contains("Web.config"));
            var packageFile = packageFileQuery.FirstOrDefault();
            var localPath = package.ProjectUrl.LocalPath;

            return packageFile != null
                       ? new WebProjectSystem(localPath)
                       : new DefaultProjectSystem(localPath);
        }
    }
}