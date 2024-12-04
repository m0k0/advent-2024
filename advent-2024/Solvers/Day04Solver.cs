using System.Collections;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 4)]
public class Day04Solver : ISolver
{
    private readonly TextReader _inputReader;
    public Day04Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }

    public Result Solve(SolutionVariant? variant)
    {
        const string searchWord = "XMAS";
        Queue<char[]> lineBuffer = [];
        
        var matchCount = 0;
        var line = _inputReader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            var lineChars = line.ToCharArray();
            
            lineBuffer.Enqueue(lineChars);
            
            if (lineBuffer.Count > searchWord.Length)
            {
                // shave off first line, no longer needed
                _ = lineBuffer.Dequeue(); 
            }

            List<char> horizontalBuffer = [];
            List<char> horizontalReverseBuffer = [];
            
            for (var i = 0; i < lineChars.Length; i++)
            {
                var c = line[i];
                
                // horizontal search
                if (c == searchWord[0] || c == searchWord[^1])
                {
                    
                    // forward
                    if (IsHorizontalMatchFromPosition(searchWord, i, lineChars))
                        matchCount++;
                    
                    // reverse
                    if (IsHorizontalMatchFromPosition(searchWord, i, lineChars, 
                            reverseSearch: true))
                        matchCount++;
                }
                
                if (lineBuffer.Count >= searchWord.Length)
                {
                    // vertical / diagonal search
                    // if enough lines are available
                    
                    // search from bottom up
                    // c == first or last char 
                }
                
            }
            
            line = _inputReader.ReadLine();
            
        }
        
        
        
        return Result.Ok(matchCount.ToString());
    }

    private static bool IsHorizontalMatchFromPosition(
        string searchWord, 
        int currentPosition, 
        char[] lineChars, 
        bool reverseSearch = false)
    {
        for (var wi = 0; wi < searchWord.Length; wi++)
        {
            var offset = wi;
            if (reverseSearch)
                offset *= -1;
            
            var charIndex = currentPosition + offset;
            if (charIndex < 0 || charIndex >= lineChars.Length)
            {
                // out of line bounds
                return false;
            }

            var charOnLine = lineChars[charIndex];
            if (charOnLine != searchWord[wi])
            {
                // not a full match
                return false;
            }
        }
        return true;
    }
}