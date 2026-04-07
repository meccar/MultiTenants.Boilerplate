using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ResendEmailConfirmation;

public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;
