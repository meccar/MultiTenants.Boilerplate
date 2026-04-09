using MediatR;

namespace BuildingBlocks.Core.Seedwork.Command;
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