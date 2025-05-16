// ⠀
// TMDBSearchRequestEntity.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

namespace TiContent.Entities.TMDB.Requests;

public record TMDBSearchRequestEntity
{
    public string Query { get; set; } = string.Empty;
    public ContentType Content { get; set; }
    public long Page { get; set; }

    public string RawContent {
        get
        {
            return Content switch
            {
                ContentType.Movies => "movie",
                ContentType.Serials => "tv",
                ContentType.Anime => "anime",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum ContentType
    {
        Movies, 
        Serials,
        Anime
    }
}