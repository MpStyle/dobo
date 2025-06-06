using System.Diagnostics.CodeAnalysis;
using dobo.waste.collection.Padova;

namespace dobo.waste.collection.test.Padova;

[ExcludeFromCodeCoverage]
public class Tests
{
    [Test]
    public void Run()
    {
        var result=new PadovaGarbageScraper().Run();
    }
}