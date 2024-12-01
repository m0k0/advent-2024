namespace AdventOfCode.Y2K24;

public class SolverFactory
{
    private Dictionary<int, Func<TextReader, ISolver?>> _registeredSolvers = [];

    public static SolverFactory Create() => new SolverFactory();
    
    public SolverFactory Register<TSolver>(int dayNumber)
        where TSolver : ISolver
    {
        _registeredSolvers[dayNumber] = reader => 
            Activator.CreateInstance(typeof(TSolver), args: reader) as ISolver;
        
        return this;
    }

    public bool HasSolverForDay(int dayNumber)
    {
        return _registeredSolvers.ContainsKey(dayNumber);
    }

    public ISolver? GetSolver(int dayNumber, TextReader input)
    {
        if (!HasSolverForDay(dayNumber))
            return null;
        
        var solverFactory = _registeredSolvers[dayNumber];
        return solverFactory.Invoke(input);
    }
}