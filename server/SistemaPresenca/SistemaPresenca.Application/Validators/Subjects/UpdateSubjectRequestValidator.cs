using FluentValidation;
using SistemaPresenca.Application.Requests.Subjects;

namespace SistemaPresenca.Application.Validators.Subjects;

public class UpdateSubjectRequestValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(10).WithMessage("Code must not exceed 10 characters.");

        RuleFor(x => x.TotalClasses)
            .GreaterThan(0).WithMessage("Total classes must be a positive number.");
    }
}
