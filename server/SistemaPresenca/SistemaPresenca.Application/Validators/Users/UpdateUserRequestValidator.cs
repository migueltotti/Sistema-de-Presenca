using FluentValidation;
using SistemaPresenca.Application.Requests.Users;

namespace SistemaPresenca.Application.Validators.Users;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Name is required and must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Email is required and must not exceed 200 characters.");

        RuleFor(x => x.TagId)
            .NotEmpty()
            .MaximumLength(16)
            .WithMessage("Tag must not exceed 16 characters.");
    }
}
