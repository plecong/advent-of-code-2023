namespace AdventOfCode2023.Day18;

using Xunit;
using AdventOfCode2023.Utils;
using System.Text.RegularExpressions;

internal partial record Instruction(Direction Direction, long Steps, string Color)
{
    public static Instruction Parse(string input)
    {
        var parts = ParseRegex().Match(input).Groups;

        return new Instruction(
            parts[1].Value switch
            {
                "U" => Direction.UP,
                "D" => Direction.DOWN,
                "L" => Direction.LEFT,
                "R" => Direction.RIGHT,
                _ => throw new ArgumentException()
            },
            long.Parse(parts[2].Value),
            parts[3].Value
        );
    }

    public Instruction FromHex()
    {
        var hex = long.Parse(Color.Substring(1, 5), System.Globalization.NumberStyles.HexNumber);
        var direction = Color.Substring(6) switch
        {
            "0" => Direction.RIGHT,
            "1" => Direction.DOWN,
            "2" => Direction.LEFT,
            "3" => Direction.UP,
            _ => throw new ArgumentException()
        };

        return new Instruction(direction, (int)hex, Color);

    }

    [GeneratedRegex(@"^([UDLR]) (\d+) \((#[0-9a-f]{6})\)$")]
    private static partial Regex ParseRegex();
}

internal record Edge(long X1, long Y1, long X2, long Y2)
{
    public long Distance
    {
        get => Math.Abs(Y2 - Y1) + Math.Abs(X2 - X1);
    }

    public long Area
    {
        get => (X1 * Y2) - (Y1 * X2);
    }
}

internal class Solution()
{
    internal long LagoonArea(IEnumerable<Instruction> instructions)
    {
        // convert instructions into coordinates
        var coords = instructions.Aggregate((IEnumerable<(long x, long y)>)[(0, 0)], (acc, instruction) =>
            (instruction.Direction, acc.Last()) switch
            {
                (Direction.UP, var last) => acc.Append(last with { y = last.y - instruction.Steps }),
                (Direction.DOWN, var last) => acc.Append(last with { y = last.y + instruction.Steps }),
                (Direction.LEFT, var last) => acc.Append(last with { x = last.x - instruction.Steps }),
                (Direction.RIGHT, var last) => acc.Append(last with { x = last.x + instruction.Steps }),
                (_, _) => throw new NotSupportedException()
            })
            .ToArray();

        // calculate edges from coordinates
        var edges = coords.Zip(coords.Skip(1), (a, b) => new Edge(a.x, a.y, b.x, b.y)).ToArray();
        return edges.Sum(x => x.Area + x.Distance) / 2 + 1;
    }

    public long Part1(IEnumerable<string> input) =>
        LagoonArea(input.Select(Instruction.Parse));

    public long Part2(IEnumerable<string> input) =>
         LagoonArea(input.Select(x => Instruction.Parse(x).FromHex()));
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            R 6 (#70c710)
            D 5 (#0dc571)
            L 2 (#5713f0)
            D 2 (#d2c081)
            R 2 (#59c680)
            D 2 (#411b91)
            L 5 (#8ceee2)
            U 2 (#caa173)
            L 1 (#1b58a2)
            U 2 (#caa171)
            R 2 (#7807d2)
            U 3 (#a77fa3)
            L 2 (#015232)
            U 2 (#7a21e3)
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(62, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        // too low
        Assert.Equal(45159, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(952_408_144_115, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(134_549_294_799_713, solution.Part2(Input));
}
