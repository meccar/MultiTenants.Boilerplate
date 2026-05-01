using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Commands.ChangePassword;

public record ChangePasswordByOwnerCommand(
    CurrentUserModel CurrentUser,
    ChangePasswordDto ChangePasswordDto) : IRequest<Result>;