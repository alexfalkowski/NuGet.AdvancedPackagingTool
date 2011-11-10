namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.IO;
    using System.Runtime.Versioning;

    using global::NuGet;

    public abstract class ProjectSystemBase : PhysicalFileSystem, IProjectSystem
    {
        private readonly string installationPath;

        protected ProjectSystemBase(string root, string installationPath)
            : base(root)
        {
            this.installationPath = installationPath;
        }

        public string InstallationPath
        {
            get
            {
                return this.installationPath;
            }
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
            return true;
        }

        public string ResolvePath(string path)
        {
            return Path.Combine(this.installationPath, path);
        }

        public bool ReferenceExists(string name)
        {
            string referencePath = this.GetReferencePath(name);
            return this.FileExists(referencePath);
        }

        public virtual void RemoveReference(string name)
        {
            this.DeleteFile(this.GetReferencePath(name));
        }

        protected abstract string GetReferencePath(string name);
    }
}