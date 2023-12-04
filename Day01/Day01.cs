using Xunit;
using AdventOfCode2023.Utils;

namespace AdventOfCode2023.Day01;

internal class Solution
{
    private Dictionary<string, string> numbers = new() {
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

    public char FirstNumber(string value, bool includeWords = false)
    {
        var index = value.IndexOfAny(numbers.Values.Select(x => x.ToCharArray()[0]).ToArray());

        if (includeWords)
        {
            var found = numbers.Keys
                .Select(x => (Key: x, Index: value.IndexOf(x)))
                .Where(x => x.Index > -1)
                .OrderBy(x => x.Index);

            if (found.Any() && ((index == -1) || (found.First().Index < index)))
            {
                return numbers[found.First().Key].ToCharArray()[0];
            }
        }

        if (index > -1)
        {
            return value.ToCharArray()[index];
        }

        return '0';
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

    public int Part1(IEnumerable<string> input) =>
        input
            .Select(x => x.Where(char.IsDigit).ToArray())
            .Where(x => x.Length > 0)
            .Select(x => new string([x.First(), x.Last()]))
            .Select(x => int.Parse(x))
            .Sum();


    public int Part2(IEnumerable<string> input) =>
         input
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new string([FirstNumber(x, true), LastNumber(x)]))
            .Select(x => int.Parse(x))
            .Sum();
}

public class Test
{
    private Solution solution = new();

    private IEnumerable<string> SamplePart1
    {
        get => """
            1abc2
            pqr3stu8vwx
            a1b2c3d4e5f
            treb7uchet
            """.ReadLines();
    }

    private IEnumerable<string> SamplePart2
    {
        get => """
            two1nine
            eightwothree
            abcone2threexyz
            xtwone3four
            4nineeightseven2
            zoneight234
            7pqrstsixteen
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }


    [Fact]
    public void Part1Sample()
    {
        var output = solution.Part1(SamplePart1);
        Assert.Equal(142, output);
    }

    [Fact]
    public void Part1()
    {
        var output = solution.Part1(Input);
        Assert.Equal(54388, output);
    }


    [Fact]
    public void TestReplace()
    {
        var day = solution;
        var input = "two1nine";
        var output = (day.FirstNumber(input, true), day.LastNumber(input));
        Assert.Equal(('2', '9'), output);
    }

    [Fact]
    public void TestReplaceOverlap()
    {
        var day = solution;
        var input = "xtwone3four";
        var output = (day.FirstNumber(input, true), day.LastNumber(input));
        Assert.Equal(('2', '4'), output);
    }

    [Fact]
    public void TestReplaceOverlap2()
    {
        var day = solution;
        var input = "zoneight234";
        var output = (day.FirstNumber(input, true), day.LastNumber(input));
        Assert.Equal(('1', '4'), output);
    }

    [Fact]
    public void Part2Sample()
    {
        var output = solution.Part2(SamplePart2);
        Assert.Equal(281, output);
    }

    [Fact]
    public void Part2()
    {
        var output = solution.Part2(Input);
        Assert.Equal(53515, output);
    }
}
