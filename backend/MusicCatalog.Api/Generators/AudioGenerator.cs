namespace MusicCatalog.Api.Generators;

public static class AudioGenerator
{
    private const int SampleRate = 44100;
    private const int Channels = 1;
    private const int BitsPerSample = 16;

    private static readonly double[] MajorScale = { 261.63, 293.66, 329.63, 349.23, 392.00, 440.00, 493.88, 523.25 };
    private static readonly double[] MinorScale = { 261.63, 293.66, 311.13, 349.23, 392.00, 415.30, 466.16, 523.25 };
    private static readonly double[] PentatonicScale = { 261.63, 293.66, 329.63, 392.00, 440.00, 523.25 };

    public static byte[] Generate(ulong contentSeed)
    {
        var rng = new SplitMix64(contentSeed);

        double duration = 8.0 + rng.NextDouble() * 4.0;
        int totalSamples = (int)(SampleRate * duration);

        double[] scale = rng.NextInt(0, 3) switch
        {
            0 => MajorScale,
            1 => MinorScale,
            _ => PentatonicScale
        };

        double bpm = 80 + rng.NextDouble() * 80;
        double beatDuration = 60.0 / bpm;

        int noteCount = (int)(duration / beatDuration);
        var notes = new List<(double freq, double start, double dur)>();

        double currentTime = 0;
        for (int i = 0; i < noteCount && currentTime < duration; i++)
        {
            double freq = rng.Choose(scale);
            int octaveShift = rng.NextInt(-1, 2);
            freq *= Math.Pow(2, octaveShift);

            double noteDur = beatDuration * (rng.NextInt(1, 4) * 0.5);
            if (rng.NextDouble() < 0.2)
            {
                noteDur = beatDuration * 0.25;
            }

            notes.Add((freq, currentTime, Math.Min(noteDur, duration - currentTime)));
            currentTime += noteDur;
        }

        var samples = new short[totalSamples];
        double baseVolume = 0.3;

        foreach (var (freq, start, dur) in notes)
        {
            int startSample = (int)(start * SampleRate);
            int noteSamples = (int)(dur * SampleRate);
            double attackTime = 0.02;
            double releaseTime = 0.05;

            for (int i = 0; i < noteSamples && startSample + i < totalSamples; i++)
            {
                double t = (double)i / SampleRate;
                double envelope = 1.0;

                if (t < attackTime)
                    envelope = t / attackTime;
                else if (t > dur - releaseTime)
                    envelope = (dur - t) / releaseTime;

                double fundamental = Math.Sin(2 * Math.PI * freq * t);
                double harmonic2 = 0.3 * Math.Sin(2 * Math.PI * freq * 2 * t);
                double harmonic3 = 0.1 * Math.Sin(2 * Math.PI * freq * 3 * t);

                double sample = (fundamental + harmonic2 + harmonic3) * envelope * baseVolume;
                int idx = startSample + i;
                samples[idx] = (short)Math.Clamp(samples[idx] + sample * 32767, short.MinValue, short.MaxValue);
            }
        }

        return CreateWavFile(samples);
    }

    private static byte[] CreateWavFile(short[] samples)
    {
        int dataSize = samples.Length * 2;
        int fileSize = 44 + dataSize;

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write(new[] { 'R', 'I', 'F', 'F' });
        writer.Write(fileSize - 8);
        writer.Write(new[] { 'W', 'A', 'V', 'E' });

        writer.Write(new[] { 'f', 'm', 't', ' ' });
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)Channels);
        writer.Write(SampleRate);
        writer.Write(SampleRate * Channels * BitsPerSample / 8);
        writer.Write((short)(Channels * BitsPerSample / 8));
        writer.Write((short)BitsPerSample);

        writer.Write(new[] { 'd', 'a', 't', 'a' });
        writer.Write(dataSize);

        foreach (var sample in samples)
        {
            writer.Write(sample);
        }

        return ms.ToArray();
    }
}
