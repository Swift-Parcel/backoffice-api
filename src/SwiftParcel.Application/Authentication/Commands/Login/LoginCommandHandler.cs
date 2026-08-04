using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authentication;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.Common.Models.Authentication;

namespace SwiftParcel.Application.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult?>>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    
    public LoginCommandHandler(
        IAppDbContext context, 
        IPasswordHasher passwordHasher, 
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    
    public async Task<Result<AuthenticationResult?>> 
        Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.Regions)
            .FirstOrDefaultAsync(u => u.Username == request.Username, 
                cancellationToken);

        if (user is null)
        {
            return Result<AuthenticationResult?>.Failure(
                Error.Failure("invalid_username_or_password"
                    , "Invalid username or password."));
        }
        
        var isPasswordValid = _passwordHasher
            .VerifyPassword(request.Password, user.PasswordHash);
        if(!isPasswordValid)
        {
            return Result<AuthenticationResult?>.Failure(
                Error.Failure("invalid_username_or_password"
                    , "Invalid username or password."));
        }
        
        user.LastLogin = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        
        var result = new AuthenticationResult(
            user.Id,
            user.Username,
            user.Email,
            user.Role.Name,
            user.Regions.Select(r => r.Id).ToList(),
            user.Role.CanAccessAllRegions,
            token);
        
        return Result<AuthenticationResult?>.Success(result);
    }
}