// ⠀
// OpenLinkHelper.cs
// TiContent.Foundation.Components
//
// Created by the_timick on 16.05.2025.
// ⠀

using System.Diagnostics;
using System.Web;

namespace TiContent.Foundation.Components.Helpers;

public static class OpenLinkHelper
{
    public enum Type
    {
        Google,
        Rutracker,
        Rutor
    }

    public static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
    }

    public static void OpenUrlForSearch(string query, Type type)
    {
        var encoded = HttpUtility.UrlEncode(query);
        switch (type)
        {
            case Type.Google:
                OpenUrl($"https://www.google.com/search?q={encoded}");
            break;
            case Type.Rutracker:
                OpenUrl($"https://rutracker.org/forum/tracker.php?nm={encoded}");
            break;
            case Type.Rutor:
                OpenUrl($"https://rutor.info/search/{encoded}");
            break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
