namespace AdventOfCode.Y2K24.Solvers;

public class Day02Solver : ISolver
{
    private readonly TextReader _inputReader;
    public Day02Solver(
        TextReader inputReader
    ){
        _inputReader = inputReader;
    }
    public Result Solve(SolutionVariant? variant = 0)
    {
        var useDampener = variant == SolutionVariant.PartTwo;
        
        var line = _inputReader.ReadLine();
        var lineNumber = 0;
        var safeReportCount = 0;
        while (!string.IsNullOrEmpty(line))
        {
            var reportResult = ParseReport(line);
            if (reportResult is FailureResult failureResult)
            {
                return Result.Fail(
                $"Error parsing report on line: {lineNumber}",
                    failureResult);
            } 
            if (reportResult is not SuccessResult<List<int>> successResult)
            {
                return Result.Fail(
                    $"Unexpected result type on line: {lineNumber}");
            }
            
            var report = successResult.Value;
            if (report is null)
            {
                return Result.Fail(
                    $"Empty report on line: {lineNumber}");
            }
            if (TestReportSafety(report, useDampener))
            {
                safeReportCount++;
            }
        
            line = _inputReader.ReadLine();
            lineNumber++;
        }
        
        return Result.Ok(safeReportCount.ToString());
    }

    
    private bool TestReportSafety(IEnumerable<int> report, bool useDampener = false)
    {
        bool isSafe = true;
        int? lastNumber = null;
        bool? isAscending = null;
        bool hasDampener = useDampener;
        foreach (var number in report)
        {
            if (lastNumber is null)
            {
                lastNumber = number;
                continue;
            }
            if (isAscending is null)
            {
                isAscending = lastNumber < number;                
            }

            // rule 1: all either increasing or decreasing
            if (isAscending.Value && lastNumber >= number ||
                !isAscending.Value && lastNumber <= number)
            {
                if (hasDampener)
                {
                    hasDampener = false;
                }
                else
                {
                    isSafe = false;
                    break;                    
                }
            }
            
            // rule 2: adjacent number differ by at least one, at most three
            var difference = Math.Abs(number - lastNumber.Value);
            if (difference < 1 || difference > 3)
            {
                if (hasDampener)
                {
                    hasDampener = false;
                }
                else
                {
                    isSafe = false;
                    break;                    
                }
            }
            
            lastNumber = number;
        }

        return isSafe;
    }
    private Result ParseReport(string line)
    {
        var reportNumbers = line.Split(' ');
        var report = new List<int>();
        
        for (var i = 0; i < reportNumbers.Length; i++)
        {
            var numberString = reportNumbers[i];
            if (!int.TryParse(numberString, out var number))
            {
                return Result.Fail($"Invalid report number at position {i+1}");
            }
            report.Add(number);
        }
        return Result.Ok(report);
    }
}