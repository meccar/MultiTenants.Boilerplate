using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.Logout;

public record LogoutCommand : IRequest<Result>;
