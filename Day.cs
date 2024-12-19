internal abstract class Day
{
    private string? _input;

    protected abstract string InputPath { get; }

    internal abstract string A();

    internal abstract string B();

    protected string Input => _input ??= File.ReadAllText($"C:\\git\\advent-2024{InputPath}");
}