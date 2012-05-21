namespace NuGet.AdvancedPackagingTool.Core
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using NuGet;

    public class VersionTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var versionAsString = value as string;

            if (versionAsString != null)
            {
                SemanticVersion version;

                if (SemanticVersion.TryParse(versionAsString, out version))
                {
                    return version;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
            {
                var version = (SemanticVersion)value;

                return version.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } 
    }
}