using System;
using System.Collections.Generic;
using System.Linq;

namespace PEengineersCAN
{
    
    /// <summary>
    /// Provides extension methods for safer and more convenient string manipulation.
    /// <para>
    /// The <see cref="StringExtension"/> class offers utility methods to handle common string operations,
    /// such as null-safe trimming and splitting with cleanup of results. These extensions simplify dealing
    /// with null or poorly formatted string input, returning predictable results and reducing the need
    /// for repetitive null checks or post-processing.
    /// </para>
    /// </summary>

    public static class StringExtension
    {
        /// <summary>
        /// Safely trims a string, handling null values by returning empty string
        /// </summary>
        public static string Trim(this string str)
        {
            if (str == null)
                return string.Empty;
                
            return str.Trim();
        }

        /// <summary>
        /// Splits a string by delimiter, trims each part, and removes empty entries
        /// </summary>
        public static List<string> Split(this string str, char delimiter)
        {
            if (str == null)
                throw new NullReferenceException();

            if (string.IsNullOrEmpty(str))
                return new List<string>();  // Return empty list for empty string

            // Split by delimiter, trim each part, and filter out empty entries
            return str.Split(delimiter)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }
    }
}