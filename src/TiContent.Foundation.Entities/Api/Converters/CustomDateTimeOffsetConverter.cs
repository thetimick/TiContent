// ⠀
// CustomDateTimeOffsetConverter.cs
// TiContent.UI.WPF.Entities
//
// Created by the_timick on 04.06.2025.
// ⠀

using System.Text.Json;
using System.Text.Json.Serialization;

namespace TiContent.Foundation.Entities.Api.Converters;

public class CustomDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (DateTimeOffset.TryParse(value, out var result))
            return result;
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString("O"));
        else
            writer.WriteNullValue();
    }
}