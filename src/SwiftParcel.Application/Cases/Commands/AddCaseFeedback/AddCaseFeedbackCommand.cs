using MediatR;
using SwiftParcel.Application.Common.Models;

namespace SwiftParcel.Application.Cases.Commands.AddCaseFeedback;

public record AddCaseFeedbackCommand(string CaseNumber, int Score) : IRequest<Result<bool>>;