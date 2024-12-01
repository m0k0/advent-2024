namespace AdventOfCode.Y2K24;

public static class ConsoleElf
{
    public struct ConsoleColorSet
    {
        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
    }
    static Stack<ConsoleColorSet> consoleColorStack = [];

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
        consoleColorStack.Push(currentColorSet);
        Console.ForegroundColor = colorSet.ForegroundColor;
        Console.BackgroundColor = colorSet.BackgroundColor;
    }

    public static void ResetColor()
    {
        var colorSet = consoleColorStack.Pop();
        Console.ForegroundColor = colorSet.ForegroundColor;
        Console.BackgroundColor = colorSet.BackgroundColor;
    }

    public static void WriteIntro()
    {
        const string star = "\u2b50\ufe0f";
        const string tree = "\ud83c\udf84";
        const string santa = "\ud83c\udf85"; 
        Console.WriteLine($"{star}{star}\tAdvent of Code 2024\t{star}{star}");
        Console.WriteLine($"{star}{star}\t{tree}\t{santa}\t{tree}\t{star}{star}");
    }
    
    public static void PrintError(string errorMessage)
    {
        ConsoleElf.SetColor(
            ConsoleColor.DarkRed, 
            ConsoleColor.White);
        
        Console.WriteLine(errorMessage);
            
        ConsoleElf.ResetColor();
    }
    public static void PrintResult(FailureResult failureResult)
    {
        var indent = "";
        ConsoleElf.SetColor(
            ConsoleColor.DarkRed, 
            ConsoleColor.White);
    
        var currentResult = failureResult;
        while (currentResult is not null)
        {
            Console.WriteLine($"{indent}{currentResult.FailureReason}");
            indent += "  ";
            currentResult = currentResult.InnerResult;
        }
        ConsoleElf.ResetColor();
    }

    public static void PrintResult<T>(SuccessResult<T> successResult)
    {
        ConsoleElf.SetColor(
            ConsoleColor.DarkGreen, 
            ConsoleColor.White);
        Console.WriteLine("Complete!");
        ConsoleElf.ResetColor();
        Console.WriteLine(successResult);
    }
}