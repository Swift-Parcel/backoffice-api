namespace SwiftParcel.Domain.Shared;

public record Error(string Message, ErrorType Type)
{
    public static Error NotFound(string message) 
        => new(message, ErrorType.NotFound);
    
    public static Error Validation(string message) 
        => new(message, ErrorType.Validation);
    
    public static Error Conflict(string message) 
        => new(message, ErrorType.Conflict);
    
    public static Error Failure(string message) 
        => new(message, ErrorType.Failure);

    public static Error Forbidden(string message) 
        => new(message, ErrorType.Forbidden);
}

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Forbidden = 4
}