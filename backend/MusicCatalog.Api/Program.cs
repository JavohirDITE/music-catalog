using MusicCatalog.Api.Generators;
using MusicCatalog.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/songs", (string locale, ulong seed, double likes, int page, int pageSize) =>
{
    locale = locale == "de-DE" ? "de-DE" : "en-US";
    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 100);

    var items = new List<SongDto>();

    for (int i = 0; i < pageSize; i++)
    {
        int globalIndex = (page - 1) * pageSize + i + 1;
        ulong contentSeed = SeedMixer.MixContentSeed(seed, locale, page, pageSize, globalIndex);

        var (title, artist, album, genre, review) = ContentGenerator.Generate(contentSeed, locale);
        int likesCount = LikesGenerator.Generate(seed, locale, page, pageSize, globalIndex, likes);

        items.Add(new SongDto
        {
            Index = globalIndex,
            Title = title,
            Artist = artist,
            Album = album,
            Genre = genre,
            Likes = likesCount,
            Review = review,
            CoverUrl = $"/api/cover?locale={locale}&seed={seed}&page={page}&pageSize={pageSize}&index={globalIndex}",
            PreviewUrl = $"/api/preview?locale={locale}&seed={seed}&page={page}&pageSize={pageSize}&index={globalIndex}"
        });
    }

    return Results.Ok(new SongsResponse
    {
        Page = page,
        PageSize = pageSize,
        Items = items
    });
});

app.MapGet("/api/cover", (string locale, ulong seed, int page, int pageSize, int index) =>
{
    ulong contentSeed = SeedMixer.MixContentSeed(seed, locale, page, pageSize, index);
    var (title, artist, _, _, _) = ContentGenerator.Generate(contentSeed, locale);
    string svg = CoverGenerator.Generate(contentSeed, title, artist);
    return Results.Content(svg, "image/svg+xml");
});

app.MapGet("/api/preview", (string locale, ulong seed, int page, int pageSize, int index) =>
{
    ulong contentSeed = SeedMixer.MixContentSeed(seed, locale, page, pageSize, index);
    byte[] wav = AudioGenerator.Generate(contentSeed);
    return Results.File(wav, "audio/wav", "preview.wav");
});

app.MapGet("/health", () => Results.Ok("healthy"));

app.MapFallbackToFile("index.html");

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");
