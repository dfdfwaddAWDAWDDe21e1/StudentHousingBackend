using FluentValidation;
using StudentHousingAPI.DTOs;

namespace StudentHousingAPI.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.LocationOrSpace)
            .NotEmpty().WithMessage("Location or Space is required")
            .MaximumLength(200).WithMessage("Location or Space must not exceed 200 characters");

        RuleFor(x => x.BuildingId)
            .GreaterThan(0).WithMessage("Valid BuildingId is required");

        RuleFor(x => x.DueDate)
            .Must(BeInTheFuture).WithMessage("Due date must be in the future");
    }

    private bool BeInTheFuture(DateTime dueDate)
    {
        return dueDate > DateTime.UtcNow;
    }
}
