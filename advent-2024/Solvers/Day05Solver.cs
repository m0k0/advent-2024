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

    class DependencyTreeNode(int value)
    {
        public int Value { get; } = value;
        public DependencyTreeNode? Parent { get; set; }
        public DependencyTreeNode? Left { get; set; }
        public DependencyTreeNode? Right { get; set; }
    }

    private readonly Dictionary<int, RuleSet> _ruleSets = [];
    private readonly List<int[]> _updateSequences = [];
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
            
            _updateSequences.Add(updateSequence.ToArray());
            
            line = _inputReader.ReadLine();
        }
        
        return Result.Ok(true);
    }

    private DependencyTreeNode? GetDependencyTree(IEnumerable<int> values)
    {

        DependencyTreeNode? topNode = null;
        
        foreach (var updatePartNumber in values)
        {
            if (topNode is null)
            {
                topNode = new DependencyTreeNode(updatePartNumber);
                continue;
            }
            var ruleSet = _ruleSets[updatePartNumber];
            var node = new DependencyTreeNode(updatePartNumber);
            
            // traverse tree
            DependencyTreeNode lastNode = topNode;
            while (true)
            {
                if (ruleSet.MustBeBefore.Contains(lastNode.Value))
                {
                    if (lastNode.Left is not null)
                    {
                        // dive
                        lastNode = lastNode.Left;
                        continue;
                    }

                    lastNode.Left = node;
                    break;

                }
                else if (ruleSet.MustBeAfter.Contains(lastNode.Value))
                {
                    if (lastNode.Right is not null)
                    {
                        // dive
                        lastNode = lastNode.Right;
                        continue;
                    }
                    lastNode.Right = node;
                    break;
                }
                
                break; // not listed, so ignore
            }

        }
        
        return topNode;
    
    }
    
    
    public Result Solve(SolutionVariant? variant)
    {
        var parseResult = ParseInput();
        if (parseResult is FailureResult failureResult)
        {
            return Result.Fail("Failed to parse input", failureResult);
        }

        var middleSum = 0;
        for (var i = 0; i < _updateSequences.Count; i++)
        {
            var updateSequence = _updateSequences[i];
            
            var isValid = TestUpdateSequence(updateSequence);
            if (variant == SolutionVariant.PartOne)
            {
                if (!isValid)
                    continue;
                
            } else if (variant == SolutionVariant.PartTwo)
            {
                if (isValid)
                    continue;
                updateSequence = SortSequence(updateSequence);
            } else
            {
                continue;
            }
            
            var middlePart = GetMiddlePart(updateSequence);  
            middleSum += middlePart;
        }
        
        return Result.Ok(middleSum.ToString());
    }

    private int GetMiddlePart(int[] updateSequence)
    {
        var middleIndex = (int)Math.Floor(updateSequence.Length / 2d);
        return updateSequence[middleIndex];
    }

    private int[] SortSequence(int[] updateSequence)
    {
        var topNode = GetDependencyTree(updateSequence);
        
        // collapse tree
        List<int> GetNodeResult(DependencyTreeNode? node)
        {
            var nodeResult = new List<int>();
            if (node is null)
                return nodeResult;

            if (node.Left is not null)
            {
                nodeResult.AddRange(GetNodeResult(node.Left));
            }
            nodeResult.Add(node.Value);
            if (node.Right is not null)
            {
                nodeResult.AddRange(GetNodeResult(node.Right));
            }
            return nodeResult;
        }
        var rankedResult = GetNodeResult(topNode);
                
        return rankedResult.ToArray();
    }
    private bool TestUpdateSequence(int[] updateSequence)
    {
        for(var i = 0; i < updateSequence.Length; i++)
        {
            var updatePart = updateSequence[i];
                
            if (!_ruleSets.TryGetValue(updatePart, out var ruleSet))
                continue; // no rules, skip
                
            var obeysRuleSet = TestRuleSet(i, updateSequence, ruleSet);
            if (!obeysRuleSet)
                return false;
        }
        return true;
    }

    private static bool TestRuleSet(int subjectIndex, int[] updateSequence, RuleSet ruleSet)
    {
        // test numbers appearing before
        for (var i2 = subjectIndex - 0; i2 >= 0; i2--)
        {
            var comparisonUpdatePart = updateSequence[i2];
            if (ruleSet.MustBeBefore.Contains(comparisonUpdatePart))
            {
                return false;
            }
        }

        // test numbers appearing after
        for (var i2 = subjectIndex + 1; i2 < updateSequence.Length; i2++)
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