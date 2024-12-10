using AdventOfCode.Y2K24.Utilities;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 10)]
public class Day10Solver : ISolver
{
    private readonly TextReader _inputReader;

    public Day10Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
        
        
    }


    public Result Solve(SolutionVariant? variant)
    {
        var map = Map2D.FromReader(
            _inputReader, (char v) => v.ToDigit());
        
        
        
        return Result.Fail("No solution found");
    }
}