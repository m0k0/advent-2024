namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 7)]
public class Day07Solver : ISolver
{
    class Equation(long value)
    {
        public long Value { get; } = value;
        public List<long> Arguments { get; } = [];
    }

    interface IOperator
    {
        int OperationOrder { get; }
        long Evaluate(long leftOperand, long rightOperand);
    }

    class MultiplyOperator : IOperator
    {
        public int OperationOrder { get; } = 0;
        public long Evaluate(long leftOperand, long rightOperand)
        {
            return leftOperand * rightOperand;
        }
    }
    class AddOperator : IOperator
    {
        public int OperationOrder { get; } = 0;
        public long Evaluate(long leftOperand, long rightOperand)
        {
            return leftOperand + rightOperand;
        }
    }
    class ConcatOperator : IOperator
    {
        public int OperationOrder { get; } = 1;
        public long Evaluate(long leftOperand, long rightOperand)
        {
            var concat = $"{leftOperand}{rightOperand}"; 
            return long.Parse(concat);
        }
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
        
        if (!long.TryParse(equationSides[0], out var value))
            return Result.Fail("Failed to parse equation value");

        var equation = new Equation(value);

        var args = equationSides[1].Split(
            ARGUMENTS_DELIMITER, 
            StringSplitOptions.RemoveEmptyEntries);
        
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            
            if (!long.TryParse(arg, out var argValue))
                return Result.Fail($"Failed to parse value of argument {i+1}");
            
            equation.Arguments.Add(argValue);
        }
        
        return Result.Ok(equation);

    }

    
    public Result Solve(SolutionVariant? variant)
    {
        Equation[] equations = [];
        var equationResult = ReadEquations();
        if (equationResult is FailureResult failureResult)
        {
            return Result.Fail("Failed to read equations", failureResult);
        } else if (equationResult is SuccessResult<Equation[]> successResult)
        {
            equations = successResult.Value ?? [];
        }

        List<IOperator> availableOperators =
        [
            new AddOperator(),
            new MultiplyOperator(),
        ];
        if (variant == SolutionVariant.PartTwo)
            availableOperators.Add(new ConcatOperator());

        
        
        long validEquationSum = 0;
        foreach (var equation in equations)
        {
            var hasSolution = TestEquation(equation, availableOperators.ToArray());
            if (!hasSolution)
                continue;
            
            validEquationSum += equation.Value;
        }
        
        
        return Result.Ok(validEquationSum.ToString());
    }
    
    
    IEnumerable<IOperator[]> GetOperatorSequences(
        IOperator[] availableOperators, 
        IOperator[]? currentSequence = null, 
        int operationCount = 0)
    {
        
        if (currentSequence is null)
            currentSequence = [];
        
        foreach (var op in availableOperators)
        {
            var newSequence = new List<IOperator>(currentSequence);
            newSequence.Add(op);
            
            var newSequenceArray = newSequence.ToArray();
            if (operationCount <= 1)
            {
                yield return newSequenceArray;
            } else {    
            
                var children =  GetOperatorSequences(
                    availableOperators,
                    newSequenceArray, 
                    operationCount - 1);
                foreach (var child in children)
                    yield return child;
            }
        }
        
    }
    private bool TestEquation(Equation equation, IOperator[] availableOperators)
    {
        var opSequences = GetOperatorSequences(
            availableOperators, 
            operationCount: equation.Arguments.Count -1);

        foreach (var opSequence in opSequences)
        {
            if (TestOperatorSequence(equation, opSequence))
                return true;
        }
        
        return false;
    }

    private static bool TestOperatorSequence(Equation equation, IOperator[] opSequence)
    {
        
        long[] ProcessArguments(long[] arguments, IOperator[] opSequence, int opLevel = 0)
        {
            var hasHigherOps = opSequence
                .Any(op => op.OperationOrder > opLevel);
            
            if (hasHigherOps)
                arguments = ProcessArguments(arguments, opSequence, opLevel + 1);

            var currentAndLowerOps = opSequence
                    .Where(op => 
                        op.OperationOrder <= opLevel)
                    .ToArray();
            
            List<long> results = [];

            long? runningValue = null;
            for (var i = 0; i < currentAndLowerOps.Length; i++)
            {
                var op = currentAndLowerOps[i];

                if (op.OperationOrder != opLevel)
                {
                    if (runningValue.HasValue)
                        results.Add(runningValue.Value);
                    runningValue = null;
                    results.Add(arguments[i]);
                    continue;
                }

                if (runningValue is null)
                    runningValue = equation.Arguments[i];
                
                runningValue = op.Evaluate(
                    runningValue.Value,
                    equation.Arguments[i + 1]);
                
            }
            if (runningValue.HasValue)
                results.Add(runningValue.Value);
            
            return results.ToArray();
        }

        var results = ProcessArguments(
            equation.Arguments.ToArray(),
            opSequence);

        foreach (var result in results)
        {
            if (result == equation.Value)
                return true;
        }
        
        return false;
    }
}