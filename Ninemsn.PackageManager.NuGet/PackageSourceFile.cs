namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.WebPages;
    using System.Xml.Linq;

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

        public void WriteSources(IEnumerable<PackageSource> sources)
        {
            WriteFeeds(sources, this.GetStreamForWrite);
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
            var filterPreferredAttribute = element.Attribute("filterpreferred");

            if ((urlAttribute == null) || (displayNameAttribute == null))
            {
                throw new FormatException();
            }

            Uri uri;

            if (!Uri.TryCreate(urlAttribute.Value, UriKind.Absolute, out uri))
            {
                throw new FormatException();
            }

            var source = new PackageSource(uri.OriginalString, displayNameAttribute.Value)
                {
                    FilterPreferredPackages = (filterPreferredAttribute != null) && filterPreferredAttribute.Value.AsBool(false)
                };
            return source;
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

        private static void WriteFeeds(IEnumerable<PackageSource> sources, Func<Stream> getStream)
        {
            var content =
                sources.Select(
                    item =>
                    new XElement(
                        "source",
                        GetAttributesForFeeds(item)));
            using (var stream = getStream())
            {
                new XDocument(new object[] { new XElement("sources", content) }).Save(stream);
            }
        }

        private static XAttribute[] GetAttributesForFeeds(PackageSource item)
        {
            return new[]
                {
                    new XAttribute("url", item.Source), new XAttribute("displayname", item.Name),
                    new XAttribute("filterpreferred", item.FilterPreferredPackages)
                };
        }

        private Stream GetStreamForRead()
        {
            return File.OpenRead(this.fileName);
        }

        private Stream GetStreamForWrite()
        {
            if (!File.Exists(this.fileName))
            {
                var directoryName = Path.GetDirectoryName(this.fileName);
                if (directoryName != null)
                {
                    Directory.CreateDirectory(directoryName);
                }

                return File.Create(this.fileName);
            }

            return File.Open(this.fileName, FileMode.Truncate);
        }
    }
}