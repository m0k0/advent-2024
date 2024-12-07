using System.Drawing;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 6)]
public class Day06Solver : ISolver
{
    class Guard(int xPosition, int yPosition)
    {

        public Point Position { get; set; } = new(xPosition, yPosition);
        public int DirectionDegrees = 0;
    }
    private readonly TextReader _inputReader;
    private readonly List<Guard> _guards = []; 
    private bool[][] _obstacleMap;
    private Size _mapSize = new();

    private const char CHAR_GUARD_UP = '^';
    private const char CHAR_GUARD_DOWN = 'v';
    private const char CHAR_GUARD_LEFT = '<';
    private const char CHAR_GUARD_RIGHT = '>';
    private const char CHAR_OBSTACLE = '#';
    
    public Day06Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }

    void ReadInput()
    {
        List<bool[]> obstacleMap = [];
        
        var line = _inputReader.ReadLine();
        var mapY = 0;
        while (!string.IsNullOrWhiteSpace(line))
        {
            var lineChars = line.ToCharArray();
            bool[] mapLine = new bool[lineChars.Length];
            obstacleMap.Add(mapLine);
            
            for (var mapX = 0; mapX < lineChars.Length; mapX++)
            {
                var currentChar = lineChars[mapX];
                switch (currentChar)
                {
                    case CHAR_GUARD_UP:
                        _guards.Add(new Guard(mapX, mapY)
                        {
                            DirectionDegrees = 0
                        });
                        break;
                    case CHAR_GUARD_DOWN:
                        _guards.Add(new Guard(mapX, mapY)
                        {
                            DirectionDegrees = 180
                        });
                        break;
                    case CHAR_GUARD_LEFT:
                        _guards.Add(new Guard(mapX, mapY)
                        {
                            DirectionDegrees = 270
                        });
                        break;
                    case CHAR_GUARD_RIGHT:
                        _guards.Add(new Guard(mapX, mapY)
                        {
                            DirectionDegrees = 90
                        });
                        break;
                    case CHAR_OBSTACLE:
                        mapLine[mapX] = true;
                        break;
                }
            }
            line = _inputReader.ReadLine();
            mapY++;
        }
        
        _obstacleMap = obstacleMap.ToArray();
        
        _mapSize = new(
            _obstacleMap.Length > 0 ? _obstacleMap[0].Length : 0,
            _obstacleMap.Length);
    }

    public Result Solve(SolutionVariant? variant)
    {
        ReadInput();

        if (_guards.Count == 0)
        {
            return Result.Fail("No guards found");
        }

        if (_obstacleMap.Length == 0)
        {
            return Result.Fail("No map loaded");
        }
        
        var trailMap =MoveGuards();

        var trailCount = trailMap.Sum(y => y.Count(x => x) );

        return Result.Ok(trailCount.ToString());
    }

    private bool[][] MoveGuards()
    {
        bool[][] trailMap = new bool[_obstacleMap.Length][];
        for (var i = 0; i < trailMap.Length; i++)
        {
            trailMap[i] = new bool[_obstacleMap[i].Length];
        }
        
        foreach (var guard in _guards)
        {
            var guardIsInside = true;
            while (guardIsInside)
            {
                // mark trail
                trailMap[guard.Position.Y][guard.Position.X] = true;
                
                var movementVector = GetMovementVector(guard.DirectionDegrees);
                
                var newPosition = new Point(
                    guard.Position.X + movementVector.X,
                    guard.Position.Y + movementVector.Y);
                
                if (newPosition.X < 0 || newPosition.X >= _mapSize.Width ||
                    newPosition.Y < 0 || newPosition.Y >= _mapSize.Height)
                {
                    // guard has left
                    guardIsInside = false;
                    continue;
                }

                var encounteredObstacle = _obstacleMap[newPosition.Y][newPosition.X];
                if (encounteredObstacle)
                {
                    guard.DirectionDegrees += 90;
                    continue;
                }
                
                // move
                guard.Position = newPosition;
            }
            
            
        }

        return trailMap;
    }

    Point GetMovementVector(int rotationDegrees)
    {
        var rotationRads = rotationDegrees * Math.PI / 180;

        double moveX = Math.Sin(rotationRads);
        double moveY = -Math.Cos(rotationRads);

        moveX = Math.Round(moveX, 0);
        moveY = Math.Round(moveY, 0);
        
        return new ((int)moveX,(int) moveY);
    }
}