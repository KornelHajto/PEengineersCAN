using System;
using System.Collections.Generic;
using System.Linq;

namespace PEengineersCAN;

public static class StringExtension
{
    /// <summary>
    /// Trims whitespace from the beginning and end of a string with null/empty check.
    /// </summary>
    /// <param name="str">The string to trim</param>
    /// <returns>The trimmed string, or empty string if input is null or empty</returns>
    public static string Trim(this string str)
    {
        return string.IsNullOrEmpty(str) ? string.Empty : str.Trim();
    }

    /// <summary>
    /// Splits a string into a list of strings using the specified delimiter.
    /// </summary>
    /// <param name="s">The string to split</param>
    /// <param name="delimiter">The character that delimits the elements in the string</param>
    /// <returns>
    /// A list of strings that:
    /// - Contains the substrings from the input string that are delimited by the delimiter
    /// - Has empty entries removed
    /// - Has each entry trimmed of whitespace
    /// - Excludes any entries that become empty after trimming
    /// </returns>
    public static List<string> Split(this string s, char delimiter)
    {
        return s.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
            .Select(token => token.Trim())
            .Where(token => !string.IsNullOrEmpty(token))
            .ToList();
    }
}