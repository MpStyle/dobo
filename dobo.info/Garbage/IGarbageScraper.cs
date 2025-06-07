using dobo.info.Garbage.Entities;
using dobo.waste.collection.Entities;

namespace dobo.info.Garbage;

public interface IGarbageScraper
{
    string City { get; }
    
    IEnumerable<GarbageDay> Run();
}