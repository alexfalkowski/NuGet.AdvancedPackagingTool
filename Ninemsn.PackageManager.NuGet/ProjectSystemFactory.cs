namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Linq;

    using global::NuGet;

    public static class ProjectSystemFactory
    {
        public static IProjectSystem CreateProjectSystem(IPackage package, string root, string installationPath)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            var packageFile = package.GetFiles().Where(file => file.Path.Contains("Web.config")).FirstOrDefault();

            if (packageFile != null)
            {
                return new WebProjectSystem(root, installationPath);
            }

            return new DefaultProjectSystem(root, installationPath); 
        }
    }
}