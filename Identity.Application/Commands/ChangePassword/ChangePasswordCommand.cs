using BuildingBlocks.Shared.Constants;
using BuildingBlocks.Shared.Utilities;
using Identity.Application.Decorators;
using MediatR;

namespace Identity.Application.Commands.ChangePassword;

[RequirePermission("")]
public record ChangePasswordCommand(
    string UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result>;
