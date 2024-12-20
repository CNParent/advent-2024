internal class Day20 : Day
{
    protected override string InputPath => "/day20/input.txt";

    internal override string A()
    {
        var track = new RaceTrack(Input);
        return $"There are {track.TimeSavesLegacy.Count(x => x >= 100)} cheats that save 100 picoseconds or more";
    }

    internal override string B()
    {
        var track = new RaceTrack(Input);
        var groups = track.TimeSaves.Where(x => x >= 50).GroupBy(x => x).OrderBy(g => g.Key);

        return $"There are {track.TimeSaves.Count(x => x >= 100)} cheats that save 100 picoseconds or more";
    }

    private class RaceTrack
    {
        private const int _maximumCheatDuration = 20;

        private readonly int[][] _directions = [[0,-1],[1,0],[0,1],[-1,0]];

        private readonly int[][] _cheatDirectionsLegacy = [[0,-2],[1,-1],[2,0],[1,1],[0,2],[-1,1],[-2,0],[-1,-1]];

        private readonly List<int[]> _cheatDirections = [];

        private readonly HashSet<string> _walls = [];

        public List<int> TimeSavesLegacy { get; } = [];

        public List<int> TimeSaves { get; } = [];

        private readonly Dictionary<string, RaceTrackPosition> _track = [];

        private readonly RaceTrackPosition _start;

        private readonly RaceTrackPosition _end;

        public RaceTrack(string input)
        {
            for(var i = -_maximumCheatDuration; i <= _maximumCheatDuration; i++)
            {
                for(var j = -_maximumCheatDuration; j <= _maximumCheatDuration; j++)
                {
                    if (Math.Abs(i) + Math.Abs(j) > _maximumCheatDuration) continue;

                    _cheatDirections.Add([i, j]);
                }
            }


            var rows = input.Split("\r\n");
            for(var j = 0; j < rows.Length; j++)
            {
                var row = rows[j];
                for(var i = 0; i < rows[j].Length; i++)
                {
                    if (row[i] == '#') _walls.Add($"{i},{j}");
                    else if (row[i] == 'S') _start = new(){ X = i, Y = j, ElapsedPicoseconds = 0 };
                    else if (row[i] == 'E') _end = new(){ X = i, Y = j, ElapsedPicoseconds = 0 };
                }
            }

            if (_start is null) throw new Exception("Track start could not be found");
            if (_end is null) throw new Exception("Track end could not be found");

            BuildTrack();
            EvaluateLegacyCheatOptions();
            EvaluateCheatOptions();
        }

        private void BuildTrack()
        {
            var p = _start;
            Console.WriteLine();
            while (p.Key != _end.Key)
            {
                _track.Add(p.Key, p);
                foreach(var d in _directions)
                {
                    int[] next = [d[0] + p.X, d[1] + p.Y];
                    var nextKey = $"{next[0]},{next[1]}";
                    if (_walls.Contains(nextKey) || _track.ContainsKey(nextKey)) continue;
                    if (_end.Key == nextKey)
                    {
                        _end.ElapsedPicoseconds = p.ElapsedPicoseconds + 1;
                        p = _end;
                        break;
                    }

                    p = new(){ X = next[0], Y = next[1], ElapsedPicoseconds = p.ElapsedPicoseconds + 1 };
                    break;
                }
            }

            _track.Add(_end.Key, _end);
        }

        private void EvaluateLegacyCheatOptions()
        {
            foreach(var p in _track.Values)
            {
                if (p == _end) continue;

                foreach(var d in _cheatDirectionsLegacy)
                {
                    int[] next = [d[0] + p.X, d[1] + p.Y];
                    var nextKey = $"{next[0]},{next[1]}";
                    if (!_track.ContainsKey(nextKey)) continue;

                    var target = _track[nextKey];
                    var timeSave = target.ElapsedPicoseconds - p.ElapsedPicoseconds - d.Sum(Math.Abs);
                    if (timeSave <= 0) continue;

                    TimeSavesLegacy.Add(timeSave);
                }
            }
        }

        private void EvaluateCheatOptions()
        {
            foreach(var p in _track.Values)
            {
                if (p == _end) continue;

                foreach(var d in _cheatDirections)
                {
                    int[] next = [d[0] + p.X, d[1] + p.Y];
                    var nextKey = $"{next[0]},{next[1]}";
                    if (!_track.ContainsKey(nextKey)) continue;

                    var target = _track[nextKey];
                    var timeSave = target.ElapsedPicoseconds - p.ElapsedPicoseconds - d.Sum(Math.Abs);
                    if (timeSave <= 0) continue;

                    TimeSaves.Add(timeSave);
                }
            }
        }
    }

    private class RaceTrackPosition
    {
        public required int X { get; init; }

        public required int Y { get; init; }

        public required int ElapsedPicoseconds { get; set; }

        public string Key => $"{X},{Y}";
    }
}
