﻿namespace NuGet.AdvancedPackagingTool.Core
{
    using System;

    public static class StringExtensions
    {
        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(source))
            {
                return true;
            }

            return source.IndexOf(value, comparison) >= 0;
        }

        public static bool IsUri(this string value)
        {
            Uri uri;
            return Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri);
        }
    }
}