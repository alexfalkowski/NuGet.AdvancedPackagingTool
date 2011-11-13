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
                var parts = versionAsString.Trim().Split(new[] { '.' });

                Version version;
                switch (parts.Length)
                {
                    case 1:
                        version = new Version(Convert.ToInt32(parts[0], CultureInfo.InvariantCulture), 0, 0, 0);
                        break;

                    case 2:
                        version = new Version(
                            Convert.ToInt32(parts[0], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[1], CultureInfo.InvariantCulture),
                            0,
                            0);
                        break;

                    case 3:
                        version = new Version(
                            Convert.ToInt32(parts[0], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[1], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[2], CultureInfo.InvariantCulture),
                            0);
                        break;

                    default:
                        version = new Version(
                            Convert.ToInt32(parts[0], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[1], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[2], CultureInfo.InvariantCulture),
                            Convert.ToInt32(parts[3], CultureInfo.InvariantCulture));
                        break;
                }

                return version;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var parts = 4;

            if (destinationType == typeof(string) && value != null)
            {
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