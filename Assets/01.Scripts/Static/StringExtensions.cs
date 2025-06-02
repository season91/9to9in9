using System;
using System.Linq;
using UnityEngine;

public static class StringExtensions
{
    
    /// <summary>
    /// [확장메서드] tool-axe 인 경우 ToolAxe 로 변환
    /// </summary>
    public static string KebabToPascal(this string kebab)
    {
        if (string.IsNullOrEmpty(kebab))
            return string.Empty;

        return string.Join("", kebab
                   .Split('-', StringSplitOptions.RemoveEmptyEntries)
                   .Select(word => char.ToUpper(word[0]) + word.Substring(1)));
    }
}