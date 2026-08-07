using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public record GetCustomerCasesQuery(string CustomerEmail) : IRequest<Result<CustomerCasesResponse>>;