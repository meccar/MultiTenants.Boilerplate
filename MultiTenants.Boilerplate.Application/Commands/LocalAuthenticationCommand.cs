using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Helpers;

public record LocalAuthenticationCommand(
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
