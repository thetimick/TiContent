// ⠀
// UrlHelper.cs
// TiContent.UI.WPF
//
// Created by the_timick on 13.05.2025.
// ⠀

namespace TiContent.Foundation.Components.Helpers;

public static class UrlHelper
{
    public static string Combine(string baseUrl, params string[] segments)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentNullException(nameof(baseUrl));

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            if (!Uri.TryCreate(baseUrl, UriKind.RelativeOrAbsolute, out baseUri))
                throw new UriFormatException("Invalid base URL format.");

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
}