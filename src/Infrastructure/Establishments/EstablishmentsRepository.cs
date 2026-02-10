using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper.Handlers;
using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper.Providers.Database.Context;
using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;
using System.Data;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;

/// <summary>
/// Provides access to establishment data stored in the SQL database.
/// Responsible for executing queries and mapping results into domain
/// <see cref="Establishment"/> objects.
/// </summary>
public sealed class EstablishmentsRepository : IEstablishmentsRepository
{
    private readonly IDbContextProvider _dbContextProvider;
    private readonly IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>> _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentsRepository"/> class.
    /// </summary>
    /// <param name="dbContextProvider">
    /// Provides database connections and transaction management.
    /// </param>
    /// <param name="mapper">
    /// Maps collections of <see cref="EstablishmentDataTransferObject"/> into
    /// domain <see cref="Establishment"/> objects.
    /// </param>
    public EstablishmentsRepository(
        IDbContextProvider dbContextProvider,
        IMapper<
            IEnumerable<EstablishmentDataTransferObject>,
            IReadOnlyCollection<Establishment>> mapper)
    {
        _dbContextProvider = dbContextProvider;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all establishments from the database and maps them into
    /// domain <see cref="Establishment"/> instances.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="Establishment"/> objects representing
    /// the establishments stored in the database.
    /// </returns>
    /// <remarks>
    /// This method performs a read-only SQL query inside a transaction with
    /// <see cref="IsolationLevel.ReadCommitted"/> isolation. The results are mapped
    /// using the injected mapper and returned as an immutable collection.
    /// </remarks>
    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        CancellationToken cancellationToken = default)
    {
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted;

        await using var dbTransaction =
            await _dbContextProvider.BeginTransactionAsync(isolationLevel, cancellationToken);

        const string SelectEstablishmentsSql =
            "SELECT URN, EstablishmentName, SchoolWebsite, TelephoneNum FROM Establishments;";

        IEnumerable<EstablishmentDataTransferObject> result =
            await _dbContextProvider.SqlQueryHandler
                .QueryAsync<EstablishmentDataTransferObject>(
                    SelectEstablishmentsSql,
                    dbTransaction,
                    new SqlRequestOptions
                    {
                        Type = CommandType.Text,
                        Parameters = new { URN = "" } // bug here... we need to make the Parameters option non-compulsory.
                    },
                    cancellationToken
                );

        await dbTransaction.CommitAsync(cancellationToken);

        // Map DTOs to domain objects.
        IReadOnlyCollection<Establishment> mapped = _mapper.Map(result);

        // Convert to a true read-only wrapper.
        Establishment[] array = [.. mapped];

        return Array.AsReadOnly(array);
    }
}
