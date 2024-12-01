using System.Text;
using System.Text.Json;
using AdventOfCode.Y2K24;
using AdventOfCode.Y2K24.Solvers;

const string optionsInputCachePath = "./options.cache.json";
SolverOptions? options = null;
var solverFactory = SolverFactory.Create()
    .Register<Day01Solver>(1);


ConsoleElf.WriteIntro();

var optionsJson = "";
if (File.Exists(optionsInputCachePath))
{
    optionsJson = File.ReadAllText(optionsInputCachePath);
    options = JsonSerializer.Deserialize<SolverOptions>(optionsJson);

    if (options is not null)
    {
        ConsoleElf.Say($"Last time we tried {options.Variant} of day {options.Day}, using '{Path.GetFileName(options.InputFilePath)}'");
        var reuseOptions = ConsoleElf.GetSelectionInput("Shall we pick up where we left off?",
        new Dictionary<string,bool>(){
            {"Yes", true},
            {"No", false}
        });
        if (!reuseOptions)
            options = null;
    }
}

if (options is null)
{
    do
    {
        options = ConsoleElf.AskSolverOptions();

    } while (options is null);
}

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

// validation done, write options to cache
optionsJson = JsonSerializer.Serialize(options);
File.WriteAllText(optionsInputCachePath, optionsJson, Encoding.UTF8);

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
