using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
