using System.Text.Json;

internal class Day16 : Day
{
    protected override string InputPath => "/day16/input.txt";

    internal override string A()
    {
        var runner = new MazeRunner(Input);
        runner.Solve();
        return $"The minimum solution score is {runner.Score}";
    }

    internal override string B()
    {
        var runner = new MazeRunner(Input);
        runner.Solve();
        Console.WriteLine(JsonSerializer.Serialize(runner.Tiles));
        return $"The number of tiles on solution paths is {runner.Tiles.Count}";
    }

    private class MazePosition
    {
        public int[] Position { get; }

        public int Facing { get; }

        public long Score { get; }

        public HashSet<string> Tiles { get; }

        public string Key => $"{Position[0]},{Position[1]},{Facing}";

        public MazePosition(int[] position, int facing, long score, HashSet<string> tiles)
        {
            Position = position;
            Facing = facing;
            Score = score;
            Tiles = tiles;
            Tiles.Add($"{position[0]},{position[1]}");
        }
    }

    private class MazeRunner
    {
        private readonly int[][] _facings = [
            [1,0],  // E
            [0,1],  // S
            [-1,0], // W
            [0,-1]  // N
        ];

        private Dictionary<string, long> _scores = [];

        private readonly Maze _maze;

        private List<MazePosition> _solutions = [];

        public long Score => _solutions.Min(x => x.Score);

        public HashSet<string> Tiles => [.._solutions.Where(x => x.Score == Score).SelectMany(x => x.Tiles)];

        public MazeRunner(string input)
        {
            _maze = new Maze(input);
        }

        public void Solve()
        {
            Explore();
        }

        private void Explore()
        {
            var positions = new LinkedList<MazePosition>();
            positions.AddFirst(new LinkedListNode<MazePosition>(new(_maze.Start, 0, 0, [])));
            while(positions.Count != 0)
            {
                var node = positions.First!;
                var p = node.Value;
                positions.RemoveFirst();
                if (_scores.ContainsKey(p.Key) && _scores[p.Key] < p.Score) continue;
                if (p.Position[0] == _maze.End[0] && p.Position[1] == _maze.End[1]) 
                {
                    _solutions.Add(p);
                    continue; 
                }
                
                if (_scores.ContainsKey(p.Key)) _scores[p.Key] = p.Score;
                else _scores.Add(p.Key, p.Score);
                
                var forward = _facings[p.Facing];
                int[] ahead = [p.Position[0] + forward[0], p.Position[1] + forward[1]];
                if (!_maze.Walls.Contains($"{ahead[0]},{ahead[1]}")) 
                {
                    positions.AddFirst(new LinkedListNode<MazePosition>(new(ahead, p.Facing, p.Score + 1, [..p.Tiles])));
                }
                
                positions.AddLast(new LinkedListNode<MazePosition>(new (p.Position, (p.Facing + 3) % 4, p.Score + 1000, [..p.Tiles])));
                positions.AddLast(new LinkedListNode<MazePosition>(new (p.Position, (p.Facing + 1) % 4, p.Score + 1000, [..p.Tiles])));
            }
        }
    }

    private class Maze
    {
        public HashSet<string> Walls { get; } = [];

        public int[] End { get; } = [];

        public int[] Start { get; } = [];
        
        public Maze(string input)
        {
            var tiles = input.Split("\r\n").Select(r => r.ToCharArray()).ToArray();
            for(var j = 0; j < tiles.Length; j++)
            {
                for(var i = 0; i < tiles[0].Length; i++)
                {
                    var c = tiles[j][i];
                    if (c == '#') Walls.Add($"{i},{j}");
                    else if (c == 'S') Start = [i,j];
                    else if (c == 'E') End = [i,j];               
                }
            }
        }
    }
}
