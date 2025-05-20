using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PEengineersCAN
{
    /// <summary>
    /// Utility class providing methods for parsing numbers and converting hex strings
    /// to byte arrays with error handling.
    /// </summary>
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

            // Check for unsupported types first - maintain original error message format
            if (type == typeof(bool) || type == typeof(string) || !type.IsPrimitive)
                throw new ArgumentException($"Unsupported type: {type.Name}");

            if (!TryParse(str, out T result, hex))
                throw new ArgumentException($"Invalid number format: {str}");

            return result;
        }

        /// <summary>
        /// Safely parses a string span to the specified numeric type without allocations.
        /// </summary>
        /// <typeparam name="T">The target numeric type (int, uint, byte, double, etc.)</typeparam>
        /// <param name="str">The string span to parse</param>
        /// <param name="hex">If true, treats the string as a hexadecimal value</param>
        /// <returns>The parsed value converted to type T</returns>
        /// <exception cref="ArgumentException">Thrown when the span is empty, in an invalid format, or the type T is not supported</exception>
        public static T SafeParse<T>(ReadOnlySpan<char> str, bool hex = false)
        {
            if (str.IsEmpty)
                throw new ArgumentException("Empty string");

            Type type = typeof(T);

            // Check for unsupported types first - maintain original error message format
            if (type == typeof(bool) || type == typeof(string) || !type.IsPrimitive)
                throw new ArgumentException($"Unsupported type: {type.Name}");

            if (!TryParseInternal(str, out T result, hex))
                throw new ArgumentException($"Invalid number format: {new string(str)}");

            return result;
        }

        /// <summary>
        /// Tries to parse a string to the specified numeric type.
        /// </summary>
        /// <typeparam name="T">The target numeric type</typeparam>
        /// <param name="str">The string to parse</param>
        /// <param name="result">When this method returns, contains the parsed value if successful</param>
        /// <param name="hex">If true, treats the string as a hexadecimal value</param>
        /// <returns>True if parsing was successful; otherwise, false</returns>
        public static bool TryParse<T>(string str, out T result, bool hex = false)
        {
            if (string.IsNullOrEmpty(str))
            {
                result = default;
                return false;
            }

            Type type = typeof(T);

            // Check for unsupported types to match original behavior
            if (type == typeof(bool) || type == typeof(string) || !type.IsPrimitive)
            {
                result = default;
                return false;
            }

            return TryParseInternal(str.AsSpan(), out result, hex);
        }

        /// <summary>
        /// Tries to parse a string span to the specified numeric type without allocations.
        /// </summary>
        /// <typeparam name="T">The target numeric type</typeparam>
        /// <param name="str">The string span to parse</param>
        /// <param name="result">When this method returns, contains the parsed value if successful</param>
        /// <param name="hex">If true, treats the string as a hexadecimal value</param>
        /// <returns>True if parsing was successful; otherwise, false</returns>
        public static bool TryParse<T>(ReadOnlySpan<char> str, out T result, bool hex = false)
        {
            if (str.IsEmpty)
            {
                result = default;
                return false;
            }

            Type type = typeof(T);

            // Check for unsupported types to match original behavior
            if (type == typeof(bool) || type == typeof(string) || !type.IsPrimitive)
            {
                result = default;
                return false;
            }

            return TryParseInternal(str, out result, hex);
        }

        /// <summary>
        /// Internal implementation of TryParse that assumes type checking has already been done.
        /// </summary>
        private static bool TryParseInternal<T>(ReadOnlySpan<char> str, out T result, bool hex = false)
        {
            result = default;
            Type type = typeof(T);

            try
            {
                if (hex)
                {
                    if (type == typeof(uint))
                    {
                        if (uint.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        if (int.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        if (long.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        if (ulong.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ulong val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        if (byte.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(short))
                    {
                        if (short.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out short val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        if (ushort.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        if (sbyte.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out sbyte val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                }
                else
                {
                    if (type == typeof(double))
                    {
                        if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(float))
                    {
                        if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out float val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out int val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(long))
                    {
                        if (long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out long val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        if (byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(uint))
                    {
                        if (uint.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(ulong))
                    {
                        if (ulong.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(short))
                    {
                        if (short.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out short val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(ushort))
                    {
                        if (ushort.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                    else if (type == typeof(sbyte))
                    {
                        if (sbyte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte val))
                        {
                            result = (T)(object)val;
                            return true;
                        }
                    }
                }
                
                return false;
            }
            catch
            {
                return false;
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
            if (string.IsNullOrEmpty(hex))
                return Array.Empty<byte>();

            return HexToBytes(hex.AsSpan());
        }

        /// <summary>
        /// Converts a hexadecimal string span to a byte array without string allocations.
        /// </summary>
        /// <param name="hex">The hexadecimal string span to convert</param>
        /// <returns>A byte array containing the converted values</returns>
        public static byte[] HexToBytes(ReadOnlySpan<char> hex)
        {
            if (hex.IsEmpty)
                return Array.Empty<byte>();

            // Trim whitespace from start and end
            hex = hex.Trim();
            
            // Remove all whitespace for consistent processing
            Span<char> cleanHex = stackalloc char[hex.Length];
            int cleanLength = 0;
            
            for (int i = 0; i < hex.Length; i++)
            {
                if (!char.IsWhiteSpace(hex[i]))
                {
                    cleanHex[cleanLength++] = hex[i];
                }
            }
            
            cleanHex = cleanHex.Slice(0, cleanLength);
            
            // Handle quoted strings
            if (cleanLength >= 2 && cleanHex[0] == '"' && cleanHex[cleanLength - 1] == '"')
            {
                cleanHex = cleanHex.Slice(1, cleanLength - 2);
                cleanLength -= 2;
            }
            
            if (cleanLength == 0)
                return Array.Empty<byte>();

            // If odd length, add a leading zero
            bool oddLength = cleanLength % 2 != 0;
            int resultLength = oddLength ? (cleanLength + 1) / 2 : cleanLength / 2;
            byte[] result = new byte[resultLength];
            
            // Parse the hex characters
            for (int i = 0; i < resultLength; i++)
            {
                int hexIndex = i * 2;
                
                // If odd length and first byte, we need to handle the single hex character
                if (oddLength && i == 0)
                {
                    if (!TryHexCharToByte(cleanHex[0], out result[0]))
                    {
                        // Match original error message format
                        throw new ArgumentException($"Invalid number format: {cleanHex[0]}");
                    }
                    continue;
                }
                
                // Adjust index if we have an odd length string
                if (oddLength)
                    hexIndex -= 1;
                
                ReadOnlySpan<char> byteStr = cleanHex.Slice(hexIndex, 2);
                if (!TryHexPairToByte(byteStr, out result[i]))
                {
                    // Match original error message format
                    throw new ArgumentException($"Invalid number format: {new string(byteStr)}");
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Tries to convert a single hex character to its corresponding byte value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryHexCharToByte(char hex, out byte result)
        {
            result = 0;
            
            if (hex >= '0' && hex <= '9')
                result = (byte)(hex - '0');
            else if (hex >= 'a' && hex <= 'f')
                result = (byte)(hex - 'a' + 10);
            else if (hex >= 'A' && hex <= 'F')
                result = (byte)(hex - 'A' + 10);
            else
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Tries to convert a pair of hex characters to a byte.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryHexPairToByte(ReadOnlySpan<char> hexPair, out byte result)
        {
            result = 0;
            
            if (hexPair.Length != 2)
                return false;
                
            if (!TryHexCharToByte(hexPair[0], out byte highNibble) || 
                !TryHexCharToByte(hexPair[1], out byte lowNibble))
                return false;
                
            result = (byte)((highNibble << 4) | lowNibble);
            return true;
        }
    }
}