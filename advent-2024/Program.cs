using AdventOfCode.Y2K24;
using AdventOfCode.Y2K24.Solvers;

ConsoleElf.WriteIntro();

//const string inputFile = "./Day01/test.input.txt";
const string inputFile = "./Day01/full.input.txt";

using var reader = new StreamReader(inputFile);
ISolver solver = new Day01Solver(reader);

var solution = solver.Solve(SolutionVariant.PartTwo);

switch (solution)
{
    case FailureResult failureResult:
        ConsoleElf.PrintResult(failureResult);
        break;
    
    case SuccessResult<string> successResult:
        ConsoleElf.PrintResult(successResult);
        break;
    
    default:
        ConsoleElf.PrintError("Unexpected solution output");
        break;
}
