namespace MusicCatalog.Api.Models;

public class SongDto
{
    public int Index { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Likes { get; set; }
    public string Review { get; set; } = string.Empty;
    public string CoverUrl { get; set; } = string.Empty;
    public string PreviewUrl { get; set; } = string.Empty;
}

public class SongsResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SongDto> Items { get; set; } = new();
}
