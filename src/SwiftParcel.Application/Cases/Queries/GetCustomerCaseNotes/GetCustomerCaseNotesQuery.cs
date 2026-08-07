using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Queries.GetCustomerCaseNotes;

public record GetCustomerCaseNotesQuery(string CaseNumber) : IRequest<Result<IReadOnlyList<CustomerFacingCaseNoteDto>>>;