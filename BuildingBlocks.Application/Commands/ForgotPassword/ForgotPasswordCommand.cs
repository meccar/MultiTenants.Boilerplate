using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;
