using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public record GetCasesQuery(
    int? CustomerId = null,
    string? CustomerEmail = null,
    string? CustomerPhone = null
) : IRequest<Result<List<CaseDto>>>;