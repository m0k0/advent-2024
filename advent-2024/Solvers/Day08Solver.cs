using System.Drawing;
using AdventOfCode.Y2K24.Utilities;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 8)]
public class Day08Solver: ISolver
{
    private readonly TextReader _inputReader;

    public Day08Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
        
    }

    public Result Solve(SolutionVariant? variant)
    {
        var map = AdventMap2D.FromReader(_inputReader);
        
        if (map is null)
            return Result.Fail("Failed to read map");

        var antiNodeMap = map.Filter([]);
        
        var antennaGroups = map
            .Where((MapPoint<char?> p)=>p.Value is not null)
            .GroupBy(p => p.Value);

        foreach (var group in antennaGroups)
        {

            var antennae = group.ToArray();
            for (var i = 0; i < antennae.Length; i++)
            {
                var sourceAntenna = antennae[i];
                
                for (var i2 = 0; i2 < antennae.Length; i2++)
                {
                    if (i == i2)
                        continue;
                    
                    var targetAntenna = antennae[i2];
                    
                    MarkAntiNodes(sourceAntenna,targetAntenna, antiNodeMap, variant);
                }
            }
            
        }
        
        foreach(var outline in AdventMap2D.ToStringLines(map))
            Console.WriteLine(outline);
        
        Console.WriteLine();

        foreach(var outline in AdventMap2D.ToStringLines(antiNodeMap))
            Console.WriteLine(outline);

        var antiNodeCount = antiNodeMap.Count((char? p) => p is not null);
        
        return Result.Ok(antiNodeCount.ToString());
    }

    private static void MarkAntiNodes(
        MapPoint<char?> sourceAntenna, 
        MapPoint<char?> targetAntenna, 
        Map2D<char?> antiNodeMap,
        SolutionVariant? variant = SolutionVariant.PartOne)
    {
        var antennaDistanceX = sourceAntenna.Location.X - targetAntenna.Location.X;
        var antennaDistanceY = sourceAntenna.Location.Y - targetAntenna.Location.Y;
        
        var antiNodeX = targetAntenna.Location.X;
        var antiNodeY = targetAntenna.Location.Y;

        if (variant == SolutionVariant.PartOne)
        {
            antiNodeX -= antennaDistanceX;
            antiNodeY -= antennaDistanceY;
        }
        
        while (antiNodeMap.IsInside(antiNodeX, antiNodeY))
        {
            antiNodeMap[antiNodeX, antiNodeY] = '#';

            antiNodeX -= antennaDistanceX;
            antiNodeY -= antennaDistanceY;

            if (variant == SolutionVariant.PartOne)
                break;
        }
        
    }
}