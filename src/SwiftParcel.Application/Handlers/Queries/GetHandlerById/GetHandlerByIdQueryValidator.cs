using FluentValidation;

namespace SwiftParcel.Application.Handlers.Queries.GetHandlerById;

public class GetHandlerByIdQueryValidator : AbstractValidator<GetHandlerByIdQuery>
{
    public GetHandlerByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Handler ID is required.");
    }
}