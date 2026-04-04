using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ManageAccount;

public record DeletePersonalDataCommand(
    string UserId,
    string Password) : IRequest<Result>;
