namespace AdventOfCode2023.Day19;

using Xunit;
using AdventOfCode2023.Utils;
using System.Text.RegularExpressions;

internal record Span(int Start, int End)
{
    public long Count { get => End - Start + 1; }
}

internal record PartSpan(Span X, Span M, Span A, Span S)
{
    public (PartSpan WhenTrue, PartSpan WhenFalse) Split(char variable, char comparison, int value)
    {
        return (variable, comparison) switch
        {
            ('x', '>') => (this with { X = new Span(value + 1, X.End) }, this with { X = new Span(X.Start, value) }),
            ('x', '<') => (this with { X = new Span(X.Start, value - 1) }, this with { X = new Span(value, X.End) }),
            ('m', '>') => (this with { M = new Span(value + 1, M.End) }, this with { M = new Span(M.Start, value) }),
            ('m', '<') => (this with { M = new Span(M.Start, value - 1) }, this with { M = new Span(value, M.End) }),
            ('a', '>') => (this with { A = new Span(value + 1, A.End) }, this with { A = new Span(A.Start, value) }),
            ('a', '<') => (this with { A = new Span(A.Start, value - 1) }, this with { A = new Span(value, A.End) }),
            ('s', '>') => (this with { S = new Span(value + 1, S.End) }, this with { S = new Span(S.Start, value) }),
            ('s', '<') => (this with { S = new Span(S.Start, value - 1) }, this with { S = new Span(value, S.End) }),
            (_, _) => throw new NotSupportedException()
        };
    }

    public long Count { get => X.Count * M.Count * A.Count * S.Count; }
}

internal record Part(int X, int M, int A, int S)
{
    public static Part Parse(string input)
    {
        var parts = Regex.Match(input, @"^{x=(\d+),m=(\d+),a=(\d+),s=(\d+)}$").Groups;
        return new Part(int.Parse(parts[1].Value), int.Parse(parts[2].Value), int.Parse(parts[3].Value), int.Parse(parts[4].Value));
    }

    public int Ratings { get => X + M + A + S; }
}

internal record Outcome(string Value)
{
    public bool Accepted { get => Value.Equals("A"); }
    public bool Rejected { get => Value.Equals("R"); }
    public bool Done { get => Accepted || Rejected; }
    public string Workflow { get => Value; }
}

internal record Rule(char? Variable, char? Comparison, int Value, Outcome Outcome)
{
    public static Rule Parse(string input)
    {
        // parse a<123:bds or bds
        var parts = Regex.Match(input, @"^([xmas][<>]\d+:)?(\w+)$").Groups;

        var check = parts[1].Value;
        var outcome = new Outcome(parts[2].Value);

        if (check.Length > 0)
        {
            var variable = check[0];
            var comparison = check[1];
            var value = int.Parse(check[2..^1]);
            return new Rule(variable, comparison, value, outcome);
        }
        else
        {
            return new Rule(null, null, 0, outcome);
        }
    }

    public bool Matches(Part p) => (Variable, Comparison) switch
    {
        ('x', '>') => p.X > Value,
        ('x', '<') => p.X < Value,
        ('m', '>') => p.M > Value,
        ('m', '<') => p.M < Value,
        ('a', '>') => p.A > Value,
        ('a', '<') => p.A < Value,
        ('s', '>') => p.S > Value,
        ('s', '<') => p.S < Value,
        _ => true
    };

    public Outcome Apply(Part p) => Outcome;
}

internal record Workflow(string Name, List<Rule> Rules)
{
    public static Workflow Parse(string input)
    {
        var parts = Regex.Match(input, @"^(\w+){(.*)}$").Groups;
        return new Workflow(
            parts[1].Value,
            parts[2].Value.Split(',').Select(Rule.Parse).ToList());
    }

    public Outcome Process(Part part) =>
        (Rules.Find(x => x.Matches(part)) ?? Rules.Last()).Apply(part);

}

