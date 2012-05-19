namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using NuGet.AdvancedPackagingTool.Core.Properties;

    public static class PackageRepositoryExtensions
    {
        public static void FindPackage(
            this IPackageRepository repository,
            string packageId,
            IVersionSpec version,
            Action<IPackage> action)
        {
            if (repository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("repository");
            }

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("version");
            }

            if (action == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("action");
            }

            var packages = repository.GetPackagesWithId(packageId);
            // ReSharper disable PossibleMultipleEnumeration
            var foundPackage = packages.FindByVersion(version).FirstOrDefault() as ZipPackage;
            // ReSharper restore PossibleMultipleEnumeration
            using (foundPackage)
            {
                action(foundPackage);
            }

            // ReSharper disable PossibleMultipleEnumeration
            DisposePackages(packages);
            // ReSharper restore PossibleMultipleEnumeration
        }

        public static void FindPackage(
            this IPackageRepository repository,
            string packageId,
            SemanticVersion version,
            Action<IPackage> action)
        {
            if (repository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("repository");
            }

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (version == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("version");
            }

            if (action == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("action");
            }

            var packages = repository.GetPackages();
            var query = from package in packages
                        where
                            package.Id.Equals(packageId, StringComparison.CurrentCultureIgnoreCase)
                            && package.Version == version
                        select (ZipPackage)package;
            var foundPackage = query.FirstOrDefault();

            if (foundPackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.NotFoundPackageErrorMessage, packageId, version));
            }

            using (foundPackage)
            {
                action(foundPackage);
            }

            DisposePackages(packages);
        }

        public static void FindPackage(
            this IPackageRepository repository,
            string packageId,
            Action<IPackage> action)
        {
            if (repository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("repository");
            }

            if (string.IsNullOrWhiteSpace(packageId))
            {
                throw ExceptionFactory.CreateArgumentNullException("packageId");
            }

            if (action == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("action");
            }

            var query = repository.GetPackagesWithId(packageId);
            var foundPackage = query.FirstOrDefault() as ZipPackage;

            if (foundPackage == null)
            {
                throw ExceptionFactory.CreateInvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.NotFoundPackageErrorMessage,
                        packageId,
                        Resources.LatestVersionInfoMessage));
            }

            using (foundPackage)
            {
                action(foundPackage);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "The repository returns IQueryable<IPackage>")]
        public static void GetPackages(this IPackageRepository repository, Action<IQueryable<IPackage>> action)
        {
            if (repository == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("repository");
            }

            if (action == null)
            {
                throw ExceptionFactory.CreateArgumentNullException("action");
            }

            var packages = repository.GetPackages();

            action(packages);

            DisposePackages(packages);
        }

        private static IEnumerable<IPackage> GetPackagesWithId(this IPackageRepository repository, string packageId)
        {
            var query = from package in repository.GetPackages()
                        where package.Id.Equals(packageId, StringComparison.CurrentCultureIgnoreCase)
                        select (ZipPackage)package;

            return query;
        }

        private static void DisposePackages(IEnumerable<IPackage> packages)
        {
            foreach (ZipPackage package in packages)
            {
                package.Dispose();
            }
        }
    }
}