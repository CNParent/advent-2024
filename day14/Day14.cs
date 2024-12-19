internal class Day14 : Day
{
    protected override string InputPath => "/day14/input.txt";

    internal override string A()
    {
        const int seconds = 100;
        var bathroom = new Bathroom(Input);
        bathroom.Move(seconds);
        bathroom.Display();
        return $"After {seconds} seconds, safety factor is {bathroom.Safety}";
    }

    internal override string B()
    {
        var bathroom = new Bathroom(Input);
        var seconds = 0;
        while(true)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            bathroom.Move(1);
            if (bathroom.TreeSign)
            {
                bathroom.Display();
                Console.WriteLine((seconds + 1).ToString());
                return $"Suspected tree found after {seconds} seconds (verify output!)";
            }

            Console.WriteLine(seconds++.ToString());
        }
    }

    internal class Robot
    {
        public long X { get; set; }

        public long Y { get; set; }

        public int VX { get; }

        public int VY { get; }

        public Robot(string input)
        {
            var values = input.Split(" ").Select(x => x[2..].Split(",").Select(int.Parse).ToArray()).ToArray();
            X = values[0][0];
            Y = values[0][1];
            VX = values[1][0];
            VY = values[1][1];
        }

        public void Move(int times, int height, int width)
        {
            X = (X + VX * times) % width;
            Y = (Y + VY * times) % height;

            Normalize(height, width);
        }

        public void Normalize(int height, int width)
        {
            if (X < 0) X += width;
            if (Y < 0) Y += height;
        }
    }

    internal class Bathroom
    {
        public int Width { get; }

        public int Height { get; }

        public Robot[] Robots { get; }

        public Bathroom(string input)
        {
            var pair = input.Split("\r\n\r\n");
            var size = pair[0].Split(",").Select(int.Parse).ToArray();
            Width = size[0];
            Height = size[1];
            Robots = pair[1].Split("\r\n").Select(x => new Robot(x)).ToArray();
            foreach(var robot in Robots)
            {
                robot.Normalize(Height, Width);
            }
        }

        public void Move(int times)
        {
            foreach(var robot in Robots)
            {
                robot.Move(times, Height, Width);
            }
        }

        public void Display()
        {
            var output = "";
            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width; i++)
                {
                    var c = Robots.Any(r => r.X == i && r.Y == j);
                    if (!c) output += ".";
                    else output += "*";
                }

                output += "\r\n";
            }

            Console.Write(output);
        }

        public bool TreeSign =>
            Robots.GroupBy(r => r.X).Any(g => g.Count() > 20) &&
            Robots.GroupBy(r => r.Y).Any(g => g.Count() > 20);

        public int Noise
        {
            get
            {                
                var quadrantHeight = Height / 2;
                var quadrantWidth = Width / 2;
                var topleft = GetRobotCount(0, quadrantWidth - 1, 0, quadrantHeight - 1);
                var topright = GetRobotCount(Width - quadrantWidth, Width - 1, 0, quadrantHeight - 1);
                var bottomleft = GetRobotCount(0, quadrantWidth - 1, Height - quadrantHeight, Height - 1);
                var bottomright = GetRobotCount(Width - quadrantWidth, Width - 1, Height - quadrantHeight, Height - 1);
                var average = (topleft + topright + bottomleft + bottomright) / 4;
                return Math.Abs(topleft - average) + 
                    Math.Abs(topright - average) +
                    Math.Abs(bottomleft - average) +
                    Math.Abs(bottomright - average);
            }
        }

        public int Overlap => Robots.Count(a => Robots.Any(b => b != a && b.X == a.X && b.Y == a.Y));

        public int Safety
        {
            get
            {
                var quadrantHeight = Height / 2;
                var quadrantWidth = Width / 2;
                var topleft = GetRobotCount(0, quadrantWidth - 1, 0, quadrantHeight - 1);
                var topright = GetRobotCount(Width - quadrantWidth, Width - 1, 0, quadrantHeight - 1);
                var bottomleft = GetRobotCount(0, quadrantWidth - 1, Height - quadrantHeight, Height - 1);
                var bottomright = GetRobotCount(Width - quadrantWidth, Width - 1, Height - quadrantHeight, Height - 1);

                Console.WriteLine($"quadrantHeight: {quadrantHeight}");
                Console.WriteLine($"quadrantWidth: {quadrantWidth}");
                Console.WriteLine($"topleft: {topleft}");
                Console.WriteLine($"topright: {topright}");
                Console.WriteLine($"bottomleft: {bottomleft}");
                Console.WriteLine($"bottomright: {bottomright}");

                return topleft * topright * bottomleft * bottomright;
            }
        }

        public int GetRobotCount(int fromX, int toX, int fromY, int toY) => 
            Robots.Count(r => r.X >= fromX && r.X <= toX && r.Y >= fromY && r.Y <= toY);
    }
}
