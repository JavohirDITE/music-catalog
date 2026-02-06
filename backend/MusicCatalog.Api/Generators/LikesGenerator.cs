namespace MusicCatalog.Api.Generators;

public static class LikesGenerator
{
    private const int LikesKey = 0x4C494B45;

    public static int Generate(ulong userSeed, string locale, int page, int pageSize, int globalIndex, double likesPerSong)
    {
        ulong likesSeed = SeedMixer.MixLikesSeed(userSeed, locale, page, pageSize, globalIndex, LikesKey);
        var rng = new SplitMix64(likesSeed);

        double L = Math.Clamp(likesPerSong, 0.0, 10.0);
        int baseLikes = (int)Math.Floor(L);
        double probability = L - baseLikes;

        int likes = baseLikes;
        if (rng.NextDouble() < probability)
        {
            likes++;
        }

        return Math.Clamp(likes, 0, 10);
    }
}
