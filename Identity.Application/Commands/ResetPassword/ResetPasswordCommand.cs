using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Token,
    ResetPasswordDto ResetPasswordDto) : IRequest<Result>;
