using Bogus;

namespace MusicCatalog.Api.Generators;

public static class ContentGenerator
{
    private static readonly string[] GenresEn = { "Rock", "Pop", "Jazz", "Blues", "Electronic", "Hip-Hop", "Classical", "Country", "R&B", "Metal", "Indie", "Folk", "Reggae", "Soul", "Punk", "House", "Techno", "Disco", "Funk", "Grunge" };
    private static readonly string[] GenresDe = { "Rock", "Pop", "Jazz", "Blues", "Elektronisch", "Hip-Hop", "Klassik", "Country", "R&B", "Metal", "Indie", "Folk", "Reggae", "Soul", "Punk", "House", "Techno", "Disco", "Funk", "Grunge" };

    private static readonly string[] TitleAdjectives = { "Midnight", "Electric", "Golden", "Silver", "Neon", "Velvet", "Crystal", "Burning", "Frozen", "Rising", "Falling", "Dancing", "Dreaming", "Fading", "Shining", "Blazing", "Silent", "Endless", "Eternal", "Wild", "Lost", "Broken", "Sweet", "Bitter", "Dark", "Bright", "Deep", "High", "Last", "First" };
    private static readonly string[] TitleNouns = { "Night", "Star", "Moon", "Sun", "Heart", "Soul", "Fire", "Rain", "Storm", "Wind", "Wave", "Dream", "Shadow", "Light", "Sky", "Ocean", "River", "Mountain", "City", "Road", "Memory", "Kiss", "Touch", "Eyes", "Love", "Life", "Time", "Summer", "Winter", "Dawn" };
    private static readonly string[] TitleVerbs = { "Running", "Flying", "Crying", "Waiting", "Calling", "Breathing", "Falling", "Rising", "Burning", "Shining", "Spinning", "Crashing", "Drifting", "Floating", "Chasing", "Breaking", "Making", "Taking", "Giving", "Leaving" };

    private static readonly string[] TitleAdjectivesDe = { "Mitternachts", "Elektrisch", "Golden", "Silbern", "Neon", "Samt", "Kristall", "Brennend", "Gefroren", "Steigend", "Fallend", "Tanzend", "Träumend", "Verblassend", "Leuchtend", "Lodernd", "Still", "Endlos", "Ewig", "Wild" };
    private static readonly string[] TitleNounsDe = { "Nacht", "Stern", "Mond", "Sonne", "Herz", "Seele", "Feuer", "Regen", "Sturm", "Wind", "Welle", "Traum", "Schatten", "Licht", "Himmel", "Ozean", "Fluss", "Berg", "Stadt", "Weg" };

    private static readonly string[] BandSuffixes = { "Express", "Machine", "Collective", "Project", "Sound", "Crew", "Band", "Group", "Orchestra", "Ensemble", "Alliance", "Union", "Society", "Club", "Connection" };
    private static readonly string[] BandPrefixes = { "The", "Electric", "Neon", "Midnight", "Golden", "Silver", "Velvet", "Crystal", "Atomic", "Cosmic", "Digital", "Analog", "Urban", "Royal", "Imperial" };
    private static readonly string[] BandNouns = { "Wolves", "Tigers", "Eagles", "Lions", "Bears", "Foxes", "Ravens", "Hawks", "Vipers", "Sharks", "Panthers", "Falcons", "Cobras", "Dragons", "Phoenix", "Rebels", "Kings", "Queens", "Knights", "Legends" };

    public static (string Title, string Artist, string Album, string Genre, string Review) Generate(ulong contentSeed, string locale)
    {
        var rng = new SplitMix64(contentSeed);
        int intSeed = rng.NextInt();

        var faker = new Faker(locale == "de-DE" ? "de" : "en");
        faker.Random = new Randomizer(intSeed);
        bool isGerman = locale == "de-DE";

        string title = GenerateTitle(rng, isGerman);
        string artist = GenerateArtist(faker, rng);
        string album = GenerateAlbum(rng, isGerman);
        string[] genres = isGerman ? GenresDe : GenresEn;
        string genre = rng.Choose(genres);
        string review = GenerateReview(rng, title, artist, isGerman);

        return (title, artist, album, genre, review);
    }

