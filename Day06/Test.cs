namespace AdventOfCode2023.Day06;

using Xunit;
using AdventOfCode2023.Utils;

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            Time:      7  15   30
            Distance:  9  40  200
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void TestRecord() =>
        Assert.Equal([2, 3, 4, 5], new Race(7, 9).FindRecords().Select(x => x.Hold));

    [Fact]
    public void TestRecordTimes() =>
        Assert.Equal([10, 12, 12, 10], new Race(7, 9).FindRecords().Select(x => x.Time));

    [Fact]
    public void TestRecordCountRace2() =>
        Assert.Equal(8, new Race(15, 40).FindRecords().Count());

    [Fact]
    public void TestRecordCountRace3() =>
        Assert.Equal(9, new Race(30, 200).FindRecords().Count());

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(288, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(800280, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(71503, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(45128024, solution.Part2(Input));
}
