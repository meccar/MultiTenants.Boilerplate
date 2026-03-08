using MediatR;

namespace MultiTenants.Boilerplate.Domain.Seedwork.Query;
public interface IQuery<out T> : IRequest<T>;

public interface IQuery : IRequest;
