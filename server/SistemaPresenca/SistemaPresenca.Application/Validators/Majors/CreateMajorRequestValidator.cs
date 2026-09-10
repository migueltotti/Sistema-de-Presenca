using FluentValidation;
using SistemaPresenca.Application.Requests.Majors;

namespace SistemaPresenca.Application.Validators.Majors;

public class CreateMajorRequestValidator : AbstractValidator<CreateMajorRequest>
{
    public CreateMajorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Major name is required and must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Major code is required and must not exceed 10 characters.");
    }
}
