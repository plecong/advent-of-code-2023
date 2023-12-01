using System.Text.RegularExpressions;
using Xunit;

namespace AdventOfCode2023;

public class Day01
{
    private Dictionary<string, string> numbers = new Dictionary<string, string> {
        { "one", "1" },
        { "two", "2" },
        { "three", "3" },
        { "four", "4" },
        { "five", "5" },
        { "six", "6" },
        { "seven", "7" },
        { "eight", "8" },
        { "nine", "9" }
    };

    public char FirstNumber(string value)
    {
        char firstDigit = '0';
        string beforeFirst = value;
        var digits = value.Where(char.IsDigit);

        if (digits.Any())
        {
            firstDigit = digits.First();
            var firstIndex = value.IndexOf(firstDigit);
            beforeFirst = value.Substring(0, firstIndex);
        }

        var found = numbers.Keys
            .Select(x => (Key: x, Index: beforeFirst.IndexOf(x)))
            .Where(x => x.Index > -1)
            .OrderBy(x => x.Index);

        if (found.Any())
        {
            return numbers[found.First().Key].ToCharArray()[0];
        }
        else
        {
            return firstDigit;
        }
    }

    public char LastNumber(string value)
    {
        char lastDigit = '0';
        string afterLast = value;
        var digits = value.Where(char.IsDigit);

        if (digits.Any())
        {
            lastDigit = value.Where(char.IsDigit).Last();
            var lastIndex = value.LastIndexOf(lastDigit);
            afterLast = value.Substring(lastIndex);
        }

        var found = numbers.Keys
            .Select(x => (Key: x, Index: afterLast.LastIndexOf(x)))
            .Where(x => x.Index > -1)
            .OrderByDescending(x => x.Index);

        if (found.Any())
        {
            return numbers[found.First().Key].ToCharArray()[0];
        }
        else
        {
            return lastDigit;
        }
    }

    private IEnumerable<string> Input { get { return File.ReadLines("input.txt"); } }

    public int Part1(IEnumerable<string>? input = null) =>
        (input ?? Input)
            .Select(x => x.Where(char.IsDigit).ToArray())
            .Where(x => x.Length > 0)
            .Select(x => new string([x.First(), x.Last()]))
            .Select(x => int.Parse(x))
            .Sum();


    public int Part2(IEnumerable<string>? input = null) =>
         (input ?? Input)
            .Select(x => new string([FirstNumber(x), LastNumber(x)]))
            .Select(x => int.Parse(x))
            .Sum();
}

public class Day01Test
{
    [Fact]
    public void SamplePart1()
    {
        var sample = @"
            1abc2
            pqr3stu8vwx
            a1b2c3d4e5f
            treb7uchet";

        var output = new Day01().Part1(sample.Split("\n"));
        Assert.Equal(142, output);
    }

    [Fact]
    public void Part1()
    {
        var output = new Day01().Part1();
        Assert.Equal(54388, output);
    }


    [Fact]
    public void TestReplace()
    {
        var day = new Day01();
        var input = "two1nine";
        var output = (day.FirstNumber(input), day.LastNumber(input));
        Assert.Equal(('2', '9'), output);
    }

    [Fact]
    public void TestReplaceOverlap()
    {
        var day = new Day01();
        var input = "xtwone3four";
        var output = (day.FirstNumber(input), day.LastNumber(input));
        Assert.Equal(('2', '4'), output);
    }

    [Fact]
    public void TestReplaceOverlap2()
    {
        var day = new Day01();
        var input = "zoneight234";
        var output = (day.FirstNumber(input), day.LastNumber(input));
        Assert.Equal(('1', '4'), output);
    }

    [Fact]
    public void SamplePart2()
    {
        var sample = @"
            two1nine
            eightwothree
            abcone2threexyz
            xtwone3four
            4nineeightseven2
            zoneight234
            7pqrstsixteen";

        var output = new Day01().Part2(sample.Split("\n"));
        Assert.Equal(281, output);
    }

    [Fact]
    public void Part2()
    {
        var output = new Day01().Part2();
        Assert.Equal(53515, output);
    }
}
