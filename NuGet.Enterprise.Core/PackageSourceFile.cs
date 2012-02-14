namespace NuGet.Enterprise.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using NuGet;

    public class PackageSourceFile : IPackagesSourceFile
    {
        private readonly string fileName;

        public PackageSourceFile(string fileName)
        {
            this.fileName = fileName;
        }

        public IEnumerable<PackageSource> ReadSources()
        {
            return ReadFeeds(this.GetStreamForRead);
        }

        public bool Exists()
        {
            return File.Exists(this.fileName);
        }

        public override string ToString()
        {
            return this.fileName;
        }

        private static PackageSource ParsePackageSource(XElement element)
        {
            var urlAttribute = element.Attribute("url");
            var displayNameAttribute = element.Attribute("displayname");

            if ((urlAttribute == null) || (displayNameAttribute == null))
            {
                throw new FormatException();
            }

            Uri uri;

            if (!Uri.TryCreate(urlAttribute.Value, UriKind.Absolute, out uri))
            {
                throw new FormatException();
            }

            return new PackageSource(uri.OriginalString, displayNameAttribute.Value);
        }

        private static IEnumerable<PackageSource> ReadFeeds(Func<Stream> getStream)
        {
            using (var stream = getStream())
            {
                return
                    XElement.Load(stream).Elements().Select(
                        ParsePackageSource).ToList();
            }
        }

        private Stream GetStreamForRead()
        {
            return File.OpenRead(this.fileName);
        }
    }
}