namespace AdventOfCode2023.Utils;

public static class Extensions
{
    public static string[] Transpose(this string[] values)
    {
        return Enumerable.Range(0, values[0].Length)
            .Select(col => new string(values.Select(row => row[col]).ToArray()))
            .ToArray();
    }

    public static void Deconstruct<T>(this IList<T> list, out T first, out IList<T> rest)
    {

        first = list.Count > 0 ? list[0] : default(T); // or throw
        rest = list.Skip(1).ToList();
    }

    public static void Deconstruct<T>(this IList<T> list, out T first, out T second, out IList<T> rest)
    {
        first = list.Count > 0 ? list[0] : default(T); // or throw
        second = list.Count > 1 ? list[1] : default(T); // or throw
        rest = list.Skip(2).ToList();
    }

    public static IEnumerable<string> ReadLines(this string value)
    {
        var reader = new StringReader(value);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }

    public static bool AddNonNull<T>(this HashSet<T> set, T? item)
        where T : class
    {
        if (item == null)
            return false;

        return set.Add(item);
    }

    public static IEnumerable<List<T>> ChunkBy<T>(this IEnumerable<T> values, Func<T, bool> separatorFunc, bool includeSeparator = false)
    {
        List<T> chunk = new();

        foreach (var item in values)
        {
            if (separatorFunc(item))
            {
                if (includeSeparator)
                {
                    chunk.Add(item);
                }

                if (chunk.Count > 0)
                {
                    yield return chunk;
                    chunk = new();
                }
            }
            else
            {
                chunk.Add(item);
            }
        }

        if (chunk.Count > 0)
        {
            yield return chunk;
        }
    }
}

