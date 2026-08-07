using FluentValidation;
using SwiftParcel.Domain.ValueObjects;

namespace SwiftParcel.Application.Common.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidTrackingNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(trackingNumber =>
                TrackingNumber.Create(trackingNumber).IsSuccess)
            .WithMessage("Tracking number format is invalid.");
    }
}