namespace AdventOfCode.Y2K24.Solvers;

[SolverFor(day: 5)]
public class Day05Solver: ISolver
{
    class RuleSet(int subject)
    {
        public int Subject { get; } = subject;
        public List<int> MustBeBefore { get; } = [];
        public List<int> MustBeAfter { get; } = [];
    }

    private readonly Dictionary<int, RuleSet> _ruleSets = [];
    private readonly List<List<int>> _updateSequences = [];
    private readonly TextReader _inputReader;
    
    public Day05Solver(TextReader inputReader)
    {
        _inputReader = inputReader;
    }

    Result ParseInput()
    {
        const char RULE_SEPARATOR = '|';
        const char UPDATE_SEPARATOR = ',';

        var lineCounter = 0;
        var line = _inputReader.ReadLine();
        
        // read rules
        while (!string.IsNullOrWhiteSpace(line))
        {
            lineCounter++;
            
            
            if (!line.Contains(RULE_SEPARATOR))
                return Result.Fail($"Invalid rule '{line}' on line {lineCounter}");
            
            var ruleParts = line.Split('|');
            if (ruleParts.Length != 2)
                return Result.Fail($"Invalid rule '{line}' on line {lineCounter}");

            if (!int.TryParse(ruleParts[0], out var leftNumber))
                return Result.Fail($"Failed to parse left number for rule '{line}' on line {lineCounter}"); 
            if (!int.TryParse(ruleParts[1], out var rightNumber))
                return Result.Fail($"Failed to parse right number for rule '{line}' on line {lineCounter}");
            
            if (!_ruleSets.ContainsKey(leftNumber))
                _ruleSets.Add(leftNumber, new RuleSet(leftNumber));
            
            if (!_ruleSets.ContainsKey(rightNumber))
                _ruleSets.Add(rightNumber, new RuleSet(rightNumber));
            
            var leftRuleSet = _ruleSets[leftNumber];
            leftRuleSet.MustBeBefore.Add(rightNumber);
            
            var rightRuleSet = _ruleSets[rightNumber];
            rightRuleSet.MustBeAfter.Add(leftNumber);
            
            line = _inputReader.ReadLine();
        }
        
        line = _inputReader.ReadLine();
        // read updates
        while (!string.IsNullOrWhiteSpace(line))
        {
            lineCounter++;

            var updateParts = line.Split(UPDATE_SEPARATOR);
            var updateSequence = new List<int>();
            foreach (var updatePart in updateParts)
            {
                if (!int.TryParse(updatePart, out var updatePartNumber))
                    return Result.Fail($"Failed to parse update '{updatePart}' on line {lineCounter}");
                updateSequence.Add(updatePartNumber);
            }
            
            _updateSequences.Add(updateSequence);
            
            line = _inputReader.ReadLine();
        }
        
        return Result.Ok(true);
    }
    public Result Solve(SolutionVariant? variant)
    {
        var parseResult = ParseInput();
        if (parseResult is FailureResult failureResult)
        {
            return Result.Fail("Failed to parse input", failureResult);
        }

        var middleSum = 0;
        foreach (var updateSequence in _updateSequences)
        {
            var isValid = TestUpdateSequence(updateSequence);
            if (!isValid)
                continue;
            
            var middlePart = GetMiddlePart(updateSequence); 
            middleSum += middlePart;
        }
        
        return Result.Ok(middleSum.ToString());
    }

    private int GetMiddlePart(List<int> updateSequence)
    {
        var middleIndex = (int)Math.Floor(updateSequence.Count / 2d);
        return updateSequence[middleIndex];
    }
    private bool TestUpdateSequence(List<int> updateSequence)
    {
        for(var i = 0; i < updateSequence.Count; i++)
        {
            var updatePart = updateSequence[i];
                
            if (!_ruleSets.TryGetValue(updatePart, out var ruleSet))
                continue; // no rules, skip
                
            var obeysRuleSet = TestRuleSet(updatePart, updateSequence, ruleSet);
            if (!obeysRuleSet)
                return false;
        }
        return true;
    }

    private static bool TestRuleSet(int subject, List<int> updateSequence, RuleSet ruleSet)
    {
        var i = updateSequence.IndexOf(subject);
        // test numbers appearing before
        for (var i2 = i - 0; i2 >= 0; i2--)
        {
            var comparisonUpdatePart = updateSequence[i2];
            if (ruleSet.MustBeBefore.Contains(comparisonUpdatePart))
            {
                return false;
            }
        }

        // test numbers appearing after
        for (var i2 = i + 1; i2 < updateSequence.Count; i2++)
        {
            var comparisonUpdatePart = updateSequence[i2];
            if (ruleSet.MustBeAfter.Contains(comparisonUpdatePart))
            {
                return false;
            }
        }

        return true;
    }
}