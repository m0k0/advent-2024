using System.Text;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(11)]
public class Day11Solver : ISolver
{
    class StoneSummary
    {
        public long StoneCount { get; init; }
        public string? Stones { get; init; }
    }
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
        

        // start tasking after x stones
        int mtThreshold = 16;
        
        int blinkDepth = 25;
        if (variant == SolutionVariant.PartTwo)
            blinkDepth = 75;

        var stones = line;
        long stoneCount = 0;

        var blinkCount = 0;
        while (stoneCount < mtThreshold)
        {
            var blinkedStones = EnumerateStones(stones, 1).ToList();
            stoneCount = blinkedStones.Count;
            stones = string.Join(" ", blinkedStones);
            blinkCount++;
        }
            
        // enough stones; now multi-task
        
        var enumerationTasks = new List<Task<int>>();
        foreach (var stone in stones.Split(' '))
        {
            var task = Task.Run(() => EnumerateStones(stone, blinkDepth - blinkCount).Count());
            enumerationTasks.Add(task);
        }
        
        var completionTask = Task.WhenAll(enumerationTasks.ToArray());
        completionTask.Wait();
        stoneCount = completionTask.Result.Sum();
        /*
        for (var i = 0; i < blinkCount; i++)
        {
            //var result = Blink(stones);
            var result = BlinkString(stones);
            if (result is FailureResult failureResult)
            {
                return Result.Fail($"Blink {i + 1} failed", failureResult);
            } else if (result is SuccessResult<StoneSummary> successResult)
            {
                if (successResult.Value is null)
                    return Result.Fail($"Blink {i + 1} has no output");
                
                stones = successResult.Value.Stones;
                stoneCount = successResult.Value.StoneCount;
            }
        }*/

        return Result.Ok(stoneCount.ToString());
    }

    private IEnumerable<string?> EnumerateStones(string? stoneStream, int blinkDepth)
    {
        if (string.IsNullOrEmpty(stoneStream))
            yield break;

        const int bufferSize = sizeof(long) * sizeof(char);
        char[] stoneBuffer = new char[bufferSize];
        int stoneNumberSize = 0;
        
        //long stoneCount = 0;
        for(var i = 0; i < stoneStream.Length; i++)
        {
            var c = stoneStream[i];

            if (i == stoneStream.Length - 1)
            {
                stoneBuffer[stoneNumberSize++] = c;
            } else if (c != ' ')
            {
                stoneBuffer[stoneNumberSize++] = c;
                continue;
            }

            if (stoneNumberSize == 1 && stoneBuffer[0] == '0')
            {
                const string value = "1";
                if (blinkDepth <= 1)
                {
                    yield return value;
                }
                else
                {
                    foreach (var stone in EnumerateStones(value, blinkDepth-1))
                        yield return stone;
                }
            } 
            else if (stoneNumberSize % 2 == 0)
            {
                var halfSize = stoneNumberSize / 2;
                var firstHalf = new string(stoneBuffer[..halfSize]);
                
                
                if (blinkDepth <= 1)
                {
                    yield return firstHalf;
                }
                else
                {
                    foreach (var stone in EnumerateStones(
                                 firstHalf, blinkDepth-1))
                        yield return stone;
                }
                
                
                
                
                if (!long.TryParse(stoneBuffer[halfSize..stoneNumberSize], out var secondHalfValue ))
                    throw new Exception($"Failed to interpret stone!");
                var secondHalf = secondHalfValue.ToString();
                
                if (blinkDepth <= 1)
                {
                    yield return secondHalf;
                }
                else
                {
                    foreach (var stone in EnumerateStones(
                                 secondHalf, blinkDepth-1))
                        yield return stone;
                }


            }
            else
            {
                if (!long.TryParse(stoneBuffer[0..stoneNumberSize], out var value))
                    throw new Exception($"Failed to interpret stone!");

                value *= 2024;

                if (blinkDepth <= 1)
                {
                    yield return value.ToString();
                }
                else
                {
                    foreach (var stone in EnumerateStones(
                                 value.ToString(), blinkDepth-1))
                        yield return stone;
                }
                
            }

            stoneNumberSize = 0;
            
        }


    }
    private Result BlinkString(ReadOnlySpan<char> stoneStream)
    {
        StringBuilder resultStoneStream = new(); 
        const int bufferSize = sizeof(long) * sizeof(char);
        Span<char> stoneBuffer = new char[bufferSize];
        int stoneNumberSize = 0;
        long stoneCount = 0;
        for(var i = 0; i < stoneStream.Length; i++)
        {
            var c = stoneStream[i];

            if (i == stoneStream.Length - 1)
            {
                stoneBuffer[stoneNumberSize++] = c;
            } else if (c != ' ')
            {
                stoneBuffer[stoneNumberSize++] = c;
                continue;
            }

            if (stoneNumberSize == 1 && stoneBuffer[0] == '0')
            {
                resultStoneStream.Append('1');
                
            } 
            else if (stoneNumberSize % 2 == 0)
            {
                var halfSize = stoneNumberSize / 2;
                var firstHalf = stoneBuffer[..halfSize];
                resultStoneStream.Append(firstHalf);
                
                resultStoneStream.Append(' ');
                var secondHalf = stoneBuffer[halfSize..stoneNumberSize];
                
                if (!long.TryParse(secondHalf, out var secondHalfValue ))
                    return Result.Fail($"Failed to interpret stone! ");
                
                resultStoneStream.Append(secondHalfValue.ToString());
                stoneCount++;
            }
            else
            {
                if (!long.TryParse(stoneBuffer[0..stoneNumberSize], out var value))
                    return Result.Fail($"Failed to interpret stone! ");

                value *= 2024;

                resultStoneStream.Append(value.ToString());
            }

            stoneNumberSize = 0;
            stoneCount++;
            if (i < stoneStream.Length - 1)
            {
                resultStoneStream.Append(' ');
            }
        }

        var result = new StoneSummary()
        {
            Stones = resultStoneStream.ToString(),
            StoneCount = stoneCount
        };
        return Result.Ok(result);
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