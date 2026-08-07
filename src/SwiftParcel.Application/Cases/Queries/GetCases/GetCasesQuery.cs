using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public record GetCasesQuery(
    int? CustomerId = null,
    string? CustomerEmail = null,
    string? CustomerPhone = null
) : IRequest<Result<List<CaseDto>>>;