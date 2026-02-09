using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;

/// <summary>
/// Defines the contract for accessing establishment data from a persistence layer.
/// </summary>
/// <remarks>
/// Implementations may retrieve data from any source (e.g., API, database, file store),
/// but must return a fully constructed and valid <see cref="Establishment"/> aggregate.
/// </remarks>
public interface IEstablishmentsRepository
{
    /// <summary>
    /// Retrieves establishment information from the underlying data source.
    /// </summary>
    /// <returns>
    /// A task that resolves to a valid <see cref="Establishment"/> instance.
    /// </returns>
    /// <remarks>
    /// Implementations should throw an appropriate exception if the establishment
    /// cannot be found or the data source is unavailable.
    /// </remarks>
    Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        CancellationToken cancellationToken = default);
}