namespace SwiftParcel.Application.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "User does not have permission to perform this action.") 
        : base(message)
    {
    }
}