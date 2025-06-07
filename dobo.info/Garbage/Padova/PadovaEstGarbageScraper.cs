using dobo.waste.collection;
using dobo.waste.collection.Entities;
using ImageMagick;
using Microsoft.Extensions.Logging;

namespace dobo.info.Garbage.Padova;

public class PadovaEstGarbageScraper(ILogger<PadovaEstGarbageScraper> logger) : IGarbageScraper
{
    private static readonly Dictionary<string, GarbageType> ColorToGarbageType = new()
    {
        {"#9FA1A3FF", GarbageType.Dry},
        {"#6D150BFF", GarbageType.Wet},
        {"#0082CAFF", GarbageType.Paper},
        {"#236D92FF", GarbageType.Paper},
        {"#F9B000FF", GarbageType.Plastic},
        {"#07A990FF", GarbageType.Glass},
        {"#FFFFFFFF", GarbageType.None}
    };

    public string City { get; } = "Padova Est";

    public IEnumerable<GarbageDay> Run()
    {
        var pdfPath = Path.Combine("Padova", "Q3_2025_dr_calendario_web.pdf");
        var (coordinateX, coordinateY) = (x: 925, y: 885);
        var settings = new MagickReadSettings
        {
            Density = new Density(300, 300),
            FrameIndex = 1,
            FrameCount = 12
        };

        using var images = new MagickImageCollection();
        
        logger.LogInformation("Reading PDF file: {PdfPath}", pdfPath);
        images.Read(pdfPath, settings);

        var garbageDays = new List<GarbageDay>();
        var pageNumber = 2;

        foreach (var image in images)
        {
            // var outputPath = Path.Combine("Output", $"Page_{pageNumber}.png");
            // Directory.CreateDirectory("Output"); // Assicurati che la directory esista
            // image.Write(outputPath);
            logger.LogInformation("Processing page {PageNumber} of the PDF", pageNumber);
            
            var pixels = image.GetPixels();

            for (var i = 0; i < 31; i++)
            {
                logger.LogInformation("Processing day {Day} on page {PageNumber}", i + 1, pageNumber);
                
                var y01 = coordinateY + (i * 105);
                var x01 = coordinateX;

                if (x01 >= image.Width || y01 >= image.Height)
                {
                    continue; // Skip out-of-bounds coordinates
                }

                var pixel01 = pixels.GetPixel(x01, y01);
                var color01 = pixel01.ToColor()?.ToString();
                var type01 = ColorToGarbageType.GetValueOrDefault(color01 ?? string.Empty, GarbageType.None);
                
                // ---
                
                var y02 = y01;
                var x02 = x01 + 165;

                if (x02 >= image.Width || y02 >= image.Height)
                {
                    continue; // Skip out-of-bounds coordinates
                }

                var pixel02 = pixels.GetPixel(x02, y02);
                var color02 = pixel02.ToColor()?.ToString();
                var type02 = ColorToGarbageType.GetValueOrDefault(color02 ?? string.Empty, GarbageType.None);

                // ---
                var types= new[] { type01, type02 };
                var garbageDay = new GarbageDay
                {
                    Year = 2025,
                    Month = pageNumber - 1,
                    Day = i + 1,
                    Types = types.Where(t => t != GarbageType.None).ToArray()
                };
                
                garbageDays.Add(garbageDay);
            }

            pageNumber++;
        }

        return garbageDays;
    }
}