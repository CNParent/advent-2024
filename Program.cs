using System.Diagnostics;

Day[] days = 
[
    new Day1(), new Day2(), new Day3(), new Day4(), new Day5(), 
    new Day6(), new Day7(), new Day8(), new Day9(), new Day10(), 
    new Day11(), new Day12(), new Day13(), new Day14(), new Day15(),
    new Day16(), new Day17(), new Day18(), new Day19(), new Day20(),
    new Day21(), new Day22(), new Day23()
];

var options = "";
for(var i = 1; i <= days.Length; i++)
{
    options += $"{i}A, {i}B, ";
}

options = options.TrimEnd(',',' ');

var sw = new Stopwatch();
while(true)
{
    Console.WriteLine("Select which day to execute or enter E to exit");
    Console.WriteLine($"Options: {options}");
    var selection = Console.ReadLine();
    if (selection == null) continue;
    if (selection.StartsWith("e", StringComparison.CurrentCultureIgnoreCase)) break;

    var option = selection.Last();
    if (!int.TryParse(selection.Trim(option), out var value)) continue;
    if (value < 1 || value > days.Length) continue;
    
    var day = days[value - 1];
    sw.Restart();
    var result = option == 'A' ? day.A() : day.B();
    sw.Stop();
    Console.WriteLine($"\r\nExecuted in {sw.Elapsed.TotalSeconds} seconds");
    Console.WriteLine($"{selection} result: {result}");
}
