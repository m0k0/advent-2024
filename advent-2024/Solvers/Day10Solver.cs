using System.Drawing;
using AdventOfCode.Y2K24.Utilities;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 10)]
public class Day10Solver : ISolver
{
    private readonly TextReader _inputReader;

    public Day10Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
        
        
    }


    public Result Solve(SolutionVariant? variant)
    {
        var map = Map2D.FromReader(
            _inputReader, (char v) => v.ToDigit());

        
        if (map is null)
            return Result.Fail("Unable to parse map");
        
        var trailMap = map.Filter([]);
        
        var trailHeads = map.Where(
            (MapPoint<int?> p) => p.Value == 0);

        var scoreSum = 0;
        foreach (var trailHead in trailHeads)
        {
            var trail = TraverseTrailFrom(
                map, trailHead.Location.X, trailHead.Location.Y);
                //.ToList();
                
                var trailEnds = trail.Where(p => p.Value == 9)
                    .Select(p => p.Location);
                    
                if (variant == SolutionVariant.PartOne)
                    trailEnds = trailEnds.Distinct();
                
                
                scoreSum += trailEnds.Count();;
                /*
                foreach (var point in trail)
                {
                    trailMap[point.Location.X, point.Location.Y] = point.Value;
                    foreach(var outline in trailMap.ToStringLines((int? value) => 
                                value?.DigitToChar() ?? '.' ))
                        Console.WriteLine(outline);
                    continue;
                }*/
            continue;
        }
        
        return Result.Ok(scoreSum.ToString());
    }

    private IEnumerable<MapPoint<int?>> TraverseTrailFrom(Map2D<int?> map, int x = 0, int y = 0)
    {
        Point[] movementVectors = [
            new(0,1),
            new(0,-1),
            new(1,0),
            new(-1,0)
        ];


        var currentHeight = map[x,y];
        yield return new MapPoint<int?>(currentHeight, new Point(x, y));
        
        foreach (var movement in movementVectors)
        {
            var nextX = movement.X + x;
            var nextY = movement.Y + y;

            if (!map.IsInside(nextX, nextY))
                continue;
            
            var nextStep = map[nextX, nextY];

            if (nextStep != currentHeight + 1)
                continue;
            
            foreach (var point in TraverseTrailFrom(map, nextX, nextY))
                yield return point;
        }

        yield break;
    }
}