internal class Day7 : Day
{
    protected override string InputPath => "/day7/input.txt";

    private const string _addMultiply = "+*";
    private const string _addMultiplyConcat = "+*|";

    private Equation[]? _equations;

    private Equation[] Equations => _equations ??= Input
        .Split("\r\n")
        .Select(x => x.Split(": ", StringSplitOptions.RemoveEmptyEntries))
        .Select(x => new Equation
        {
            Result = long.Parse(x[0]),
            Arguments = x[1].Split(" ").Select(y => long.Parse(y)).ToArray()
        }).ToArray();

    internal override string A()
    {
        var possible = Equations.Where(e => e.IsPossible([.._addMultiply]));
        return $"The sum of all posible results is {possible.Sum(x => x.Result)}";
    }

    internal override string B()
    {
        var possible = Equations.Where(e => e.IsPossible([.._addMultiplyConcat]));
        return $"The sum of all posible results is {possible.Sum(x => x.Result)}";
    }

    private class Equation
    {
        internal long Result { get; init; } = 0;

        internal long[] Arguments { get; init; } = [];

        internal bool IsPossible(char[] operators)
        {
            var ops = Arguments.Length - 1;
            var recipes = BuildOperators(operators, []);
            return recipes.Any(x => TryOperators(x));
        }

        internal char[][] BuildOperators(char[] operators, char[] recipe, int depth = 0)
        {
            if (depth == Arguments.Length - 1) return [recipe];

            char[][] results = [];
            foreach(var op in operators)
            {
                results = [..results.Concat(BuildOperators(operators, [..recipe.Append(op)], depth + 1))];
            }
            
            return results;
        }

        internal bool TryOperators(char[] operators)
        {
            var result = Arguments[0];
            for(var i = 0; i< operators.Length; i++)
            {
                var op = operators[i];
                if (op == '+') result += Arguments[i+1];
                else if (op == '*') result *= Arguments[i+1];
                else if (op == '|') result = long.Parse($"{result}{Arguments[i+1]}");
            }

            return result == Result;
        }
    }
}
