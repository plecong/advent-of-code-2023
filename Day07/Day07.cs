namespace AdventOfCode2023.Day07;

using Xunit;
using AdventOfCode2023.Utils;

internal enum Strength : int
{
    FiveOfAKind = 7,
    FourOfAKind = 6,
    FullHouse = 5,
    ThreeOfAKind = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1
}

internal class HandComparer : IComparer<Hand>
{
    public static HandComparer Instance = new();
    public int Compare(Hand? x, Hand? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var initial = x.Strength.CompareTo(y.Strength);
        if (initial != 0) return initial;

        return x.Ranks
            .Zip(y.Ranks)
            .Select(z => z.First.CompareTo(z.Second))
            .Where(z => z != 0)
            .FirstOrDefault(0);
    }
}

internal class JokerComparer : IComparer<Hand>
{
    public static JokerComparer Instance = new();
    public int Compare(Hand? x, Hand? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        var initial = x.StrengthWithJokers.CompareTo(y.StrengthWithJokers);
        if (initial != 0) return initial;

        return x.RanksWithJokers
            .Zip(y.RanksWithJokers)
            .Select(z => z.First.CompareTo(z.Second))
            .Where(z => z != 0)
            .FirstOrDefault(0);
    }
}

internal record Hand(string Cards, int Bid = 0)
{
    static char[] CARD = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];

    static char[] CARD_WITH_JOKERS = ['A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J'];

    static List<(Strength Strength, Func<string, bool> Test)> parser = [
        ( Strength.FiveOfAKind, (cards) => cards.All(c => c == cards[0]) ),
        ( Strength.FourOfAKind, (cards) => cards.Any(c => cards.Count(x => x == c) == 4) ),
        ( Strength.FullHouse, (cards) => cards.Distinct().Count() == 2 && cards.Distinct().Any(c => cards.Count(x => x == c) == 2) ),
        ( Strength.ThreeOfAKind, (cards) => cards.Any(c => cards.Count(x => x == c) == 3) ),
        ( Strength.TwoPair, (cards) => cards.Distinct().Count(c => cards.Count(x => x == c) == 2) == 2 ),
        ( Strength.OnePair, (cards) => cards.Distinct().Any(c => cards.Count(x => x == c) == 2) ),
        ( Strength.HighCard, (cards) => cards.Distinct().Count() == 5 ),
    ];

    public Strength Strength
    {
        get => parser.Find(x => x.Test(Cards)).Strength;
    }

    public IEnumerable<int> Ranks
    {
        get => Cards.Select(x => CARD.Length - Array.IndexOf(CARD, x));
    }

    public Strength StrengthWithJokers
    {
        get => (Strength, Cards.Where(x => x == 'J').Count()) switch
        {
            // hand doesn't contain a joker
            (_, 0) => Strength,
            // 5 of a kind cannot be upgraded
            (Strength.FiveOfAKind, _) => Strength,
            // 4 of a kind with joker upgrade to 5 of a kind
            (Strength.FourOfAKind, _) => Strength.FiveOfAKind,
            // full house can have either 2 or 3 jokers
            (Strength.FullHouse, _) => Strength.FiveOfAKind,
            // 3 of a kind with 3 jokers or 1 joker can convert to 4 of a kind
            (Strength.ThreeOfAKind, _) => Strength.FourOfAKind,
            // 2 pair with 1 pair J go to 4 of a kind
            (Strength.TwoPair, 2) => Strength.FourOfAKind,
            // 2 pair with single J go to 3 of a kind + 1 pair => full house
            (Strength.TwoPair, 1) => Strength.FullHouse,
            // 1 pair with either 2 J or 1 J becomes 3 of a kind
            (Strength.OnePair, _) => Strength.ThreeOfAKind,
            // high card becomes pair
            (Strength.HighCard, _) => Strength.OnePair,
            // default return original
            (_, _) => Strength
        };
    }

    public IEnumerable<int> RanksWithJokers
    {
        get => Cards.Select(x => CARD_WITH_JOKERS.Length - Array.IndexOf(CARD_WITH_JOKERS, x));
    }
}

internal class Solution
{
    private int CompareHands(IEnumerable<string> input, IComparer<Hand> comparer) =>
        input
            .Select(x => x.Split())
            .Select(x => new Hand(x[0], int.Parse(x[1])))
            .Order(comparer)
            .Select((x, rank) => x.Bid * (rank + 1))
            .Sum();

    public int Part1(IEnumerable<string> input) => CompareHands(input, HandComparer.Instance);

    public long Part2(IEnumerable<string> input) => CompareHands(input, JokerComparer.Instance);
}


public class Test
{
    private Solution solution = new();

    private IEnumerable<string> Sample
    {
        get => """
            32T3K 765
            T55J5 684
            KK677 28
            KTJJT 220
            QQQJA 483
            """.ReadLines();
    }

    private IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void TestFiveOfAKind() =>
        Assert.Equal(Strength.FiveOfAKind, new Hand("AAAAA").Strength);

    [Fact]
    public void TestFourOfAKind() =>
        Assert.Equal(Strength.FourOfAKind, new Hand("AA8AA").Strength);

    [Fact]
    public void TestFullHouse() =>
        Assert.Equal(Strength.FullHouse, new Hand("23332").Strength);

    [Fact]
    public void TestThreeOfAKind() =>
        Assert.Equal(Strength.ThreeOfAKind, new Hand("TTT98").Strength);

    [Fact]
    public void TestTwoPair() =>
        Assert.Equal(Strength.TwoPair, new Hand("23432").Strength);

    [Fact]
    public void TestOnePair() =>
        Assert.Equal(Strength.OnePair, new Hand("A23A4").Strength);

    [Fact]
    public void TestHighCard() =>
        Assert.Equal(Strength.HighCard, new Hand("23456").Strength);

    [Fact]
    public void TestCompareHands() =>
        Assert.Equal(
            new Hand("33332"),
            new[] { new Hand("33332"), new Hand("2AAAA") }
                .OrderDescending(HandComparer.Instance)
                .First());

    [Fact]
    public void TestCompareHandsAlternate() =>
        Assert.Equal
            (new Hand("33332"),
            new[] { new Hand("2AAAA"), new Hand("33332") }
                .OrderDescending(HandComparer.Instance)
                .First());

    [Fact]
    public void TestCompareHandsFullHouse() =>
        Assert.Equal(new Hand("77888"),
            new[] { new Hand("77888"), new Hand("77788") }
                .OrderDescending(HandComparer.Instance)
                .First());

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(6440, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(253638586, solution.Part1(Input));

    [Fact]
    public void TestUpgradeHandType()
    {
        Assert.Equal(Strength.FourOfAKind, new Hand("QJJQ2").StrengthWithJokers);
        Assert.Equal(Strength.OnePair, new Hand("32T3K").StrengthWithJokers);
        Assert.Equal(Strength.TwoPair, new Hand("KK677").StrengthWithJokers);
        Assert.Equal(Strength.FourOfAKind, new Hand("T55J5").StrengthWithJokers);
        Assert.Equal(Strength.FullHouse, new Hand("AAJ77").StrengthWithJokers);
    }

    [Fact]
    public void TestCompareJokerHands() =>
        Assert.Equal(
            new Hand("QQQQ2"),
            new[] { new Hand("JKKK2"), new Hand("QQQQ2") }
                .OrderDescending(JokerComparer.Instance)
                .First());

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(5905, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(253253225, solution.Part2(Input));
}
