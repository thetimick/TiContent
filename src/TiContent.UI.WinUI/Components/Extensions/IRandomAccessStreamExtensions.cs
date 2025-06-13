// ⠀
// IRandomAccessStreamExtensions.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 13.06.2025.
// ⠀

using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using TiContent.Foundation.Components.Extensions;
using Windows.Storage.Streams;

namespace TiContent.UI.WinUI.Components.Extensions;

public static class IRandomAccessStreamExtensions
{
    public static async Task<BitmapImage> CreateBitmapAsync(this IRandomAccessStream stream)
    {
        return await new BitmapImage().DoAsync(async image => await image.SetSourceAsync(stream));
    }
}
