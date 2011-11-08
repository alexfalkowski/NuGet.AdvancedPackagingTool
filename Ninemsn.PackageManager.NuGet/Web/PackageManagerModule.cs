namespace Ninemsn.PackageManager.NuGet.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;

    public class PackageManagerModule
    {
        private readonly HttpContextBase httpContext;

        private readonly IPackagesSourceFile sourceFile;

        private readonly ISet<WebPackageSource> packageSources;

        public PackageManagerModule(HttpContextBase httpContext, IPackagesSourceFile sourceFile)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            if (sourceFile == null)
            {
                throw new ArgumentNullException("sourceFile");
            }

            this.httpContext = httpContext;
            this.sourceFile = sourceFile;

            if (!this.sourceFile.Exists())
            {
                throw new InvalidOperationException();
            }

            this.InitPackageSourceFile(this.sourceFile, out this.packageSources);
        }

        public WebPackageSource ActiveSource
        {
            get
            {
                return this.packageSources.First();
            }
        }

        public bool Available
        {
            get
            {
                return this.httpContext.Request.IsLocal;
            }
        }

        public IEnumerable<WebPackageSource> PackageSources
        {
            get
            {
                return this.packageSources;
            }
        }

        public string SiteRoot
        {
            get
            {
                return HostingEnvironment.MapPath("~/");
            }
        }

        public bool AddPackageSource(WebPackageSource packageSource)
        {
            return AddPackageSource(this.sourceFile, this.packageSources, packageSource);
        }

        public bool AddPackageSource(string source, string name)
        {
            var nameOfSource = string.IsNullOrEmpty(name) ? source : name;
            var packageSource = new WebPackageSource(source, nameOfSource);
            return AddPackageSource(this.sourceFile, this.packageSources, packageSource);
        }

        public bool AddPackageSource(
            IPackagesSourceFile packageSourceFile,
            ISet<WebPackageSource> packageSourcesSet,
            WebPackageSource packageSource)
        {
            if (GetSource(packageSourcesSet, packageSource.Name) != null)
            {
                return false;
            }

            packageSourcesSet.Add(packageSource);
            packageSourceFile.WriteSources(packageSourcesSet);

            return true;
        }

        public WebPackageSource GetSource(string sourceName)
        {
            return GetSource(this.PackageSources, sourceName);
        }

        public WebPackageSource GetSource(IEnumerable<WebPackageSource> packageSourcesSet, string sourceName)
        {
            Func<WebPackageSource, bool> predicate = source => source.Name.Equals(sourceName, StringComparison.OrdinalIgnoreCase);
            return packageSourcesSet.Where(predicate).FirstOrDefault();
        }

        public void RemovePackageSource(string sourceName)
        {
            RemovePackageSource(this.sourceFile, this.packageSources, sourceName);
        }

        public void RemovePackageSource(
            IPackagesSourceFile packageSourceFile, 
            ISet<WebPackageSource> packageSourcesSet, 
            string name)
        {
            var item = GetSource(packageSourcesSet, name);

            if (item != null)
            {
                packageSourcesSet.Remove(item);
                packageSourceFile.WriteSources(packageSourcesSet);
            }
        }

        private void InitPackageSourceFile(IPackagesSourceFile packageSourceFile, out ISet<WebPackageSource> packageSourcesSet)
        {
            packageSourcesSet = new HashSet<WebPackageSource>(packageSourceFile.ReadSources());
        }
    }
}