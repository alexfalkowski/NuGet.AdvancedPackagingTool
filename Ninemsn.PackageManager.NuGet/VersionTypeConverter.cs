namespace Ninemsn.PackageManager.NuGet
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

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
                Version version;

                if (Version.TryParse(versionAsString, out version))
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
                var parts = 4;
                var version = (Version)value;

                if (version.Revision == -1)
                {
                    parts--;
                }

                if (version.Build == -1)
                {
                    parts--;
                }

                return version.ToString(parts);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        } 
    }
}