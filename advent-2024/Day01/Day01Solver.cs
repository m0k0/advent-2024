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

        public Result ParseInput()
        {
            List<int> leftList = [];
            List<int> rightList = [];
            
            var lineNumber = 0;
            var line = _inputReader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                var lineElements = line.Split(' ', 
                    StringSplitOptions.TrimEntries |
                    StringSplitOptions.RemoveEmptyEntries);
                
                if (lineElements.Length != 2)
                {
                    return Result.Fail(
                        $"Wrong number of elements on line {lineNumber}");
                }
                if (!int.TryParse(lineElements[0], out int leftNumber))
                {
                    return Result.Fail(
                        $"Invalid integer in left list on line {lineNumber}");
                }
                if (!int.TryParse(lineElements[1], out int rightNumber))
                {
                    return Result.Fail(
                        $"Invalid integer in right list on line {lineNumber}");
                }
                
                leftList.Add(leftNumber);
                rightList.Add(rightNumber);
                
                line = _inputReader.ReadLine();
                lineNumber++;
            }

            return Result.Ok(
                (leftList, rightList));
        }
        
        public Result Solve(SolutionVariant? solutionVariant = 0)
        {
            var input = ParseInput();

            if (input is FailureResult failureResult )
            {
                return Result.Fail("Failed to parse input.", failureResult);
            }
            if (input is not SuccessResult<(List<int>, List<int>)> successResult)
            {
                return Result.Fail("Invalid result type");
            }
            
            var (leftList, rightList) = successResult.Value;


            var result = SolvePart1(leftList, rightList);

            return result;
        }

        Result SolvePart1(List<int> leftList, List<int> rightList)
        {
            
            leftList.Sort();
            rightList.Sort();

            if (leftList.Count != rightList.Count)
                return Result.Fail(
                    $"Left ({leftList.Count}) and right ({rightList.Count}) list count mismatch");

            int distanceSum = 0;
            for (int i = 0; i < leftList.Count; i++)
            {
                var distance = Math.Abs(leftList[i] - rightList[i]);
                distanceSum += distance;
            }
            
            return Result.Ok(distanceSum.ToString());
        }
    }
}