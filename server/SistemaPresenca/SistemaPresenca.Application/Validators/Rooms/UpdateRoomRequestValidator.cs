using FluentValidation;
using SistemaPresenca.Application.Requests.Rooms;

namespace SistemaPresenca.Application.Validators.Rooms;

public sealed class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
{
    public UpdateRoomRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Name is required and must not exceed 200 characters.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Location is required and must not exceed 200 characters.");

        RuleFor(x => x.MicrocontrollerId)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Microcontroller id is required and must not exceed 50 characters.");
    }
}