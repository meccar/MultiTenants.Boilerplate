using MediatR;

namespace BuildingBlocks.Core.Seedwork.Command;
public interface ICommand<out T> : IRequest<T>;

public interface ICommand : IRequest;
