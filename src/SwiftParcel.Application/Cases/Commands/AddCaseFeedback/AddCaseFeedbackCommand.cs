using MediatR;
using SwiftParcel.Application.Common.Models;
using SwiftParcel.Domain.Shared;

namespace SwiftParcel.Application.Cases.Commands.AddCaseFeedback;

public record AddCaseFeedbackCommand(string CaseNumber, int Score) : IRequest<Result<bool>>;