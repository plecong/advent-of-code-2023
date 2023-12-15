namespace AdventOfCode2023.Day13;

using System.Collections;
using Xunit;
using AdventOfCode2023.Utils;

internal record Pattern(string[] Rows)
{
    public string[] Transpose
    {
        get => Enumerable.Range(0, Rows[0].Length)
            .Select(col => new string(Rows.Select(row => row[col]).ToArray()))
            .ToArray();
    }

    internal int BitErrors(BitArray first, BitArray second) =>
        new BitArray(first).Xor(second).OfType<bool>().Count(x => x);

    internal bool IsReflection(BitArray[] rows, int left, int right, int errorLimit) =>
        Enumerable
            .Range(0, Math.Min(left + 1, rows.Length - right))
            .Select(x => BitErrors(rows[left - x], rows[right + x]))
            .Sum() == errorLimit;

    internal int FindReflection(string[] values, int errors)
    {
        // convert to bit array and add index
        var rows = values
            .Select(x => new BitArray(x.Select(b => b == '#').ToArray()))
            .Select((x, idx) => (Value: x, Index: idx))
            .ToArray();

        // iterate through pairs to find matches
        return rows
            .Take(rows.Length - 1).Zip(rows.Skip(1))
            .Where(x => BitErrors(x.First.Value, x.Second.Value) <= errors)
            .Where(x => IsReflection(
                rows.Select(x => x.Value).ToArray(),
                x.First.Index,
                x.Second.Index,
                errors))
            .Select(x => x.First.Index + 1)
            .FirstOrDefault();
    }

    public int FindReflectionRows(int errors = 0) =>
         FindReflection(Rows, errors);

    public int FindReflectionCols(int errors = 0) =>
         FindReflection(Transpose, errors);
}

public class Solution()
{
    private int FindReflections(IEnumerable<string> input, int errors) =>
        input
            .ChunkBy(string.IsNullOrWhiteSpace)
            .Select(x => new Pattern(x.ToArray()))
            .Sum(x => x.FindReflectionRows(errors) * 100 + x.FindReflectionCols(errors));

    public int Part1(IEnumerable<string> input) => FindReflections(input, 0);
    public int Part2(IEnumerable<string> input) => FindReflections(input, 1);
}

public class Test()
{
    private Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            #.##..##.
            ..#.##.#.
            ##......#
            ##......#
            ..#.##.#.
            ..##..##.
            #.#.##.#.

            #...##..#
            #....#..#
            ..##..###
            #####.##.
            #####.##.
            ..##..###
            #....#..#
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void TestFindReflectionRows()
    {
        var lines = """
            #...##..#
            #....#..#
            ..##..###
            #####.##.
            #####.##.
            ..##..###
            #....#..#
            """.ReadLines();
        var pattern = new Pattern(lines.ToArray());
        var output = pattern.FindReflectionRows();
        Assert.Equal(4, output);
    }

    [Fact]
    public void TestFindReflectionRowsWithError()
    {
        var lines = """
            #...##..#
            #....#..#
            ..##..###
            #####.##.
            #####.##.
            ..##..###
            #....#..#
            """.ReadLines();
        var pattern = new Pattern(lines.ToArray());
        var output = pattern.FindReflectionRows(1);
        Assert.Equal(1, output);
    }

    [Fact]
    public void TestFindReflectionCols()
    {
        var lines = """
            #.#..##..#.#..###
            #.##.##.##.#.####
            #.#......#.#.#...
            #..........#.####
            .##########..#.##
            #..######..####..
            ##...###..##.####
            #..........######
            #.########.#..#..
            .#..#..#..#.###..
            .##..##..##..#...
            #.########.#..#..
            ...#....#...#..##
            """.ReadLines();
        var pattern = new Pattern(lines.ToArray());
        var output = pattern.FindReflectionCols();
        Assert.Equal(16, output);
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(405, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(27300, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(400, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(29276, solution.Part2(Input));
}
