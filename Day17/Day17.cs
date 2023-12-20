namespace AdventOfCode2023.Day17;

using Xunit;
using AdventOfCode2023.Utils;

internal record Vertex(Direction Direction, int Consecutive, int Row, int Col);

internal class Graph(IEnumerable<string> input, int Part)
{
    private readonly int[][] weights = input.Select((x, row) => x.Select((y, col) => y - '0').ToArray()).ToArray();

    public IEnumerable<Vertex> Neighbors(Vertex vertex) => (Part == 1) ? NeighborsPart1(vertex) : NeighborsPart2(vertex);

    public IEnumerable<Vertex> NeighborsPart1(Vertex vertex)
    {
        IEnumerable<Vertex> directions = [
            new Vertex(Direction.LEFT, 1, vertex.Row, vertex.Col - 1),
            new Vertex(Direction.RIGHT, 1, vertex.Row, vertex.Col + 1),
            new Vertex(Direction.UP, 1, vertex.Row - 1, vertex.Col),
            new Vertex(Direction.DOWN, 1, vertex.Row + 1, vertex.Col)];

        return directions
            .Select(x => x.Direction == vertex.Direction ? x with { Consecutive = vertex.Consecutive + 1 } : x)
            .Where(x => x.Direction != vertex.Direction.Opposite())
            .Where(x => x.Consecutive <= 3)
            .Where(x => x.Row >= 0 && x.Row < weights.Length)
            .Where(x => x.Col >= 0 && x.Col < weights[0].Length);
    }

    public IEnumerable<Vertex> NeighborsPart2(Vertex vertex)
    {
        var d = vertex.Direction;

        IEnumerable<Vertex> directions = [
            new Vertex(Direction.LEFT, d == Direction.LEFT ? vertex.Consecutive + 1 : 4, vertex.Row, vertex.Col - (d == Direction.LEFT ? 1 : 4)),
            new Vertex(Direction.RIGHT, d == Direction.RIGHT ? vertex.Consecutive + 1 : 4, vertex.Row, vertex.Col + (d == Direction.RIGHT ? 1 : 4)),
            new Vertex(Direction.UP, d == Direction.UP ? vertex.Consecutive + 1 : 4, vertex.Row - (d == Direction.UP ? 1 : 4), vertex.Col),
            new Vertex(Direction.DOWN, d == Direction.DOWN ? vertex.Consecutive + 1 : 4, vertex.Row + (d == Direction.DOWN ? 1 : 4), vertex.Col)];

        return directions
            .Where(x => x.Direction != vertex.Direction.Opposite())
            .Where(x => x.Consecutive <= 10)
            .Where(x => x.Row >= 0 && x.Row < weights.Length)
            .Where(x => x.Col >= 0 && x.Col < weights[0].Length);
    }

    public int MinimumHeat()
    {
        PriorityQueue<Vertex, int> queue = new();
        Dictionary<Vertex, int> distances = [];

        var source = new Vertex(Direction.NONE, 0, 0, 0);
        queue.Enqueue(source, 0);

        while (queue.TryDequeue(out var current, out var distance))
        {
            var neighbors = Neighbors(current).ToArray();

            foreach (var neighbor in neighbors)
            {
                var alt = distance + GetWeight(current, neighbor);
                if (alt < distances.GetValueOrDefault(neighbor, int.MaxValue) && !distances.ContainsKey(neighbor))
                {
                    distances.Add(neighbor, alt);
                    queue.Enqueue(neighbor, alt);
                }

                if (Done(neighbor))
                {
                    return alt;
                }
            }
        }
        throw new NotSupportedException();
    }

    public int GetWeight(Vertex start, Vertex end)
    {
        if (start.Row == end.Row)
        {
            var columns = Math.Min(start.Col, end.Col)..(Math.Max(start.Col, end.Col) + 1);
            return weights[start.Row][columns].Sum() - weights[start.Row][start.Col];
        }
        if (start.Col == end.Col)
        {
            var rows = Math.Min(start.Row, end.Row)..(Math.Max(start.Row, end.Row) + 1);
            return weights[rows].Select(x => x[start.Col]).Sum() - weights[start.Row][start.Col];
        }
        throw new NotSupportedException();
    }

    public bool Done(Vertex vertex) =>
        vertex.Row == weights.Length - 1 && vertex.Col == weights[0].Length - 1;
}

internal class Solution()
{
    public int Part1(IEnumerable<string> input)
    {
        var graph = new Graph(input, 1);
        return graph.MinimumHeat();
    }

    public int Part2(IEnumerable<string> input)
    {
        var graph = new Graph(input, 2);
        return graph.MinimumHeat();
    }
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Simple
    {
        get => """
            12311
            99921
            78911
            """.ReadLines();
    }

    public IEnumerable<string> Sample
    {
        get => """
            2413432311323
            3215453535623
            3255245654254
            3446585845452
            4546657867536
            1438598798454
            4457876987766
            3637877979653
            4654967986887
            4564679986453
            1224686865563
            2546548887735
            4322674655533
            """.ReadLines();
    }

    public IEnumerable<string> Sample2
    {
        get => """
            111111111111
            999999999991
            999999999991
            999999999991
            999999999991
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Simple() =>
        Assert.Equal(10, solution.Part1(Simple));

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(102, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(845, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(94, solution.Part2(Sample));

    [Fact]
    public void Part2Sample2() =>
        Assert.Equal(71, solution.Part2(Sample2));

    [Fact]
    public void Part2() =>
        Assert.Equal(993, solution.Part2(Input));
}
