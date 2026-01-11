using FluentValidation;
using StudentHousingAPI.DTOs;

namespace StudentHousingAPI.Validators;

public class CreateIssueRequestValidator : AbstractValidator<CreateIssueRequest>
{
    public CreateIssueRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.SharedSpace)
            .MaximumLength(200).WithMessage("SharedSpace must not exceed 200 characters");

        RuleFor(x => x.BuildingId)
            .GreaterThan(0).WithMessage("Valid BuildingId is required");
    }
}

public class UpdateIssueRequestValidator : AbstractValidator<UpdateIssueRequest>
{
    public UpdateIssueRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => status == null || new[] { "Open", "InProgress", "Resolved", "Closed" }.Contains(status))
            .WithMessage("Status must be one of: Open, InProgress, Resolved, Closed");

        RuleFor(x => x.AssignedToUserId)
            .GreaterThan(0).When(x => x.AssignedToUserId.HasValue)
            .WithMessage("AssignedToUserId must be greater than 0 when provided");
    }
}
