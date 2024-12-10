using System.Text;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 9)]
public class Day09Solver :ISolver
{
    const bool DEBUG = false;
    class BlockSummary
    {
        public int?[] Blocks { get; init; } = [];
        public int FreeBlocks { get; init; }
        public int LastFile { get; init; }
        public DiskSegment[] FileSegments { get; init; } = [];
        public DiskSegment[] SpaceSegments { get; init; } = [];

    }

    class DiskSegment(int blockIndex)
    {
        public int BlockIndex { get; set; } = blockIndex;
        public int Size { get; set; }
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
        
        if (variant == SolutionVariant.PartOne)
            DefragBlocks(summary.Blocks);
        else if (variant == SolutionVariant.PartTwo)
            DefragBlocksBySegment(summary.Blocks, summary.FileSegments, summary.SpaceSegments);

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
                continue;
            
            checksum += block.Value * blockIndex;
        }
        
        return checksum;
    }

    private void DefragBlocksBySegment(
        int?[] blocks, 
        DiskSegment[] fileSegments, 
        DiskSegment[] spaceSegments)
    {
        
        DiskSegment? FindFreeSpace(int blockSize, int rangeStart, int rangeEnd)
        {
            for (var i = 0; i < spaceSegments.Length; i++)
            {
                var segment = spaceSegments[i];
                if (segment.BlockIndex < rangeStart || segment.BlockIndex > rangeEnd)
                    continue;
                
                if (segment.Size >= blockSize)
                    return segment;
            }

            return null;
        }

        void MoveFile(DiskSegment from, DiskSegment to)
        {
            var sourceIndex = from.BlockIndex;
            var targetIndex = to.BlockIndex;

            for (var i = 0; i < from.Size; i++)
            {
                blocks[targetIndex + i] = blocks[sourceIndex + i];
                blocks[sourceIndex + i] = null;
            }
            from.BlockIndex = targetIndex;
            to.BlockIndex = sourceIndex;
            
        }

        for (var i = fileSegments.Length -1; i >= 0; i--)
        {
            var fileSegment = fileSegments[i];

            var freeSegment = FindFreeSpace(fileSegment.Size, 0 , fileSegment.BlockIndex);
            if (freeSegment is null)
                continue;

            MoveFile(fileSegment, freeSegment);

            PrintFileSystem(blocks);
        }
    }

    private void PrintFileSystem(int?[] blocks)
    {
        if(!DEBUG)
            return;
        var output = string.Join("",
            blocks.Select(b => b.HasValue ? DigitToChar(b.Value): '.'));
        Console.WriteLine(output);
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
            
            PrintFileSystem(blocks);
        }
    }

    BlockSummary GetBlockSummary(IEnumerable<char> diskMap)
    {
        var blocks = new List<int?>();
        var files = new List<DiskSegment>();
        var freeSpaces = new List<DiskSegment>();
            
        // get blocks
        var blockIndex = 0;
        var diskIndex = -1;
        bool IsFile() => diskIndex % 2 == 0; 
        foreach (var c in diskMap)
        {
            diskIndex++;
            var segment = new DiskSegment(blockIndex);
            var blockSize = CharToDigit(c);
            
            for (var i2 = 0; i2 < blockSize; i2++)
            {
                if (IsFile())
                {
                    blocks.Add(files.Count);
                }
                else
                {
                    blocks.Add(null);
                }
                segment.Size++;
            }

            if (IsFile())
            {
                files.Add(segment);
            }
            else
            {
                freeSpaces.Add(segment);
            }

            blockIndex += blockSize;
            
            PrintFileSystem(blocks.ToArray());
        }

        return new BlockSummary()
        {
            Blocks = blocks.ToArray(),
            FileSegments = files.ToArray(),
            SpaceSegments = freeSpaces.ToArray()
        };
    }
}