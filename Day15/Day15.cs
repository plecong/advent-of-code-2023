namespace AdventOfCode2023.Day15;

using Xunit;
using AdventOfCode2023.Utils;
using System.Text;
using System.Text.RegularExpressions;

internal enum Op
{
    Remove,
    Insert
}

internal partial record Step(string Code)
{
    public string Label { get; init; } = MyRegex().Match(Code).Groups["Label"].Value;
    public int Box { get => Solution.Hash(Label); }
    public Op Op { get => Code[Label.Length] == '=' ? Op.Insert : Op.Remove; }
    public Lens? Lens { get => new(Label, int.Parse(Code[(Label.Length + 1)..])); }

    [GeneratedRegex(@"^(?<Label>\w+)")]
    private static partial Regex MyRegex();
}

internal record Lens(string Label, int FocalLength);

internal class HashMap
{
    private readonly List<Lens>[] boxes = new List<Lens>[256];

    public HashMap()
    {
        for (var i = 0; i < boxes.Length; i++)
        {
            boxes[i] = [];
        }
    }

    public HashMap Process(Step step)
    {
        // get the box
        var box = boxes[step.Box];

        // check if the box contains lens by same label
        var index = box.FindIndex(x => x.Label.Equals(step.Label));

        // not found, add to end of box
        if (step.Op == Op.Insert)
        {
            if (index == -1)
            {
                box.Add(step.Lens!);
            }
            else
            {
                box[index] = step.Lens!;
            }
        }
        else
        {
            if (index > -1)
            {
                box.RemoveAt(index);
            }
        }


        return this;
    }

    public int FocusingPower() =>
        boxes
            .Select((box, index) =>
                box.Select((lens, slot) => (index + 1) * (slot + 1) * lens.FocalLength).Sum())
            .Sum();
}

internal class Solution()
{
    public static int Hash(string value) =>
        Encoding.ASCII.GetBytes(value)
            .Aggregate(0, (current, x) => (current + x) * 17 % 256);

    public int Part1(IEnumerable<string> input) =>
        input
            .First()
            .Split(",")
            .Select(Hash)
            .Sum();

    public int Part2(IEnumerable<string> input) =>
        input
            .First()
            .Split(",")
            .Select(x => new Step(x))
            .Aggregate(new HashMap(), (map, x) => map.Process(x))
            .FocusingPower();
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Hash() => Assert.Equal(52, Solution.Hash("HASH"));

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(1320, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(509152, solution.Part1(Input));

    [Fact]
    public void TestLensInsert()
    {
        var step = new Step("rn=1");
        Assert.Equal("rn", step.Label);
        Assert.Equal(Op.Insert, step.Op);
        Assert.Equal(1, actual: step.Lens?.FocalLength);
    }

    [Fact]
    public void TestLensRemove()
    {
        var step = new Step("cm-");
        Assert.Equal("cm", step.Label);
        Assert.Equal(Op.Remove, step.Op);
    }

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(145, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(244403, solution.Part2(Input));
}
