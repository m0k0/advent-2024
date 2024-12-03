using System.Text;
using System.Text.Json;
using AdventOfCode.Y2K24;
using AdventOfCode.Y2K24.Solvers;

const string optionsInputCachePath = "./options.cache.json";
SolverOptionsTemplate? optionsTemplate = null;
SolverOptions? options = null;
var solverFactory = SolverFactory.Create()
    .Register<Day01Solver>(1)
    .Register<Day02Solver>(2);


ConsoleElf.WriteIntro();

var optionsJson = "";
if (File.Exists(optionsInputCachePath))
{
    optionsJson = File.ReadAllText(optionsInputCachePath);
    optionsTemplate = JsonSerializer.Deserialize<SolverOptionsTemplate>(optionsJson);

    if (optionsTemplate is not null)
    {
        ConsoleElf.Say($"Last time we tried '{optionsTemplate.Variant
            }' of day {optionsTemplate.Day
            }, using '{Path.GetFileName(optionsTemplate.InputFilePath)}'");
        
        var optionsAction = ConsoleElf.GetSelectionInput("Shall we pick up where we left off?",
        new Dictionary<string,Action<SolverOptionsTemplate>>(){
            {"Yes please!", (opts)=>{ }},
            {"Start from scratch", (opts) =>
                {
                    opts.Day = null;
                    opts.InputFilePath = null;
                    opts.Variant = null;
                } },
            {"Change the day", (opts) =>
            {
                opts.Day = null;
            } },
            {"Change the input file", (opts) =>
            {
                opts.InputFilePath = null;
            } },
            {"Change the variant", (opts) =>
            {
                opts.Variant = null;
            } },
        });
        optionsAction(optionsTemplate);
    }
}


do
{
    options = ConsoleElf.AskSolverOptions(optionsTemplate);
    if (options is null)
        continue;
    
    // validate
    if (!solverFactory.HasSolverForDay(options.Day))
    {
        ConsoleElf.SayError("Argh! We don't have a solution for this day yet!");
        options = null;
        if (optionsTemplate is not null)
            optionsTemplate.Day = null;
        continue;
    }

    if (string.IsNullOrEmpty(options.InputFilePath) ||
        !File.Exists(options.InputFilePath))
    {
        ConsoleElf.SayError($"We've lost the input file!");
        options = null;
        if (optionsTemplate is not null)
            optionsTemplate.InputFilePath = null;
        continue;
    }

} while (options is null);


using var inputFile = File.OpenRead(options.InputFilePath);
using var reader = new StreamReader(inputFile);
ISolver? solver = solverFactory.GetSolver(options.Day, reader);
if (solver is null)
{
    ConsoleElf.SayError(
        $"Ugh! There's an issue with setting up the solver for day {options.Day}!");
    return;
}

// setup done, write options to cache
optionsJson = JsonSerializer.Serialize(options);
File.WriteAllText(optionsInputCachePath, optionsJson, Encoding.UTF8);

// get solution
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
