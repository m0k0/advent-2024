using System.Drawing;

namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 6)]
public class Day06Solver : ISolver
{
    class Guard(int xPosition, int yPosition)
    {

        public Point Position { get; set; } = new(xPosition, yPosition);
        public int DirectionDegrees = 0;
        public List<Guard> PastSelves { get; } = [];

        public override bool Equals(object? obj)
        {
            if (obj is not Guard other)
                return false;
            
            return Position.Equals(other.Position) && 
                   DirectionDegrees % 360 == other.DirectionDegrees % 360;
        }
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

        var result = 0;
        
        foreach (var guard in _guards)
        {
            var status = MoveGuard(guard, _obstacleMap);
            
            var visitedPositions = guard.PastSelves
                .Select(g => g.Position)
                .Distinct();
            
            if (variant == SolutionVariant.PartOne)
            {
                result += visitedPositions.Count();
                
            } else if (variant == SolutionVariant.PartTwo)
            {
                var startingSelf = guard.PastSelves.FirstOrDefault();
                if (startingSelf is null)
                    continue; // guard hasn't moved


                List<Task<bool>> scenarioTasks = [];
                var scenariosExecuted = 0;
                foreach (var position in visitedPositions.Skip(1))
                {
                    var scenarioTask = Task.Run(() =>
                    {
                        bool[][] parallelObstacleMap = CopyMap(_obstacleMap);
                        parallelObstacleMap[position.Y][position.X] = true;

                        var parallelGuard = CloneGuard(startingSelf);
                        var parallelGuardStatus = MoveGuard(parallelGuard, parallelObstacleMap);

                        scenariosExecuted++;
                        return parallelGuardStatus == MovementStatus.GuardIsStuck;

                    });
                    scenarioTasks.Add(scenarioTask);
                }

                var completionTask = Task.WhenAll(scenarioTasks.ToArray());
                result += completionTask.Result.Count(r=> r);

            }
        }
    

        return Result.Ok(result.ToString());
    }

    enum MovementStatus
    {
        Moving,
        GuardHasLeft,
        GuardIsStuck
    }
    private MovementStatus MoveGuard(Guard guard, bool[][] obstacleMap)
    {
        var status = MovementStatus.Moving;
        
    

        while (status == MovementStatus.Moving)
        {
            // record history
            guard.PastSelves.Add(
                CloneGuard(guard));
           
            var movementVector = GetMovementVector(guard.DirectionDegrees);
            
            var newPosition = new Point(
                guard.Position.X + movementVector.X,
                guard.Position.Y + movementVector.Y);
            
            if (!IsInsideMap(newPosition))
            {
                // guard has left
                status = MovementStatus.GuardHasLeft;
                continue;
            }

            var encounteredObstacle = obstacleMap[newPosition.Y][newPosition.X];
            if (encounteredObstacle)
            {
                guard.DirectionDegrees += 90;
                continue;
            }
            
            // move
            guard.Position = newPosition;
                            
            if (HasBeenHereBefore(guard))
            {
                status = MovementStatus.GuardIsStuck;
            }
        }

        return status;
    }

    Guard CloneGuard(Guard guard)
    {
        return new Guard(guard.Position.X, guard.Position.Y)
        {
            DirectionDegrees = guard.DirectionDegrees,
        };
    }

    bool IsInsideMap(Point position)
    {
        return !(position.X < 0 || position.X >= _mapSize.Width ||
                 position.Y < 0 || position.Y >= _mapSize.Height);
    }
    bool HasBeenHereBefore(Guard guard)
    {
        foreach (var pastSelf in guard.PastSelves)
        {
            if (pastSelf.Equals(guard))
                return true;
        }
        return false;
    }

    bool[][] CopyMap(bool[][] map)
    {
        var copy = new bool[map.Length][];
        for (var i = 0; i < map.Length; i++)
        {
            copy[i] = new bool[map[i].Length];
            map[i].CopyTo(copy[i],0);
        }
        return copy;
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