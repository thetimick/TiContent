// ⠀
// TMDBTrendingRequestEntity.cs
// TiContent
// 
// Created by the_timick on 13.05.2025.
// ⠀

namespace TiContent.Entities.TMDB.Requests;

public record TMDBTrendingRequestEntity
{
    public ContentType Content { get; init; }
    public PeriodType Period { get; set; }

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
    
    public string RawRange {
        get
        {
            return Period switch
            {
                PeriodType.Day => "day",
                PeriodType.Week => "week",
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
    
    public enum PeriodType
    {
        Day,
        Week
    }
};