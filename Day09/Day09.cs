namespace AdventOfCode2023.Day09;

using Xunit;
using AdventOfCode2023.Utils;

internal static class NumberExtensions
{
    public static int[] Diff(this int[] values) =>
        values.Skip(1)
            .Zip(values.Take(values.Length - 1))
            .Select(x => x.First - x.Second)
            .ToArray();
}

internal class Solution()
{
    static int PredictNext(int[] values)
    {
        // push last value on stack
        var stack = new List<int> { values[^1] };

        while (!values.All(x => x == values[0]))
        {
            // find the differences between each value
            var diffs = values.Diff();
            // add the last item to make up the triangle
            stack.Add(diffs[^1]);
            values = diffs;
        }

        return stack.Sum();
    }

    static int PredictPrevious(int[] values)
    {
        // push first value on stack
        var stack = new Stack<int>();
        stack.Push(values[0]);

        while (!values.All(x => x == values[0]))
        {
            var diffs = values.Diff();
            stack.Push(diffs[0]);
            values = diffs;
        }

        return stack.Aggregate(0, (curr, x) => x - curr);
    }

    public long Part1(IEnumerable<string> input) =>
         input
            .Select(x => x.Split().Select(int.Parse).ToArray())
            .Select(PredictNext)
            .Sum();

    public long Part2(IEnumerable<string> input)
    {
        return input
           .Select(x => x.Split().Select(int.Parse).ToArray())
           .Select(PredictPrevious)
           .Sum();
    }
}

public class Test()
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            0 3 6 9 12 15
            1 3 6 10 15 21
            10 13 16 21 30 45
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(114, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(1877825184, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(2, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(1108, solution.Part2(Input));
}
