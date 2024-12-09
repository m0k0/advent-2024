using System.Text;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 9)]
public class Day09Solver :ISolver
{
    struct BlockSummary
    {
        public int?[] Blocks { get; init; }
        public int FreeBlocks { get; init; }
        public int LastFile { get; init; }
    }
    
    private readonly TextReader _inputReader;
    public Day09Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }

    int CharToDigit(char c)
    {
        return (c - '0') % 10;
    }
    char DigitToChar(int digit)
    {
        return (char)((digit % 10) + '0');
    }
    public Result Solve(SolutionVariant? variant)
    {
        var line = _inputReader.ReadLine();

        while (!string.IsNullOrEmpty(line))
        {
            var lineParts = line.ToCharArray();

            var summary = GetBlockSummary(lineParts);

            // defrag
            var emptyBlocks = summary.FreeBlocks;
            
            var leftIndex = 0;
            for (var rightIndex = 1; 
                 rightIndex <= summary.Blocks.Length && emptyBlocks > 0; 
                 rightIndex++)
            {
                var block = summary.Blocks[^rightIndex];
                if (block is null)
                    continue;
                
                // find space
                while(summary.Blocks[leftIndex] is not null &&
                      leftIndex < summary.Blocks.Length)
                    leftIndex++;

                summary.Blocks[leftIndex] = block;
                summary.Blocks[^rightIndex] = null;
                emptyBlocks--;
            }    
            
        
            
            
            foreach (var block in summary.Blocks)
                Console.Write((block is null ? ' ' : DigitToChar(block.Value)));
            
            line = _inputReader.ReadLine();
        }
        
        
        
        return Result.Fail("No solution yet");
    }

    BlockSummary GetBlockSummary(IEnumerable<char> diskMap)
    {
        var lastFileId = 0;
        var freeSpaceCount = 0;
        var blocks = new List<int?>();
            
        // get blocks
        var diskIndex = 0;
        bool IsFile() => diskIndex % 2 == 0; 
        foreach (var c in diskMap)
        {
            var blockSize = CharToDigit(c);
            for (var i2 = 0; i2 < blockSize; i2++)
            {
                if (IsFile())
                {
                    blocks.Add(lastFileId);
                }
                else
                {
                    blocks.Add(null);
                    freeSpaceCount++;
                }
            }

            if (IsFile())
            {
                lastFileId++;
            }

            diskIndex++;
        }

        return new BlockSummary()
        {
            Blocks = blocks.ToArray(),
            FreeBlocks = freeSpaceCount,
            LastFile = lastFileId,
        };
    }
}