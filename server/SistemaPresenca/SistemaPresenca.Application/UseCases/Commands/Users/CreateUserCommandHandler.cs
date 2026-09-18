using LiteBus.Commands.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SistemaPresenca.Application.Mappers;
using SistemaPresenca.Domain.Entities;
using SistemaPresenca.Domain.Errors;
using SistemaPresenca.Domain.Interfaces.Repositories;
using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Application.UseCases.Commands.Users;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, Result>
{
    public async Task<Result> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var existingUser = await userRepository.GetOneAsync(x =>
            x.Cpf == command.Request.Cpf ||
            x.Email == command.Request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            logger.LogError("An User with same email {Email} or cpf {Cpf} already exists.", command.Request.Email, command.Request.Cpf);
            return Result.Failure(UserErrors.EmailOrCpfAlreadyExists);
        }

        var user = command.Request.ToEntity();

        var hashedPassword = passwordHasher.HashPassword(user, command.Request.Password);

        user.Password = hashedPassword;

        await userRepository.AddAsync(user, cancellationToken);

        logger.LogInformation("User with id {Id} and email {Email} created successfully.", user.Id, user.Email);

        return Result.Success();
    }
}
