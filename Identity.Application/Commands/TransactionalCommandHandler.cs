using BuildingBlocks.Core.Seedwork.Command;
using BuildingBlocks.Core.Seedwork.Interface;

namespace Identity.Application.Commands;

public abstract class TransactionalCommandHandler<TCommand, TResponse>
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    protected TransactionalCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await HandleCommandAsync(request, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    protected abstract Task<TResponse> HandleCommandAsync(
        TCommand request,
        CancellationToken cancellationToken);
}
