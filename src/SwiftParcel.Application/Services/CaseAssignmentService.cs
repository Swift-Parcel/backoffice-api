using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SwiftParcel.Application.Cases.Dtos;
using SwiftParcel.Application.Common.Interfaces;
using SwiftParcel.Application.Common.Interfaces.Repositories;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Enums;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Services;

public class CaseAssignmentService : ICaseAssignmentService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IHandlerRepository _handlerRepository;

    public CaseAssignmentService(
        IAppDbContext dbContext,
        ICurrentUserService currentUser,
        IMapper mapper,
        IHandlerRepository handlerRepository)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _mapper = mapper;
        _handlerRepository = handlerRepository;
    }

    public async Task<Result<CaseSummaryDto>> AssignCaseAsync(string caseNumber, int handlerId,
        CancellationToken cancellationToken = default)
    {
        var @case = await _dbContext.Cases.FirstOrDefaultAsync(c => c.CaseNumber == caseNumber, cancellationToken);
        if (@case is null)
            return Result<CaseSummaryDto>.Failure(Error.NotFound("Case.NotFound",
                $"Case with case number{caseNumber} is not found."));

        var handler = await _handlerRepository.GetWithLockAndCasesAsync(handlerId, cancellationToken);

        if (handler is null)
            return Result<CaseSummaryDto>.Failure(Error.NotFound("Handler.NotFound",
                $"Handler with id {handlerId} is not found."));

        if (_currentUser.Role != UserRole.Admin && !_currentUser.HasAccessToRegion(@case.RegionId))
        {
            return Result<CaseSummaryDto>.Failure(Error.Failure(
                "code which we dont use",
                "You do not have permission to reassign cases outside your regions."));
        }


        if (!handler.CanAssignCase())
        {
            return Result<CaseSummaryDto>.Failure(Error.Conflict(
                "Handler.CapacityExceeded",
                $"Handler '{handler.Id}' has reached maximum capacity ({handler.MaxCases})."));
        }

        var requiredDepartment = @case.GetRequiredDepartment();

        if (handler.Department != requiredDepartment)
        {
            return Result<CaseSummaryDto>.Failure(Error.Validation(
                "Handler.InvalidDepartment",
                $"This case requires the '{requiredDepartment}' department, but handler is in '{handler.Department}'."));
        }

        handler.AssignCase(@case);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<CaseSummaryDto>(@case);

        return Result<CaseSummaryDto>.Success(dto);
    }
}