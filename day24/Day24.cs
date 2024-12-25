internal class Day24 : Day
{
    protected override string InputPath => "/day24/input.txt";
    
    internal override string A()
    {
        var sections = Input.Split("\r\n\r\n");
        var states = sections[0].Split("\r\n").Select(x => x.Split(": ")).ToDictionary(x => x[0], x => x[1] == "1") ?? 
            throw new Exception("Could not create initial state map");

        Process(states, [.. sections[1].Split("\r\n").Select(x => new Instruction(x))]);
        return $"Value of z is {GetCircuitValue('z', states)}";
    }

    internal override string B()
    {        
        var sections = Input.Split("\r\n\r\n");
        var states = sections[0].Split("\r\n").Select(x => x.Split(": ")).ToDictionary(x => x[0], x => x[1] == "1") ?? 
            throw new Exception("Could not create initial state map");
        
        var instructions = sections[1].Split("\r\n").Select(x => new Instruction(x)).ToArray();

        Process(states, [.. instructions]);
        Console.WriteLine(string.Join("\r\n", states.Select(s => $"{s.Key}: {(s.Value ? 1 : 0)}")));
        var x = GetCircuitValue('x', states);
        var y = GetCircuitValue('y', states);
        var expected = x + y;
        var expectedb = Convert.ToString(expected, 2);
        var actual = GetCircuitValue('x', states);
        var error = expected ^ actual;        
        var errorb = Convert.ToString(error, 2);
        Console.WriteLine($"Error {errorb}b");
        var zerrors = errorb
            .ToCharArray()
            .Reverse()
            .Select((x,i) => new { Key = $"z{i:D2}", Value = x == '1' })
            .Where(x => x.Value && states.ContainsKey(x.Key))
            .Select(x => x.Key)
            .ToList();

        var zgoods = states.Where(x => x.Key.StartsWith('z') && !zerrors.Contains(x.Key)).Select(x => x.Key).ToList();
        var bads = GetAncestors(instructions, [..zerrors]);
        Console.WriteLine("\r\n>>>>>>>>>>>>>>>>>>>>>>>> BADS >>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        Console.WriteLine(string.Join("\r\n", bads.Select(c => c.Text).Order()));
        Console.WriteLine("\r\n>>>>>>>>>>>>>>>>>>>>>>>> GOODS >>>>>>>>>>>>>>>>>>>>>>>>>>>>");
        var goods = GetAncestors(instructions, [..zgoods]);
        Console.WriteLine(string.Join("\r\n", goods.Select(c => c.Text).Order()));
        Console.WriteLine("\r\n>>>>>>>>>>>>>>>>>>>>>>>> SWAPPABLE >>>>>>>>>>>>>>>>>>>>>>>>");
        var goodResults = goods.Select(g => g.Result);
        var goodArguments = goods.SelectMany(g => g.Arguments);
        var swappables = bads.Where(b => !goods.Contains(b) && !goodArguments.Contains(b.Result)).ToArray();
        Console.WriteLine(string.Join("\r\n", swappables.Select(c => c.Text).Order()));

        var swaptions = new Dictionary<Instruction, Instruction[]>();
        foreach(var swappable in swappables)
        {
            var descendants = swappables.Where(x => !GetDescendants(instructions, x).Contains(swappable));
            Console.WriteLine($"{swappable.Text} can be swapped with {descendants.Count()} options");
        }


        return "Not done yet";
    }

    private static long GetCircuitValue(char series, Dictionary<string, bool> states)
    {
        var wires = states.Where(x => x.Key.StartsWith(series)).OrderBy(x => x.Key);
        return wires.Select((x,i) => (long)Math.Pow(2, i) * (x.Value ? 1 : 0)).Sum();
    }

    private static void Process(Dictionary<string, bool> states, List<Instruction> instructions)
    {
        while(instructions.Count != 0)
        {
            foreach(var instruction in instructions.ToArray())
            {
                if (!instruction.Compute(states)) continue;
                
                instructions.Remove(instruction);
            }
        }
    }

    private static IEnumerable<Instruction> GetAncestors(Instruction[] instructions, string[] keys)
    {
        var results = new HashSet<Instruction>();
        List<Instruction> remaining = instructions.Where(i => keys.Contains(i.Result)).ToList();
        while(remaining.Count != 0)
        {
            var next = remaining.First();
            remaining.Remove(next);
            results.Add(next);
            remaining.AddRange(GetAncestors(instructions, next.Arguments[0]));
            remaining.AddRange(GetAncestors(instructions, next.Arguments[1]));
            remaining = remaining.Distinct().ToList();
        }

        return results;
    }

    private static IEnumerable<Instruction> GetAncestors(Instruction[] instructions, string key) => 
        instructions.Where(i => i.Result == key);

    private static IEnumerable<Instruction> GetDescendants(Instruction[] instructions, Instruction instruction)
    {
        var results = new HashSet<Instruction>();
        List<Instruction> remaining = [instruction];
        while(remaining.Count != 0)
        {
            var next = remaining.First();
            remaining.Remove(next);
            results.Add(next);
            remaining.AddRange(instructions.Where(i => i.Arguments.Contains(next.Result)));
            remaining = remaining.Distinct().ToList();
        }

        return results;
    }

    private class Instruction
    {
        public string Result { get; }

        public string Operation { get; }

        public string[] Arguments { get; }

        public bool? IsCorrect { get; set; }

        public string Text { get; }

        public Instruction(string input)
        {
            Text = input;
            var args = input.Split(' ');
            Result = args[4];
            Operation = args[1];
            Arguments = [args[0], args[2]];
        }

        public bool Compute(Dictionary<string, bool> states)
        {
            if (!states.ContainsKey(Arguments[0]) || !states.ContainsKey(Arguments[1])) return false;

            var a = states[Arguments[0]];
            var b = states[Arguments[1]];
            if (Operation == "AND") states.Add(Result, a & b);
            else if (Operation == "OR") states.Add(Result, a | b);
            else if (Operation == "XOR") states.Add(Result, a ^ b);

            return true;
        }

        public bool[][] GetInputsFor(bool expectedResult)
        {
            if (Operation == "AND" && expectedResult) return [[true, true]];
            else if (Operation == "AND" && !expectedResult) return [[false, true], [true, false], [false, false]];
            else if (Operation == "OR" && !expectedResult) return [[false, false]];
            else if (Operation == "OR" && expectedResult) return [[false, true], [true, false], [true, true]];
            else if (Operation == "XOR" && expectedResult) return [[true, false],[false, true]];
            else if (Operation == "XOR" && !expectedResult) return [[true, true],[false, false]];

            throw new Exception($"Unaccounted: {Operation} -> {expectedResult}");
        }
    }
}
