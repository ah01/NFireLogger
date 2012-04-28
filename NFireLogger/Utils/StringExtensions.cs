using System;
using System.Security.Cryptography;
using System.Text;

namespace NFireLogger.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Split string in to array by given length
        /// </summary>
        /// <param name="str">string for splitting</param>
        /// <param name="chars">leng of one chunk</param>
        /// <returns>array of chunks</returns>
        public static string[] SplitToArray(this string str, int chars)
        {
            var count = (int) Math.Ceiling((double)str.Length/chars);
            var result = new string[count];
            var ch = Math.Min(chars, str.Length);
            
            for (var i = 0; i < count; i++)
            {
                result[i] = str.Substring(i*chars, ch);
                ch = Math.Min(chars, str.Length - (i+1) * chars);
            }

            return result;
        }


        /// <summary>
        /// Extension alias for string.Format(...)
        /// </summary>
        public static string FormatWith(this string str, params object[] args)
        {
            if (args != null)
            {
                return String.Format(str, args);
            }
            else
            {
                return str;
            }
        }


        /// <summary>
        /// Escape non-ASCII characters with \u#### escape sequence
        /// </summary>
        /// <param name="value">string for escape</param>
        /// <returns>escaped string</returns>
        public static string EncodeNonAsciiCharacters(this string value)
        {
            var sb = new StringBuilder();
            foreach (var c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    var encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Return md5 hash of string in hex format
        /// </summary>
        /// <param name="value">string for hash calculation</param>
        /// <returns>md5 hash</returns>
        public static string ToMd5(this string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytes = Encoding.Default.GetBytes(value);
            
            //var hash = BitConverter.ToString(md5.ComputeHash(bytes));
            var hash = md5.ComputeHash(bytes);

            var s = new StringBuilder();
            foreach (byte b in hash)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }
    }
}
