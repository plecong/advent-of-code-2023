using Xunit;
using AdventOfCode2023.Utils;
using System.Text.RegularExpressions;

namespace AdventOfCode023.Day04;

public class Card
{
    public int Id { get; init; }
    public ISet<int> Winning { get; init; }
    public ISet<int> Have { get; init; }
    public int OverlapCount => Have.Intersect(Winning).Count();
    public int Points => OverlapCount == 0 ? 0 : (int)Math.Pow(2, OverlapCount - 1);
    public int Count { get; set; } = 1;

    public void Increment(int value = 1)
    {
        Count = Count + value;
    }

    public Card(string line)
    {
        var regex = new Regex(@"^Card +(\d+): ([\d ]+) \| ([\d ]+)\z");
        var match = regex.Match(line);

        if (!match.Success)
        {
            throw new NotSupportedException();
        }

        Id = int.Parse(match.Groups[1].Value);
        Winning = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
        Have = match.Groups[3].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToHashSet();
    }
}

public class Solution
{
    public int Part1(IEnumerable<string> input) =>
        input.Select(x => new Card(x)).Sum(x => x.Points);

    public int Part2(IEnumerable<string> input)
    {
        var cards = input.Select(x => new Card(x)).ToList();

        // iterate through each card and increment counts down
        foreach (var card in cards)
        {
            for (var i = 0; i < card.OverlapCount && card.Id + i < cards.Count; i++)
            {
                cards[card.Id + i].Increment(card.Count);
            }
        }

        return cards.Sum(x => x.Count);
    }
}

public class Test
{
    private Solution solution = new Solution();

    public IEnumerable<string> Sample
    {
        get => """
            Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
            Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
            Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
            Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
            Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
            Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void ParseCard()
    {
        var card = new Card("Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53");
        Assert.Equal(1, card.Id);
        Assert.Equal(5, card.Winning.Count);
        Assert.Equal(8, card.Have.Count);
    }

    [Fact]
    public void Part1Sample()
    {
        var output = solution.Part1(Sample);
        Assert.Equal(13, output);
    }

    [Fact]
    public void Part1()
    {
        var output = solution.Part1(Input);
        Assert.Equal(26443, output);
    }

    [Fact]
    public void Part2Sample()
    {
        var output = solution.Part2(Sample);
        Assert.Equal(30, output);
    }

    [Fact]
    public void Part2()
    {
        var output = solution.Part2(Input);
        Assert.Equal(6284877, output);
    }
}