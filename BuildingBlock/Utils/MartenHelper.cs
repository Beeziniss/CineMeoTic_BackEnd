using BuildingBlocks.Exceptions;
using Marten;
using Marten.Services.BatchQuerying;

namespace BuildingBlocks.Utils;

/*
 * Only for executing batch query, not for subquery (depends on the "parent" query) and transaction, because Marten's batch query does not support transaction, if want to execute multiple queries in a transaction, should use IDocumentSession instead of IBatchedQuery
*/

public static class MartenHelper
{
    public static async Task ExecuteBatchAsync(IDocumentSession documentSession, Action<IBatchedQuery> configureBatch, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documentSession);
        ArgumentNullException.ThrowIfNull(configureBatch);

        try
        {
            IBatchedQuery batch = documentSession.CreateBatchQuery();
            configureBatch(batch);

            await batch.Execute(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new BaseException("An error occurred while executing the batch query\n" + ex.Message);
        }

    }

    public static async Task<TResult> ExecuteBatchAsync<TResult>(IDocumentSession documentSession, Func<IBatchedQuery, TResult> configureBatch, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(documentSession);
        ArgumentNullException.ThrowIfNull(configureBatch);


        try
        {
            IBatchedQuery batch = documentSession.CreateBatchQuery();
            TResult result = configureBatch(batch);

            await batch.Execute(cancellationToken);
            return result;
        }
        catch(Exception ex)
        {
            throw new BaseException("An error occurred while executing the batch query\n" + ex.Message);
        }
    }

}
