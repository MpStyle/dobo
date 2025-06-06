using System.Diagnostics.CodeAnalysis;
using dobo.waste.collection.Padova;
using Microsoft.Extensions.Logging.Abstractions;

namespace dobo.waste.scraper.test.Padova;

[ExcludeFromCodeCoverage]
public class Tests
{
    [Test]
    public void Run()
    {
        var result=new PadovaEstGarbageScraper(NullLogger<PadovaEstGarbageScraper>.Instance).Run();
    }
}