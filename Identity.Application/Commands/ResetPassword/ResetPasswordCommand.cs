using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<IdentityResult>;
