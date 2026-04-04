using MediatR;

namespace BuildingBlocks.Domain.Seedwork.Command;
public interface ICommandHandler
    <in TCommand, TCommandResponse>
    : IRequestHandler<TCommand, TCommandResponse>
    where TCommand : ICommand<TCommandResponse>
{
}

public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}