using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Code) : IRequest<Result>;
