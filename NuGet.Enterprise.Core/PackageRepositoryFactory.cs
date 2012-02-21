﻿namespace NuGet.Enterprise.Core
{
    public static class PackageRepositoryFactory
    {
         public static IPackageRepository CreatePackageRepository(string source)
         {
             return new DiskPackageRepository(source);
         }
    }
}