using dobo.waste.collection.Entities;

namespace dobo.info.Garbage.Entities;

public record GarbageDay
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int Day { get; init; }
    public GarbageType[] Types { get; init; }
}