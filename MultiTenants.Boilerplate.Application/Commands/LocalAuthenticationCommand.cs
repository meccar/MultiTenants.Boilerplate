using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public record LocalAuthenticationCommand(
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
