namespace AdventOfCode.Y2K24;

public enum SolutionVariant
{
    PartOne = 0,
    PartTwo = 1
}
public interface ISolver
{
    Result Solve(SolutionVariant? variant = 0);
}