// ⠀
// OpenUrlHelper.cs
// TiContent.UI.WPF
// 
// Created by the_timick on 16.05.2025.
// ⠀

using System.Diagnostics;

namespace TiContent.UI.WPF.Components.Helpers;

public static class OpenHelper
{
    public static void OpenUrl(string url)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = url, 
                UseShellExecute = true
            }
        );
    }
}