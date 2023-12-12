namespace AdventOfCode2023.Day10;

using Xunit;
using AdventOfCode2023.Utils;

internal enum Direction
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

internal enum Side
{
    UNKNOWN,
    LEFT,
    RIGHT,
    NONE
}

internal enum Loop
{
    INSIDE,
    OUTSIDE
}

internal class Node(char node, int row, int col)
{
    private Side side = Side.UNKNOWN;

    public int Row { get; } = row;
    public int Col { get; } = col;

    public char Current { get; private set; } = node;

    public char Original { get; } = node;

    public bool IsStarting { get => Original == 'S'; }

    public bool IsGround { get => Current == '.'; }

    public bool IsPath { get; set; } = false;

    public Side Side
    {
        get => IsGround ? side : Side.NONE;
    }

    public void ConvertJunk()
    {
        Current = '.';
    }

    public void SetSide(Side value)
    {
        // can only set side on "Ground"
        if (!IsGround) return;
        // never overwrite the side
        if (side != Side.UNKNOWN) return;
        side = value;
    }

    public IEnumerable<Direction> GetDirections()
    {
        return Current switch
        {
            '|' => [Direction.NORTH, Direction.SOUTH],
            '-' => [Direction.WEST, Direction.EAST],
            'L' => [Direction.NORTH, Direction.EAST],
            'J' => [Direction.NORTH, Direction.WEST],
            '7' => [Direction.SOUTH, Direction.WEST],
            'F' => [Direction.SOUTH, Direction.EAST],
            _ => []
        };
    }
}

internal class Maze
{
    private static readonly Direction[] DIRECTIONS = [Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST];

    private Node[][] grid;

    public Node Starting { get; init; }

    public List<Direction> StartingDirections = new();

    public Maze(IEnumerable<string> input)
    {
        grid = input
            .Select((x, row) => x
                .Select((y, col) => new Node(y, row, col))
                .ToArray())
            .ToArray();

        Starting = grid
            .Where(x => x.Any(y => y.IsStarting)).First()
            .Where(x => x.IsStarting).First();

        // find the available directions based on neighbors
        var north = this[(Starting.Row - 1, Starting.Col)];
        var east = this[(Starting.Row, Starting.Col + 1)];
        var south = this[(Starting.Row + 1, Starting.Col)];
        var west = this[(Starting.Row, Starting.Col - 1)];

        if ((north?.GetDirections() ?? []).Contains(Direction.SOUTH)) StartingDirections.Add(Direction.NORTH);
        if ((east?.GetDirections() ?? []).Contains(Direction.WEST)) StartingDirections.Add(Direction.EAST);
        if ((south?.GetDirections() ?? []).Contains(Direction.NORTH)) StartingDirections.Add(Direction.SOUTH);
        if ((west?.GetDirections() ?? []).Contains(Direction.EAST)) StartingDirections.Add(Direction.WEST);
    }

    public Node? this[(int row, int col) coord]
    {
        get =>
            (coord.row < 0 || coord.row >= grid.Length || coord.col < 0 || coord.col >= grid[coord.row].Length)
                ? null
                : grid[coord.row][coord.col];

    }

    public Node? Next(Node entry, Direction direction)
    {
        var (row, col) = direction switch
        {
            Direction.NORTH => (entry.Row - 1, entry.Col),
            Direction.EAST => (entry.Row, entry.Col + 1),
            Direction.SOUTH => (entry.Row + 1, entry.Col),
            Direction.WEST => (entry.Row, entry.Col - 1),
            _ => throw new NotImplementedException()
        };

        return this[(row, col)];
    }

    public Direction Opposite(Direction direction)
    {
        return direction switch
        {
            Direction.NORTH => Direction.SOUTH,
            Direction.SOUTH => Direction.NORTH,
            Direction.EAST => Direction.WEST,
            Direction.WEST => Direction.EAST,
            _ => throw new NotImplementedException()
        };
    }


