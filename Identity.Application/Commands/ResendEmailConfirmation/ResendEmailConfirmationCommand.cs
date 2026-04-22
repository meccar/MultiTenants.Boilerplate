using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.ResendEmailConfirmation;

public record ResendEmailConfirmationCommand(string Email) : IRequest<Result>;
