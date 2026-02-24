using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.Login;

public record LocalAuthenticationCommand(
  string TenantId,
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
