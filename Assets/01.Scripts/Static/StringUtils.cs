using System;
using System.Linq;
using UnityEngine;

public static class StringUtils
{
    public static string KebabToPascal(string kebab)
    {
        if (string.IsNullOrEmpty(kebab))
            return string.Empty;

        return string.Join("", kebab
                   .Split('-', StringSplitOptions.RemoveEmptyEntries)
                   .Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }
}