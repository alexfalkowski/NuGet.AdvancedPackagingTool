namespace NuGet.Enterprise.Core
{
    using System.IO;

    public static class PackagePathResolverExtensions
    {
         public static string GetInstallFileName(this IPackagePathResolver pathResolver, IPackage package)
         {
             if (pathResolver == null)
             {
                 throw ExceptionFactory.CreateArgumentNullException("pathResolver");
             }

             if (package == null)
             {
                 throw ExceptionFactory.CreateArgumentNullException("package");
             }

             return Path.Combine(pathResolver.GetInstallPath(package), pathResolver.GetPackageFileName(package));
         }
    }
}