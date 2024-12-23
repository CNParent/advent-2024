internal class Day21 : Day
{

    private static Device _keypad = new(File.ReadAllText("C:\\git\\advent-2024\\day21\\keypad.txt"));

    private static Device _controller = new(File.ReadAllText("C:\\git\\advent-2024\\day21\\controller.txt"));

    private static Dictionary<string, string[]>? _instructions;

    private static Dictionary<string, string[]> Instructions => _instructions ??= [];

    private static Dictionary<string, long> Evaluated { get; } = [];

    protected override string InputPath => "/day21/input.txt";

    internal override string A()
    {
        _controller.EnsureControlsAreMapped();
        _keypad.EnsureControlsAreMapped();
        return $"Total complexity of all instructions is {CalculateComplexity(3)}";
    }

    internal override string B()
    {
        _controller.EnsureControlsAreMapped();
        _keypad.EnsureControlsAreMapped();
        return $"Total complexity of all instructions is {CalculateComplexity(26)}";
    }

    internal long CalculateComplexity(int nRobots)
    {
        long complexity = 0;
        var sequences = Input.Split("\r\n");
        foreach(var s in sequences)
        {
            var sequence = $"A{s}";
            var totalLength = GetSequenceLength(sequence, nRobots);
            var keypadValue = int.Parse(s.TrimEnd('A'));
            complexity += totalLength * keypadValue;
        }

        return complexity;
    }
    
    private long GetSequenceLength(IEnumerable<char> sequence, int iterationsRemaining)
    {
        if (iterationsRemaining == 0) return sequence.Count() - 1;

        var evalKey = $"{sequence}|{iterationsRemaining}";
        if (Evaluated.ContainsKey(evalKey)) return Evaluated[evalKey];
        else Evaluated.Add(evalKey, 0);

        long sum = 0;
        var prev = sequence.First();
        foreach(var next in sequence.Skip(1))
        {
            var instruction = Instructions[$"{prev}{next}"];
            var lengths = instruction.Select(o => GetSequenceLength($"A{o}", iterationsRemaining - 1));
            sum += lengths.Min();
            prev = next;
        }

        Evaluated[evalKey] = sum;
        return sum;
    }

    private static long GetEntropy(string value)
    {
        var entropy = 0;
        for (var i = 1; i < value.Length; i++)
        {
            entropy += _controller.GetDistance(value[i], value[i-1]);
        }

        return entropy;
    }

    private class Device
    {
        public Dictionary<char, int[]> Controls { get; } = [];

        public char[][] Grid { get; }

        public Device(string input)
        {
            Grid = input
                .Split("\r\n")
                .Select(x => x.ToCharArray())
                .ToArray();

            for(var j = 0; j < Grid.Length; j++)
            {
                for(var i = 0; i < Grid[j].Length; i++)
                {
                    var c = Grid[j][i];
                    Controls.Add(c, [i,j]);
                }
            }
        }

        public void EnsureControlsAreMapped()
        {
            if (Controls.Count == 0) return;

            foreach(var a in Controls)
            {
                foreach(var b in Controls)
                {
                    var key = $"{a.Key}{b.Key}";
                    if (Instructions.ContainsKey(key) || a.Key == ' ' || b.Key == ' ') continue;

                    var results = MoveTowards(a.Value, b.Value, "").Select(r => new { Entropy = GetEntropy(r), Value = r }).ToArray();
                    var minEntropy = results.Min(r => r.Entropy);
                    var filtered = results.Where(r => r.Entropy == minEntropy).Select(r => r.Value).ToArray();
                    Instructions.Add(key, filtered);
                }
            }
        }

        public int GetDistance(char from, char to)
        {
            var fromP = Controls[from];
            var toP = Controls[to];
            return Math.Abs(fromP[0] - toP[0]) + Math.Abs(fromP[1] - toP[1]);
        }

        private IEnumerable<string> MoveTowards(int[] from, int[] to, string path)
        {
            if (from[0] == to[0] && from[1] == to[1])
            {
                yield return $"{path}A";
                yield break;
            }

            var current = Grid[from[1]][from[0]];
            if (current == ' ') yield break;

            if (from[0] < to[0])
            {
                var options = MoveTowards([from[0] + 1, from[1]], to, $"{path}>");
                foreach(var option in options)
                {
                    yield return option;
                }
            }

            if (from[0] > to[0])
            {
                var options = MoveTowards([from[0] - 1, from[1]], to, $"{path}<");
                foreach(var option in options)
                {
                    yield return option;
                }
            }

            if (from[1] < to[1])
            {
                var options = MoveTowards([from[0], from[1] + 1], to, $"{path}v");
                foreach(var option in options)
                {
                    yield return option;
                }
            }

            if (from[1] > to[1])
            {
                var options = MoveTowards([from[0], from[1] - 1], to, $"{path}^");
                foreach(var option in options)
                {
                    yield return option;
                }
            }
        }
    }
}
