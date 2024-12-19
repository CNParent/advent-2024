internal class Day19 : Day
{
    protected override string InputPath => "/day19/input.txt";

    internal override string A()
    {
        var tr = new TowelRack(Input);
        tr.StitchTowels();
        return $"{tr.Possible} patterns are possible";
    }

    internal override string B()
    {
        var tr = new TowelRack(Input);
        tr.StitchTowels();
        return $"{tr.Permutations} cumulative stitch patterns are possible";
    }

    private class StitchPattern
    {
        public required string DesignRemaining { get; init; }

        public long Instances { get; set; } = 1;
    }

    private class Arrangement
    {

    }

    private class TowelRack
    {
        private string[] _fragments;

        private string[] _designs;

        private HashSet<string> _analyzed = [];

        public int Possible { get; private set; }

        public long Permutations { get; private set; }

        public TowelRack(string input)
        {
            var pair = input.Split("\r\n\r\n");
            _fragments = pair[0].Split(", ");
            _designs = pair[1].Split("\r\n");
        }

        public void StitchTowels()
        {
            Possible = 0;
            Permutations = 0;
            foreach(var design in _designs)
            {
                var recipes = CountRecipes(design);
                Permutations += recipes;
                if (recipes > 0) Possible ++;
            }
        }

        private long CountRecipes(string design)
        {
            long recipes = 0;
            List<StitchPattern> options = [new(){ DesignRemaining = design }];
            while(options.Count > 0)
            {
                var o = options.OrderByDescending(x => x.DesignRemaining.Length).First();
                var group = options.Where(x => x.DesignRemaining == o.DesignRemaining).ToArray();
                options.RemoveAll(x => x.DesignRemaining == o.DesignRemaining);
                o.Instances = group.Sum(o => o.Instances);
                if (o.DesignRemaining == "")
                {
                    recipes += o.Instances;
                }
                else
                {
                    options.AddRange(BranchPattern(o));
                }
            }

            return recipes;
        }

        private StitchPattern[] BranchPattern(StitchPattern start)
        {
            var design = start.DesignRemaining;
            var options = _fragments.Where(design.StartsWith);
            return options
                .Select(o => new StitchPattern { DesignRemaining = design[o.Length..], Instances = start.Instances })
                .ToArray();
        }
    }
}
