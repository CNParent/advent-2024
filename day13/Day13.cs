internal class Day13 : Day
{
    protected override string InputPath => "/day13/input.txt";

    private ClawMachine[] GetClawMachines(int? clamp = null, long? prizeAdjustment = null) => 
        Input.Split("\r\n\r\n")
            .Select(x => new ClawMachine(x, clamp, prizeAdjustment)).ToArray();

    internal override string A()
    {
        var machines = GetClawMachines(100);
        var winners = machines.Where(x => x.BestCost is not null);
        return $"Total tokens to win all winnable prizes is {winners.Sum(x => x.BestCost)}";
    }

    internal override string B()
    {
        var machines = GetClawMachines(prizeAdjustment: 10000000000000);
        var winners = machines.Where(x => x.BestCost is not null);
        return $"Total tokens to win all winnable prizes is {winners.Sum(x => x.BestCost)}";
    }

    private class Point
    {
        public long X { get; set; }

        public long Y { get; set; }

        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }
    }

    private class ClawMachine
    {
        private const long _costA = 3;

        private const long _costB = 1;

        public long? BestCost { get; private set; }

        public Point ButtonA { get; }

        public long MaxA { get; }

        public Point ButtonB { get; }

        public long MaxB { get; }

        public Point Prize { get; }

        public ClawMachine(string input, int? clamp = null, long? prizeAdjustment = null)
        {
            var lines = input.Split("\r\n");
            var a = lines[0].Split(": ")[1].Split(", ").Select(x => x[2..]).Select(int.Parse).ToArray();
            var b = lines[1].Split(": ")[1].Split(", ").Select(x => x[2..]).Select(int.Parse).ToArray();
            var p = lines[2].Split(": ")[1].Split(", ").Select(x => x[2..]).Select(int.Parse).ToArray();

            ButtonA = new(a[0], a[1]);
            ButtonB = new(b[0], b[1]);
            Prize = new(p[0], p[1]);

            if (prizeAdjustment is not null)
            {
                Prize.X += prizeAdjustment.Value;
                Prize.Y += prizeAdjustment.Value;
            }

            MaxA = Math.Min(Prize.X / ButtonA.X, Prize.Y / ButtonA.Y);
            MaxB = Math.Min(Prize.X / ButtonB.X, Prize.Y / ButtonB.Y);

            if (clamp is not null && MaxA > clamp) MaxA = clamp.Value;
            if (clamp is not null && MaxB > clamp) MaxB = clamp.Value;

            Console.WriteLine($"Evaluating routines for machine with prize at {Prize.X},{Prize.Y}");
            EvaluateAllRoutines();
        }

        private void EvaluateAllRoutines()
        {
            var b = (Prize.Y * ButtonA.X - Prize.X * ButtonA.Y) / (ButtonB.Y * ButtonA.X - ButtonA.Y * ButtonB.X);
            var position = new Point(b * ButtonB.X, b * ButtonB.Y);
            var a = (Prize.X - position.X) / ButtonA.X;
            position.X += a * ButtonA.X;
            position.Y += a * ButtonA.Y;
            if (a >= 0 && b >= 0 && a <= MaxA && b <= MaxB && position.X == Prize.X && position.Y == Prize.Y)
            {
                Console.WriteLine($"Found solution A: {a} B: {b}");
                BestCost = GetCost(a, b);
            }

            a = (Prize.Y * ButtonB.X - Prize.X * ButtonB.Y) / (ButtonA.Y * ButtonB.X - ButtonB.Y * ButtonA.X);
            position = new Point(a * ButtonA.X, a * ButtonA.Y);
            b = (Prize.X - position.X) / ButtonB.X;
            position.X += b * ButtonB.X;
            position.Y += b * ButtonB.Y;
            if (a >= 0 && b >= 0 && a <= MaxA && b <= MaxB && position.X == Prize.X && position.Y == Prize.Y)
            {
                Console.WriteLine($"Found solution A: {a} B: {b}");
                var cost = GetCost(a, b);
                if (BestCost is null || cost < BestCost) BestCost = cost;
            }
        }

        private long GetCost(long a, long b) => a * _costA + b * _costB;
    }
}
