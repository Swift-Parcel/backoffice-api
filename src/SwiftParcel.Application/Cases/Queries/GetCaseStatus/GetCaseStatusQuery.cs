using FluentValidation;
using MediatR;
using SwiftParcel.Application.Common.Interfaces.Authorization;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;
using SwiftParcel.Domain.Enums;

namespace SwiftParcel.Application.Cases.Queries.GetCaseStatus;

public record GetCaseStatusQuery(string CaseNumber) : IRequest<Result<CaseStatusResponse>>
{
};