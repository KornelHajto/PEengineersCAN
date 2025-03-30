using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PEengineersCAN
{
    public static class Utils
    {
        /// <summary>
        /// Safely parses a string to the specified numeric type with error handling.
        /// </summary>
        /// <typeparam name="T">The target numeric type (int, uint, byte, double, etc.)</typeparam>
        /// <param name="str">The string to parse</param>
        /// <param name="hex">If true, treats the string as a hexadecimal value</param>
        /// <returns>The parsed value converted to type T</returns>
        /// <exception cref="ArgumentException">Thrown when the string is empty, in an invalid format, or the type T is not supported</exception>
        public static T SafeParse<T>(string str, bool hex = false)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("Empty string");

            Type type = typeof(T);

            // Check for unsupported types first
            if (type == typeof(bool) || type == typeof(string) || !type.IsPrimitive)
                throw new ArgumentException($"Unsupported type: {type.Name}");

            try
            {
                if (hex)
                {
                    if (type == typeof(uint))
                        return (T)Convert.ChangeType(Convert.ToUInt64(str, 16), typeof(T));
                    else if (type == typeof(int))
                        return (T)Convert.ChangeType(Convert.ToInt32(str, 16), typeof(T));
                    else if (type == typeof(long))
                        return (T)Convert.ChangeType(Convert.ToInt64(str, 16), typeof(T));
                    else if (type == typeof(ulong))
                        return (T)Convert.ChangeType(Convert.ToUInt64(str, 16), typeof(T));
                    else if (type == typeof(byte))
                        return (T)Convert.ChangeType(Convert.ToByte(str, 16), typeof(T));
                }
                else if (type == typeof(double) || type == typeof(float))
                {
                    return (T)Convert.ChangeType(double.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }
                else if (type == typeof(int))
                {
                    return (T)Convert.ChangeType(int.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }
                else if (type == typeof(long))
                {
                    return (T)Convert.ChangeType(long.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }
                else if (type == typeof(byte))
                {
                    return (T)Convert.ChangeType(byte.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }
                else if (type == typeof(uint))
                {
                    return (T)Convert.ChangeType(uint.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }
                else if (type == typeof(ulong))
                {
                    return (T)Convert.ChangeType(ulong.Parse(str, CultureInfo.InvariantCulture), typeof(T));
                }

                // Unreachable code - kept for defensive programming, I tried so hard to trigger it in unit test, but I couldn't
                throw new ArgumentException($"Unsupported type: {type.Name}");
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid number format: {str}", ex);
            }
        }

        /// <summary>
        /// Converts a hexadecimal string representation to a byte array.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert</param>
        /// <returns>A byte array containing the converted values</returns>
        /// <remarks>
        /// This method:
        /// - Removes whitespace from the input string
        /// - Strips surrounding quotes if present
        /// - Adds a leading zero if the hex string has an odd length
        /// - Converts each pair of hex characters to a byte value
        /// </remarks>
        public static byte[] HexToBytes(string hex)
        {
            string cleanHex = Regex.Replace(hex.Trim(), @"\s+", "");

            if (cleanHex.Length >= 2 && cleanHex[0] == '"' && cleanHex[cleanHex.Length - 1] == '"')
            {
                cleanHex = cleanHex.Substring(1, cleanHex.Length - 2);
            }

            if (cleanHex.Length == 0)
            {
                return new byte[0];
            }

            if (cleanHex.Length % 2 != 0)
            {
                cleanHex = "0" + cleanHex;
            }

            byte[] bytes = new byte[cleanHex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string byteStr = cleanHex.Substring(i * 2, 2);
                bytes[i] = SafeParse<byte>(byteStr, true);
            }

            return bytes;
        }
    }
}