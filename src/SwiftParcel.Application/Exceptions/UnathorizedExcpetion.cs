namespace SwiftParcel.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "User is not signed in.") 
        : base(message)
    {
    }
}