namespace SwiftParcel.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TValue> : Result
{
    public TValue? Value { get; }

    private Result(TValue value) : base(true, null) => Value = value;
    private Result(Error error) : base(false, error) => Value = default;

    public static Result<TValue> Success(TValue value) => new(value);
    public new static Result<TValue> Failure(Error error) => new(error);
}