namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 9)]
public class Day09Solver :ISolver
{
    private readonly TextReader _inputReader;
    public Day09Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }
    public Result Solve(SolutionVariant? variant)
    {
        var line = _inputReader.ReadLine();

        while (string.IsNullOrEmpty(line))
        {
        
            line = _inputReader.ReadLine();
        }
        
        return Result.Fail("No solution yet");
    }
}