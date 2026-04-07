using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.Logout;

public record LogoutCommand : IRequest<Result>;
