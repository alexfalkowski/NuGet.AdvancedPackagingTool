namespace Ninemsn.PackageManager.NuGet.Web
{
    using System;
    using System.Linq;

    using global::NuGet;

    public static class Utils
    {
        public static IQueryable<IPackage> FilterPreferredPackages(IQueryable<IPackage> packages)
        {
            return packages.Where(x => x.Tags.ToLower().Contains("aspnetwebpages"));
        }

        public static string GetPackagesHome()
        {
            return GetVirtualPath("~/packages");
        }

        public static WebPackageSource GetPackageSource(PackageManagerModule module, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return module.ActiveSource;
            }

            return module.GetSource(name) ?? module.ActiveSource;
        }

        public static string GetPageVirtualPath(string page)
        {
            return GetVirtualPath("~/packages/" + page);
        }

        public static string GetVirtualPath(string virtualPath)
        {
            if (virtualPath == null)
            {
                throw new ArgumentNullException("virtualPath");
            }

            if (virtualPath.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            {
                virtualPath = virtualPath.Substring(2);
            }

            if (virtualPath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                virtualPath = virtualPath.Substring(1);
            }

            return virtualPath;
        }
    }
}