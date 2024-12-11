namespace AdventOfCode.Y2K24;

public class SuccessResult: Result
{
    public SuccessResult()
    {
        IsSuccess = true;
    }
}
public class SuccessResult<T> : Result
{
    public T? Value { get; set; }

    public SuccessResult()
    {
        IsSuccess = true;
    }
    public override string ToString()
    {
        return this.Value?.ToString() ?? string.Empty;
    }
}

public class FailureResult : Result
{
    public string? FailureReason { get; set; }
    public FailureResult? InnerResult { get; set; }

    public FailureResult()
    {
        IsSuccess = false;
    }
    public override string ToString()
    {
        return this.FailureReason?.ToString() ?? string.Empty;
    }
}
public class Result
{
    public bool IsSuccess { get; set; }
    
    public static FailureResult Fail(
        string reason, 
        FailureResult? innerResult = null)
    {
        return new FailureResult()
        {
            FailureReason = reason,
            InnerResult = innerResult
        };
    }

    public static SuccessResult Ok()
    {
        return new SuccessResult();
    }

    public static SuccessResult<TResult> Ok<TResult>(
        TResult value)
    {
        return new SuccessResult<TResult>()
        {
            Value = value
        };
    }
    
}