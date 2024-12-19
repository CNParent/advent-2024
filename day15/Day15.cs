using System.Text.Json;

internal class Day15 : Day
{
    protected override string InputPath => "/day15/input.txt";

    internal override string A()
    {
        var warehouse = new Warehouse(Input);
        warehouse.Arrange();
        return $"The sum of all boxes' GPS coordinates is {warehouse.SumGps}";
    }

    internal override string B()
    {
        var warehouse = new Warehouse(Input, 2);
        warehouse.Arrange();
        return $"The sum of all boxes' GPS coordinates is {warehouse.SumGps}";
    }

    private class WarehouseFeature
    {
        public required int[][] Spaces { get; init; }

        public int[][] GetTargets(int[] direction) =>
            Spaces.Select<int[], int[]>(s => [s[0] + direction[0], s[1] + direction[1]])
                .Where(s => !Has(s))
                .ToArray();

        public void Move(int[] direction)
        {
            foreach(var space in Spaces)
            {
                space[0] += direction[0];
                space[1] += direction[1];
            }
        }

        public bool Has(int[] otherSpace)
        {
            foreach(var space in Spaces)
            {
                if (space[0] == otherSpace[0] && space[1] == otherSpace[1]) return true;
            }

            return false;
        }
    }

    private class Warehouse
    {
        private readonly Dictionary<char, int[]> _movements = new()
        {
            {'^', [0,-1]},
            {'>', [1,0]},
            {'v', [0,1]},
            {'<', [-1,0]}
        };

        public List<WarehouseFeature> Boxes { get; } = [];

        public List<WarehouseFeature> Walls { get; } = [];

        public int Width { get; }

        public int Height { get; }

        public int[][] Instructions { get; }

        public int[] Robot { get; }

        public long SumGps
        {
            get
            {
                var sum = 0;
                foreach(var box in Boxes)
                {
                    sum += box.Spaces.Min(x => x[1]) * 100 + box.Spaces.Min(x => x[0]);
                }

                return sum;
            }
        }

        public Warehouse(string input, int stretch = 1)
        {
            var pair = input.Split("\r\n\r\n");
            var grid = pair[0]
                .Split("\r\n")
                .Select(x => x.ToCharArray())
                .ToArray();
            
            Height = grid.Length;
            Width = grid[0].Length * stretch;
            
            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width / stretch; i++)
                {
                    var c = grid[j][i];
                    if (c == '.') continue;
                    else if (c == '#') Walls.Add(new(){ Spaces = (new int[stretch][]).Select<int[], int[]>((e, n) => [i * stretch + n, j]).ToArray()});
                    else if (c == 'O') Boxes.Add(new(){ Spaces = (new int[stretch][]).Select<int[], int[]>((e, n) => [i * stretch + n, j]).ToArray()});
                    else if (c == '@') Robot = [i * stretch,j];
                }
            }

            Instructions = pair[1]
                .Split("\r\n")
                .SelectMany(x => x.ToCharArray())
                .Select(x => _movements[x])
                .ToArray();

            if (Robot is null) throw new Exception("Robot not found on grid");
        }

        public void Arrange()
        {
            foreach(var instruction in Instructions)
            {
                if (TryMove(Robot, instruction))
                {
                    Robot[0] += instruction[0];
                    Robot[1] += instruction[1];
                    MoveBoxes([..Robot], instruction);
                }

                //Display();
            }

            //Display();
        }

        public bool TryMove(int[] from, int[] direction)
        {
            int[] target = [from[0] + direction[0], from[1] + direction[1]];
            var box = Boxes.FirstOrDefault(b => b.Has(target));
            if (box is not null) return TryMove(box, direction);
            else if (Walls.Any(w => w.Has(target))) return false;

            return true;
        }

        public bool TryMove(WarehouseFeature box, int[] direction) =>
            box.GetTargets(direction).All(target =>
            {
                var other = Boxes.FirstOrDefault(b => b.Has(target));
                if (other is not null) return TryMove(other, direction);
                else if (Walls.Any(w => w.Has(target))) return false;                
                return true;
            });

        public void MoveBoxes(int[] from, int[] direction)
        {
            var box = Boxes.FirstOrDefault(b => b.Has(from));
            if (box is null) return;
            var targets = box.GetTargets(direction);
            foreach(var target in targets)
            {                
                MoveBoxes(target, direction);
            }

            box.Move(direction);
        }

        private void Display()
        {
            var output = "";
            for(var j = 0; j < Height; j++)
            {
                for(var i = 0; i < Width; i++)
                {
                    if (i == Robot[0] && j == Robot[1]) output += '@';
                    else if (Walls.Any(w => w.Has([i,j]))) output += '#';
                    else if (Boxes.Any(b => b.Has([i,j]))) output += 'O';
                    else output += '.';
                }

                output += "\r\n";
            }
            Console.Clear();
            Console.Write(output);
            Console.ReadKey();
        }
    }
}
