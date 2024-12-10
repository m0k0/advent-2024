namespace AdventOfCode.Y2K24.Utilities;

public static class CharExtensions
{
    public static int? ToDigit(this char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        return null;
    }
    public static char? DigitToChar(this int digit)
    {
        if (digit >= 0 && digit <= 9)
            return (char)(digit + '0');
        return null;
    }
}