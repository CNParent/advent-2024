internal class Day25 : Day
{
    protected override string InputPath => "/day25/input.txt";

    internal override string A()
    {
        var groups = Input.Split("\r\n\r\n").Select(x => x.Split("\r\n").Select(r => r.ToCharArray()).ToArray()).ToArray();
        var locks = groups.Where(g => g[0][0] == '#').Select(GetHeight).Select(x => x.ToArray()).ToArray();
        var keys = groups.Where(g => g[0][0] == '.').Select(GetHeight).Select(x => x.ToArray()).ToArray();

        var possible = 0;
        foreach (var l in locks)
        {
            foreach (var k in keys)
            {
                if (Match(l, k)) possible++;
            }
        }

        return $"There are {possible} sets of matching locks and keys";
    }

    internal override string B()
    {
        return "That's all folks";
    }

    private bool Match(int[] @lock, int[] key)
    {
        for(var i = 0; i < key.Length; i++)
        {
            if (key[i] + @lock[i] > 7) return false;
        }

        return true;
    }

    private IEnumerable<int> GetHeight(char[][] group)
    {
        for (var i = 0; i < group[0].Length; i++)
        {
            yield return group.Count(g => g[i] == '#');
        }
    }
}
