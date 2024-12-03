using System.Reflection;

namespace AdventOfCode.Y2K24;

public class SolverFactory
{
    private Dictionary<int, Func<TextReader, ISolver?>> _registeredSolvers = [];

    public static SolverFactory Create() => new SolverFactory();
    
    public SolverFactory Register<TSolver>(int dayNumber)
        where TSolver : ISolver
    {
        Register(typeof(TSolver),dayNumber);
        return this;
    }
    public SolverFactory RegisterSolvers()
    {
        var solvers = GetSolverTypes();
        foreach (var solver in solvers)
        {
            var solverForAttribute = solver.GetCustomAttribute<SolverForAttribute>();
            if (solverForAttribute is null)
                continue;
            
            Register(solver, solverForAttribute.Day);
        }

        return this;
    }

    private void Register(Type solverType, int dayNumber)
    {
        _registeredSolvers[dayNumber] = reader => 
            Activator.CreateInstance(solverType, args: reader) as ISolver;
    }

    public bool HasSolverForDay(int dayNumber)
    {
        return _registeredSolvers.ContainsKey(dayNumber);
    }

    public IEnumerable<Type> GetSolverTypes()
    {
        return Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                typeof(ISolver).IsAssignableFrom(t));
    }

   
    public ISolver? GetSolver(int dayNumber, TextReader input)
    {
        if (!HasSolverForDay(dayNumber))
            return null;
        
        var solverFactory = _registeredSolvers[dayNumber];
        return solverFactory.Invoke(input);
    }
}