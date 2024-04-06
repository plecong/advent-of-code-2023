namespace AdventOfCode2023.Day22;

using Xunit;
using AdventOfCode2023.Utils;

internal class Solution()
{
    public long Part1(IEnumerable<string> input) =>
        throw new NotImplementedException();

    public long Part2(IEnumerable<string> input) =>
        throw new NotImplementedException();
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            1,0,1~1,2,1
            0,0,2~2,0,2
            0,2,3~2,2,3
            0,0,4~0,2,4
            2,0,5~2,2,5
            0,1,6~2,1,6
            1,1,8~1,1,9
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(5, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(0, solution.Part1(Input));

    [Fact]
    public void Part2() =>
        Assert.Equal(0, solution.Part2(Input));
}
