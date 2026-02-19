using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;

/// <summary>
/// Defines the contract for retrieving <see cref="EstablishmentGroup"/> domain
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

    /// <summary>
    /// Retrieves a single <see cref="EstablishmentGroup"/> aggregate by its unique identifier.
    /// </summary>
    /// <param name="uid">
    /// The unique identifier of the group to retrieve.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The fully constructed <see cref="EstablishmentGroup"/> domain model
    /// corresponding to the specified <paramref name="uid"/>, or <c>null</c>
    /// if no matching group exists.
    /// </returns>
    Task<EstablishmentGroup?> GetEstablishmentGroup(
        int uid,
        CancellationToken cancellationToken = default);
}
