namespace AdventOfCode2023.Day08;

using Xunit;
using AdventOfCode2023.Utils;
using System.Linq;

internal class Network
{
    public IReadOnlyDictionary<string, (string Left, string Right)> Nodes { get; init; }

    private char[] instructions;

    public Network(IEnumerable<string> input)
    {
        var chunks = input.ChunkBy(string.IsNullOrWhiteSpace).ToArray();
        instructions = chunks[0].Single().ToArray();
        Nodes = chunks[1].ToDictionary(
            x => x.Substring(0, 3),
            x => (Left: x.Substring(7, 3), Right: x.Substring(12, 3)));
    }

    public (int Steps, string Node) Navigate(string start, Predicate<string> stopFunc)
    {
        var current = start;
        for (var step = 0; step < int.MaxValue; step++)
        {
            var instruction = instructions[step % instructions.Length];
            current = instruction switch
            {
                'L' => Nodes[current].Left,
                'R' => Nodes[current].Right,
                _ => throw new InvalidOperationException()
            };

            if (stopFunc(current))
            {
                return (step + 1, current);
            }
        }

        return (-1, "");
    }

    public (long Steps, string Node) CycleLength(string start, Predicate<string> stopFunc)
    {
        var (steps, node) = Navigate(start, stopFunc);
        var (cycle, _) = Navigate(node, x => x.Equals(node));
        return (cycle, node);
    }
}

internal class Solution()
{
    static long GreatestCommonFactor(long a, long b) =>
        b == 0 ? a : GreatestCommonFactor(b, a % b);

    static long LeastCommonMultiple(long a, long b) =>
        a / GreatestCommonFactor(a, b) * b;

    public int Part1(IEnumerable<string> input)
    {
        return new Network(input)
            .Navigate("AAA", x => x.Equals("ZZZ")).Steps;
    }

    public long Part2(IEnumerable<string> input)
    {
        var network = new Network(input);

        return network.Nodes.Keys
            .Where(x => x[^1] == 'A')
            .Select(x => network.CycleLength(x, x => x[^1] == 'Z').Steps)
            .Aggregate(LeastCommonMultiple);
    }
}

public class Test()
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            RL

            AAA = (BBB, CCC)
            BBB = (DDD, EEE)
            CCC = (ZZZ, GGG)
            DDD = (DDD, DDD)
            EEE = (EEE, EEE)
            GGG = (GGG, GGG)
            ZZZ = (ZZZ, ZZZ)
            """.ReadLines();
    }

    private IEnumerable<string> Sample2
    {
        get => """
            LLR

            AAA = (BBB, BBB)
            BBB = (AAA, ZZZ)
            ZZZ = (ZZZ, ZZZ)
            """.ReadLines();
    }

    private IEnumerable<string> Sample3
    {
        get => """
            LR

            11A = (11B, XXX)
            11B = (XXX, 11Z)
            11Z = (11B, XXX)
            22A = (22B, XXX)
            22B = (22C, 22C)
            22C = (22Z, 22Z)
            22Z = (22B, 22B)
            XXX = (XXX, XXX)
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(2, solution.Part1(Sample));

    [Fact]
    public void Part1Sample2() =>
        Assert.Equal(6, solution.Part1(Sample2));

    [Fact]
    public void Part1() =>
        Assert.Equal(16343, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(6, solution.Part2(Sample3));

    [Fact]
    public void Part2() =>
        Assert.Equal(15299095336639, solution.Part2(Input));
}
