internal class Day12 : Day
{
    protected override string InputPath => "/day12/input.txt";

    internal override string A()
    {
        var garden = new Garden(Input);
        return $"{garden.Regions.Sum(r => r.Cost)} cost to fence all regions";
    }

    internal override string B()
    {
        var garden = new Garden(Input);
        return $"{garden.Regions.Sum(r => r.DiscountedCost)} discounted cost to fence all regions";
    }

    private class Garden
    {
        public Dictionary<string, Plot> Plots { get; }

        public HashSet<Region> Regions { get; }

        public int Width { get; }

        public int Height { get; }

        public Garden(string input)
        {
            Plots = [];
            Regions = [];
            var crops = input.Split("\r\n").Select(x => x.ToCharArray()).ToArray();

            Width = crops[0].Length;
            Height = crops.Length;

            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width; i++)
                {
                    var p = new Plot
                    {
                        X = i,
                        Y = j,
                        Crop = crops[j][i].ToString()
                    };

                    Plots.Add(p.Key, p);
                }
            }

            JoinPlots();
        }

        private void JoinPlots()
        {
            foreach (var plot in Plots.Values)
            {
                var north = $"{plot.X},{plot.Y - 1}";
                var east = $"{plot.X + 1},{plot.Y}";
                var south = $"{plot.X},{plot.Y + 1}";
                var west = $"{plot.X - 1},{plot.Y}";
                plot.North = Plots.ContainsKey(north) && Plots[north].Crop == plot.Crop ? Plots[north] : null;
                plot.East = Plots.ContainsKey(east) && Plots[east].Crop == plot.Crop ? Plots[east] : null;
                plot.South = Plots.ContainsKey(south) && Plots[south].Crop == plot.Crop ? Plots[south] : null;
                plot.West = Plots.ContainsKey(west) && Plots[west].Crop == plot.Crop ? Plots[west] : null;
            }
            
            foreach(var plot in Plots.Values)
            {
                if (plot.Region is null)
                {
                    plot.Region = new Region();
                    plot.Region.Plots.Add(plot);
                    Regions.Add(plot.Region);
                }

                plot.Explore();
            }
        }
    }

    private class Region
    {
        public HashSet<Plot> Plots { get; } = [];

        public int Area => Plots.Count;

        public int Cost => Area * Perimeter;

        public int DiscountedCost => Area * Edges;

        public int Perimeter
        {
            get
            {
                var sum = 0;
                foreach(var plot in Plots)
                {
                    sum += 4 - plot.Neighbours.Length;
                }

                return sum;
            }
        }

        public int Edges
        {
            get
            {
                var northCounted = new HashSet<string>();
                var eastCounted = new HashSet<string>();
                var southCounted = new HashSet<string>();
                var westCounted = new HashSet<string>();
                var sum = 0;
                foreach(var plot in Plots.OrderBy(x => x.X).ThenBy(x => x.Y))
                {
                    Console.WriteLine($"Counting edges at {plot.Key}");
                    if (plot.North is null) 
                    {
                        sum += !northCounted.Contains(plot.West?.Key ?? "") && !northCounted.Contains(plot.East?.Key ?? "") ? 1 : 0;
                        Console.WriteLine($"Found north edge, new sum is {sum}");
                        northCounted.Add(plot.Key);
                    }
                    if (plot.East is null) 
                    {
                        sum += !eastCounted.Contains(plot.North?.Key ?? "") && !eastCounted.Contains(plot.South?.Key ?? "") ? 1 : 0;
                        Console.WriteLine($"Found east edge, new sum is {sum}");
                        eastCounted.Add(plot.Key);
                    }
                    if (plot.South is null) 
                    {
                        sum += !southCounted.Contains(plot.West?.Key ?? "") && !southCounted.Contains(plot.East?.Key ?? "") ? 1 : 0;
                        Console.WriteLine($"Found south edge, new sum is {sum}");
                        southCounted.Add(plot.Key);
                    }
                    if (plot.West is null) 
                    {
                        sum += !westCounted.Contains(plot.North?.Key ?? "") && !westCounted.Contains(plot.South?.Key ?? "") ? 1 : 0;
                        Console.WriteLine($"Found west edge, new sum is {sum}");
                        westCounted.Add(plot.Key);
                    }
                }

                Console.WriteLine($"Region {Plots.First().Crop} has {sum} edges");
                return sum;
            }
        }
    }

    private class Plot
    {
        public required string Crop { get; set; }

        public required int X { get; init; }

        public required int Y { get; init; }

        public Plot? North { get; set; }

        public Plot? East { get; set; }

        public Plot? South { get; set; }

        public Plot? West { get; set; }

        public Region? Region { get; set; }

        public string Key => $"{X},{Y}";

        public Plot[] Neighbours => new [] {North, East, West, South}.Where(x => x is not null).Cast<Plot>().ToArray();

        public void Explore()
        {
            if (Region is null) throw new Exception("Cannot explore if Region is null");

            foreach(var n in Neighbours)
            {   
                if (n.Region is not null) continue;

                n.Region = Region;
                Region.Plots.Add(n);
                n.Explore();
            }
        }
    }
}
