namespace Ninemsn.PackageManager.NuGet.Web
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using global::NuGet;

    public class WebProjectSystem : PhysicalFileSystem, IProjectSystem
    {
        public WebProjectSystem(string root)
            : base(root)
        {
        }

        public string ProjectName
        {
            get
            {
                return this.Root;
            }
        }

        public FrameworkName TargetFramework
        {
            get
            {
                return VersionUtility.DefaultTargetFramework;
            }
        }

        public void AddFrameworkReference(string name)
        {
            throw new NotSupportedException();
        }

        public void AddReference(string referencePath, Stream stream)
        {
            var fileName = Path.GetFileName(referencePath);
            var fullPath = this.GetFullPath(this.GetReferencePath(fileName));
            this.AddFile(fullPath, stream);
        }

        public object GetPropertyValue(string propertyName)
        {
            if ((propertyName != null) && propertyName.Equals("RootNamespace", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return null;
        }

        public bool IsSupportedFile(string path)
        {
            var fileName = Path.GetFileName(path);
            return fileName != null
                   &&
                   (!path.StartsWith("tools", StringComparison.OrdinalIgnoreCase)
                    && !fileName.Equals("app.config", StringComparison.OrdinalIgnoreCase));
        }

        public string ResolvePath(string path)
        {
            throw new NotSupportedException();
        }

        public bool ReferenceExists(string name)
        {
            string referencePath = this.GetReferencePath(name);
            return this.FileExists(referencePath);
        }

        public void RemoveReference(string name)
        {
            this.DeleteFile(this.GetReferencePath(name));
            if (!this.GetFiles("bin").Any())
            {
                this.DeleteDirectory("bin");
            }
        }

        protected virtual string GetReferencePath(string name)
        {
            return Path.Combine("bin", name);
        }
    }
}