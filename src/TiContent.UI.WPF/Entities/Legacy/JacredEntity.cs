using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Humanizer;
using Humanizer.Bytes;
using TiContent.UI.WPF.Components.Extensions;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.UI.WPF.Entities.Legacy;

public record JacredEntity
{
    public enum VideoTypeEntity
    {
        [Description("SDR")]
        SDR,

        [Description("HDR")]
        HDR,

        [Description("n/n")]
        None,
    }

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
        Rutracker,

        [Description("n/n")]
        None,
    }

    [JsonPropertyName("tracker")]
    [JsonConverter(typeof(TrackerConverter))]
    public TrackerEntity Tracker { get; init; }

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
    [JsonConverter(typeof(VideoTypeConverter))]
    public VideoTypeEntity VideoType { get; init; }

    [JsonPropertyName("quality")]
    public int? Quality { get; init; }

    [JsonPropertyName("voices")]
    public List<string>? Voices { get; init; }

    [JsonPropertyName("seasons")]
    public List<long>? Seasons { get; init; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; init; }

    public string AggregatedDescription => MakeDescription();

    private string MakeDescription()
    {
        var builder = new StringBuilder();

        builder.Append(Tracker.Humanize());
        if (!Types.IsEmpty())
            builder.Append($" · {Types.Humanize()}");
        if (!Seasons.IsEmpty())
            builder.Append($" · сезон(-ы): {Seasons.Humanize()}");
        if (!Voices.IsEmpty())
            builder.Append($" · озвучка(-и): {Voices.Humanize()}");

        return builder.ToString();
    }
}

internal class TrackerConverter : JsonConverter<JacredEntity.TrackerEntity>
{
    public override JacredEntity.TrackerEntity Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString()?.Trim().Humanize(LetterCasing.LowerCase);
        return value switch
        {
            "bitru" => JacredEntity.TrackerEntity.Bitru,
            "kinozal" => JacredEntity.TrackerEntity.Kinozal,
            "megapeer" => JacredEntity.TrackerEntity.Megapeer,
            "nnmclub" => JacredEntity.TrackerEntity.NNMClub,
            "rutor" => JacredEntity.TrackerEntity.Rutor,
            "rutracker" => JacredEntity.TrackerEntity.Rutracker,
            _ => JacredEntity.TrackerEntity.None,
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        JacredEntity.TrackerEntity value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}

public class ByteSizeConverter : JsonConverter<ByteSize>
{
    public override ByteSize Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var size = reader.GetInt64();
        return ByteSize.FromBytes(size);
    }

    public override void Write(Utf8JsonWriter writer, ByteSize value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class DateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.GetString() is { } value)
            return DateTime.Parse(value);
        return null;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime? value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}

internal class VideoTypeConverter : JsonConverter<JacredEntity.VideoTypeEntity>
{
    public override JacredEntity.VideoTypeEntity Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString()?.Trim().Humanize(LetterCasing.LowerCase);
        return value switch
        {
            "sdr" => JacredEntity.VideoTypeEntity.SDR,
            "hdr" => JacredEntity.VideoTypeEntity.HDR,
            _ => JacredEntity.VideoTypeEntity.None,
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        JacredEntity.VideoTypeEntity value,
        JsonSerializerOptions options
    )
    {
        throw new NotImplementedException();
    }
}
