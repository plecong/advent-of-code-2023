namespace AdventOfCode2023.Day16;

using Xunit;
using AdventOfCode2023.Utils;
using System.Data;

internal enum Direction
{
    UP, RIGHT, DOWN, LEFT
}

internal record Beam(int Row, int Col, Direction Direction)
{
    public (Beam, Beam?) Encounter(Node node)
    {
        // mark the node visited
        node.Energize();

        return (Direction, node.Value) switch
        {
            // empty space continues same direction
            (var d, '.') => (this.Go(d), null),
            // mirrors
            (Direction.UP, '/') => (this.Go(Direction.RIGHT), null),
            (Direction.RIGHT, '/') => (this.Go(Direction.UP), null),
            (Direction.DOWN, '/') => (this.Go(Direction.LEFT), null),
            (Direction.LEFT, '/') => (this.Go(Direction.DOWN), null),
            (Direction.UP, '\\') => (this.Go(Direction.LEFT), null),
            (Direction.RIGHT, '\\') => (this.Go(Direction.DOWN), null),
            (Direction.DOWN, '\\') => (this.Go(Direction.RIGHT), null),
            (Direction.LEFT, '\\') => (this.Go(Direction.UP), null),

            // horizontal splitter
            (Direction.UP or Direction.DOWN, '-') => (this.Go(Direction.RIGHT), this.Go(Direction.LEFT)),
            (var d, '-') => (this.Go(d), null),

            // vertical splitter
            (Direction.LEFT or Direction.RIGHT, '|') => (this.Go(Direction.UP), this.Go(Direction.DOWN)),
            (var d, '|') => (this.Go(d), null),

            // default
            (_, _) => throw new NotImplementedException()
        };
    }

    public Beam Go(Direction direction)
    {
        return direction switch
        {
            Direction.UP => new Beam(Row - 1, Col, direction),
            Direction.RIGHT => new Beam(Row, Col + 1, direction),
            Direction.DOWN => new Beam(Row + 1, Col, direction),
            Direction.LEFT => new Beam(Row, Col - 1, direction),
            _ => throw new NotImplementedException(),
        };
    }
}

internal record Node(char Value)
{
    public bool Energized { get; private set; } = false;
    public void Energize() { Energized = true; }
}

internal class Contraption(IEnumerable<string> input)
{
    private readonly Node[][] grid =
        input.Select(x => x.Select(y => new Node(y)).ToArray()).ToArray();

    public IEnumerable<Node> Nodes
    {
        get => grid.SelectMany(x => x);
    }

    public bool Valid(Beam beam) =>
        !(beam.Row < 0 || beam.Row >= grid.Length) && !(beam.Col < 0 || beam.Col >= grid[beam.Row].Length);

    public Contraption Run(Beam init)
    {
        // keep stack of beams to process and capture splits
        Stack<Beam> beams = new();
        beams.Push(init);

        // cache visited locations to terminate beams that cycle
        HashSet<Beam> cache = [];

        // process stack of beams
        while (beams.Count > 0)
        {
            var current = beams.Pop();
            var active = true;
            while (active)
            {
                if (!Valid(current) || cache.Contains(current))
                {
                    active = false;
                }
                else
                {
                    // valid and not in cache, so remember it
                    cache.Add(current);

                    // we encounter node and get the next beam
                    var (next, other) = current.Encounter(grid[current.Row][current.Col]);
                    current = next;

                    // push any split beams onto stack 
                    if (other != null)
                    {
                        beams.Push(other);
                    }
                }
            }
        }

        return this;
    }

    public IEnumerable<Beam> GetEdgeBeams()
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        var top = Enumerable.Range(1, cols - 2).Select(col => new Beam(0, col, Direction.DOWN));
        var bottom = Enumerable.Range(1, cols - 2).Select(col => new Beam(rows - 1, col, Direction.UP));
        var left = Enumerable.Range(1, rows - 2).Select(row => new Beam(row, 0, Direction.RIGHT));
        var right = Enumerable.Range(1, rows - 2).Select(row => new Beam(row, cols - 1, Direction.LEFT));

        List<Beam> corners = [
            new Beam(0, 0, Direction.RIGHT), new Beam(0, 0, Direction.DOWN),
            new Beam(0, cols - 1, Direction.LEFT), new Beam(0, cols - 1, Direction.DOWN),
            new Beam(rows - 1, 0, Direction.RIGHT), new Beam(rows - 1, 0, Direction.UP),
            new Beam(rows - 1, cols - 1, Direction.LEFT), new Beam(rows - 1, cols - 1, Direction.UP),
        ];

        return corners.Concat(top).Concat(bottom).Concat(left).Concat(right);
    }
}

internal class Solution()
{
    public int Part1(IEnumerable<string> input)
    {
        // start top-left in right direction
        var init = new Beam(0, 0, Direction.RIGHT);
        return new Contraption(input).Run(init).Nodes.Count(x => x.Energized);
    }

    public int Part2(IEnumerable<string> input)
    {
        var lines = input.ToArray();
        var contraption = new Contraption(lines);
        var inits = contraption.GetEdgeBeams();

        return inits.Select(x => new Contraption(lines).Run(x).Nodes.Count(x => x.Energized)).Max();
    }
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            .|...\....
            |.-.\.....
            .....|-...
            ........|.
            ..........
            .........\
            ..../.\\..
            .-.-/..|..
            .|....-|.\
            ..//.|....
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(46, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(8323, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(51, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(8491, solution.Part2(Input));
}
