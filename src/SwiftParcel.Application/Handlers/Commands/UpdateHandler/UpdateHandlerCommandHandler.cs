using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Handlers.Commands.UpdateHandler;

public class UpdateHandlerCommandHandler : IRequestHandler<UpdateHandlerCommand, Result>
{
    private readonly IAppDbContext _context;

    public UpdateHandlerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateHandlerCommand command, CancellationToken cancellationToken)
    {
        var handler = await _context.Handlers
            .FirstAsync(h => h.Id == command.Id, cancellationToken);

        handler.UpdateHandler(
            userId: command.UserId,
            department: command.Department,
            hireDate: command.HireDate,
            maxCases: command.MaxCases,
            isActive: command.IsActive);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}