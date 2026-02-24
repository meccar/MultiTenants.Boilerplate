using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
