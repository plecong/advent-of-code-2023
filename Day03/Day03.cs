using Xunit;
using AdventOfCode2023.Utils;

namespace AdventOfCode023;

public class Part
{
    public IList<char> Chars { get; } = new List<char>();
    public int Value { get => int.Parse(new string(Chars.ToArray())); }
    public bool Adjacent { get; set; }
    public void MarkAdjacent()
    {
        Adjacent = true;
    }
}

public class Cell(char value)
{
    public char Value { get; } = value;
    public Part? Part { get; set; }
}

public class Day03
{
    private Cell[][]? matrix;

    private IList<Part> parts = new List<Part>();

    public void LoadMatrix(IEnumerable<string> input)
    {
        // read input into matrix
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
                var cell = line[col];
                if (char.IsDigit(cell.Value))
                {
                    if (col > 0)
                    {
                        // check previous
                        var previous = line[col - 1];
                        if (previous.Part != null)
                        {
                            cell.Part = previous.Part;
                            cell.Part.Chars.Add(cell.Value);
                            continue;
                        }
                        else
                        {
                            // create a new part
                            cell.Part = new Part();
                            cell.Part.Chars.Add(cell.Value);
                            parts.Add(cell.Part);
                        }
                    }
                    else
                    {
                        // create a new part
                        cell.Part = new Part();
                        cell.Part.Chars.Add(cell.Value);
                        parts.Add(cell.Part);
                    }
                }
            }
        }
    }

    public int Part1(IEnumerable<string> input)
    {
        LoadMatrix(input);
        ArgumentNullException.ThrowIfNull(matrix);

        // iterate through line by line to find symbols 
        for (var row = 0; row < matrix.Length; row++)
        {
            var line = matrix[row];
            for (var col = 0; col < line.Length; col++)
            {
                var cell = line[col];

                if (!char.IsDigit(cell.Value) && cell.Value != '.')
                {
                    // check adjacency
                    if (row > 0)
                    {
                        var above = matrix[row - 1];
                        if (col > 0) { above[col - 1].Part?.MarkAdjacent(); }
                        above[col].Part?.MarkAdjacent();
                        if (col < above.Length - 1) { above[col + 1].Part?.MarkAdjacent(); }
                    }

                    if (col > 0) { line[col - 1].Part?.MarkAdjacent(); }
                    line[col].Part?.MarkAdjacent();
                    if (col < line.Length - 1) { line[col + 1].Part?.MarkAdjacent(); }

                    if (row < matrix.Length - 1)
                    {
                        var below = matrix[row + 1];
                        if (col > 0) { below[col - 1].Part?.MarkAdjacent(); }
                        below[col].Part?.MarkAdjacent();
                        if (col < below.Length - 1) { below[col + 1].Part?.MarkAdjacent(); }
                    }
                }
            }
        }

        return parts.Where(x => x.Adjacent).Sum(x => x.Value);
    }

    public int Part2(IEnumerable<string> input)
    {
        LoadMatrix(input);
        ArgumentNullException.ThrowIfNull(matrix);

        var ratios = new List<int>();

        for (var row = 0; row < matrix.Length; row++)
        {
            var line = matrix[row];
            for (var col = 0; col < line.Length; col++)
            {
                var cell = line[col];
                var adjacent = new HashSet<Part>();

                void AddIfNotNull(Part? part)
                {
                    if (part != null)
                    {
                        adjacent.Add(part);
                    }
                }

                if (cell.Value == '*')
                {
                    // check adjacency
                    if (row > 0)
                    {
                        var above = matrix[row - 1];
                        if (col > 0) { AddIfNotNull(above[col - 1].Part); }
                        AddIfNotNull(above[col].Part);
                        if (col < above.Length - 1) { AddIfNotNull(above[col + 1].Part); }
                    }

                    if (col > 0) { AddIfNotNull(line[col - 1].Part); }
                    AddIfNotNull(line[col].Part);
                    if (col < line.Length - 1) { AddIfNotNull(line[col + 1].Part); }

                    if (row < matrix.Length - 1)
                    {
                        var below = matrix[row + 1];
                        if (col > 0) { AddIfNotNull(below[col - 1].Part); }
                        AddIfNotNull(below[col].Part);
                        if (col < below.Length - 1) { AddIfNotNull(below[col + 1].Part); }
                    }
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

public class Day02Test
{
    private Day03 day = new Day03();

    public IEnumerable<string> Sample
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

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void TestPart1Sample()
    {
        var output = day.Part1(Sample);
        Assert.Equal(4361, output);
    }

    [Fact]
    public void TestPart1()
    {
        var output = day.Part1(Input);
        Assert.Equal(544664, output);
    }

    [Fact]
    public void TestPart2Sample()
    {
        var output = day.Part2(Sample);
        Assert.Equal(467835, output);
    }

    [Fact]
    public void TestPart2()
    {
        var output = day.Part2(Input);
        Assert.Equal(84495585, output);
    }
}