    public (Node Current, Direction Next) Navigate(Node entry, Direction direction)
    {
        var current = Next(entry, direction);
        var inbound = Opposite(direction);
        var outbound = (current?.GetDirections() ?? []).Where(x => x != inbound).FirstOrDefault();

        // find the other directions, set their side
        switch (inbound, outbound)
        {
            // for the |
            case (Direction.NORTH, Direction.SOUTH):
                // from top -> East is Left, West is Right
                Next(current, Direction.EAST)?.SetSide(Side.LEFT);
                Next(current, Direction.WEST)?.SetSide(Side.RIGHT);
                break;
            case (Direction.SOUTH, Direction.NORTH):
                // from bottom -> East is Right, West is Left
                Next(current, Direction.EAST)?.SetSide(Side.RIGHT);
                Next(current, Direction.WEST)?.SetSide(Side.LEFT);
                break;
            // for the -
            case (Direction.EAST, Direction.WEST):
                // from right -> East is Left, West is Right
                Next(current, Direction.NORTH)?.SetSide(Side.RIGHT);
                Next(current, Direction.SOUTH)?.SetSide(Side.LEFT);
                break;
            case (Direction.WEST, Direction.EAST):
                // from bottom -> East is Right, West is Left
                Next(current, Direction.NORTH)?.SetSide(Side.LEFT);
                Next(current, Direction.SOUTH)?.SetSide(Side.RIGHT);
                break;

            // for the L
            case (Direction.NORTH, Direction.EAST):
                Next(current, Direction.WEST)?.SetSide(Side.RIGHT);
                Next(current, Direction.SOUTH)?.SetSide(Side.RIGHT);
                break;
            case (Direction.EAST, Direction.NORTH):
                Next(current, Direction.WEST)?.SetSide(Side.LEFT);
                Next(current, Direction.SOUTH)?.SetSide(Side.LEFT);
                break;
            // for the J
            case (Direction.NORTH, Direction.WEST):
                Next(current, Direction.EAST)?.SetSide(Side.LEFT);
                Next(current, Direction.SOUTH)?.SetSide(Side.LEFT);
                break;
            case (Direction.WEST, Direction.NORTH):
                Next(current, Direction.EAST)?.SetSide(Side.RIGHT);
                Next(current, Direction.SOUTH)?.SetSide(Side.RIGHT);
                break;

            // for the 7
            case (Direction.SOUTH, Direction.WEST):
                Next(current, Direction.EAST)?.SetSide(Side.RIGHT);
                Next(current, Direction.NORTH)?.SetSide(Side.RIGHT);
                break;
            case (Direction.WEST, Direction.SOUTH):
                Next(current, Direction.EAST)?.SetSide(Side.LEFT);
                Next(current, Direction.NORTH)?.SetSide(Side.LEFT);
                break;
            // for the F
            case (Direction.SOUTH, Direction.EAST):
                Next(current, Direction.WEST)?.SetSide(Side.LEFT);
                Next(current, Direction.NORTH)?.SetSide(Side.LEFT);
                break;
            case (Direction.EAST, Direction.SOUTH):
                Next(current, Direction.WEST)?.SetSide(Side.RIGHT);
                Next(current, Direction.NORTH)?.SetSide(Side.RIGHT);
                break;
        }

        return (current, outbound);
    }

    public void ConvertToJunk()
    {
        // deal with junk (stuff not on path or ground)
        var junk = grid.SelectMany(x => x.Where(y => !y.IsPath && !y.IsGround && !y.IsStarting));
        foreach (var node in junk)
        {
            node.ConvertJunk();
        }
    }

    public void ColorSides()
    {



        var ground = grid.SelectMany(x => x.Where(y => y.IsGround)).ToList();
        var uncolored = ground.Where(x => x.Side == Side.UNKNOWN);
        var remaining = uncolored.Count();

        while (remaining > 0)
        {
            var colored = ground.Where(x => x.Side != Side.UNKNOWN);
            foreach (var node in colored)
            {
                // color each of touching side the same color
                foreach (var direction in DIRECTIONS)
                {
                    Next(node, direction)?.SetSide(node.Side);
                }
            }

            remaining = ground.Where(x => x.Side == Side.UNKNOWN).Count();
        }
    }

