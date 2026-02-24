using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ResendEmailConfirmation;

public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;
