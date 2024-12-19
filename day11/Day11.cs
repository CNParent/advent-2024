using System.Text.Json;

internal class Day11 : Day
{
    protected override string InputPath => "/day11/input.txt";

    internal override string A()
    {
        var line = new StoneLine(Input);
        var blinks = 25;
        for(var i = 0; i < blinks; i++)
        {
            line.Blink();
            line.Draw();
        }

        return $"Number of stones after {blinks} blinks is {line.StoneCount}";
    }

    internal override string B()
    {
        var line = new StoneLine(Input);
        var blinks = 75;
        for(var i = 0; i < blinks; i++)
        {
            line.Blink();
            line.Draw();
        }

        return $"Number of stones after {blinks} blinks is {line.StoneCount}";
    }

    private class StoneLine
    {
        private readonly bool _draw = false;

        public Dictionary<long, long> Stones { get; set; } = [];

        public long StoneCount => Stones.Sum(x => x.Value);

        public StoneLine(string input)
        {
            Stones = new (input.Split(' ').Select(long.Parse).Select(x => new KeyValuePair<long, long>(x, 1)));
        }

        private void Add(long value, long n)
        {
            if (Stones.ContainsKey(value)) Stones[value] += n;
            else Stones.Add(value, n);
        }

        private void Remove(long value, long n)
        {
            Stones[value] -= n;
            if (Stones[value] == 0) Stones.Remove(value);
        }

        public void Blink()
        {
            foreach(var stone in Stones.ToArray())
            {
                var value = stone.Key;
                var instances = stone.Value;
                Remove(value, instances);
                if (value == 0)
                {
                    Add(1, instances);
                }
                else if (value.ToString().Length % 2 == 0)
                {
                    var text = value.ToString();
                    Add(long.Parse(text[..(text.Length / 2)]), instances);
                    Add(long.Parse(text.Substring(text.Length / 2, text.Length / 2)), instances);
                }
                else
                {
                    Add(value * 2024, instances);
                }
            }
        }

        public void Draw()
        {
            if (!_draw) return;

            Console.WriteLine(JsonSerializer.Serialize(Stones.ToArray()));
            Console.ReadKey();
        }
    }
}
