using MediatR;

namespace MultiTenants.Boilerplate.Domain.Seedwork.Command;
public interface ICommand<out T> : IRequest<T>;

public interface ICommand : IRequest;
