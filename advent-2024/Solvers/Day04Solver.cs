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
        var reversedSearchWord = new string(searchWord.Reverse().ToArray());
        LinkedList<char[]> lineBuffer = [];
        
        
        var matchCount = 0;
        var line = _inputReader.ReadLine();
        var yPos = -1;
        while (!string.IsNullOrEmpty(line))
        {
            var currentLine= lineBuffer.AddLast(line.ToCharArray());
            yPos++;
            
            if (lineBuffer.Count > searchWord.Length)
            {
                // shave off first line, no longer needed
                lineBuffer.RemoveFirst();
            }

            List<char> horizontalBuffer = [];
            List<char> horizontalReverseBuffer = [];
            
            for (var xPos = 0; xPos < currentLine.Value.Length; xPos++)
            {
                var c = line[xPos];
                
                // horizontal search
                if (c == searchWord[0] || c == searchWord[^1])
                {
                    
                    // forwards
                    if (IsHorizontalMatchFromPosition(searchWord, xPos, currentLine.Value))
                        matchCount++;
                    
                    // forwards reverse
                    if (IsHorizontalMatchFromPosition(reversedSearchWord, xPos, currentLine.Value))
                        matchCount++;
                }
                
                // vertical match
                if (lineBuffer.Count >= searchWord.Length)
                {
                    
                    // above
                    if (IsVerticalMatchFromPosition(searchWord, xPos, currentLine))
                        matchCount++;
                    // above reverse
                    if (IsVerticalMatchFromPosition(reversedSearchWord, xPos, currentLine))
                        matchCount++;
                    
                    // diagonal forwards
                    if (IsVerticalMatchFromPosition(searchWord, xPos, currentLine, 1))
                        matchCount++;
                    // diagonal forwards reverse
                    if (IsVerticalMatchFromPosition(reversedSearchWord, xPos, currentLine, 1))
                        matchCount++;
                    
                    // diagonal backwards
                    if (IsVerticalMatchFromPosition(searchWord, xPos, currentLine,-1))
                        matchCount++;
                    // diagonal backwards reverse
                    if (IsVerticalMatchFromPosition(reversedSearchWord, xPos, currentLine,-1))
                        matchCount++;
                }
                
            }
            
            line = _inputReader.ReadLine();
            
        }
        
        
        
        return Result.Ok(matchCount.ToString());
    }

    private static bool IsVerticalMatchFromPosition(
        string searchWord,
        int currentPosition,
        LinkedListNode<char[]> fromLine,
        int xVector = 0)
    {
        var currentLine = fromLine;
        for (var wi = 0; wi < searchWord.Length; wi++)
        {
            if (currentLine is null)
                return false;
            
            var charIndex = currentPosition + (wi * xVector);
            if (charIndex < 0 || charIndex >= currentLine.Value.Length)
            {
                // out of line bounds
                return false;
            }

            var foundChar = currentLine.Value[charIndex];
            if (foundChar != searchWord[wi])
            {
                // not a full match
                return false;
            }
            currentLine = currentLine.Previous;
                
        }
        return true;
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