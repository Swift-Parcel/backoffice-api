using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQuery : IRequest<Result<List<CaseDto>>>
{
}