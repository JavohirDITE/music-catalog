using System.Text;
using System.Web;

namespace MusicCatalog.Api.Generators;

public static class CoverGenerator
{
    private static readonly string[] Colors = { "#FF6B6B", "#4ECDC4", "#45B7D1", "#96CEB4", "#FFEAA7", "#DDA0DD", "#98D8C8", "#F7DC6F", "#BB8FCE", "#85C1E9", "#F8B500", "#00CED1", "#FF69B4", "#7B68EE", "#00FA9A" };

    public static string Generate(ulong contentSeed, string title, string artist)
    {
        var rng = new SplitMix64(contentSeed);

        int template = rng.NextInt(0, 4);
        string bgColor = rng.Choose(Colors);
        string accentColor = rng.Choose(Colors);
        string textColor = GetContrastColor(bgColor);

        int year = rng.NextInt(1970, 2025);
        string label = GenerateLabel(rng);

        string displayTitle = Truncate(title, 25);
        string displayArtist = Truncate(artist, 30);

        return template switch
        {
            0 => GenerateWaves(rng, bgColor, accentColor, textColor, displayTitle, displayArtist, year, label),
            1 => GenerateCircles(rng, bgColor, accentColor, textColor, displayTitle, displayArtist, year, label),
            2 => GenerateGeometry(rng, bgColor, accentColor, textColor, displayTitle, displayArtist, year, label),
            _ => GenerateVinyl(rng, bgColor, accentColor, textColor, displayTitle, displayArtist, year, label)
        };
    }

