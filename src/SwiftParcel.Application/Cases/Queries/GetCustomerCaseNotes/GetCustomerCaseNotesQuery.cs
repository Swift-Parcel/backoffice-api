using FluentValidation;
using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public record GetCustomerCaseNotesQuery(string CaseNumber) : IRequest<Result<IReadOnlyList<CaseNoteDto>>>;