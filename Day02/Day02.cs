using Xunit;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Day02;

internal record Grab(int Red, int Green, int Blue)
{
    public Grab(string line) : this(0, 0, 0)
    {
        var parts = line.Parts(",").Select(x =>
        {
            var (count, color, _) = x.Split(" ");
            return (Count: count, Color: color);
        }).ToLookup(x => x.Color);

        Red = int.Parse(parts["red"].FirstOrDefault(("0", "red")).Item1);
        Green = int.Parse(parts["green"].FirstOrDefault(("0", "green")).Item1);
        Blue = int.Parse(parts["blue"].FirstOrDefault(("0", "blue")).Item1);
    }
    public bool Possible(Grab max) => max.Red >= Red && max.Green >= Green && max.Blue >= Blue;
    public int Power() => Red * Green * Blue;
}

internal record Game(int Id, List<Grab> Grabs)
{
    public Game(string line) : this(0, new List<Grab>())
    {
        var (game, grabs, _) = line.Parts(":");
        var (_, gameId, _) = game.Parts(" ");

        Id = int.Parse(gameId);
        Grabs = grabs.Parts(";").Select(x => new Grab(x)).ToList();
    }

    public bool Possible(Grab max) => Grabs.All(x => x.Possible(max));

    public Grab Fewest() => new(
        Grabs.Select(x => x.Red).Max(),
        Grabs.Select(x => x.Green).Max(),
        Grabs.Select(x => x.Blue).Max());
}

internal static class Extension
{
    public static IList<string> Parts(this string value, string? separator) =>
        value.Split(separator).Select(x => x.Trim()).ToList();
}

internal class Solution
{
    public int Part1(IEnumerable<string> lines, Grab max) =>
        lines.Select(x => new Game(x)).Where(x => x.Possible(max)).Select(x => x.Id).Sum();

    public int Part2(IEnumerable<string> lines) =>
        lines.Select(x => new Game(x)).Select(x => x.Fewest().Power()).Sum();

}

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void TestParseGrab()
    {
        var output = new Grab("1 red, 2 green, 6 blue");
        Assert.Equal(new Grab(1, 2, 6), output);
    }

    [Fact]
    public void TestPartialParseGrab()
    {
        var output = new Grab("3 blue, 4 red");
        Assert.Equal(new Grab(4, 0, 3), output);
    }

    [Fact]
    public void TestMultiDigitParseGrab()
    {
        var output = new Grab("5 blue, 4 red, 13 green");
        Assert.Equal(new Grab(4, 13, 5), output);
    }

    [Fact]
    public void TestParseGame()
    {
        var output = new Game("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green");
        Assert.Equal(1, output.Id);
        Assert.Equal(new List<Grab>() { new(4, 0, 3), new(1, 2, 6), new(0, 2, 0) }, output.Grabs);
    }

    [Fact]
    public void Part1Sample()
    {
        var output = solution.Part1(Sample, new Grab(12, 13, 14));
        Assert.Equal(8, output);
    }

    [Fact]
    public void Part1()
    {
        var output = solution.Part1(Input, new Grab(12, 13, 14));
        Assert.Equal(2771, output);
    }

    [Fact]
    public void FewestGame()
    {
        var output = Sample.Select(x => new Game(x)).Select(x => x.Fewest());
        Assert.Equal(new List<Grab>() {
            new(4, 2, 6),
            new(1, 3, 4),
            new(20, 13, 6),
            new(14, 3, 15),
            new(6, 3, 2)
        }, output);
    }

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(2286, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(70924, solution.Part2(Input));
}