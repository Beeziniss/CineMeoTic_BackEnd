using BuildingBlocks.CQRS;
using CineMeoTic.UserService.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace CineMeoTic.UserService.API.CQRS.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>(IUserDbContext userDbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUserDbContext _userDbContext = userDbContext;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand)
        {
            return await next(cancellationToken);
        }

        if (_userDbContext.Database.CurrentTransaction != null)
        {
            return await next(cancellationToken);
        }

        await using IDbContextTransaction transaction = await _userDbContext.Database
            .BeginTransactionAsync(cancellationToken);

        try
        {
            TResponse? response = await next(cancellationToken);

            await _userDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
