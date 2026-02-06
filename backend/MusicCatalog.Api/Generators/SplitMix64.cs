namespace MusicCatalog.Api.Generators;

public class SplitMix64
{
    private ulong _state;

    public SplitMix64(ulong seed)
    {
        _state = seed;
    }

    public ulong Next()
    {
        ulong z = (_state += 0x9E3779B97F4A7C15UL);
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    public int NextInt()
    {
        return (int)(Next() >> 33);
    }

    public int NextInt(int min, int max)
    {
        if (min >= max) return min;
        ulong range = (ulong)(max - min);
        return min + (int)(Next() % range);
    }

    public double NextDouble()
    {
        return (Next() >> 11) * (1.0 / (1UL << 53));
    }

    public T Choose<T>(T[] items)
    {
        return items[NextInt(0, items.Length)];
    }

    public T Choose<T>(List<T> items)
    {
        return items[NextInt(0, items.Count)];
    }
}
