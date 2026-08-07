using FluentValidation;
using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCaseStatus;

public record GetCaseStatusQuery(string CaseNumber) : IRequest<Result<CaseStatusResponse>>;