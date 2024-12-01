namespace AdventOfCode.Y2K24.Day01
{
    public class Day01Solver : ISolver
    {
        private readonly TextReader _inputReader;
        public Day01Solver(
            TextReader inputReader
        ){
            _inputReader = inputReader;
        }
        
        public string Solve()
        {
            return _inputReader.ReadToEnd();
        }
    }
}