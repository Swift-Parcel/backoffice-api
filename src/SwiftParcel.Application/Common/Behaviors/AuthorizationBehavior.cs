using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Exceptions;

namespace SwiftParcel.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;

    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IAuthorizableRequest authorizableRequest)
        {
            return await next();
        }
        
        if (!await _currentUserService.IsActiveAsync(cancellationToken))
        {
            throw new ForbiddenException("Your account has been disabled."); 
        }
        
        if (authorizableRequest.RequireAuthentication && !_currentUserService.IsAuthenticated)
        {
            throw new UnauthorizedException();
        }

        var allowedRoles = authorizableRequest.AllowedRoles;

        if (allowedRoles.Count > 0)
        {
            var userRole = _currentUserService.Role;

            if (userRole == null || !allowedRoles.Contains(userRole.Value))
            {
                throw new ForbiddenException(); 
            }
        }
        
        return await next();
    }
}