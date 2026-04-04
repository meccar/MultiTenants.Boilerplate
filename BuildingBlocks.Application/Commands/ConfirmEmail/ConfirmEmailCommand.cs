using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Code) : IRequest<Result>;
