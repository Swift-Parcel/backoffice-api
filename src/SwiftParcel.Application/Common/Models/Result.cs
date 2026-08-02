namespace SwiftParcel.Application.Common.Models;

public class Result<TValue>
{
    public bool IsSuccess { get; }
    public TValue? Value { get; }
    public Error? Error { get; }

    private Result(TValue value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Value = default;
        Error = error;
    }

    public static Result<TValue> Success(TValue value) => new(value);
    public static Result<TValue> Failure(Error error) => new(error);
}