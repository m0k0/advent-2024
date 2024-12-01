using AdventOfCode.Y2K24;
using AdventOfCode.Y2K24.Day01;

const string inputFile = "./Day01/test.input.txt";

using var reader = new StreamReader(inputFile);
ISolver solver = new Day01Solver(reader);

var solution = solver.Solve();

Console.WriteLine(solution);