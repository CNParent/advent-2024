internal class Day21 : Day
{

    private static Device _keypad = new(File.ReadAllText("C:\\git\\advent-2024\\day21\\keypad.txt"));

    private static Device _controller = new(File.ReadAllText("C:\\git\\advent-2024\\day21\\controller.txt"));

    protected override string InputPath => "/day21/small.txt";

    internal override string A()
    {
        Robot[] robots = [
            new (_keypad),
            new (_controller),
            new (_controller)
        ];

        List<IEnumerable<char>> instructions = [];
        var sequences = Input.Split("\r\n").Select(x => x.ToCharArray());
        foreach(var sequence in sequences)
        {
            IEnumerable<char> inputs = sequence;
            foreach (var robot in robots)
            {
                var result = robot.Device.GetInputSequence(inputs, robot.Position);
                robot.Position = inputs.Last();
                inputs = result.SelectMany(x => x);
            }

            instructions.Add(inputs);
            Console.WriteLine();
        }

        var complexity = 0;
        for(var i = 0; i < instructions.Count; i++)
        {
            var instruction = instructions[i];
            var sequence = sequences.ElementAt(i);
            var numeric = int.Parse(string.Join("", sequence).TrimEnd('A'));
            complexity += numeric * instruction.Count();

            Console.WriteLine($"{string.Join("", sequence)}: {string.Join("", instruction)}");
        }

        return $"Total complexity of all instructions is {complexity}";
    }

    internal override string B()
    {
        throw new NotImplementedException();
    }

    private class Robot
    {
        public Device Device { get; }

        public char Position { get; set; } = 'A';

        public Robot(Device device)
        {
            Device = device;
        }
    }

    private class Device
    {
        public Dictionary<char, int[]> Controls { get; } = [];

        public Dictionary<string, string> Instructions { get; } = [];

        public Device(string input)
        {
            var values = input
                .Split("\r\n")
                .Select(x => x.ToCharArray())
                .ToArray();

            for(var j = 0; j < values.Length; j++)
            {
                for(var i = 0; i < values[j].Length; i++)
                {
                    var c = values[j][i];
                    Controls.Add(c, [i,j]);
                }
            }

            foreach(var a in Controls)
            {
                foreach(var b in Controls)
                {
                    Instructions.Add($"{a.Key}{b.Key}", string.Join("", GetInputSequence(b.Key,a.Key)));
                }
            }
        }

        public IEnumerable<IEnumerable<char>> GetInputSequence(IEnumerable<char> input, char start)
        {
            ////Console.WriteLine($"Calculating input sequence for {string.Join("", input)} starting from {start}");
            input = [start, ..input];
            for (var i = 1; i < input.Count(); i++)
            {
                yield return GetInputSequence(input.ElementAt(i), input.ElementAt(i - 1));
            }
        }

        public IEnumerable<char> GetInputSequence(char target, char start)
        {
            ////Console.WriteLine($"Calculating input sequence {start} -> {target}");
            var pStart = Controls[start];
            var pTarget = Controls[target];
            var horizontal = pTarget[0] - pStart[0];
            var vertical = pTarget[1] - pStart[1];
            while (horizontal < 0)
            {
                yield return '<';
                horizontal++;
            }
            while (horizontal > 0)
            {
                yield return '>';
                horizontal--;
            }
            while (vertical < 0)
            {
                yield return '^';
                vertical++;
            }
            while (vertical > 0)
            {
                yield return 'v';
                vertical--;
            }

            yield return 'A';
        }
    }
}
