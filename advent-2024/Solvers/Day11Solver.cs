namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(11)]
public class Day11Solver : ISolver
{
    private readonly TextReader _inputReader;

    public Day11Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }
    public Result Solve(SolutionVariant? variant)
    {
        var line = _inputReader.ReadLine();

        if (string.IsNullOrEmpty(line))
            return Result.Fail("No input");
        
        var numbers = line.Split(' ');
        var stones = new LinkedList<string>(numbers);

        const int blinkCount = 25;
        for (var i = 0; i < blinkCount; i++)
        {
            var result = Blink(stones);
            if (result is FailureResult failureResult)
                return Result.Fail($"Blink {i + 1} failed", failureResult);
            
        }

        var solution = stones.Count;
        
        return Result.Ok(solution.ToString());
    }

    private Result Blink(LinkedList<string> stones)
    {
        var stone = stones.First;
        while (stone is not null)
        {
            if (stone.Value == "0")
            {
                stone.Value = "1";
            } else if (stone.Value.Length % 2 == 0)
            {
                var firstHalf = stone.Value
                    .Substring(0, stone.Value.Length / 2);
                var secondHalf = stone.Value
                    .Substring(stone.Value.Length / 2);

                if (!long.TryParse(firstHalf, out var firstValue) ||
                    !long.TryParse(secondHalf, out var secondValue))
                    return Result.Fail($"Failed to split stone '{stone}'  in half! ");

                stone.Value = firstValue.ToString();
                stone = stones.AddAfter(stone, secondValue.ToString());
            } 
            else
            {
                if (!long.TryParse(stone.Value, out var value))
                    return Result.Fail($"Failed to interpret stone '{stone}'! ");

                value *= 2024;
                
                stone.Value = value.ToString();
            }
            
            stone = stone.Next;
        }

        return Result.Ok();
    }
}