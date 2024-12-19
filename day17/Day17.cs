using System.Text.Json;

internal class Day17 : Day
{
    protected override string InputPath => "/day17/input.txt";

    internal override string A()
    {
        var hhd = new HandheldDebugger();
        hhd.Run(Input);
        return $"Debugger output: {hhd.Output}";
    }

    internal override string B()
    {
        var hhd = new HandheldDebugger();
        var n = hhd.Solve(Input);
        return $"Debugger replicates program at a={n}";
    }

    private class HandheldDebugger
    {
        private const uint _clamp = 40;

        private ulong _registerA;

        private ulong _registerB;

        private ulong _registerC;

        private int _instruction = 0;

        private readonly List<ulong> _output = [];

        public string Output => string.Join(',', _output);

        public void Run(string input)
        {
            _instruction = 0;
            _output.Clear();

            var args = input.Split("\r\n\r\n");
            var registers = args[0].Split("\r\n").Select(x => ulong.Parse(x.Split(": ")[1])).ToArray();
            _registerA = registers[0];
            _registerB = registers[1];
            _registerC = registers[2];

            var values = args[1].Split(": ")[1].Split(",").Select(ulong.Parse).ToArray();
            while(_instruction < values.Length)
            {
                var opcode = values[_instruction];
                var operand = values[_instruction + 1];
                var result = Operation(opcode)(operand);
                if (result is null) continue;

                _output.Add((ulong)result);
            }
        }

        public ulong Solve(string input)
        {
            var args = input.Split("\r\n\r\n");
            var registers = args[0].Split("\r\n").Select(x => ulong.Parse(x.Split(": ")[1])).ToArray();
            void reset(ulong a)
            {
                _instruction = 0;
                _output.Clear();
                _registerA = a;
                _registerB = registers[1];
                _registerC = registers[2];
            }

            var programText = args[1].Split(": ")[1];
            var program = programText.Split(",").Select(ulong.Parse).ToArray();
            List<Candidate> candidates = [];
            List<Candidate> solutions = [];
            var digits = program.Length;
            var step = (ulong)Math.Pow(8, digits - 1);
            for(ulong i = 0; i < 8; i++)
            {
                candidates.Add(new Candidate { Digit = digits, Value = step * i });
            }

            while(candidates.Count > 0)
            {
                var a = candidates.First();
                candidates.Remove(a);
                reset(a.Value);
                if (!Try(program, a.Digit)) continue;
                
                if (a.Digit == 1)
                {
                    solutions.Add(a);
                }
                else
                {
                    step = (ulong)Math.Pow(8, a.Digit - 2);
                    for(ulong i = 0; i < 8; i++)
                    {
                        candidates.Add(new Candidate { Digit = a.Digit - 1, Value = a.Value + step * i });
                    }
                }
            }

            return solutions.OrderBy(x => x.Value).First().Value;
        }

        private bool Try(ulong[] program, int digit)
        {
            var d = 0;
            while(_instruction < program.Length)
            {
                var opcode = program[_instruction];
                var operand = program[_instruction + 1];
                var result = Operation(opcode)(operand);
                if (result is not null && d + 1 == digit) return program[d] == result;
                
                d += result is not null ? 1 : 0;
            }

            return false;
        }

        private Func<ulong, ulong?> Operation(ulong opcode) =>
            opcode switch
            {
                0 => Adv,
                1 => Bxl,
                2 => Bst,
                3 => Jnz,
                4 => Bxc,
                5 => Out,
                6 => Bdv,
                7 => Cdv,
                _ => throw new Exception($"Invlalid opcode {opcode} at {_instruction}")
            };

        private ulong Combo(ulong operand) => 
            operand switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => _registerA,
                5 => _registerB,
                6 => _registerC,
                _ => throw new Exception($"Invalid operand {operand} at {_instruction}")
            };

        private ulong? Adv(ulong operand)
        {
            var c = Combo(operand);
            if (c > _registerA || c > _clamp) _registerA = 0;
            else _registerA /= Power2(c, _registerA);
            _instruction += 2;
            return null;
        }

        private ulong? Bxl(ulong operand)
        {
            _registerB ^= operand;
            _instruction += 2;
            return null;
        }

        private ulong? Bst(ulong operand)
        {
            _registerB = Combo(operand) % 8;
            _instruction += 2;
            return null;
        }

        private ulong? Jnz(ulong operand)
        {
            if (_registerA == 0) _instruction += 2;
            else _instruction = (int)operand;

            return null;
        }

        private ulong? Bxc(ulong operand)
        {
            _registerB ^= _registerC;
            _instruction += 2;
            return null;
        }

        private ulong? Out(ulong operand)
        {
            _instruction += 2;
            return Combo(operand) % 8;
        }

        private ulong? Bdv(ulong operand)
        {
            var c = Combo(operand);
            if (c > _registerA || c > _clamp) _registerB = 0;
            else _registerB = _registerA / Power2(c, _registerA);
            _instruction += 2;
            return null;
        }

        private ulong? Cdv(ulong operand)
        {
            var c = Combo(operand);
            if (c > _registerA || c > _clamp) _registerC = 0;
            else _registerC = _registerA / Power2(c, _registerA);
            _instruction += 2;
            return null;
        }

        private static ulong Power2(ulong pow, ulong limit)
        {
            ulong result = 1;
            for(ulong i = 0; i < pow; i++)
            {
                result *= 2;
                if (result > limit) return result;
            }

            return result;
        }
    }

    private class Candidate
    {
        public required int Digit { get; init; }

        public required ulong Value { get; init; } 
    }
}
