namespace AdventOfCode2023.Day06;

internal record Race(int Time, long Distance)
{
    public IEnumerable<(long Hold, long Time)> FindRecords() =>
        Enumerable.Range(1, Time)
            .Select(x => (Hold: (long)x, Time: (Time - (long)x) * x))
            .Where(x => x.Time > Distance);
}

internal class Solution
{
    public int Part1(IEnumerable<string> input)
    {
        var values = input.Select(x =>
            x[11..].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
        );

        var races = values.First()
            .Zip(values.Last())
            .Select(x => new Race(x.First, x.Second));

        return races
            .Select(x => x.FindRecords().Count())
            .Aggregate(1, (acc, x) => acc * x);
    }

    public long Part2(IEnumerable<string> input)
    {
        var values = input.Select(x => x.Substring(11).Replace(" ", string.Empty));
        var race = new Race(int.Parse(values.First()), long.Parse(values.Last()));
        return race.FindRecords().Count();
    }
}
