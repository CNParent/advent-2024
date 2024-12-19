internal class Day10 : Day
{
    private bool _draw = false;

    protected override string InputPath => "/day10/input.txt";

    private readonly int[][] _directions = [[1,0],[0,1],[-1,0],[0,-1]];        

    internal override string A()
    {
        var sum = 0;
        var map = new Map(Input);
        var progress = 0;
        var total = map.Height * map.Width;
        for(var j = 0; j < map.Height; j++)
        {
            for(var i = 0; i < map.Width; i++)
            {
                Console.Write($"\r{++progress}/{total} [{i},{j}]");
                if (map.HeightValues[j][i] != 0) continue;

                var peaks = GetPeaks(i, j, map, []);
                sum += peaks.Count;
            }
        }

        return $"Trailhead score sum is {sum}";
    }

    internal override string B()
    {
        var sum = 0;
        var map = new Map(Input);
        var progress = 0;
        var total = map.Height * map.Width;
        for(var j = 0; j < map.Height; j++)
        {
            for(var i = 0; i < map.Width; i++)
            {
                Console.Write($"\r{++progress}/{total} [{i},{j}]");
                if (map.HeightValues[j][i] != 0) continue;

                var paths = GetPaths(i, j, map, []);
                sum += paths.Count;
            }
        }

        return $"Trailhead score sum is {sum}";
    }

    private HashSet<string> GetPeaks(int x, int y, Map map, HashSet<string> path)
    {
        Draw(x, y, map);
        var h = map.HeightValues[y][x];
        var key = $"{x},{y}";
        path.Add(key);
        var results = new HashSet<string>();

        if (h == 9) 
        {
            results.Add(string.Join('|', path));
            return results;
        }

        foreach(var direction in _directions)
        {
            var xn = x + direction[1];
            var yn = y + direction[0];
            var keyn = $"{xn},{yn}";
            if (xn < 0 || yn < 0 || xn >= map.Width || yn >= map.Height || path.Contains(keyn)) continue;

            var hn = map.HeightValues[yn][xn];
            if (hn - h != 1 || hn == 0) continue;

            foreach(var result in GetPaths(xn, yn, map, path))
            {
                results.Add(result);
            }
        }

        return results;
    }

    private HashSet<string> GetPaths(int x, int y, Map map, HashSet<string> path)
    {
        Draw(x, y, map);
        var h = map.HeightValues[y][x];
        var key = $"{x},{y}";
        path.Add(key);
        var results = new HashSet<string>();

        if (h == 9) 
        {
            results.Add(string.Join('|', path));
            return results;
        }

        foreach(var direction in _directions)
        {
            var xn = x + direction[1];
            var yn = y + direction[0];
            var keyn = $"{xn},{yn}";
            if (xn < 0 || yn < 0 || xn >= map.Width || yn >= map.Height || path.Contains(keyn)) continue;

            var hn = map.HeightValues[yn][xn];
            if (hn - h != 1 || hn == 0) continue;

            foreach(var result in GetPeaks(xn, yn, map, [..path]))
            {
                results.Add(result);
            }
        }

        return results;
    }

    private void Draw(int x, int y, Map map)
    {
        if(!_draw) return;

        var xMin = x - 10;
        var yMin = y - 10;
        var xMax = x + 10;
        var yMax = y + 10;

        var display = "";
        for (var j = yMin; j <= yMax; j++)
        {
            for (var i = xMin; i <= xMax; i++)
            {
                if (i < 0 || i > map.Width - 1 || j < 0 || j > map.Height - 1) display += " ";
                else if (i == x && j == y) display += "^";
                else display += map.HeightValues[j][i].ToString();
            }

            display += "\r\n";
        }

        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"\r\n{display}");
        Console.WriteLine($"\r\n[{x},{y}]");
        Console.ReadKey();
    }

    private class Map
    {
        internal int[][] HeightValues { get; }

        internal int Width { get; }

        internal int Height { get; }

        public Map(string input)
        {
            HeightValues = input.Split("\r\n")
                .Select(x => x.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray())
                .ToArray();
            
            Width = HeightValues[0].Length;
            Height = HeightValues.Length;
        }
    }
}
