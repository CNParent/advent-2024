internal class Day23 : Day
{
    protected override string InputPath => "/day23/input.txt";

    private HashSet<string>  _history = [];

    private Dictionary<string, HashSet<string>>? _network;

    private Dictionary<string, HashSet<string>> Network
    {
        get
        {
            if (_network is not null) return _network;

            var pairs = Input.Split("\r\n").Select(x => x.Split("-"));
            _network = [];
            foreach (var pair in pairs)
            {
                if (!_network.ContainsKey(pair[0])) _network.Add(pair[0], [pair[1]]);
                else _network[pair[0]].Add(pair[1]);

                if (!_network.ContainsKey(pair[1])) _network.Add(pair[1], [pair[0]]);
                else _network[pair[1]].Add(pair[0]);
            }

            return _network;
        }
    }

    internal override string A()
    {
        _history.Clear();
        HashSet<string> threeKeys = [];
        var pairs = Input.Split("\r\n").Select(x => x.Split("-"));
        foreach(var pair in pairs)
        {
            var others = Network[pair[0]].Intersect(Network[pair[1]]);
            foreach(var other in others)
            {
                var sorted = new[] { pair[0], pair[1], other }.Order();
                threeKeys.Add(string.Join('-', sorted));
            }
        }

        var threes = threeKeys.Select(x => x.Split('-'));
        return $"{threes.Where(x => x.Any(c => c.StartsWith('t'))).Count()} computers in networks of size 3 begin with a t";
    }

    internal override string B()
    {
        _history.Clear();
        HashSet<string> groups = [];
        foreach(var node in Network)
        {
            foreach(var group in FindGroups([node.Key]))
            {
                groups.Add(group);
            }
        }

        return $"The password is {groups.OrderByDescending(g => g.Length).First()}";
    }

    private string[] FindGroups(string[] group)
    {
        var key = string.Join(',', group.Order());
        if (_history.Contains(key)) return [];

        _history.Add(key);
        var others = Network[group[0]].AsEnumerable();
        foreach(var node in group)
        {
            others = Network[node].Intersect(others);
        }

        if (!others.Any()) 
        {
            return [key];
        }

        var found = new List<string>();
        foreach (var other in others)
        {
            found.AddRange(FindGroups([other, ..group]));
        }

        return [.. found];
    }
}
