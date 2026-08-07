using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public record GetCaseNotesQuery(string CaseNumber) : IRequest<Result<IReadOnlyList<CaseNoteDto>>>;