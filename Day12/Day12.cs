namespace AdventOfCode2023.Day12;

using Xunit;
using AdventOfCode2023.Utils;

internal record Record(char[] Positions, int[] Groups)
{
    public static Record Parse(string input)
    {
        var splits = input.Split();
        return new Record(
            splits[0].ToCharArray(),
            splits[1].Split(',').Select(int.Parse).ToArray());
    }

    public static Record ParseFolded(string input)
    {
        var splits = input.Split();
        return new Record(
            string.Join('?', Enumerable.Repeat(splits[0], 5).ToArray()).ToCharArray(),
            string.Join(',', Enumerable.Repeat(splits[1], 5).ToArray()).Split(',').Select(int.Parse).ToArray());
    }

    public long CountCandidates()
    {
        Dictionary<(int, int), long> cache = new();
        return _CountCandidates(Positions, Groups);

        // recursively parse the characters and match with groups
        long _CountCandidates(char[] chars, int[] groups)
        {
            var key = (chars.Length, groups.Length);
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            // base case - ran out of groups
            if (groups.Length == 0)
            {
                return chars.Any(x => x == '#') ? 0 : 1;
            }

            // not more characters for groups - not valid
            if (chars.Length == 0)
            {
                return 0;
            }

            var current = chars[0];
            var group = groups[0];

            if (current == '.')
            {
                return _CountCandidates(chars[1..], groups);
            }

            if (current == '#' || current == '?')
            {
                // not enough chars - not valid
                if (chars.Length < group)
                {
                    return 0;
                }

                // all group are spring or wilcard, followed by space or wildcard
                if (chars[0..group].All(x => x == '#' || x == '?')
                    && (chars.Length == group || chars[group] == '?' || chars[group] == '.'))
                {
                    // can consume - valid
                    var value =
                        (current == '?' ? _CountCandidates(chars[1..], groups) : 0)
                        + _CountCandidates(chars[Math.Min(chars.Length, group + 1)..], groups[1..]);
                    cache[key] = value;
                    return value;
                }

                return current == '?' ? _CountCandidates(chars[1..], groups) : 0; ;
            }

            throw new ArgumentException();
        }
    }
}

internal class Solution()
{
    public long Part1(IEnumerable<string> input) =>
        input
            .Select(Record.Parse)
            .Select(x => x.CountCandidates())
            .Sum();


    public long Part2(IEnumerable<string> input) =>
        input
            .Select(Record.ParseFolded)
            .Select(x => x.CountCandidates())
            .Sum();
}

public class Test()
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            ???.### 1,1,3
            .??..??...?##. 1,1,3
            ?#?#?#?#?#?#?#? 1,3,1,6
            ????.#...#... 4,1,1
            ????.######..#####. 1,6,5
            ?###???????? 3,2,1
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1SampleLine1() =>
        Assert.Equal(1, Record.Parse("???.### 1,1,3").CountCandidates());

    [Fact]
    public void Part1SampleLine2() =>
        Assert.Equal(4, Record.Parse(".??..??...?##. 1,1,3").CountCandidates());

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(21, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(7344, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(525152, solution.Part2(Sample));

    [Fact]
    public void Part2SampleLine1() =>
        Assert.Equal(1, Record.ParseFolded("???.### 1,1,3").CountCandidates());

    [Fact]
    public void Part2SampleLine2() =>
        Assert.Equal(16384, Record.ParseFolded(".??..??...?##. 1,1,3").CountCandidates());

    [Fact]
    public void Part2SampleLine3() =>
        Assert.Equal(1, Record.ParseFolded("?#?#?#?#?#?#?#? 1,3,1,6").CountCandidates());

    [Fact]
    public void Part2SampleLine4() =>
        Assert.Equal(16, Record.ParseFolded("????.#...#... 4,1,1").CountCandidates());

    [Fact]
    public void Part2SampleLine5() =>
        Assert.Equal(2500, Record.ParseFolded("????.######..#####. 1,6,5").CountCandidates());

    [Fact]
    public void Part2SampleLine6() =>
        Assert.Equal(506250, Record.ParseFolded("?###???????? 3,2,1").CountCandidates());

    [Fact]
    public void Part2() =>
        Assert.Equal(1_088_006_519_007, solution.Part2(Input));
}
