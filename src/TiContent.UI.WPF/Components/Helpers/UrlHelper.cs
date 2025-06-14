// ⠀
// UrlHelper.cs
// TiContent.UI.WPF
//
// Created by the_timick on 13.05.2025.
// ⠀

using System.Web;

namespace TiContent.UI.WPF.Components.Helpers;

public static class UrlHelper
{
    /// <summary>
    /// Combines a base URL with path segments and optional query parameters into a valid URL.
    /// </summary>
    /// <param name "baseUrl">The base URL (e.g., "https://example.com").</param>
    /// <param name "segments">Path segments to append (e.g., "api", "resource").</param>
    /// <returns>A combined URL string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if baseUrl is null or empty.</exception>
    /// <exception cref="UriFormatException">Thrown if baseUrl is invalid.</exception>
    public static string Combine(string baseUrl, params string[] segments)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentNullException(nameof(baseUrl));

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
        {
            if (!Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out baseUri))
                throw new UriFormatException("Invalid base URL format.");
        }

        var cleanedBase = baseUri.GetLeftPart(UriPartial.Path).TrimEnd('/');

        var cleanedSegments = segments
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim('/'))
            .Where(s => !string.IsNullOrEmpty(s));

        var combinedPath = string.Join("/", cleanedSegments);

        var result = cleanedBase;
        if (!string.IsNullOrEmpty(combinedPath))
            result += "/" + combinedPath;

        if (!string.IsNullOrEmpty(baseUri.Query))
            result += baseUri.Query;

        return result;
    }

    /// <summary>
    /// Combines a base URL with path segments and query parameters.
    /// </summary>
    /// <param name "baseUrl">The base URL (e.g., "https://example.com").</param>
    /// <param name "queryParams">Query parameters as key-value pairs (e.g., ("id", "123")).</param>
    /// <param name "segments">Path segments to append (e.g., "api", "resource").</param>
    /// <returns>A combined URL string with query parameters.</returns>
    public static string CombineWithQuery(string baseUrl, (string key, string value)[]? queryParams, params string[] segments)
    {
        var url = Combine(baseUrl, segments);

        if (queryParams == null || queryParams.Length == 0)
            return url;

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        foreach (var (key, value) in queryParams)
            if (!string.IsNullOrWhiteSpace(key))
                queryString[key] = value;

        if (queryString.Count > 0)
            url += "?" + queryString;

        return url;
    }
}
