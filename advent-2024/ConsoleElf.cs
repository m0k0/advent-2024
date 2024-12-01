namespace AdventOfCode.Y2K24;

public static class ConsoleElf
{
    class FestiveEmoji
    {
        public const string Star = "\u2b50\ufe0f";
        public const string Tree = "\ud83c\udf84";
        public const string Santa = "\ud83c\udf85";
        public const string Sleigh = "\ud83d\udef7";   
        public const string Present = "\ud83c\udf81";
    }
    
    public struct ConsoleColorSet
    {
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
    }
    static Stack<ConsoleColorSet> _consoleColorStack = [];
    public static void SetColor(
        ConsoleColor? background,
        ConsoleColor? foreground)
    {
        var colorSet = new ConsoleColorSet();
        if (foreground.HasValue)
            colorSet.ForegroundColor = foreground.Value;
        if (background.HasValue)
            colorSet.ForegroundColor = background.Value;
        SetColor(colorSet);
    }
    public static void SetColor(ConsoleColorSet colorSet)
    {
        var currentColorSet = new ConsoleColorSet
        {
            ForegroundColor = Console.ForegroundColor,
            BackgroundColor = Console.BackgroundColor
        };
        _consoleColorStack.Push(currentColorSet);
        Console.ForegroundColor = colorSet.ForegroundColor;
        Console.BackgroundColor = colorSet.BackgroundColor;
    }

    public static void ResetColor()
    {
        var colorSet = _consoleColorStack.Pop();
        Console.ForegroundColor = colorSet.ForegroundColor;
        Console.BackgroundColor = colorSet.BackgroundColor;
        Console.WriteLine();
        Console.SetCursorPosition(0, Console.CursorTop-1);
        
    }

    public static void WriteIntro()
    {
        var pad = $"{FestiveEmoji.Star}{FestiveEmoji.Star}";
        var centre = $"{FestiveEmoji.Tree}\t{FestiveEmoji.Santa}\t{FestiveEmoji.Tree}";
        Console.WriteLine("");
        Console.WriteLine($"{pad}\tAdvent of Code 2024\t{pad}");
        Console.WriteLine($"{pad}\t{centre}\t{pad}");
        Console.WriteLine("");
    }
    
    public static void Say(string message)
    {
        Console.WriteLine($"{FestiveEmoji.Tree}> {message}");
    }
    public static void SayOk(string message)
    {
        ConsoleElf.SetColor(
            ConsoleColor.DarkGreen, 
            ConsoleColor.White);
        
        Console.WriteLine($"{FestiveEmoji.Tree}> {message}");
        
        ConsoleElf.ResetColor();
    }
    public static void SayError(string errorMessage)
    {
        
        ConsoleElf.SetColor(
            ConsoleColor.DarkRed, 
            ConsoleColor.White);
        
        ConsoleElf.Say(errorMessage);
            
        ConsoleElf.ResetColor();
    }
    public static void SayResult(FailureResult failureResult)
    {
        var indent = "";
        ConsoleElf.SetColor(
            ConsoleColor.DarkRed, 
            ConsoleColor.White);
    
        var currentResult = failureResult;
        
        ConsoleElf.SayError("Oh no! We've got a problem!");
        
        while (currentResult is not null)
        {
            Console.WriteLine($"{indent}{currentResult.FailureReason}");
            indent += "  ";
            currentResult = currentResult.InnerResult;
        }
        ConsoleElf.ResetColor();
    }

    public static void SayResult<T>(SuccessResult<T> successResult)
    {
        ConsoleElf.SayOk($"All done! {FestiveEmoji.Present}");
        Console.WriteLine(successResult);
    }

    public static int GetIntInput(string? prompt = null)
    {
        int number = 0;
        if (!string.IsNullOrEmpty(prompt))
        {
            ConsoleElf.Say(prompt);
        }
        var input = Console.ReadLine();
        while (!int.TryParse(input, out number))
        {
            ConsoleElf.SayError("That's not a number!");
            input = Console.ReadLine();
        }
        return number;
    }

    public static TOption GetSelectionInput<TOption>(string prompt, IEnumerable<TOption> options)
    {
        var optionsDictionary = new Dictionary<string, TOption>();
        var optionIndex = 0;
        foreach (var option in options)
        {
            optionsDictionary.Add(option?.ToString() ?? optionIndex.ToString(), option);
            optionIndex++;
        }
        return GetSelectionInput(prompt, optionsDictionary);
    }
    public static TOption GetSelectionInput<TOption>(string prompt, Dictionary<string,TOption> options)
    {
        ConsoleElf.Say(prompt);
        
        var displayOptions = options.Keys.ToArray();
        var optionIndex = 1;
        foreach (var option in displayOptions)
        {
            Console.WriteLine($"\t[{optionIndex}]: {option?.ToString()}");
            optionIndex++;
        }

        var selection = GetIntInput();
        while (selection < 1 || selection > displayOptions.Length)
        {
            ConsoleElf.SayError("That's not one of the options!");
            selection = GetIntInput();
        }

        return options[displayOptions[selection - 1]];
    }

    public static SolverOptions? AskSolverOptions()
    {
        
        var options = new SolverOptions();
// ------ day ------
        options.Day = ConsoleElf
            .GetIntInput("Which day are we solving for?");

        var zeroPaddedDay = options.Day < 10 ? $"0{options.Day}" : $"{options.Day}";
        var inputPath = $"./Input/Day{zeroPaddedDay}";
        
        ConsoleElf.SayOk($"Ok! So that's day {options.Day}");

// ------ input file ------
        if (!Directory.Exists(inputPath))
        {
            ConsoleElf.SayError("Snap! We don't have any input for this day yet!");
            return null;
        }
        var inputFiles = Directory.EnumerateFiles(
                inputPath, "*.txt")
            .ToDictionary(
                f => Path.GetFileName(f) ?? f,
                f => f);
        if (inputFiles.Count == 0)
        {
            ConsoleElf.SayError("Snap! We don't have any input for this day yet!");
            return null;
        }

        options.InputFilePath = ConsoleElf.GetSelectionInput(
            "Which puzzle input are we using?",
            inputFiles
        );
        ConsoleElf.SayOk($"" +
                         $"Alrighty! we'll be using input {Path.GetFileName(options.InputFilePath)}");

// ------ variant ------
        var variants = Enum.GetValues<SolutionVariant>()
            .ToDictionary(
                e => e.ToString(), 
                e => e);  
        var variantName = ConsoleElf.GetSelectionInput(
            "Which variant do we need?",
            variants
        );
        ConsoleElf.SayOk($"Sure! Solution for '{options.Variant}' coming up!");

        return options;
    }
}