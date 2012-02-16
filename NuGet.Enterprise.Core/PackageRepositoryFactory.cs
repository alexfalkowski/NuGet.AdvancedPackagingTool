namespace NuGet.Enterprise.Core
{
    using System;

    public static class PackageRepositoryFactory
    {
         public static IPackageRepository CreatePackageRepository(string source)
         {
             return new DiskPackageRepository(new Uri(source).LocalPath);
         }
    }
}