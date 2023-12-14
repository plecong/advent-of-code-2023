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

    public bool Valid(char[] value)
    {
        return Positions.Zip(value).All(pair => pair.First == pair.Second || pair.First == '?');
    }

    public IEnumerable<int[]> GenerateGaps()
    {
        var length = Groups.Length + 1;
        var target = Positions.Length - Groups.Sum();
        return _GenerateSpaces(new int[length], 0, 0);

        IEnumerable<int[]> _GenerateSpaces(int[] combo, int index, int current)
        {
            if (index == length)
            {
                if (current == target)
                {
                    yield return combo.ToArray();
                }
                yield break;
            }

            var start = (index == 0 || index == length - 1) ? 0 : 1;
            for (var i = start; i <= target - current; i++)
            {
                combo[index] = i;
                foreach (var result in _GenerateSpaces(combo, index + 1, current + i))
                {
                    yield return result;
                }
            }
        }
    }

    public IEnumerable<char[]> GenerateCandidates()
    {
        return _GenerateCandidates().Where(Valid);

        IEnumerable<char[]> _GenerateCandidates()
        {
            var gaps = GenerateGaps().ToList();

            foreach (var gap in gaps)
            {
                List<char> candidate = new();

                for (var i = 0; i < gap.Length - 1; i++)
                {
                    candidate.AddRange(Enumerable.Repeat('.', gap[i]));
                    candidate.AddRange(Enumerable.Repeat('#', Groups[i]));
                }

                candidate.AddRange(Enumerable.Repeat('.', gap[^1]));
                yield return candidate.ToArray();
            }
        }
    }
}

internal class Solution()
{
    public long Part1(IEnumerable<string> input) =>
        input
            .Select(Record.Parse)
            .Select(x => x.GenerateCandidates().Count())
            .Sum();


    public long Part2(IEnumerable<string> input) =>
        input
            .Select(Record.ParseFolded)
            .Select(x => x.GenerateCandidates().Count())
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
        Assert.Single(Record.Parse("???.### 1,1,3").GenerateCandidates());

    [Fact]
    public void Part1SampleLine2() =>
        Assert.Equal(4, Record.Parse(".??..??...?##. 1,1,3").GenerateCandidates().Count());

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
    public void Part2() =>
        Assert.Equal(0, solution.Part2(Input));
}
