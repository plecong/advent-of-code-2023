using Xunit;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Day05;

internal class Solution
{
    public int Part1(IEnumerable<string> input) =>
        throw new NotImplementedException();

    public int Part2(IEnumerable<string> input) =>
        throw new NotImplementedException();
}

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(0, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(0, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(0, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(0, solution.Part2(Input));
}