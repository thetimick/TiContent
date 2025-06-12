// ⠀
// ByteArrayExtensions.cs
// TiContent.UI.WinUI
//
// Created by the_timick on 07.06.2025.
// ⠀

using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace TiContent.UI.WinUI.Components.Extensions;

public static class ByteArrayExtensions
{
    public static async Task<IRandomAccessStream> ToRandomAccessStreamAsync(this byte[] byteArray)
    {
        var randomAccessStream = new InMemoryRandomAccessStream();
        using (var dataWriter = new DataWriter(randomAccessStream.GetOutputStreamAt(0)))
        {
            dataWriter.WriteBytes(byteArray);
            await dataWriter.StoreAsync();
            dataWriter.DetachStream();
        }
        randomAccessStream.Seek(0);
        return randomAccessStream;
    }
}
