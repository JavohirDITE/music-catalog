using System.Text;

namespace MusicCatalog.Api.Generators;

public static class SeedMixer
{
    private const ulong FnvOffsetBasis = 14695981039346656037UL;
    private const ulong FnvPrime = 1099511628211UL;

    public static ulong MixContentSeed(ulong userSeed, string locale, int page, int pageSize, int globalIndex)
    {
        ulong hash = FnvOffsetBasis;
        hash = HashBytes(hash, BitConverter.GetBytes(userSeed));
        hash = HashBytes(hash, Encoding.UTF8.GetBytes(locale));
        hash = HashBytes(hash, BitConverter.GetBytes(page));
        hash = HashBytes(hash, BitConverter.GetBytes(pageSize));
        hash = HashBytes(hash, BitConverter.GetBytes(globalIndex));
        return hash;
    }

    public static ulong MixLikesSeed(ulong userSeed, string locale, int page, int pageSize, int globalIndex, int likesKey)
    {
        ulong hash = FnvOffsetBasis;
        hash = HashBytes(hash, BitConverter.GetBytes(userSeed));
        hash = HashBytes(hash, Encoding.UTF8.GetBytes(locale));
        hash = HashBytes(hash, BitConverter.GetBytes(page));
        hash = HashBytes(hash, BitConverter.GetBytes(pageSize));
        hash = HashBytes(hash, BitConverter.GetBytes(globalIndex));
        hash = HashBytes(hash, BitConverter.GetBytes(likesKey));
        return hash;
    }

    private static ulong HashBytes(ulong hash, byte[] bytes)
    {
        foreach (byte b in bytes)
        {
            hash ^= b;
            hash *= FnvPrime;
        }
        return hash;
    }
}
