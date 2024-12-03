namespace AdventOfCode.Y2K24.Solvers;

public class Day03Solver : ISolver
{
    class Command(string command)
    {
        public string CommandText { get; set; } = command;
        public List<string> Args { get; set; } = [];
        public int Location { get; set; } = -1;
    }
    
    private readonly TextReader _inputReader;
    public Day03Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }


    string? ExtractCommandText(Span<char> buffer, string[] availableCommands)
    {
        var potentialMatches = availableCommands.ToList();
 
        // walk buffer backwards
        for (var i = 1 ; i <= buffer.Length; i++)
        {
            var bufferChar = buffer[^i];
            for (var ci =0; ci < potentialMatches.Count; ci++)
            {
                var command = potentialMatches[ci];
                // match commands to buffer
                if (bufferChar != command[^i])
                {
                    potentialMatches.RemoveAt(ci); // command doesn't match
                    continue;
                }
                if (command.Length - i == 0) // match
                {
                    return command;
                }
            }

            if (potentialMatches.Count == 0)
                break;
        }
        return string.Empty;
    }

    IEnumerable<Command> ReadCommands(string[] availableCommands)
    {
        const int BUFFER_SIZE = 1024;
        char[] charBuffer = new char[BUFFER_SIZE];
        
        List<Char> commandBuffer = new();
        List<Char> argBuffer = new();
        
        var minCommandLength = availableCommands.Min(c => c.Length);
        var maxCommandLength = availableCommands.Max(c => c.Length);

        Command? currentCommand = null;
        var isInsideArgs = false;
        var isReadingArg = false;

        var readCount = _inputReader.Read(charBuffer, 0, BUFFER_SIZE);
        var totalCharCounter = 0;
        while (readCount > 0)
        {
            totalCharCounter++;
            
            for (int i = 0; i < readCount; i++)
            {
                var c = charBuffer[i];

                if (c == '(' && commandBuffer.Count > minCommandLength) // start of function args
                {
                    
                    var commandText = ExtractCommandText(
                        commandBuffer.ToArray(), 
                        availableCommands);

                    if (string.IsNullOrEmpty(commandText))
                    {
                        // unknown command
                        DropCommand();
                        continue;
                    }

                    currentCommand = new Command(commandText);
                    currentCommand.Location = totalCharCounter;
                    MoveInsideCommand();
                    
                    continue;
                }

                if (isInsideArgs && currentCommand is not null)
                {

                    if (char.IsDigit(c))
                    {
                        MoveInsideArg();

                    } else if (c == ',' && isReadingArg) // end of current function arg
                    {
                        CommitArg();
                        continue;
                    }
                    else if (c == ')' && isReadingArg) // end of function args 
                    {
                        CommitArg();
                        yield return currentCommand;
                        CommitCommand();
                        continue;
                    }
                    else // invalid char, drop command
                    {
                        DropCommand();
                        continue;
                    }
                    argBuffer.Add(c);
                }
                
                commandBuffer.Add(c);
            }



            readCount = _inputReader.Read(charBuffer, 0, BUFFER_SIZE);
        }
        
        
        void MoveInsideCommand()
        {
            isInsideArgs = true;
            commandBuffer.Clear();
        }
        void MoveInsideArg()
        {
            isReadingArg = true;
        }
        void CommitArg()
        {
            var arg = string.Join(string.Empty, argBuffer);
            currentCommand.Args.Add(arg);
            argBuffer.Clear();
        }

        void CommitCommand()
        {
            currentCommand = null;
            isInsideArgs = false;
        }
        void DropCommand()
        {
            argBuffer.Clear();
            currentCommand = null;
            isInsideArgs = false;
        }
    }
    public Result Solve(SolutionVariant? variant)
    {
        string[] availableCommands = new[]
        {
            "mul"
        };
        
        var commands = ReadCommands(availableCommands);
        var resultSum = 0;
        foreach (var command in commands)
        {
            var commandResult = 0;
            
            if (command.CommandText == "mul")
            {
                var product = 1;
                for (var i = 0; i < command.Args.Count; i++)
                {
                    if (!int.TryParse(command.Args[i], out int arg))
                        Result.Fail($"Failed to parse argument {i + 1
                            } of {command.CommandText
                            } at position {command.Location}");
                    product *= arg;
                }
                commandResult = product;
            }

            resultSum += commandResult;
        }
        
        
        return Result.Ok(resultSum.ToString());


    }

}