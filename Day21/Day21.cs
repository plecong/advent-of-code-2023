namespace AdventOfCode2023.Day21;

using Xunit;
using AdventOfCode2023.Utils;
using Xunit.Abstractions;

internal record Tile(char Value, int Row, int Col)
{
    public bool Valid { get => Value != '#'; }
}

internal record Coord(int Row, int Col);

internal class Solution(ITestOutputHelper output)
{
    public (Tile[][], Coord) Parse(IEnumerable<string> input)
    {
        Tile[][] grid = input
            .Select((line, row) =>
                line.Select((value, col) => new Tile(value, row, col)).ToArray())
            .ToArray();

        var start = grid
            .SelectMany(row => row.Where(x => x.Value == 'S').Select(x => new Coord(x.Row, x.Col)))
            .Single();

        return (grid, start);
    }

    public Dictionary<Coord, int> BreadthFirstSearch(IEnumerable<string> input)
    {
        var (grid, start) = Parse(input);

        Dictionary<Coord, int> distances = new() { { start, 0 } };
        Queue<(Coord, int)> queue = new();
        queue.Enqueue((start, 0));

        while (queue.TryDequeue(out var current))
        {
            var (coord, distance) = current;

            List<Coord> dirCoords = [
                new (coord.Row - 1, coord.Col),
                new (coord.Row + 1, coord.Col),
                new (coord.Row, coord.Col - 1),
                new (coord.Row, coord.Col + 1)
            ];

            var nextCoords = dirCoords
                .Where(x => x.Row >= 0 && x.Row < grid.Length)
                .Where(x => x.Col >= 0 && x.Col < grid[0].Length)
                .Where(x => grid[x.Row][x.Col].Valid)
                .Distinct();

            foreach (var next in nextCoords)
            {
                if (!distances.ContainsKey(next))
                {
                    distances.Add(next, distance + 1);
                    queue.Enqueue((next, distance + 1));
                }
            }
        }

        return distances;
    }

    public long Part1(IEnumerable<string> input, int steps) =>
        // figure out all the distances, then count up where coords up to steps and same parity
        BreadthFirstSearch(input).Values.Count(x => x <= steps && x % 2 == (steps % 2));

    public long Part2(IEnumerable<string> input, long steps)
    {
        var (grid, start) = Parse(input);

        // using the logic from https://github.com/villuna/aoc23/wiki/A-Geometric-solution-to-advent-of-code-2023,-day-21
        var distances = BreadthFirstSearch(input).Values;

        var tiles = (steps - start.Row) / grid.Length;
        var oddTiles = (long)Math.Pow(tiles + 1, 2) * distances.Count(x => x % 2 == 1);
        var evenTiles = (long)Math.Pow(tiles, 2) * distances.Count(x => x % 2 == 0);
        var oddCorners = (tiles + 1) * distances.Count(x => x % 2 == 1 && x > 65);
        var evenCorners = tiles * distances.Count(x => x % 2 == 0 && x > 65);

        return oddTiles + evenTiles + evenCorners - oddCorners;
    }
}

public class Test(ITestOutputHelper output)
{
    private readonly Solution solution = new(output);

    public IEnumerable<string> Sample
    {
        get => """
            ...........
            .....###.#.
            .###.##..#.
            ..#.#...#..
            ....#.#....
            .##..S####.
            .##..#...#.
            .......##..
            .##.#.####.
            .##..##.##.
            ...........
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(16, solution.Part1(Sample, 6));

    [Fact]
    public void Part1() =>
        Assert.Equal(3598, solution.Part1(Input, 64));

    [Fact]
    public void Part2() =>
        Assert.Equal(601_441_063_166_538, solution.Part2(Input, 26501365));
}
