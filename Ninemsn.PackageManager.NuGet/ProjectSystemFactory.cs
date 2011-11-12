namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Linq;

    using global::NuGet;

    public static class ProjectSystemFactory
    {
        public static IProjectSystem CreateProjectSystem(IPackage package, string installationPath)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            var packageFileQuery = package.GetContentFiles().Where(file => file.Path.Contains("Web.config"));
            var packageFile = packageFileQuery.FirstOrDefault();

            return packageFile != null
                       ? new WebProjectSystem(installationPath)
                       : new DefaultProjectSystem(installationPath);
        }
    }
}