namespace AdventOfCode2023.Day05;

using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AdventOfCode2023.Utils;

internal record Mapper(long Destination, long Source, long Range);

internal record Range(long Start, long Length)
{

}

internal class Map
{
    public List<Mapper> Mappers { get; set; } = new();

    public long Translate(long position)
    {
        var mapper = Mappers.Where(x => position >= x.Source && position < x.Source + x.Range).FirstOrDefault();
        if (mapper != null)
        {
            var offset = mapper.Destination - mapper.Source;
            return position + offset;
        }
        else
        {
            return position;
        }
    }

    public long Reverse(long position)
    {
        var mapper = Mappers.Where(x => position >= x.Destination && position < x.Destination + x.Range).FirstOrDefault();
        if (mapper != null)
        {
            var offset = mapper.Source - mapper.Destination;
            return position + offset;
        }
        else
        {
            return position;
        }
    }
}

internal record Seed(long Start, long Length)
{
    public bool Contains(long index) => index >= Start && index < Start + Length;
}

internal class Solution
{
    List<long> seeds;
    List<Map> maps = new();

    private void LoadInput(IEnumerable<string> input)
    {
        var lines = input.ToList();

        seeds = lines[0].Substring(6).Trim()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();

        Map? current = null;
        for (var i = 1; i < lines.Count; i++)
        {
            if (Regex.IsMatch(lines[i], @"[\w\-]+ map:"))
            {
                current = new Map();
            }

            if (string.IsNullOrWhiteSpace(lines[i]) && current != null)
            {
                current.Mappers = current.Mappers.OrderBy(x => x.Destination).ToList();
                maps.Add(current);
                current = null;
            }

            var parts = lines[i].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 3 && current != null)
            {
                current.Mappers.Add(new Mapper(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2])));
            }
        }

        if (current != null)
        {
            maps.Add(current);
        }


    }

    public long Part1(IEnumerable<string> input)
    {
        LoadInput(input);
        var plans = seeds.Select(x => maps.Aggregate(x, (pos, cur) => cur.Translate(pos)));
        return plans.Min();
    }

    public long Part2(IEnumerable<string> input)
    {
        LoadInput(input);
        List<long> positions = new();

        var ranges = seeds.Chunk(2).Select(x => new Seed(x[0], x[1])).ToList();
        maps.Reverse();
        long current = 0;

        while (true)
        {
            var position = maps.Aggregate(current, (pos, cur) => cur.Reverse(pos));

            if (ranges.Any(x => x.Contains(position)))
            {
                break;
            }

            current++;
        }

        return current;


        // foreach (var seed in )
        // {
        //     var start = seed[0];
        //     var length = seed[1];

        //     for (long i = 0; i < length; i++)
        //     {
        //         var current = start + i;
        //         positions.Add(maps.Aggregate(current, (pos, cur) => cur.Translate(pos)));
        //     }
        // }

        // return positions.Min();
    }

}
