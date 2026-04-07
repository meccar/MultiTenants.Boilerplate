using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ChangePassword;

public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result>;
