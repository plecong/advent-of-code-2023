namespace AdventOfCode2023.Day13;

using Xunit;
using AdventOfCode2023.Utils;

internal enum Direction
{
    NORTH, WEST, SOUTH, EAST
}

internal class Platform(IEnumerable<string> input)
{
    public string[] Input { get; } = input.ToArray();

    public string Key { get; } = input.Aggregate(string.Empty, (agg, x) => agg + x);

    public int Load
    {
        get => Enumerable.Range(0, Input.Count())
            .Select(idx => (Input.Count() - idx) * Input[idx].Count(x => x == 'O'))
            .Sum();
    }

    public string Shift(string input, bool start)
    {
        var parts = input.Split('#')
            .Select(x => (Count: x.Count(w => w == 'O'), Length: x.Length))
            .Select(x => new string(start ? 'O' : '.', start ? x.Count : x.Length - x.Count) + new string(start ? '.' : 'O', start ? x.Length - x.Count : x.Count))
            .ToArray();
        return string.Join('#', parts);
    }

    public Platform Tilt(Direction direction)
    {
        string[] rows = Input;

        if (direction == Direction.NORTH || direction == Direction.SOUTH)
        {
            rows = rows.Transpose();
        }

        // within each row migrate any O to the first open spot '.'
        // split each on barriers, then sort each segment, then join
        var tilted = rows.Select(x => Shift(x, direction == Direction.NORTH || direction == Direction.WEST)).ToArray();

        // reverse the tranpose
        if (direction == Direction.NORTH || direction == Direction.SOUTH)
        {
            tilted = tilted.Transpose();
        }

        return new Platform(tilted);
    }

    public Platform Spin()
    {
        List<Direction> cycle = [Direction.NORTH, Direction.WEST, Direction.SOUTH, Direction.EAST];
        return cycle.Aggregate(this, (current, direction) => current.Tilt(direction));
    }
}

internal class Solution()
{
    public int Part1(IEnumerable<string> input)
    {
        var platform = new Platform(input);
        var titled = platform.Tilt(Direction.NORTH);
        return titled.Load;
    }

    public int Part2(IEnumerable<string> input)
    {
        Platform platform = new Platform(input);
        Platform spun = platform;

        Dictionary<string, Platform> cache = new();
        List<int> loads = new();
        string? start = null;
        int offset = 0;

        for (int i = 0; i < int.MaxValue; i++)
        {
            if (cache.ContainsKey(platform.Key))
            {
                if (start == null)
                {
                    offset = i;
                    start = platform.Key;
                }
                else if (start.Equals(platform.Key))
                {
                    // completed the cycle
                    break;
                }

                // collect the cycle
                loads.Add(platform.Load);
                spun = cache[platform.Key];
            }
            else
            {
                spun = platform.Spin();
                cache[platform.Key] = spun;
            }

            platform = spun;
        }

        return loads[(1_000_000_000 - offset) % loads.Count()];
    }
}

public class Test()
{
    private Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            O....#....
            O.OO#....#
            .....##...
            OO.#O....O
            .O.....O#.
            O.#..O.#.#
            ..O..#O..O
            .......O..
            #....###..
            #OO..#....
            """.ReadLines();
    }

    public IEnumerable<string> SampleTilted
    {
        get => """
            OOOO.#.O..
            OO..#....#
            OO..O##..O
            O..#.OO...
            ........#.
            ..#....#.#
            ..O..#.O.O
            ..O.......
            #....###..
            #....#....
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1SampleTilted() =>
        Assert.Equal(136, new Platform(SampleTilted).Load);

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(136, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(109665, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(64, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(96061, solution.Part2(Input));
}
