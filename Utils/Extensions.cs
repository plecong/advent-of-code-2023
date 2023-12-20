using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode2023.Utils;

public static class Extensions
{
    public static string[] Transpose(this string[] values)
    {
        return Enumerable.Range(0, values[0].Length)
            .Select(col => new string(values.Select(row => row[col]).ToArray()))
            .ToArray();
    }

    public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
    {

        first = list.Count > 0 ? list[0] : default(T); // or throw
        rest = list.Skip(1).ToList();
    }

    public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
    {
        first = list.Count > 0 ? list[0] : default(T); // or throw
        second = list.Count > 1 ? list[1] : default(T); // or throw
        rest = list.Skip(2).ToList();
    }

    public static IEnumerable<string> ReadLines(this string value)
    {
        var reader = new StringReader(value);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    public static bool AddNonNull<T>(this HashSet<T> set, T? item)
        where T : class
    {
        if (item == null)
            return false;

        return set.Add(item);
    }

    public static IEnumerable<List<T>> ChunkBy<T>(this IEnumerable<T> values, Func<T, bool> separatorFunc, bool includeSeparator = false)
    {
        List<T> chunk = new();

        foreach (var item in values)
        {
            if (separatorFunc(item))
            {
                if (includeSeparator)
                {
                    chunk.Add(item);
                }

                if (chunk.Count > 0)
                {
                    yield return chunk;
                    chunk = new();
                }
            }
            else
            {
                chunk.Add(item);
            }
        }

        if (chunk.Count > 0)
        {
            yield return chunk;
        }
    }
}

public enum Direction
{
    UP,
    LEFT,
    DOWN,
    RIGHT,
    NONE
}

public static class EnumExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return direction switch
        {
            Direction.UP => Direction.DOWN,
            Direction.DOWN => Direction.UP,
            Direction.LEFT => Direction.RIGHT,
            Direction.RIGHT => Direction.LEFT,
            Direction.NONE => Direction.NONE,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static char AsChar(this Direction direction)
    {
        return direction switch
        {
            Direction.UP => '^',
            Direction.DOWN => 'v',
            Direction.LEFT => '<',
            Direction.RIGHT => '>',
            _ => throw new ArgumentOutOfRangeException()

        };
    }
}

public class Grid<T>(T[][] grid)
{
    private readonly T[][] grid = grid;

    public Grid(IEnumerable<string> input, Func<char, int, int, T> convert)
        : this(input.Select((x, row) => x.Select((y, col) => convert(y, row, col)).ToArray()).ToArray())
    {

    }

    public IEnumerable<T> Nodes
    {
        get => grid.SelectMany(x => x);
    }

    public T[][] Matrix { get => grid; }

    public int Rows { get => grid.Length; }
    public int Cols { get => grid.Length > 0 ? grid[0].Length : 0; }

    public T? this[(int row, int col) coord]
    {
        get
        {
            if (coord.row < 0 || coord.row >= grid.Length) return default;
            if (coord.col < 0 || coord.col >= grid[coord.row].Length) return default;
            return grid[coord.row][coord.col];
        }
    }

    public (int Row, int Col) Go(int Row, int Col, Direction direction)
    {
        return direction switch
        {
            Direction.UP => (Row - 1, Col),
            Direction.RIGHT => (Row, Col + 1),
            Direction.DOWN => (Row + 1, Col),
            Direction.LEFT => (Row, Col - 1),
            _ => throw new NotImplementedException(),
        };
    }
}
