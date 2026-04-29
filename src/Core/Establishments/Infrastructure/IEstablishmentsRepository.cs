using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.SearchByFilters;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;

/// <summary>
/// Defines the contract for accessing establishment data from a persistence layer.
/// </summary>
/// <remarks>
/// Implementations may retrieve data from any source (e.g., API, database, file store),
/// but must return fully constructed and valid <see cref="Establishment"/> aggregates.
/// </remarks>
public interface IEstablishmentsRepository
{
    /// <summary>
    /// Retrieves all establishments from the underlying data source.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task that resolves to a read‑only collection of <see cref="Establishment"/> instances.
    /// </returns>
    /// <remarks>
    /// Implementations should throw an appropriate exception if the data source is unavailable
    /// or if retrieval fails unexpectedly.
    /// </remarks>
    Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        HashSet<string> requiredFields,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single establishment identified by its URN.
    /// </summary>
    /// <param name="urn">
    /// The unique reference number of the establishment to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task that resolves to the matching <see cref="Establishment"/> instance.
    /// </returns>
    /// <remarks>
    /// Implementations should throw an appropriate exception if the data source is unavailable
    /// or if retrieval fails unexpectedly. If no establishment exists for the specified URN,
    /// implementations may return <c>null</c> or throw a not‑found exception, depending on the
    /// application's error‑handling strategy.
    /// </remarks>
    Task<Establishment> GetEstablishment(
        int urn,
        CancellationToken cancellationToken = default);



    Task<IReadOnlyCollection<Establishment>> SearchFuzzyAsync(
        string term,
        double similarityThreshold,
        int limit,
        CancellationToken cancellationToken);

    Task<EstablishmentFilterSearchResponse> SearchFilteredAsync(
        EstablishmentFilterCriteria criteria,
        double similarityThreshold,
        CancellationToken cancellationToken);
}
