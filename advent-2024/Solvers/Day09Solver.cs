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
        if (string.IsNullOrWhiteSpace(line))
            return Result.Fail("Failed to read input");
        
        var lineParts = line.ToCharArray();

        var summary = GetBlockSummary(lineParts);
        
        DefragBlocks(summary.Blocks);

        var checksum = CalculateChecksum(summary.Blocks);
        
        
        return Result.Ok(checksum.ToString());
    }

    private long CalculateChecksum(int?[] blocks)
    {
        long checksum = 0;
        for (var blockIndex = 0; blockIndex < blocks.Length; blockIndex++)
        {
            var block = blocks[blockIndex];
            if (block == null)
                break;
            
            checksum += block.Value * blockIndex;
        }
        
        return checksum;
    }

    private void DefragBlocks(int?[] blocks)
    {
        var leftIndex = 0;
        var rightIndex = blocks.Length;
            
        while (rightIndex >= 0)
        {
            rightIndex--;
                
            var block = blocks[rightIndex];
            if (block is null)
                continue;

            // find space
            while(blocks[leftIndex] is not null &&
                  leftIndex < blocks.Length)
                leftIndex++;

            if (leftIndex > rightIndex)
                break;
                
            blocks[leftIndex] = block;
            blocks[rightIndex] = null;
        }
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