using System;

namespace MAction.BaseClasses.Extentions
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
    }
}
