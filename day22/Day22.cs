internal class Day22 : Day
{
    private const long _pruneValue = 16777216;

    protected override string InputPath => "/day22/input.txt";

    private long[]? _buyers;

    private long[] Buyers => _buyers ??= Input.Split("\r\n").Select(long.Parse).ToArray();

    internal override string A()
    {
        var results = Buyers.Select(b => {
            var secret = b;
            for(var i = 0; i < 2000; i++)
            {
                secret = EvolveSecret(secret);
            }

            return secret;
        });

        return $"The sum of all buyer secrets is {results.Sum()}";
    }

    internal override string B()
    {
        Dictionary<string, int> purchaseOptions = [];
        var buyers = Buyers.Select(b => {
            var secret = b;
            var prices = new Dictionary<string, int>();
            var sequence = new LinkedList<int>();
            sequence.AddFirst((int)secret % 10);
            for(var i = 0; i < 2000; i++)
            {
                if (sequence.Count == 4) sequence.RemoveFirst();

                var previousPrice = (int)secret % 10;
                secret = EvolveSecret(secret);

                var price = (int)secret % 10;
                sequence.AddLast(previousPrice - price);
                if (sequence.Count != 4) continue;

                var key = string.Join("", sequence.Select(s => s));
                if (prices.ContainsKey(key)) continue;
                
                prices.Add(key, price);
                if (!purchaseOptions.ContainsKey(key))
                {
                    purchaseOptions.Add(key, 0);
                }

                purchaseOptions[key] += price;
            }

            return prices;
        }).ToArray();

        return $"Maximum bananas is {purchaseOptions.Values.Max()} for sequence {purchaseOptions.First(kvp => kvp.Value == purchaseOptions.Values.Max()).Key}";
    }

    private long EvolveSecret(long value)
    {
        var secret = value;
        secret ^= secret * 64;
        secret %= _pruneValue;
        secret ^= secret / 32;
        secret %= _pruneValue;        
        secret ^= secret * 2048;
        secret %= _pruneValue;
        return secret;
    }
}
