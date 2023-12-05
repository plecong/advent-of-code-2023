namespace AdventOfCode2023.Day05;

using AdventOfCode2023.Utils;

internal record Mapper(long Destination, long Start, long Length) : Range(Start, Length)
{
    public long Offset { get => Destination - Start; }
}

internal record Range(long Start, long Length)
{
    public long End { get => Start + Length; }

    public bool Intersects(Range range) =>
        Start < range.End && range.Start < End;

    public bool Intersects(long position) =>
        position >= Start && position < End;

    public Range Intersection(Range other) =>
        new Range(
            Math.Max(Start, other.Start),
            Math.Min(other.End, End) - Math.Max(Start, other.Start));

    /// <summary>
    /// Applies the provided mappers to the range and returns a new list of ranges
    /// including offsets applied for the mappers 
    /// </summary>
    /// <param name="mappers"></param>
    /// <returns></returns>
    public IEnumerable<Range> Split(IEnumerable<Mapper> mappers)
    {
        // find all the intersected segments from the mappers
        var segments = mappers
            .Where(Intersects)
            .Select(x => (Mapper: x, Range: Intersection(x)))
            .OrderBy(x => x.Range.Start);

        var current = Start;

        foreach (var (mapper, range) in segments)
        {
            if (current < range.Start)
            {
                // return the gap from current to range
                yield return new Range(current, range.Start - current);
                current = range.Start;
            }

            // return the range with offset applied
            yield return new Range(range.Start + mapper.Offset, range.Length);
            current = range.Start + range.Length;
        }

        if (current < End)
        {
            yield return new Range(current, End - current);
        }
    }
}

internal record Map(IList<Mapper> Mappers)
{
    /// <summary>
    /// Translate a position into a new position by applying the mappers
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public long Project(long position) =>
        position + (Mappers.Where(x => x.Intersects(position)).FirstOrDefault()?.Offset ?? 0);

    /// <summary>
    /// Projects the provided set of ranges into a new list of ranges by applying
    /// the mappers to each of the ranges
    /// </summary>
    /// <param name="ranges"></param>
    /// <returns></returns>
    public IEnumerable<Range> Project(IEnumerable<Range> ranges) =>
        ranges.SelectMany(range => range.Split(Mappers));
}

internal class Solution
{
    private (IEnumerable<long>, IEnumerable<Map>) LoadInput(IEnumerable<string> input)
    {
        // parse "seeds: 1 2 3 4" into [1, 2, 3, 4]
        var seeds = input.First()
            .Substring(7)
            .Split()
            .Select(long.Parse);

        var maps = input
            .Skip(2)
            .ChunkBy(string.IsNullOrWhiteSpace)
            .Select(x => x
                .Skip(1)
                .Select(l => l.Split().Select(long.Parse).ToArray())
                .Select(l => new Mapper(l[0], l[1], l[2]))
                .ToList()
            )
            .Select(x => new Map(x));

        return (seeds, maps);
    }

    public long Part1(IEnumerable<string> input)
    {
        var (seeds, maps) = LoadInput(input);

        return seeds
            .Select(seed => maps.Aggregate(seed, (pos, map) => map.Project(pos)))
            .Min();
    }

    public long Part2(IEnumerable<string> input)
    {
        var (seeds, maps) = LoadInput(input);

        // load current ranges
        var ranges = seeds
            .Chunk(2)
            .Select(x => new Range(x[0], x[1]));

        // fold over each map to take the set of ranges and 
        // create a new set of mapped ranges based on splits
        return maps
            .Aggregate(ranges, (ranges, map) => map.Project(ranges))
            .Min(x => x.Start);
    }

}
