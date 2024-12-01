using AdventOfCode.Y2K24;
using AdventOfCode.Y2K24.Solvers;

ConsoleElf.WriteIntro();

var solverFactory = SolverFactory.Create()
    .Register<Day01Solver>(1);


SolverOptions? options;
do
{
    options = ConsoleElf.AskSolverOptions();
    
} while (options is null);

if (!solverFactory.HasSolverForDay(options.Day))
{
    ConsoleElf.SayError("Argh! We don't have a solution for this day yet!");
    return;
}

if (string.IsNullOrEmpty(options.InputFilePath) ||
    !File.Exists(options.InputFilePath))
{
    ConsoleElf.SayError($"We've lost the input file!");
    return;
}

using var inputFile = File.OpenRead(options.InputFilePath);
using var reader = new StreamReader(inputFile);
ISolver? solver = solverFactory.GetSolver(options.Day, reader);
if (solver is null)
{
    ConsoleElf.SayError(
        $"Ugh! There's an issue with setting up the solver for day {options.Day}!");
    return;
}


var solution = solver.Solve(options.Variant);

switch (solution)
{
    case FailureResult failureResult:
        ConsoleElf.SayResult(failureResult);
        break;
    
    case SuccessResult<string> successResult:
        ConsoleElf.SayResult(successResult);
        break;
    
    default:
        ConsoleElf.SayError("Unexpected solution output");
        break;
}
