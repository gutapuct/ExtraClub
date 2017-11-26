using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ExtraClub.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string CalculateSHA1(this string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }

        public static string[] SplitByLen(this string text, int length)
        {
            var res = new List<string>();
            for (int i = 0; i < text.Length / length + 1; i++)
            {
                if ((i + 1) * length > text.Length)
                {
                    res.Add(text.Substring(i * length, text.Length - i * length));
                }
                else
                {
                    res.Add(text.Substring(i * length, length));
                }
            }
            return res.ToArray();
        }

        public static string Log(this string original, string str, params object[] parameters)
        {
            Logger.Log(str, parameters);
            return original + "\n" + String.Format(str, parameters);
        }

        public static bool In<TSource>(this TSource number, params TSource[] values)
    where TSource : struct
        {
            return values.Contains(number);
        }

        public static bool In(this string str, params string[] values)
        {
            return values.Contains(str);
        }
    }
}
