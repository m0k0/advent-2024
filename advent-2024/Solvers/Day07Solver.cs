namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 7)]
public class Day07Solver : ISolver
{
    class Equation(long value)
    {
        public long Value { get; } = value;
        public List<int> Arguments { get; } = [];
    }

    enum Operator
    {
        Add,
        Subtract,
        Multiply,
        Divide,
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
        
        if (!long.TryParse(equationSides[0], out long value))
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
        Equation[] equations = [];
        var equationResult = ReadEquations();
        if (equationResult is FailureResult failureResult)
        {
            return Result.Fail("Failed to read equations", failureResult);
        } else if (equationResult is SuccessResult<Equation[]> successResult)
        {
            equations = successResult.Value ?? [];
        }

        Operator[] availableOperators =
        [
            Operator.Add,
            Operator.Multiply
        ];

        
        
        long validEquationSum = 0;
        foreach (var equation in equations)
        {
            var hasSolution = TestEquation(equation, availableOperators);
            if (!hasSolution)
                continue;
            
            validEquationSum += equation.Value;
        }
        
        
        return Result.Ok(validEquationSum.ToString());
    }
    
    
    IEnumerable<Operator[]> GetOperatorSequences(
        Operator[] availableOperators, 
        Operator[]? currentSequence = null, 
        int operationCount = 0)
    {
        
        if (currentSequence is null)
            currentSequence = [];
        
        foreach (var op in availableOperators)
        {
            var newSequence = new List<Operator>(currentSequence);
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
    private bool TestEquation(Equation equation, Operator[] availableOperators)
    {
        var opSequences = GetOperatorSequences(
            availableOperators, 
            operationCount: equation.Arguments.Count -1);

        foreach (var opSequence in opSequences)
        {
            long currentValue = equation.Arguments[0];
            for (var i = 0; i < opSequence.Length; i++)
            {
                var op = opSequence[i];
                currentValue = PerformOperation(op,
                    currentValue,
                    equation.Arguments[i + 1]);
            }
            if (currentValue == equation.Value)
                return true;
        }
        
        return false;
    }
    
    

    private long PerformOperation(Operator op, long leftOperand, int rightOperand)
    {
        
        return op switch
        {
            Operator.Add => leftOperand + rightOperand,
            Operator.Subtract => leftOperand - rightOperand,
            Operator.Multiply => leftOperand * rightOperand,
            Operator.Divide => leftOperand / rightOperand,
            _ => throw new NotImplementedException()
        };
    }
}