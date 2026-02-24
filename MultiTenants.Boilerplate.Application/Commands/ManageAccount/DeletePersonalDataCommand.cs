using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ManageAccount;

public record DeletePersonalDataCommand(
    string UserId,
    string Password) : IRequest<Result>;
