using MediatR;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Entities;

namespace SwiftParcel.Application.Handlers.Commands.CreateHandler;

public class CreateHandlerCommandHandler : IRequestHandler<CreateHandlerCommand, Result<int>>
{
    private readonly IHandlerRepository _handlerRepository;

    public CreateHandlerCommandHandler(IHandlerRepository handlerRepository)
    {
        _handlerRepository = handlerRepository;
    }

    public async Task<Result<int>> Handle(CreateHandlerCommand request, CancellationToken cancellationToken)
    {
        var newHandler = new Handler
        (
            userId: request.UserId,
            department: request.Department,
            maxCases: request.MaxCases,
            hireDate: request.HireDate ?? DateTime.UtcNow
        );

        await _handlerRepository.AddAsync(newHandler, cancellationToken);

        return Result<int>.Success(newHandler.Id);
    }
}