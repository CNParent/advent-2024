internal class Day3 : Day
{
    const int _maxDigits = 3;

    protected override string InputPath => "/day3/input.txt";

    private readonly string _command;

    private int _index = 0;

    private bool _enabled = true;

    private int _sum = 0;

    internal Day3()
    {
        _command = Input;
    }

    internal override string A()
    {
        _index = 0;
        _enabled = true;
        _sum = 0;

        while(_index < _command.Length)
        {
            if (_index + 4 < _command.Length && _command.Substring(_index,4) == "mul(") ParseMul();
            else 
            {
                _index++;
            }
        }

        return $"The sum of all 'mul' instructions is {_sum}";
    }

    internal override string B()
    {
        _index = 0;
        _enabled = true;
        _sum = 0;

        while(_index < _command.Length)
        {
            if (_index + 4 < _command.Length && _command.Substring(_index,4) == "mul(") ParseMul();
            else if (_index + 4 < _command.Length && _command.Substring(_index, 4) == "do()")
            {
                _enabled = true;
                _index += 4;
            }
            else if (_index + 7 < _command.Length && _command.Substring(_index, 7) == "don't()")
            {
                _enabled = false;
                _index += 7;
            }
            else 
            {
                _index++;
            }
        }

        return $"The sum of all 'mul' instructions is {_sum}";
    }

    private void ParseMul()
    {
        _index += 4;

        var a = ParseDigits();
        if (a is null) return;

        if (_command[_index] != ',') return;
        _index++;

        var b = ParseDigits();
        if (b is null) return;

        if(_command[_index] != ')') return;

        if (!_enabled) return;

        _sum += a.Value * b.Value;
    }

    private int? ParseDigits()
    {
        var j = 0;
        while(_command[_index+j] <= '9' && _command[_index+j] >= '0') j++;

        if (j > _maxDigits || j == 0) return null;

        var text = _command.Substring(_index, j);
        _index += j;
        return int.Parse(text);
    }
}
