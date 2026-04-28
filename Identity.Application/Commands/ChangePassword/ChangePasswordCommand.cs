using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result>;
