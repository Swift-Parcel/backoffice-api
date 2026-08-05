using MediatR;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCases;

public class GetCasesQuery : IRequest<List<CaseDto>>
{
}