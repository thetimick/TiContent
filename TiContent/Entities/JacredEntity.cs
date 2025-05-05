using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using ByteSizeLib;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.Entities;

public record JacredEntity
{
    public enum TrackerEntity
    {
        [Description("bitru")]
        Bitru,
        [Description("kinozal")]
        Kinozal,
        [Description("megapeer")]
        Megapeer,
        [Description("nnmclub")]
        NNMClub,
        [Description("rutor")]
        Rutor,
        [Description("rutracker")]
        Rutracker
    }
    
    [JsonPropertyName("tracker")]
    [JsonConverter(typeof(TrackerConverter))]
    public TrackerEntity? Tracker { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(ByteSizeConverter))]
    public ByteSize Size { get; init; }

    [JsonPropertyName("sizeName")]
    public string? SizeName { get; init; }

    [JsonPropertyName("createTime")]
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime? CreateTime { get; init; }

    [JsonPropertyName("sid")]
    public int? Sid { get; init; }

    [JsonPropertyName("pir")]
    public int? Pir { get; init; }

    [JsonPropertyName("magnet")]
    public string? Magnet { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("originalname")]
    public string? OriginalName { get; init; }

    [JsonPropertyName("relased")]
    public int Released { get; init; }

    [JsonPropertyName("videotype")]
    public string? VideoType { get; init; }
    
    [JsonPropertyName("quality")]
    public int? Quality { get; init; }
    
    [JsonPropertyName("voices")]
    public List<string>? Voices { get; init; }
    
    [JsonPropertyName("seasons")]
    public List<long>? Seasons { get; init; }
    
    [JsonPropertyName("types")]
    public List<string>? Types { get; init; }
}

internal class TrackerConverter : JsonConverter<JacredEntity.TrackerEntity?>
{
    public override JacredEntity.TrackerEntity? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "bitru" => JacredEntity.TrackerEntity.Bitru,
            "kinozal" => JacredEntity.TrackerEntity.Kinozal,
            "megapeer" => JacredEntity.TrackerEntity.Megapeer,
            "nnmclub" => JacredEntity.TrackerEntity.NNMClub,
            "rutor" => JacredEntity.TrackerEntity.Rutor,
            "rutracker" => JacredEntity.TrackerEntity.Rutracker,
            _ => null
        };
    }

    public override void Write(Utf8JsonWriter writer, JacredEntity.TrackerEntity? value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        switch (value.Value)
        {
            case JacredEntity.TrackerEntity.Bitru:
                writer.WriteStringValue("bitru");
                break;
            case JacredEntity.TrackerEntity.Kinozal:
                writer.WriteStringValue("kinozal");
                break;
            case JacredEntity.TrackerEntity.Megapeer:
                writer.WriteStringValue("megapeer");
                break;
            case JacredEntity.TrackerEntity.NNMClub:
                writer.WriteStringValue("nnmclub");
                break;
            case JacredEntity.TrackerEntity.Rutor:
                writer.WriteStringValue("rutor");
                break;
            case JacredEntity.TrackerEntity.Rutracker:
                writer.WriteStringValue("rutracker");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}

public class ByteSizeConverter : JsonConverter<ByteSize>
{
    public override ByteSize Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var size = reader.GetInt64();
        return ByteSize.FromBytes(size);
    }

    public override void Write(Utf8JsonWriter writer, ByteSize value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Bytes);
    }
}

public class DateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } value)
            return DateTime.Parse(value);
        return null;
    }
    
    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value?.ToString("yyyy-MM-ddTHH:mm:ss"));
    }
}
