using dobo.waste.collection.Entities;

namespace dobo.waste.collection;

public interface IGarbageScraper
{
    string City { get; }
    
    IEnumerable<GarbageDay> Run();
}