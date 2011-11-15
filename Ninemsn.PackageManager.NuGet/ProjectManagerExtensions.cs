﻿namespace Ninemsn.PackageManager.NuGet
{
    using System.Linq;

    using global::NuGet;

    public static class ProjectManagerExtensions
    {
        public static IPackage GetUpdate(this IProjectManager manager, IPackage package)
        {
            if (manager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("manager");
            }

            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return manager.SourceRepository.GetUpdates(new[] { package }).SingleOrDefault();
        }

        public static void InstallPackage(this IProjectManager manager, IPackageMetadata package)
        {
            if (manager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("manager");
            }

            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            manager.AddPackageReference(package.Id, package.Version, false);
        }

        public static bool IsPackageInstalled(this IProjectManager manager, IPackageMetadata package)
        {
            if (manager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("manager");
            }
            
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            return manager.LocalRepository.Exists(package);
        }

        public static void UninstallPackage(this IProjectManager manager, IPackageMetadata package, bool removeDependencies)
        {
            if (manager == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("manager");
            }
            
            if (package == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("package");
            }

            manager.RemovePackageReference(package.Id, false, removeDependencies);
        }
    }
}