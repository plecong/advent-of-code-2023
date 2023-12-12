namespace AdventOfCode2023.Day11;

using Xunit;
using AdventOfCode2023.Utils;

internal record Node(char Value, int Row, int Col);

internal class Solution()
{
    public long Distances(IEnumerable<string> input, long expansion = 2)
    {
        Node[][] grid = input.Select((x, row) => x.Select((n, col) => new Node(n, row, col)).ToArray()).ToArray();

        // check each row
        long[] rowWidths = grid
            .Select(x => x.All(n => n.Value == '.') ? expansion : 1)
            .ToArray();

        // check each column
        long[] colWidths = Enumerable.Range(0, grid[0].Length)
            .Select(col => grid.Select(row => row[col]).All(node => node.Value == '.') ? expansion : 1)
            .ToArray();

        // find all the galaxies
        var galaxies = grid
            .SelectMany(x => x)
            .Where(x => x.Value == '#')
            .ToList();

        // get all pairs of galaxies
        var pairs = galaxies
            .SelectMany((left, leftIdx) => galaxies.Select((right, rightIdx) => (Left: left, Right: right, Diff: rightIdx - leftIdx)))
            .Where(tuple => tuple.Diff > 0);

        // get the distance for each pair
        var distances = pairs.Select(pair =>
                rowWidths[Math.Min(pair.Left.Row, pair.Right.Row)..Math.Max(pair.Left.Row, pair.Right.Row)].Sum() +
                colWidths[Math.Min(pair.Left.Col, pair.Right.Col)..Math.Max(pair.Left.Col, pair.Right.Col)].Sum());

        return distances.Sum();
    }

    public long Part1(IEnumerable<string> input)
    {
        return Distances(input, 2);
    }

    public long Part2(IEnumerable<string> input)
    {
        return Distances(input, 1_000_000);
    }
}

public class Test()
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            ...#......
            .......#..
            #.........
            ..........
            ......#...
            .#........
            .........#
            ..........
            .......#..
            #...#.....
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(374, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(10_077_850, solution.Part1(Input));

    [Fact]
    public void Part2SampleWith10() =>
        Assert.Equal(1030, solution.Distances(Sample, 10));

    [Fact]
    public void Part2SampleWith100() =>
        Assert.Equal(8410, solution.Distances(Sample, 100));

    [Fact]
    public void Part2() =>
        Assert.Equal(504_715_068_438, solution.Part2(Input));
}
