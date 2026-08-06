using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandHandler : IRequestHandler<UpdateHandlerCommand, Result<Unit>>
{
    private readonly IAppDbContext _context;

    public UpdateHandlerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Unit>> Handle(UpdateHandlerCommand request, CancellationToken cancellationToken)
    {
        var handler = await _context.Handlers
            .FirstAsync(h => h.Id == request.Id, cancellationToken);

        if (request.Department != null)
            handler.Department = request.Department;

        if (request.MaxCases.HasValue)
            handler.MaxCases = request.MaxCases.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}