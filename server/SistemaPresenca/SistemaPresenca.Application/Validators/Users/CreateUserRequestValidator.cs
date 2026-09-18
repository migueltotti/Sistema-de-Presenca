using FluentValidation;
using SistemaPresenca.Application.Requests.Users;

namespace SistemaPresenca.Application.Validators.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Name is required and must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Email is required and must not exceed 200 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Password is required and must not exceed 200 characters.");

        RuleFor(x => x.RegistrationId)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("RegistrationId is required and must not exceed 10 characters.");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .MaximumLength(11)
            .WithMessage("Cpf is required and must not exceed 10 characters.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .IsInEnum()
            .WithMessage("Role is required and must be Admin, Professor or Student");
    }
}
