namespace AdventOfCode2023.Day05;

using Xunit;
using AdventOfCode2023.Utils;

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            seeds: 79 14 55 13

            seed-to-soil map:
            50 98 2
            52 50 48

            soil-to-fertilizer map:
            0 15 37
            37 52 2
            39 0 15

            fertilizer-to-water map:
            49 53 8
            0 11 42
            42 0 7
            57 7 4

            water-to-light map:
            88 18 7
            18 25 70

            light-to-temperature map:
            45 77 23
            81 45 19
            68 64 13

            temperature-to-humidity map:
            0 69 1
            1 0 69

            humidity-to-location map:
            60 56 37
            56 93 4
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(35, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(621354867, solution.Part1(Input));

    [Fact]
    public void TestIntersectAround()
    {
        var range = new Range(5, 5);
        var mapper = new Mapper(-1000, 0, 20);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(5, 5), output);
    }

    [Fact]
    public void TestIntersectStartBeforeEndBefore()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(-1005, -5, 10);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(0, 5), output);
    }

    [Fact]
    public void TestIntersectStartBeforeEndAt()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(-1005, -5, 15);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(0, 10), output);
    }

    [Fact]
    public void TestIntersectStartAtEndBefore()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(-1000, 0, 5);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(0, 5), output);
    }

    [Fact]
    public void TestIntersectStartAtEndAfter()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(-1000, 0, 20);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(0, 10), output);
    }

    [Fact]
    public void TestIntersectStartAtEndAt()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(-1000, 0, 10);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(0, 10), output);
    }

    [Fact]
    public void TestIntersectStartAfterEndAfter()
    {
        var range = new Range(0, 20);
        var mapper = new Mapper(-1000, 10, 20);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(10, 10), output);
    }

    [Fact]
    public void TestIntersectStartAfterEndAfterBy1()
    {
        var range = new Range(0, 10);
        var mapper = new Mapper(19, 9, 10);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(9, 1), output);
    }

    [Fact]
    public void TestIntersectStartAfterEndBefore()
    {
        var range = new Range(0, 20);
        var mapper = new Mapper(-1000, 10, 5);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(10, 5), output);
    }

    [Fact]
    public void TestIntersectStartAfterEndAt()
    {
        var range = new Range(0, 20);
        var mapper = new Mapper(-1000, 10, 10);
        var output = range.Intersection(mapper);
        Assert.Equal(new Range(10, 10), output);
    }

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(46, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(15880236, solution.Part2(Input));
}
