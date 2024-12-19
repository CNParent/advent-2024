using System.Text.Json;

internal class Day2 : Day
{
    const int _dangerThreshold = 3;

    protected override string InputPath => "/day2/input.txt";
    internal override string A()
    {
        var levels = GetLevels();
        var safe = levels.Where(IsSafe).Count();
        return $"{safe} reports are safe";
    }

    internal override string B()
    {
        var levels = GetLevels();
        var safe = levels.Where(l =>
        {
            if (IsSafe(l)) return true;

            for(var i = 0; i < l.Length; i++)
            {
                if (IsSafe(l.Where((x, ix) => ix != i).ToArray())) return true;
            }

            return false;
        }).Count();

        return $"{safe} reports are safe";
    }

    private bool IsSafe(int[] levels)
    {
        var differences = new List<int>();
        for(var i = 0; i < levels.Length - 1; i++)
            differences.Add(levels[i] - levels[i+1]);

        return
            differences.All(d => d > 0 && d <= _dangerThreshold) ||
            differences.All(d => d < 0 && d >= -_dangerThreshold);;
    }

    private int[][] GetLevels() =>
        Input.Split("\r\n")
            .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(y => int.Parse(y)).ToArray())
            .ToArray();
}
