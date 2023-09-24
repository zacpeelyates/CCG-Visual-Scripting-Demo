// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: StringUtils.cs
// Modified: 2023/05/13 @ 22:29

#region

using System.Linq;
using System.Text.RegularExpressions;

#endregion

public static class StringUtils
{
    public static string RemoveString(this string str, params string[] remove)
    {
        return remove.Aggregate(str, (current, s) => current.Replace(s, string.Empty));
    }

    public static string[] SplitOnCapitals(this string str)
    {
        return Regex.Split(str, @"(?<=[a-z])(?=[A-Z])");
    }
}