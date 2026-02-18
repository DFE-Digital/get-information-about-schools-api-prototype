using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;

/// <summary>
/// Defines the contract for retrieving Group domain
/// models from an underlying data source.
/// </summary>
public interface IEstablishmentGroupsRepository
{
    /// <summary>
    /// Retrieves all available <see cref="EstablishmentGroup"/> aggregates from the repository.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read‑only collection of fully constructed <see cref="EstablishmentGroup"/> domain models.
    /// </returns>
    Task<IReadOnlyCollection<EstablishmentGroup>> GetEstablishmentGroups(
        CancellationToken cancellationToken = default);
}
