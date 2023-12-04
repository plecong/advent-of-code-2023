using Xunit;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Day03;

internal class Part
{
    public List<char> Chars { get; } = new();
    public int Value { get => int.Parse(new string(Chars.ToArray())); }
    public bool Adjacent { get; private set; }
    public void MarkAdjacent() => Adjacent = true;
}

internal class Cell(char value)
{
    public char Value { get; } = value;
    public Part? Part { get; set; }
}

internal class Matrix
{
    private Cell[][] matrix;
    public List<Part> Parts { get; } = new();
    public int Rows => matrix.Length;
    public int Cols => matrix[0].Length;

    public Matrix(IEnumerable<string> input)
    {
        matrix = input
            .Select(x =>
                x.ToCharArray()
                    .Select(x => new Cell(x))
                    .ToArray())
            .ToArray();

        // iterate through line by line to find number and spans
        foreach (var line in matrix)
        {
            for (var col = 0; col < line.Length; col++)
            {
                Cell cell = line[col];
                Cell? previous = col > 0 ? line[col - 1] : null;

                if (char.IsDigit(cell.Value))
                {
                    if (previous?.Part != null)
                    {
                        cell.Part = previous.Part;
                    }
                    else
                    {
                        cell.Part = new();
                        Parts.Add(cell.Part);
                    }
                    cell.Part.Chars.Add(cell.Value);
                }
            }
        }
    }

    public char GetValue(int row, int col) =>
        matrix[row][col].Value;


    public Part? this[(int row, int col) coord]
    {
        get =>
            (coord.row < 0 || coord.row >= matrix.Length
            || coord.col < 0 || coord.col >= matrix[coord.row].Length)
                ? null
                : matrix[coord.row][coord.col].Part;
    }
}

internal class Solution
{
    public int Part1(IEnumerable<string> input)
    {
        var matrix = new Matrix(input);

        // iterate through line by line to find symbols 
        for (var row = 0; row < matrix.Rows; row++)
        {
            for (var col = 0; col < matrix.Cols; col++)
            {
                var cell = matrix.GetValue(row, col);
                if (!char.IsDigit(cell) && cell != '.')
                {
                    // check adjacency
                    matrix[(row - 1, col - 1)]?.MarkAdjacent();
                    matrix[(row - 1, col + 0)]?.MarkAdjacent();
                    matrix[(row - 1, col + 1)]?.MarkAdjacent();
                    matrix[(row + 0, col - 1)]?.MarkAdjacent();
                    matrix[(row + 0, col + 0)]?.MarkAdjacent();
                    matrix[(row + 0, col + 1)]?.MarkAdjacent();
                    matrix[(row + 1, col - 1)]?.MarkAdjacent();
                    matrix[(row + 1, col + 0)]?.MarkAdjacent();
                    matrix[(row + 1, col + 1)]?.MarkAdjacent();
                }
            }
        }

        return matrix.Parts.Where(x => x.Adjacent).Sum(x => x.Value);
    }

    public int Part2(IEnumerable<string> input)
    {
        var matrix = new Matrix(input);
        var ratios = new List<int>();

        for (var row = 0; row < matrix.Rows; row++)
        {
            for (var col = 0; col < matrix.Cols; col++)
            {
                var adjacent = new HashSet<Part>();
                if (matrix.GetValue(row, col) == '*')
                {
                    adjacent.AddNonNull(matrix[(row - 1, col - 1)]);
                    adjacent.AddNonNull(matrix[(row - 1, col + 0)]);
                    adjacent.AddNonNull(matrix[(row - 1, col + 1)]);
                    adjacent.AddNonNull(matrix[(row + 0, col - 1)]);
                    adjacent.AddNonNull(matrix[(row + 0, col + 0)]);
                    adjacent.AddNonNull(matrix[(row + 0, col + 1)]);
                    adjacent.AddNonNull(matrix[(row + 1, col - 1)]);
                    adjacent.AddNonNull(matrix[(row + 1, col + 0)]);
                    adjacent.AddNonNull(matrix[(row + 1, col + 1)]);
                }

                if (adjacent.Count == 2)
                {
                    ratios.Add(adjacent.First().Value * adjacent.Last().Value);
                }
            }
        }

        return ratios.Sum();
    }

}

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            467..114..
            ...*......
            ..35..633.
            ......#...
            617*......
            .....+.58.
            ..592.....
            ......755.
            ...$.*....
            .664.598..
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(4361, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(544664, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(467835, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(84495585, solution.Part2(Input));
}