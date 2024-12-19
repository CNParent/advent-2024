internal class Day8 : Day
{
    protected override string InputPath => "/day8/input.txt";

    internal override string A()
    {
        var grid = new FrequencyGrid(Input);
        grid.FindMirroredAntinodes();
        return $"Found {grid.Antinodes.Count} antinodes";
    }

    internal override string B()
    {
        var grid = new FrequencyGrid(Input);
        grid.FindAntinodes();
        return $"Found {grid.Antinodes.Count} antinodes";
    }

    private class Antenna
    {
        internal required char Frequency { get; init; }
        internal required int X { get; init; }
        internal required int Y { get; init; }
        internal string Key => $"{X},{Y}";

        internal (int,int) DistanceFrom(Antenna other)
        {
            return (X - other.X, Y - other.Y);
        }

        internal int[] GetVector(Antenna other)
        {
            var (dx, dy) = DistanceFrom(other);
            while((dx % 2 == 0 && dy % 2 == 0) || (dx % 3 == 0 && dy % 3 == 0))
            {
                if (dx % 2 == 0 && dy % 2 == 0)
                {
                    dx /= 2;
                    dy /= 2;
                }
                else
                {
                    dx /= 3;
                    dy /= 3;
                }
            }

            return [dx, dy];
        }

        internal string[] GetAntinodes(Antenna other, int toX, int toY)
        {
            var vector = GetVector(other);
            var antinodes = new List<string>();
            var x = X;
            var y = Y;
            while(x >= 0 && y >= 0 && x < toX && y < toY)
            {
                x += vector[0];
                y += vector[1];
            }

            vector[0] = -vector[0];
            vector[1] = -vector[1];
            x += vector[0];
            y += vector[1];
            while(x >= 0 && y >= 0 && x < toX && y < toY)
            {
                antinodes.Add($"{x},{y}");
                x += vector[0];
                y += vector[1];
            }

            return [..antinodes];
        }

        internal int[][] GetMirroredAntinodes(Antenna other)
        {
            var (dx, dy) = DistanceFrom(other);
            return [[X + dx, Y + dy],[other.X - dx, other.Y - dy]];
        }
    }

    private class FrequencyGrid
    {
        internal Dictionary<string, Antenna> Antennae { get; } = [];

        internal HashSet<string> Antinodes { get; } = [];

        internal int Width { get; }

        internal int Height { get; }

        internal FrequencyGrid(string input)
        {
            var points = input.Split("\r\n").Select(x => x.ToCharArray()).ToArray();
            Height = points.Length;
            Width = points[0].Length;
            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width; i++)
                {
                    if (points[i][j] == '.') continue;

                    var antenna = new Antenna { Frequency = points[i][j], X = i, Y = j };
                    Antennae.Add(antenna.Key, antenna);
                }
            }
        }

        internal void FindMirroredAntinodes()
        {
            var groups = Antennae.GroupBy(g => g.Value.Frequency);
            foreach (var group in groups)
            {
                for(var i = 0; i < group.Count() - 1; i++)
                {
                    for(var j = i + 1; j < group.Count(); j++)
                    {
                        var antinodes = group.ElementAt(i).Value.GetMirroredAntinodes(group.ElementAt(j).Value);
                        antinodes = antinodes.Where(p => p[0] >= 0 && p[0] < Width).ToArray();
                        antinodes = antinodes.Where(p => p[1] >= 0 && p[1] < Height).ToArray();
                        foreach(var point in antinodes)
                        {
                            Antinodes.Add($"{point[0]},{point[1]}");
                        }
                    }
                }
            }
        }

        internal void FindAntinodes()
        {
            var groups = Antennae.GroupBy(g => g.Value.Frequency);
            foreach (var group in groups)
            {
                for(var i = 0; i < group.Count() - 1; i++)
                {
                    for(var j = i + 1; j < group.Count(); j++)
                    {
                        var antinodes = group.ElementAt(i).Value.GetAntinodes(group.ElementAt(j).Value, Width, Height);
                        foreach(var point in antinodes)
                        {
                            Antinodes.Add(point);
                        }
                    }
                }
            }
        }
    }
}
