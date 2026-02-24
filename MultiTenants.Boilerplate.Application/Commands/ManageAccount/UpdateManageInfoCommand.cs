using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ManageAccount;

public record UpdateManageInfoCommand(
    string UserId,
    string? Email,
    string? UserName) : IRequest<Result>;