    private static string GenerateWaves(SplitMix64 rng, string bg, string accent, string text, string title, string artist, int year, string label)
    {
        var sb = new StringBuilder();
        sb.Append($@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 400 400"">");
        sb.Append($@"<rect width=""400"" height=""400"" fill=""{bg}""/>");

        for (int i = 0; i < 5; i++)
        {
            int y = 50 + i * 60;
            int amp = rng.NextInt(10, 30);
            double freq = rng.NextDouble() * 0.05 + 0.02;
            sb.Append($@"<path d=""M0 {y}");
            for (int x = 0; x <= 400; x += 10)
            {
                int py = (int)(y + Math.Sin(x * freq + i) * amp);
                sb.Append($" L{x} {py}");
            }
            sb.Append($@""" stroke=""{accent}"" stroke-width=""3"" fill=""none"" opacity=""0.6""/>");
        }

        AppendTextOverlay(sb, text, title, artist, year, label);
        sb.Append("</svg>");
        return sb.ToString();
    }

    private static string GenerateCircles(SplitMix64 rng, string bg, string accent, string text, string title, string artist, int year, string label)
    {
        var sb = new StringBuilder();
        sb.Append($@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 400 400"">");
        sb.Append($@"<rect width=""400"" height=""400"" fill=""{bg}""/>");

        for (int i = 0; i < 8; i++)
        {
            int cx = rng.NextInt(50, 350);
            int cy = rng.NextInt(50, 350);
            int r = rng.NextInt(20, 80);
            string color = rng.Choose(Colors);
            double opacity = rng.NextDouble() * 0.5 + 0.2;
            sb.Append($@"<circle cx=""{cx}"" cy=""{cy}"" r=""{r}"" fill=""{color}"" opacity=""{opacity:F2}""/>");
        }

        AppendTextOverlay(sb, text, title, artist, year, label);
        sb.Append("</svg>");
        return sb.ToString();
    }

    private static string GenerateGeometry(SplitMix64 rng, string bg, string accent, string text, string title, string artist, int year, string label)
    {
        var sb = new StringBuilder();
        sb.Append($@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 400 400"">");
        sb.Append($@"<rect width=""400"" height=""400"" fill=""{bg}""/>");

        for (int i = 0; i < 6; i++)
        {
            int shape = rng.NextInt(0, 3);
            string color = rng.Choose(Colors);
            double opacity = rng.NextDouble() * 0.6 + 0.2;

            if (shape == 0)
            {
                int x = rng.NextInt(0, 300);
                int y = rng.NextInt(0, 300);
                int w = rng.NextInt(30, 100);
                int h = rng.NextInt(30, 100);
                sb.Append($@"<rect x=""{x}"" y=""{y}"" width=""{w}"" height=""{h}"" fill=""{color}"" opacity=""{opacity:F2}""/>");
            }
            else if (shape == 1)
            {
                int x1 = rng.NextInt(0, 400);
                int y1 = rng.NextInt(0, 400);
                int x2 = rng.NextInt(0, 400);
                int y2 = rng.NextInt(0, 400);
                int x3 = rng.NextInt(0, 400);
                int y3 = rng.NextInt(0, 400);
                sb.Append($@"<polygon points=""{x1},{y1} {x2},{y2} {x3},{y3}"" fill=""{color}"" opacity=""{opacity:F2}""/>");
            }
            else
            {
                int cx = rng.NextInt(50, 350);
                int cy = rng.NextInt(50, 350);
                int r = rng.NextInt(20, 60);
                sb.Append($@"<circle cx=""{cx}"" cy=""{cy}"" r=""{r}"" fill=""{color}"" opacity=""{opacity:F2}""/>");
            }
        }

        AppendTextOverlay(sb, text, title, artist, year, label);
        sb.Append("</svg>");
        return sb.ToString();
    }

    private static string GenerateVinyl(SplitMix64 rng, string bg, string accent, string text, string title, string artist, int year, string label)
    {
        var sb = new StringBuilder();
        sb.Append($@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 400 400"">");
        sb.Append($@"<rect width=""400"" height=""400"" fill=""{bg}""/>");

        for (int i = 0; i < 12; i++)
        {
            int angle = i * 30;
            string rayColor = i % 2 == 0 ? accent : bg;
            sb.Append($@"<path d=""M200 200 L{200 + 200 * Math.Cos(angle * Math.PI / 180):F0} {200 + 200 * Math.Sin(angle * Math.PI / 180):F0} A200 200 0 0 1 {200 + 200 * Math.Cos((angle + 30) * Math.PI / 180):F0} {200 + 200 * Math.Sin((angle + 30) * Math.PI / 180):F0} Z"" fill=""{rayColor}"" opacity=""0.3""/>");
        }

        sb.Append($@"<circle cx=""200"" cy=""200"" r=""120"" fill=""#1a1a1a""/>");
        sb.Append($@"<circle cx=""200"" cy=""200"" r=""100"" fill=""#333""/>");
        for (int i = 0; i < 5; i++)
        {
            int r = 40 + i * 15;
            sb.Append($@"<circle cx=""200"" cy=""200"" r=""{r}"" fill=""none"" stroke=""#444"" stroke-width=""1""/>");
        }
        sb.Append($@"<circle cx=""200"" cy=""200"" r=""20"" fill=""{accent}""/>");
        sb.Append($@"<circle cx=""200"" cy=""200"" r=""5"" fill=""#1a1a1a""/>");

        AppendTextOverlay(sb, text, title, artist, year, label);
        sb.Append("</svg>");
        return sb.ToString();
    }

    private static void AppendTextOverlay(StringBuilder sb, string textColor, string title, string artist, int year, string label)
    {
        sb.Append($@"<rect x=""20"" y=""290"" width=""360"" height=""90"" fill=""rgba(0,0,0,0.7)"" rx=""5""/>");
        sb.Append($@"<text x=""200"" y=""325"" text-anchor=""middle"" font-family=""Arial, sans-serif"" font-size=""20"" font-weight=""bold"" fill=""white"">{HttpUtility.HtmlEncode(title)}</text>");
        sb.Append($@"<text x=""200"" y=""350"" text-anchor=""middle"" font-family=""Arial, sans-serif"" font-size=""14"" fill=""#ccc"">{HttpUtility.HtmlEncode(artist)}</text>");
        sb.Append($@"<text x=""30"" y=""375"" font-family=""Arial, sans-serif"" font-size=""10"" fill=""#888"">{year}</text>");
        sb.Append($@"<text x=""370"" y=""375"" text-anchor=""end"" font-family=""Arial, sans-serif"" font-size=""10"" fill=""#888"">{HttpUtility.HtmlEncode(label)}</text>");
    }

    private static string GenerateLabel(SplitMix64 rng)
    {
        string[] labels = { "Records", "Music", "Audio", "Sound", "Studio", "Label", "Productions" };
        string[] prefixes = { "Star", "Nova", "Echo", "Wave", "Pulse", "Sonic", "Beat", "Rhythm", "Melody", "Harmony" };
        return rng.Choose(prefixes) + " " + rng.Choose(labels);
    }

    private static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength) return text;
        return text.Substring(0, maxLength - 3) + "...";
    }

    private static string GetContrastColor(string hexColor)
    {
        int r = Convert.ToInt32(hexColor.Substring(1, 2), 16);
        int g = Convert.ToInt32(hexColor.Substring(3, 2), 16);
        int b = Convert.ToInt32(hexColor.Substring(5, 2), 16);
        double luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
        return luminance > 0.5 ? "#000000" : "#FFFFFF";
    }
}
