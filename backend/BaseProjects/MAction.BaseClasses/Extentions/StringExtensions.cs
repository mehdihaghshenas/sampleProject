using System;

namespace MAction.BaseClasses.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static string En2Fa(this string str)
        {
            throw new NotImplementedException();

        }

        public static string Fa2En(this string str)
        {
            throw new NotImplementedException();
        }

        public static string FixPersianChars(this string str)
        {
            throw new NotImplementedException();
        }

        public static string CleanString(this string str)
        {
            return str.Trim().FixPersianChars().Fa2En().NullIfEmpty();
        }

        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        public static bool IsValidUri(this string uri)
        {
            // just so the validation passes if the uri is not required / nullable
            return string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _);
        }
    }
}
