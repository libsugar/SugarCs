using System;
using System.Collections.Generic;
using System.Text;

namespace LibSugar;

public static partial class Sugar
{
    /// <summary>Indicates whether a specified string is <c>null</c>, empty, or consists only of white-space characters.</summary>
    public static bool IsNullOrWhiteSpace(this string s) => string.IsNullOrWhiteSpace(s);
    /// <summary>Indicates whether the specified string is <c>null</c> or an empty string ("").</summary>
    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
    /// <summary>Retrieves the system's reference to the specified <see cref="string"/>.</summary>
    public static string Intern(this string s) => string.Intern(s);
    /// <summary>Retrieves a reference to a specified <see cref="string"/>.</summary>
    public static string? IsInterned(this string s) => string.IsInterned(s);
}