    public Side FindOutsideSide()
    {
        // across top
        var top = grid[0].Where(x => x.IsGround).FirstOrDefault()?.Side;
        var bottom = grid[^1].Where(x => x.IsGround).FirstOrDefault()?.Side;
        var left = grid.Select(x => x[0]).Where(x => x.IsGround).FirstOrDefault()?.Side;
        var right = grid.Select(x => x[^1]).Where(x => x.IsGround).FirstOrDefault()?.Side;

        return top ?? bottom ?? left ?? right ?? Side.UNKNOWN;
    }

    public int CountSide(Side side)
    {
        return grid.SelectMany(x => x.Where(y => y.Side == side)).Count();
    }

    public List<Node> Loop()
    {
        var nodes = new List<Node>();

        var current = Starting;
        var next = StartingDirections[0];

        current.IsPath = true;
        nodes.Add(current);

        while (true)
        {
            (current, next) = Navigate(current, next);

            if (current.IsStarting)
            {
                break;
            }

            current.IsPath = true;
            nodes.Add(current);
        }

        return nodes;
    }

}


internal class Solution()
{
    public int Part1(IEnumerable<string> input)
    {
        var maze = new Maze(input);
        var nodes = maze.Loop();
        return nodes.Count / 2;
    }

    public int Part2(IEnumerable<string> input)
    {
        var maze = new Maze(input);
        var nodes = maze.Loop();

        maze.ConvertToJunk();
        maze.Loop();
        maze.ColorSides();
        var outside = maze.FindOutsideSide();
        var inside = outside switch
        {
            Side.LEFT => Side.RIGHT,
            Side.RIGHT => Side.LEFT,
            _ => throw new NotImplementedException()
        };
        return maze.CountSide(inside);
    }
}

public class Test()
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            -L|F7
            7S-7|
            L|7||
            -L-J|
            L|-JF
            """.ReadLines();
    }

    private IEnumerable<string> Sample2
    {
        get => """
            7-F7-
            .FJ|7
            SJLL7
            |F--J
            LJ.LJ
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(4, solution.Part1(Sample));

    [Fact]
    public void Part1Sample2() =>
        Assert.Equal(8, solution.Part1(Sample2));

    [Fact]
    public void Part1() =>
        Assert.Equal(6599, solution.Part1(Input));


    private IEnumerable<string> Sample3
    {
        get => """
            ...........
            .S-------7.
            .|F-----7|.
            .||.....||.
            .||.....||.
            .|L-7.F-J|.
            .|..|.|..|.
            .L--J.L--J.
            ...........
            """.ReadLines();
    }

    private IEnumerable<string> Sample4
    {
        get => """
            .F----7F7F7F7F-7....
            .|F--7||||||||FJ....
            .||.FJ||||||||L7....
            FJL7L7LJLJ||LJ.L-7..
            L--J.L7...LJS7F-7L7.
            ....F-J..F7FJ|L7L7L7
            ....L7.F7||L7|.L7L7|
            .....|FJLJ|FJ|F7|.LJ
            ....FJL-7.||.||||...
            ....L---J.LJ.LJLJ...
            """.ReadLines();
    }

    private IEnumerable<string> Sample5
    {
        get => """
            FF7FSF7F7F7F7F7F---7
            L|LJ||||||||||||F--J
            FL-7LJLJ||||||LJL-77
            F--JF--7||LJLJ7F7FJ-
            L---JF-JLJ.||-FJLJJ7
            |F|F-JF---7F7-L7L|7|
            |FFJF7L7F-JF7|JL---7
            7-L-JL7||F7|L7F-7F7|
            L.L7LFJ|||||FJL7||LJ
            L7JLJL-JLJLJL--JLJ.L
            """.ReadLines();
    }

    [Fact]
    public void Part2Sample3() =>
        Assert.Equal(4, solution.Part2(Sample3));

    [Fact]
    public void Part2Sample4() =>
        Assert.Equal(8, solution.Part2(Sample4));

    [Fact]
    public void Part2Sample5() =>
        Assert.Equal(10, solution.Part2(Sample5));

    [Fact]
    public void Part2() =>
        Assert.Equal(477, solution.Part2(Input));
}
