using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ConfirmEmail;

public record ConfirmEmailCommand(
    string Email,
    string Token
    ) : IRequest<IdentityResult>;
