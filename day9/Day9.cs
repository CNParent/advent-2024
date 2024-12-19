internal class Day9 : Day
{
    protected override string InputPath => "/day9/input.txt";

    internal override string A()
    {
        var s = new Storage(Input);
        s.Reallocate();
        return $"Storage checksum is {s.Checksum}";
    }

    internal override string B()
    {
        var s = new Storage(Input);
        s.Defrag();
        return $"Storage checksum is {s.Checksum}";
    }

    internal class Storage
    {
        internal int[] Blocks { get; }

        internal long Checksum
        {
            get
            {
                long sum = 0;
                for(var i = 0; i < Blocks.Length; i++)
                {
                    if (Blocks[i] < 0) continue;

                    sum += i * Blocks[i];
                }

                return sum;
            }
        }

        internal Storage(string input)
        {
            var id = 0;
            var blocks = new LinkedList<int>();
            for(var i = 0; i < input.Length; i++)
            {
                var size = int.Parse($"{input[i]}");
                var value = i % 2 == 1 ? -1 : id++;
                
                for(var j = 0; j < size; j++)
                    blocks.AddLast(value);
            }

            Blocks = [..blocks];
        }

        internal void Reallocate()
        {
            var j = Blocks.Length - 1;
            for (var i = 0; i < j; i++)
            {
                if (Blocks[i] != -1) continue;

                (Blocks[j], Blocks[i]) = (Blocks[i], Blocks[j]);
                j--;
                while(Blocks[j] < 0) j--;
            }
        }

        internal int FindGap(int size, int before)
        {
            var i = 0;
            while(i < before)
            {
                if (Blocks[i] != -1) 
                {
                    i++;
                    continue;
                }

                var j = i;
                var s = 0;
                while(Blocks[j] == -1)
                {
                    s++;
                    j++;
                }

                if (s >= size) return i;

                i = j;
            }

            return -1;
        }

        internal void Defrag()
        {
            var id = Blocks.Max();
            while(id >= 0)
            {
                var index = Array.IndexOf(Blocks, id);
                var size = Blocks.Count(x => x == id);
                var gapStart = FindGap(size, index);
                if (gapStart != -1)
                {
                    for(var i = 0; i < size; i++)
                    {
                        Blocks[gapStart + i] = id;
                        Blocks[index + i] = -1;
                    }
                }

                id--;
                Console.Write($"\rBlocks remaining: {id + 1}");
            }
        }
    }
}
