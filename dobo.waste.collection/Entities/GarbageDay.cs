namespace dobo.waste.collection.Entities;

public record GarbageDay
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int Day { get; init; }
    public GarbageType[] Types { get; init; }
}