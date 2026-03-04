using System.Data;
using System.Data.Common;
using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper.Handlers;
using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper.Handlers.Query;
using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper.Providers.Database.Context;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;

/// <summary>
/// Provides read‑only access to SQL data using parameterised queries.
/// Wraps transaction handling and query execution behind a simple, reusable API.
/// </summary>
/// <remarks>
/// This service is intended for safe, read‑only SQL operations.  
/// It automatically manages transaction boundaries using
/// <see cref="IsolationLevel.ReadCommitted"/> and delegates execution to the
/// configured <see cref="ISqlQueryHandler"/>.
/// </remarks>
public sealed class SqlReader : ISqlReader
{
    private readonly IDbContextProvider _dbContextProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlReader"/> class.
    /// </summary>
    /// <param name="dbContextProvider">
    /// Provides access to database connections, transactions, and SQL execution handlers.
    /// </param>
    public SqlReader(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    /// <summary>
    /// Executes a SQL query expected to return a single row.
    /// </summary>
    /// <typeparam name="TResult">The DTO type to map the returned row into.</typeparam>
    /// <param name="sql">The SQL query text.</param>
    /// <param name="parameters">The query parameters, or <c>null</c>.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns>The mapped DTO instance returned by the query.</returns>
    public Task<TResult> QuerySingleAsync<TResult>(
        string sql,
        object? parameters,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            parameters,
            (handler, tx, opts, ct) =>
                handler.QuerySingleAsync<TResult>(sql, tx, opts, ct),
            cancellationToken);

    /// <summary>
    /// Executes a SQL query expected to return multiple rows.
    /// </summary>
    /// <typeparam name="TResult">The DTO type to map each returned row into.</typeparam>
    /// <param name="sql">The SQL query text.</param>
    /// <param name="parameters">The query parameters, or <c>null</c>.</param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns>A sequence of mapped DTO instances returned by the query.</returns>
    public Task<IEnumerable<TResult>> QueryAsync<TResult>(
        string sql,
        object? parameters,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            parameters,
            (handler, tx, opts, ct) =>
                handler.QueryAsync<TResult>(sql, tx, opts, ct),
            cancellationToken);

    /// <summary>
    /// Executes a SQL query inside a managed transaction and returns the result.
    /// </summary>
    /// <typeparam name="T">The type returned by the underlying query operation.</typeparam>
    /// <param name="parameters">The query parameters, or <c>null</c>.</param>
    /// <param name="executor">
    /// A delegate that performs the actual query using the provided
    /// <see cref="ISqlQueryHandler"/>, transaction, and request options.
    /// </param>
    /// <param name="cancellationToken">A token that may be used to cancel the operation.</param>
    /// <returns>The result produced by the delegated query operation.</returns>
    /// <remarks>
    /// This method centralises transaction creation, execution, and commit logic,
    /// ensuring consistent behaviour across all SQL read operations.
    /// </remarks>
    private async Task<TResult> ExecuteAsync<TResult>(
        object? parameters,
        Func<ISqlQueryHandler, DbTransaction, SqlRequestOptions, CancellationToken, Task<TResult>> executor,
        CancellationToken cancellationToken)
    {
        await using var tx =
            await _dbContextProvider.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken);

        SqlRequestOptions options = new()
        {
            Type = CommandType.Text,
            Parameters = parameters
        };

        TResult result =
            await executor(
                _dbContextProvider.SqlQueryHandler,
                tx,
                options,
                cancellationToken);

        await tx.CommitAsync(cancellationToken);

        return result;
    }
}
