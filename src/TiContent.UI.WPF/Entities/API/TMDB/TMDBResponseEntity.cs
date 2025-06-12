// ⠀
// TMDBEntity.cs
// TiContent.UI.WPF
//
// Created by the_timick on 12.05.2025.
// ⠀

using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace TiContent.UI.WPF.Entities.API.TMDB;

public record TMDBResponseEntity
{
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    [JsonPropertyName("results")]
    public List<ItemEntity>? Results { get; set; }

    [JsonPropertyName("total_pages")]
    public long? TotalPages { get; set; }

    [JsonPropertyName("total_results")]
    public long? TotalResults { get; set; }

    public record ItemEntity
    {
        [JsonPropertyName("adult")]
        public bool? Adult { get; set; }

        [JsonPropertyName("genre_ids")]
        public List<long>? GenreIds { get; set; }

        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("original_language")]
        public string? OriginalLanguage { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("original_title")]
        public string? OriginalTitle { get; set; }

        [JsonPropertyName("names")]
        public List<string>? Names { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("release_date")]
        public DateTimeOffset? ReleaseDate { get; set; }

        [JsonPropertyName("video")]
        public bool Video { get; set; }

        [JsonPropertyName("vote_average")]
        public double? VoteAverage { get; set; }

        [JsonPropertyName("vote_count")]
        public long? VoteCount { get; set; }

        [JsonPropertyName("popularity")]
        public double? Popularity { get; set; }

        [JsonPropertyName("PG")]
        public long? Pg { get; set; }

        [JsonPropertyName("release_quality")]
        public string? ReleaseQuality { get; set; }

        [JsonPropertyName("imdb_id")]
        public string? IMDBId { get; set; }

        [JsonPropertyName("backdrop_path")]
        public string? BackdropPath { get; set; }

        [JsonPropertyName("kinopoisk_id")]
        public string? KinopoiskId { get; set; }

        [JsonPropertyName("imdb_rating")]
        public string? ImdbRating { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("original_name")]
        public string? OriginalName { get; set; }

        [JsonPropertyName("origin_country")]
        public List<string>? OriginCountry { get; set; }

        [JsonPropertyName("kp_rating")]
        public string? KpRating { get; set; }
    }
}