    private static string GenerateTitle(SplitMix64 rng, bool isGerman)
    {
        var adjectives = isGerman ? TitleAdjectivesDe : TitleAdjectives;
        var nouns = isGerman ? TitleNounsDe : TitleNouns;

        int style = rng.NextInt(0, 8);
        return style switch
        {
            0 => rng.Choose(adjectives) + " " + rng.Choose(nouns),
            1 => (isGerman ? "Der " : "The ") + rng.Choose(adjectives) + " " + rng.Choose(nouns),
            2 => rng.Choose(TitleVerbs) + " " + (isGerman ? "im " : "in the ") + rng.Choose(nouns),
            3 => rng.Choose(nouns) + " " + (isGerman ? "von " : "of ") + rng.Choose(nouns),
            4 => rng.Choose(adjectives) + " " + rng.Choose(nouns) + " " + rng.Choose(TitleVerbs),
            5 => rng.Choose(nouns),
            6 => rng.Choose(adjectives).ToUpperInvariant() + " " + rng.Choose(nouns).ToUpperInvariant(),
            _ => rng.Choose(TitleVerbs) + " " + rng.Choose(nouns)
        };
    }

    private static string GenerateArtist(Faker faker, SplitMix64 rng)
    {
        int style = rng.NextInt(0, 7);
        return style switch
        {
            0 => faker.Name.FullName(),
            1 => rng.Choose(BandPrefixes) + " " + rng.Choose(BandNouns),
            2 => faker.Name.FirstName() + " & The " + rng.Choose(BandNouns),
            3 => rng.Choose(BandPrefixes) + " " + rng.Choose(BandSuffixes),
            4 => faker.Name.FirstName() + " " + faker.Name.LastName(),
            5 => faker.Name.LastName() + " " + rng.Choose(BandSuffixes),
            _ => "DJ " + faker.Name.FirstName()
        };
    }

    private static string GenerateAlbum(SplitMix64 rng, bool isGerman)
    {
        if (rng.NextDouble() < 0.25)
        {
            return "Single";
        }

        var adjectives = isGerman ? TitleAdjectivesDe : TitleAdjectives;
        var nouns = isGerman ? TitleNounsDe : TitleNouns;

        int style = rng.NextInt(0, 6);
        return style switch
        {
            0 => rng.Choose(adjectives) + " " + rng.Choose(nouns),
            1 => (isGerman ? "Das " : "The ") + rng.Choose(nouns) + " Album",
            2 => rng.Choose(nouns) + " Vol. " + rng.NextInt(1, 10),
            3 => rng.Choose(adjectives) + " " + rng.Choose(adjectives),
            4 => rng.Choose(nouns) + " & " + rng.Choose(nouns),
            _ => rng.Choose(nouns) + " " + rng.NextInt(2018, 2026)
        };
    }

    private static string GenerateReview(SplitMix64 rng, string title, string artist, bool isGerman)
    {
        string[] intros = isGerman
            ? new[] { "Dieser Track", "Das Lied", "Dieses Stück", "Dieser Hit", "Die Single" }
            : new[] { "This track", "The song", "This piece", "This hit", "The single" };

        string[] qualities = isGerman
            ? new[] { "beeindruckt mit", "überzeugt durch", "besticht mit", "fesselt mit", "glänzt durch" }
            : new[] { "impresses with", "convinces with", "captivates with", "shines with", "delivers" };

        string[] aspects = isGerman
            ? new[] { "kraftvollen Melodien", "emotionaler Tiefe", "eingängigen Rhythmen", "atmosphärischen Klängen", "innovativen Sounds", "pulsierenden Beats", "harmonischen Arrangements" }
            : new[] { "powerful melodies", "emotional depth", "catchy rhythms", "atmospheric sounds", "innovative production", "pulsating beats", "harmonic arrangements" };

        string[] moods = isGerman
            ? new[] { "Die Stimmung ist perfekt eingefangen.", "Ein unvergessliches Hörerlebnis.", "Absolut empfehlenswert.", "Ein Muss für jeden Musikfan.", "Pure musikalische Brillanz." }
            : new[] { "The mood is perfectly captured.", "An unforgettable listening experience.", "Highly recommended.", "A must-have for any music fan.", "Pure musical brilliance." };

        string[] closings = isGerman
            ? new[] { "Ein echtes Highlight!", "Sehr gelungen!", "Beeindruckend!", "Fantastisch produziert!", "Absolut hörenswert!" }
            : new[] { "A true highlight!", "Very well done!", "Impressive!", "Fantastically produced!", "Absolutely worth listening!" };

        return $"{rng.Choose(intros)} \"{title}\" by {artist} {rng.Choose(qualities)} {rng.Choose(aspects)}. {rng.Choose(moods)} {rng.Choose(closings)}";
    }
}
