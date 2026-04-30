using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Commands.Logout;

public record LogoutCommand(
    CurrentUserModel CurrentUser
) : IRequest<Result>;
