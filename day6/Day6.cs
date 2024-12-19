internal class Day6 : Day
{
    protected override string InputPath => "/day6/input.txt";

    internal override string A()
    {
        var grid = new Grid(Input);
        grid.Navigate();

        Console.Write("\r\n");
        return $"The guard visited {grid.Position.VisitedPoints.Count} points";
    }

    internal override string B()
    {
        var defaultGrid = new Grid(Input);
        defaultGrid.Navigate();
        for(var i = 0; i < defaultGrid.Position.VisitedPoints.Count; i++)
        {
            var point = defaultGrid.Position.VisitedPoints.ElementAt(i);
            var grid = new Grid(Input);
            grid.Points[point].Wall = true;
            defaultGrid.Points[point].LoopCandidate = grid.IsLoop;
            Console.Write($"\rChecking candidates: {i}/{defaultGrid.Position.VisitedPoints.Count}");
        }

        Console.WriteLine();
        var loopCount = defaultGrid.Points.Values.Count(p => p.LoopCandidate);
        return $"There are {loopCount} options for creating a patrol loop";
    }

    private class Grid
    {
        private readonly bool _draw = false;

        private static int[][] _facings = [[0,-1],[1,0],[0,1],[-1,0]];

        internal Dictionary<string, Point> Points { get; private init; }

        internal Position Start { get; set; }

        internal Position Position { get; set; }

        internal Grid(string input)
        {
            Points = input
                .Split("\r\n")
                .SelectMany((row,j) => row.ToCharArray().Select((p,i) => new Point
                {
                    X = i,
                    Y = j,
                    Wall = p == '#',
                    Start = p == '^'
                }))
                .ToDictionary(x => $"{x.X},{x.Y}");

            Start = new()
            {
                Point = Points.Values.Single(x => x.Start),
                Facing = 0
            };

            Position = Start;
        }

        internal void ContinuePatrol()
        {
            if (Position.Point is null) throw new Exception("position.Point cannot be null");

            var ahead = Look();

            if (ahead is null) Position.Point = null;
            else if (ahead.Wall || ahead == Position.Candidate) Position.Turn();
            else Position.Point = ahead;
        }

        internal Point? Look()
        {
            if (Position.Point is null) throw new Exception("position.Point cannot be null");

            var facing = _facings[Position.Facing];
            var key = $"{Position.Point.X + facing[0]},{Position.Point.Y + facing[1]}";
            return Points.ContainsKey(key) ? Points[key] : null;
        }

        internal bool IsLoop
        {
            get
            {
                while(Position.Point is not null)
                {
                    if (Position.VisitedPositions.Contains(Position.Key)) return true;

                    Position.Visit();
                    ContinuePatrol();
                    Draw();
                }

                return false;
            }
        }
        
        internal void Navigate()
        {
            Position = Start;
            var steps = 0;
            while(Position.Point is not null)
            {
                Draw();
                Position.Visit();
                ContinuePatrol();
                Console.Write($"\rSteps: {steps++}");
            }

            Console.WriteLine();
        }

        internal void Draw()
        {
            if (!_draw) return;
            if (Position.Point is null) return;

            var display = "";
            var width = Points.Values.Max(x => x.X);
            var height = Points.Values.Max(x => x.Y);
            var xMin = Position.Point.X - 10;
            var yMin = Position.Point.Y - 10;
            var xMax = Position.Point.X + 10;
            var yMax = Position.Point.Y + 10;

            if (xMin < 0) xMin = 0;
            if (yMin < 0) yMin = 0;
            if (xMax > width) xMax = width;
            if (yMax > height) yMax = height;

            for (var y = yMin; y <= yMax; y++)
            {
                for (var x = xMin; x <= xMax; x++)
                {
                    var key = $"{x},{y}";
                    var p = Points[key];
                    if (p.Wall) display += "#";
                    else if (p == Position.Candidate) display += "#";
                    else if (p.LoopCandidate) display += "O";
                    else if (p == Position.Point && Position.Facing == 0) display += "^";
                    else if (p == Position.Point && Position.Facing == 1) display += ">";
                    else if (p == Position.Point && Position.Facing == 2) display += "v";
                    else if (p == Position.Point && Position.Facing == 3) display += "<";
                    else if (Position.VisitedPoints.Contains(key)) display += ".";
                    else if (!Position.VisitedPoints.Contains(key)) display += " ";
                }

                display += "\r\n";
            }

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Write(display);
            Console.ReadKey();
            Thread.Sleep(10);
        }
    }

    private class Position
    {
        public Point? Point { get; set; }

        public Point? Candidate { get; set; }

        public int Facing { get; set; } = 0;

        public void Turn() => Facing = (Facing + 1) % 4;

        public HashSet<string> VisitedPoints { get; init; } = [];

        public HashSet<string> VisitedPositions { get; init; } = [];

        public string Key => $"{Point?.X},{Point?.Y},{Facing}";

        public void Visit()
        {
            VisitedPositions.Add(Key);
            if (Point is not null) VisitedPoints.Add(Point.Key);
        }
    }

    private class Point
    {
        public required int X { get; init; }

        public required int Y { get; init; }

        public required bool Start { get; init; }

        public required bool Wall { get; set; }

        public bool LoopCandidate { get; set; }

        public string Key => $"{X},{Y}";
    }
}
