using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;

// ReSharper disable MemberCanBePrivate.Global

namespace TiContent.Foundation.Components.Extensions;

public static class StringExtension
{
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s)
	{
		return string.IsNullOrEmpty(s);
	}
	
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}
	
	public static string Or(this string s, string or)
	{
		return s.IsNullOrWhiteSpace() 
			? or
			: s;
	}
	
	public static string OrEmpty(this string s)
	{
		return s.Or(string.Empty);
	}
}
