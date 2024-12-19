internal class Day4 : Day
{
    private readonly char[] _xmas = "XMAS".ToCharArray();

    private readonly char[] _mas = "MAS".ToCharArray();

    private readonly int[][] _corners = [[-1,-1],[-1,1],[1,-1],[1,1]];

    private char[][]? _grid = null;

    protected override string InputPath => "/day4/input.txt";

    private char[][] Grid => _grid ??= Input.Split("\r\n").Select(x => x.ToCharArray()).ToArray();

    private int Width => Grid[0].Length;

    private int Height => Grid.Length;

    internal override string A()
    {
        var occurrences = 0;
        for(var y = 0; y < Height; y++)
        {
            for(var x = 0; x < Width; x++)
            {
                occurrences += CountXmas(x, y);
            }
        }

        return $"Found \"XMAS\" {occurrences} times";
    }

    internal override string B()
    {
        var occurrences = 0;
        for(var y = 0; y < Height; y++)
        {
            for(var x = 0; x < Width; x++)
            {
                occurrences += IsXMas(x, y) ? 1 : 0;
            }
        }

        return $"Found \"X-MAS\" {occurrences} times";
    }

    private int CountXmas(int x, int y)
    {
        var occurrences = 0;
        for(var dx = -1; dx <= 1; dx++)
        {
            for(var dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (Look(x, y, dx, dy, _xmas)) occurrences++;
            }
        }

        return occurrences;
    }

    private bool IsXMas(int x, int y)
    {
        var count = 0;
        foreach(var corner in _corners)
        {
            count += Look(x + corner[0], y + corner[1], -corner[0], -corner[1], _mas) ? 1 : 0;
        }

        if (count > 2) throw new Exception($"Impossible configuration detected at ({x}, {y}): found {count} instances of \"MAS\"");

        return count == 2;
    }

    private bool Look(int x, int y, int dx, int dy, char[] target, int expectedIndex = 0)
    {
        if (expectedIndex == target.Length) return true;
        if (x < 0) return false;
        if (y < 0) return false;
        if (x >= Width) return false;
        if (y >= Height) return false;

        var expected = target[expectedIndex];
        if (Grid[y][x] != expected) return false;

        return Look(x + dx, y + dy, dx, dy, target, expectedIndex + 1);
    }
}
