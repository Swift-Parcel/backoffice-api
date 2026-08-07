using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Application.DTO.Cases;

namespace SwiftParcel.Application.Cases.Queries.GetCaseNotes;

public record GetCaseNotesQuery(string CaseNumber) : IRequest<Result<IReadOnlyList<CaseNoteDto>>>;