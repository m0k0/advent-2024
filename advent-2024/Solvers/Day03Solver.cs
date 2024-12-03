namespace AdventOfCode.Y2K24.Solvers;

public class Day03Solver : ISolver
{
    class Command(string command)
    {
        public string CommandText { get; set; } = command;
        public List<string> Args { get; set; } = [];
    }
    
    private readonly TextReader _inputReader;
    public Day03Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }


    string? ExtractCommand(string buffer, string[] availableCommands)
    {
        var potentialMatches = availableCommands.ToList();
 
        // walk buffer backwards
        for (var i = buffer.Length; i > 0; i--)
        {
            var bufferChar = buffer[i];
            for (var ci =0; ci < potentialMatches.Count; ci++)
            {
                var command = potentialMatches[ci];
                // match commands to buffer
                if (bufferChar != command[^i])
                {
                    potentialMatches.RemoveAt(ci); // command doesn't match
                    continue;
                }
                if (ci == 0) // match
                {
                    return command;
                }
            }

            if (potentialMatches.Count == 0)
                break;
        }
        return String.Empty;
    }
    public Result Solve(SolutionVariant? variant)
    {
        const int BUFFER_SIZE = 1024;
        char[] charBuffer = new char[BUFFER_SIZE];
        List<Command> commands = new();
        List<Char> commandBuffer = new();
        List<Char> argBuffer = new();

        string[] availableCommands = new[]
        {
            "mul"
        };

        Command? currentCommand = null;
        var isInsideArgs = false;
        var isReadingArg = false;
        
        var readCount = _inputReader.Read(charBuffer, 0, BUFFER_SIZE);
        while (readCount > -1)
        {
            
            for (int i = 0; i < readCount; i++)
            {
                var c = charBuffer[i];

                
                
                if (c == '(') // start of function args
                {
                    var commandText = commandBuffer.ToString();
                    if (string.IsNullOrEmpty(commandText))
                        continue; // empty command
                    
                    
                    commandText = ExtractCommand(commandText, availableCommands);
                    
                    currentCommand = new Command(commandText);
                    isInsideArgs = true;
                    commandBuffer.Clear();
                    continue;
                }

                if (isInsideArgs && currentCommand is not null)
                {
                    if (char.IsDigit(c))
                    {
                        argBuffer.Add(c);
                        isReadingArg = true;
                    } else if (c == ',' && isReadingArg) // end of current function arg
                    {
                        currentCommand.Args.Add(argBuffer.ToString());
                        argBuffer.Clear();
                    }
                    else if (c == ')' && isReadingArg) // end of function args 
                    {
                        currentCommand.Args.Add(argBuffer.ToString());
                        argBuffer.Clear();
                        commands.Add(currentCommand);
                        currentCommand = null;
                        isInsideArgs = false;
                    }
                    else // invalid char, drop command
                    {
                        argBuffer.Clear();
                        currentCommand = null;
                        isInsideArgs = false;
                    }

                }
                
                
            }



            readCount = _inputReader.Read(charBuffer, 0, BUFFER_SIZE);
            continue;
            var buffer = charBuffer.ToString();

            if (string.IsNullOrEmpty(buffer))
            {
                continue;
            }

            var foundCommand = "";
            foreach (var command in availableCommands)
            {
                var commandIndex = buffer.IndexOf(command, StringComparison.Ordinal);
                if (commandIndex > -1)
                {
                    foundCommand = command;
                }
            }
            
            readCount = _inputReader.Read(charBuffer, 0, BUFFER_SIZE);
        } 
        
        
        return Result.Fail("No solution yet");
    }
}