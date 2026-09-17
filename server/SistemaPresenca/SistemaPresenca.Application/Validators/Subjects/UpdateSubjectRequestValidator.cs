using FluentValidation;
using SistemaPresenca.Application.Requests.Subjects;

namespace SistemaPresenca.Application.Validators.Subjects;

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Name is required and must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Code is required and must not exceed 10 characters.");

        RuleFor(x => x.TotalClasses)
            .GreaterThan(0)
            .WithMessage("Total classes is required and must be a positive number.");
    }
}
