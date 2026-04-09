using MediatR;

namespace BuildingBlocks.Core.Seedwork.Query;
public interface IQuery<out T> : IRequest<T>;

public interface IQuery : IRequest;
