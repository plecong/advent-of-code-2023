namespace AdventOfCode2023.Day21;

using Xunit;
using AdventOfCode2023.Utils;

internal record Tile(char Value, int Row, int Col)
{
    public bool Valid { get => Value != '#'; }
}

internal class Solution()
{
    public long Part1(IEnumerable<string> input, int steps)
    {
        Tile[][] grid = input.Select((line, row) => line.Select((value, col) => new Tile(value, row, col)).ToArray()).ToArray();
        HashSet<Tile> current = grid.SelectMany(row => row.Where(x => x.Value == 'S')).ToHashSet();

        for (int i = 0; i < steps; i++)
        {
            List<Tile> next = new();
            foreach (var tile in current)
            {
                List<(int Row, int Col)> coords = [
                    (tile.Row - 1, tile.Col),
                    (tile.Row + 1, tile.Col),
                    (tile.Row, tile.Col - 1),
                    (tile.Row, tile.Col + 1)
                ];

                var tileNext = coords
                    .Where(x => x.Row >= 0 && x.Row < grid.Length)
                    .Where(x => x.Col >= 0 && x.Col < grid[0].Length)
                    .Where(x => grid[x.Row][x.Col].Valid)
                    .Select(x => grid[x.Row][x.Col])
                    .Distinct();

                next.AddRange(tileNext);
            }
            current = next.ToHashSet();
        }

        return current.Count;
    }

    public long Part2(IEnumerable<string> input)
    {
        return 0;
    }
}

public class Test()
{
    private readonly Solution solution = new();

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
        Assert.Equal(0, solution.Part2(Input));
}
