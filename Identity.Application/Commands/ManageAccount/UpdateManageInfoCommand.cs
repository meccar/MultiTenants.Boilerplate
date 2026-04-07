using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ManageAccount;

public record UpdateManageInfoCommand(
    string UserId,
    string? Email,
    string? UserName) : IRequest<Result>;
