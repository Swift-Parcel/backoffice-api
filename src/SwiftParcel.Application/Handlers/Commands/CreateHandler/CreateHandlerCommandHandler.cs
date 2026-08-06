using MediatR;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandHandler : IRequestHandler<CreateHandlerCommand, Result<int>>
{
    private readonly IAppDbContext _context;

    public CreateHandlerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(CreateHandlerCommand request, CancellationToken cancellationToken)
    {
        var newHandler = new Handler
        {
            UserId = request.UserId,
            Department = request.Department,
            MaxCases = request.MaxCases,
            HireDate = request.HireDate ?? DateTime.UtcNow,
            IsActive = true
        };

        _context.Handlers.Add(newHandler);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(newHandler.Id);
    }
}