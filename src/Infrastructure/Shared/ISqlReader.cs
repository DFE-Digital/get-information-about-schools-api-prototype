namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;

/// <summary>
/// Provides read‑only access to SQL data using parameterised queries.
/// Wraps transaction handling and query execution behind a simple API.
/// </summary>
public interface ISqlReader
{
    /// <summary>
    /// Executes a SQL query expected to return a single row.
    /// </summary>
    /// <typeparam name="TResult">The DTO type to map the row into.</typeparam>
    /// <param name="sql">The SQL query text.</param>
    /// <param name="parameters">The query parameters, or <c>null</c>.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task<TResult> QuerySingleAsync<TResult>(
        string sql,
        object? parameters,
        CancellationToken cancellationToken);

    /// <summary>
    /// Executes a SQL query expected to return multiple rows.
    /// </summary>
    /// <typeparam name="TResult">The DTO type to map each row into.</typeparam>
    /// <param name="sql">The SQL query text.</param>
    /// <param name="parameters">The query parameters, or <c>null</c>.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task<IEnumerable<TResult>> QueryAsync<TResult>(
        string sql,
        object? parameters,
        CancellationToken cancellationToken);
}

