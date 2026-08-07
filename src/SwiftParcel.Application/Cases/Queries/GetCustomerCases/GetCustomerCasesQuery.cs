using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCases;

public record GetCustomerCasesQuery(string CustomerEmail) : IRequest<Result<CustomerCasesResponse>>;