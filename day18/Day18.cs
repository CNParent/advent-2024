using System.Text.Json;

internal class Day18 : Day
{
    protected override string InputPath => "/day18/input.txt";

    internal override string A()
    {
        var grid = new Grid(Input);
        grid.Explore();
        return $"Shortest path to end is {grid.MinimumSteps} steps";
    }

    internal override string B()
    {
        var grid = new Grid(Input);
        var b = grid.FindLastByte();
        return $"The last byte will fall at {b}";
    }

    private class Grid
    {
        private readonly int[][] _directions = [[0,-1],[1,0],[0,1],[-1,0]];

        private HashSet<string> _corruption = [];

        private readonly string[] _bytes;

        private readonly int _defaultTicks;

        private readonly Dictionary<string, GridPosition> _positions = [];

        private readonly List<GridPosition> _solutions = []; 

        private readonly int _size;

        private string End => $"{_size},{_size}";

        public int MinimumSteps => _solutions.Min(s => s.History.Count);

        public Grid(string input)
        {
            var pair = input.Split("\r\n\r\n");
            _defaultTicks = int.Parse(pair[1]);
            _size = int.Parse(pair[0]);
            _bytes = pair[2].Split("\r\n");
        }

        public string FindLastByte()
        {
            var ticks = _bytes.Length;
            Console.WriteLine();
            while(true)
            {
                Console.Write($"\rEvaluating {ticks}");
                Explore(ticks);
                if (_solutions.Any()) return _bytes[ticks];

                ticks--;
            }
        }

        public void Explore(int? ticks = null)
        {
            ticks ??= _defaultTicks;
            _corruption = [.._bytes.Take(ticks.Value)];
            _solutions.Clear();
            _positions.Clear();
            _positions.Add("0,0", new GridPosition { X = 0, Y = 0 });
            while (_positions.Count(x => !x.Value.Evaluated) > 0)
            {
                var p = _positions.OrderByDescending(p => _size * 2 - p.Value.X - p.Value.Y).First(x => !x.Value.Evaluated);
                p.Value.Evaluated = true;
                if (p.Key == End)
                {
                    _solutions.Add(p.Value);
                    continue;
                }

                foreach(var direction in _directions)
                {
                    TryMove(p.Value, direction);
                }
            }
        }

        private void TryMove(GridPosition p, int[] direction)
        {
            var next = new GridPosition { X = p.X + direction[0], Y = p.Y + direction[1], History = [p.Key, ..p.History] };
            if (next.X < 0 || next.X > _size || next.Y < 0 || next.Y > _size)
            {
                return;
            }
            else if (_corruption.Contains(next.Key))
            {
                return;
            }
            else if (p.History.Contains(next.Key))
            {
                return;
            }
            
            if (!_positions.ContainsKey(next.Key))
            {
                _positions.Add(next.Key, next);
                return;
            }

            var other = _positions[next.Key];
            if (other.History.Count <= next.History.Count) return;

            _positions[next.Key] = next;
        }
    }

    private class GridPosition
    {
        public int X { get; set; }

        public int Y { get; set; }

        public HashSet<string> History { get; init; } = [];

        public bool Evaluated { get; set; } = false;

        public string Key => $"{X},{Y}";
    }
}
