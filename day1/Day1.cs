internal class Day1 : Day
{
    protected override string InputPath => "/day1/input.txt";

    internal override string A()
    {
        var (left, right) = GetLists();
        var sum = 0;
        for(var i = 0; i < left.Length; i++)
        {
            sum += Math.Abs(left[i] - right[i]);
        }

        return $"The sum of all differences is {sum}";
    }

    internal override string B()
    {
        var (left, right) = GetLists();
        var sum = left.Aggregate(0, (a,b) => a + (b * right.Count(x => x == b)));
        return $"The similarity score is {sum}";
    }

    private (int[], int[]) GetLists()
    {
        var parsed = Input.Split("\r\n")
            .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            .Select(x => new { Left = x[0], Right = x[1]});

        var l = parsed.Select(x => int.Parse(x.Left));
        var r = parsed.Select(x => int.Parse(x.Right));

        var left = l.Order().ToArray();
        var right = r.Order().ToArray();
        return (left, right);
    }
}