internal class Engine(List<Workflow> workflows)
{
    private readonly Dictionary<string, Workflow> lookup = workflows.ToDictionary(w => w.Name, w => w);

    public bool Accepted(Part part)
    {
        var outcome = new Outcome("in");
        while (!outcome.Done)
        {
            var workflow = lookup[outcome.Workflow];
            outcome = workflow.Process(part);
        }
        return outcome.Accepted;
    }

    public long FindCombinations()
    {
        var initial = new PartSpan(new Span(1, 4000), new Span(1, 4000), new Span(1, 4000), new Span(1, 4000));
        return _Process(initial, lookup["in"]).Sum(x => x.Count);

        // recursive process the workflow tree
        IEnumerable<PartSpan> _Process(PartSpan part, Workflow workflow)
        {
            List<(PartSpan, Workflow)> outcomes = new();
            var current = part;

            foreach (var rule in workflow.Rules)
            {
                // when there's a comparison, split and process each part
                if (rule.Variable != null && rule.Comparison != null)
                {
                    var split = current.Split(rule.Variable.Value, rule.Comparison.Value, rule.Value);

                    if (rule.Outcome.Accepted)
                    {
                        yield return split.WhenTrue;
                    }
                    else if (!rule.Outcome.Rejected)
                    {
                        foreach (var inner in _Process(split.WhenTrue, lookup[rule.Outcome.Value]))
                        {
                            yield return inner;
                        }
                    }

                    current = split.WhenFalse;
                }
                else
                {
                    if (rule.Outcome.Accepted)
                    {
                        yield return current;
                    }
                    else if (!rule.Outcome.Rejected)
                    {
                        foreach (var inner in _Process(current, lookup[rule.Outcome.Value]))
                        {
                            yield return inner;
                        }
                    }
                }
            }
        }
    }
}

internal class Solution()
{
    public long Part1(IEnumerable<string> input)
    {
        var chunks = input.ChunkBy(string.IsNullOrWhiteSpace).ToList();
        var workflows = chunks[0].Select(Workflow.Parse).ToList();
        var engine = new Engine(workflows);

        return chunks[1].Select(Part.Parse).Where(x => engine.Accepted(x)).Sum(x => x.Ratings);
    }

    public long Part2(IEnumerable<string> input)
    {
        var chunks = input.ChunkBy(string.IsNullOrWhiteSpace).ToList();
        var workflows = chunks[0].Select(Workflow.Parse).ToList();
        var engine = new Engine(workflows);

        return engine.FindCombinations();
    }
}

public class Test()
{
    private readonly Solution solution = new();

    public IEnumerable<string> Sample
    {
        get => """
            px{a<2006:qkq,m>2090:A,rfg}
            pv{a>1716:R,A}
            lnx{m>1548:A,A}
            rfg{s<537:gd,x>2440:R,A}
            qs{s>3448:A,lnx}
            qkq{x<1416:A,crn}
            crn{x>2662:A,R}
            in{s<1351:px,qqz}
            qqz{s>2770:qs,m<1801:hdj,R}
            gd{a>3333:R,R}
            hdj{m>838:A,pv}

            {x=787,m=2655,a=1222,s=2876}
            {x=1679,m=44,a=2067,s=496}
            {x=2036,m=264,a=79,s=2244}
            {x=2461,m=1339,a=466,s=291}
            {x=2127,m=1623,a=2188,s=1013}
            """.ReadLines();
    }

    public IEnumerable<string> Input
    {
        get => File.ReadAllLines("input.txt");
    }

    [Fact]
    public void Part1Sample() =>
        Assert.Equal(19114, solution.Part1(Sample));

    [Fact]
    public void Part1() =>
        Assert.Equal(287054, solution.Part1(Input));

    [Fact]
    public void Part2Sample() =>
        Assert.Equal(167_409_079_868_000, solution.Part2(Sample));

    [Fact]
    public void Part2() =>
        Assert.Equal(131_619_440_296_497, solution.Part2(Input));
}
