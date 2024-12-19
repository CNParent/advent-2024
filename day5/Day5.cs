internal class Day5 : Day
{
    protected override string InputPath => "/day5/input.txt";

    private int[][]? _updates;

    private Rule[]? _rules;

    private int[][] Updates => _updates ??= Input
        .Split("\r\n\r\n")[1]
        .Split("\r\n")
        .Select(x => x.Split(",").Select(y => int.Parse(y)).ToArray())
        .ToArray();

    private Rule[] Rules => _rules ??= Input
        .Split("\r\n\r\n")[0]
        .Split("\r\n")
        .Select(x => x.Split("|"))
        .Select(x => new Rule { Before = int.Parse(x[0]), After = int.Parse(x[1])})
        .ToArray();

    internal override string A()
    {
        var sum = 0;
        foreach(var update in Updates)
        {
            sum += ValidateUpdate(update);
        }

        return $"The sum of all middle pages from correctly ordered updates is {sum}";
    }

    internal override string B()
    {
        var sum = 0;
        var unordered = Updates.Where(x => !IsOrdered(x));
        foreach(var update in unordered)
        {
            var sorted = Sort(update);
            sum += ValidateUpdate(sorted);
        }

        return $"The sum of all middle pages from incorrectly ordered updates after sorting is {sum}";
    }

    private int ValidateUpdate(int[] update)
    {
        if (!IsOrdered(update)) return 0;

        var middle = ((update.Length + 1) / 2) - 1;
        return update[middle];
    }

    private bool IsOrdered(int[] update)
    {
        for (var i = 0; i < update.Length; i++)
        {
            for (var j = 0; j < update.Length; j++)
            {
                if (j == i) continue;

                var after = j < i ? update[i] : update[j];
                var before = j < i ? update[j] : update[i];
                var rule = Rules.FirstOrDefault(x => x.Before == after && x.After == before);
                if (rule != null) return false;
            }
        }

        return true;
    }

    private int[] Sort(int[] update)
    {
        if (IsOrdered(update)) return update;

        for (var i = 0; i < update.Length; i++)
        {
            for (var j = 0; j < update.Length; j++)
            {
                if (j == i) continue;

                var after = j < i ? update[i] : update[j];
                var before = j < i ? update[j] : update[i];
                var rule = Rules.FirstOrDefault(x => x.Before == after && x.After == before);
                if (rule != null)
                {
                    (update[j], update[i]) = (update[i], update[j]);
                    return Sort(update);
                }
            }
        }

        throw new Exception("List is not sorted, but could not find an unsorted pair!");
    }

    private class Rule
    {
        public required int Before { get; init; }

        public required int After { get; init; }
    }
}
