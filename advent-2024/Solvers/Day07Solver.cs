namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 7)]
public class Day07Solver : ISolver
{
    class Equation(int value)
    {
        public int Value { get; } = value;
        public List<int> Arguments { get; } = [];
    }
    
    
    private readonly TextReader _inputReader;
    
    public Day07Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }


    public Result ReadEquations()
    {
        List<Equation> equations = [];
        var line = _inputReader.ReadLine();
        var lineNumber = 0;
        while (!string.IsNullOrEmpty(line))
        {
            lineNumber++;
            
            var equationResult = ParseEquation(line);
            if (equationResult is FailureResult failureResult)
            {
                return Result.Fail("Error on line " + lineNumber, failureResult);
            } else if (equationResult is SuccessResult<Equation> successResult)
            {
                if (successResult.Value is not null)
                    equations.Add(successResult.Value);
            }

            line = _inputReader.ReadLine();
        }

        return Result.Ok(equations.ToArray());
    }

    private Result ParseEquation(string line)
    {
        const char EQUATION_DELIMITER = ':';
        const char ARGUMENTS_DELIMITER = ' ';

        var equationSides = line.Split(EQUATION_DELIMITER);
        if (equationSides.Length != 2)
            return Result.Fail("Unable to find sides of equation");
        
        if (!int.TryParse(equationSides[0], out int value))
            return Result.Fail("Failed to parse equation value");

        var equation = new Equation(value);

        var args = equationSides[1].Split(
            ARGUMENTS_DELIMITER, 
            StringSplitOptions.RemoveEmptyEntries);
        
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            
            if (!int.TryParse(arg, out int argValue))
                return Result.Fail($"Failed to parse value of argument {i+1}");
            
            equation.Arguments.Add(argValue);
        }
        
        return Result.Ok(equation);

    }

    public Result Solve(SolutionVariant? variant)
    {
        Equation[] equations;
        var equationResult = ReadEquations();
        if (equationResult is FailureResult failureResult)
        {
            return Result.Fail("Failed to read equations", failureResult);
        } else if (equationResult is SuccessResult<Equation[]> successResult)
        {
            equations = successResult.Value ?? [];
        }
        
        
        
        return Result.Fail("No solution found");
    }
}