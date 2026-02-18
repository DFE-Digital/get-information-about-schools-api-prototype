using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

/// <summary>
/// Represents a group of establishments within the domain.
/// This aggregate enforces invariants such as requiring valid group details
/// and at least one associated establishment.
/// </summary>
public sealed class EstablishmentGroup : AggregateRoot<EstablishmentGroupIdentifier>
{
    /// <summary>
    /// Gets the basic identifying and descriptive details of the group.
    /// </summary>
    public EstablishmentGroupDetails BasicDetails { get; }

    /// <summary>
    /// Gets the collection of establishments that belong to this group.
    /// This collection is guaranteed to contain at least one item.
    /// </summary>
    public IReadOnlyCollection<EstablishmentOverview> GroupEstablishments { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentGroup"/> aggregate.
    /// Ensures that group details and establishment information meet
    /// the required domain invariants.
    /// </summary>
    /// <param name="identifier">The unique identifier for the group.</param>
    /// <param name="basicDetails">The descriptive details associated with the group.</param>
    /// <param name="groupEstablishments">The establishments that form part of the group.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when <paramref name="basicDetails"/> is null.
    /// </exception>
    /// <exception cref="EstablishmentGroupException">
    /// Thrown when <paramref name="groupEstablishments"/> is null or empty.
    /// </exception>
    public EstablishmentGroup(
        EstablishmentGroupIdentifier identifier,
        EstablishmentGroupDetails basicDetails,
        IReadOnlyCollection<EstablishmentOverview> groupEstablishments)
        : base(identifier)
    {
        BasicDetails = basicDetails
            ?? throw new EstablishmentGroupException(
                "An initialised 'GroupDetails' object must be provided.");

        ValidateGroupEstablishments(groupEstablishments);
        GroupEstablishments = groupEstablishments;
    }

    /// <summary>
    /// Factory method for creating a new <see cref="EstablishmentGroup"/> instance.
    /// Provides a more expressive and intention‑revealing way to construct the aggregate.
    /// </summary>
    /// <param name="identifier">The unique identifier for the group.</param>
    /// <param name="groupDetails">The descriptive details associated with the group.</param>
    /// <param name="groupEstablishments">The establishments that form part of the group.</param>
    /// <returns>A fully validated <see cref="EstablishmentGroup"/> instance.</returns>
    public static EstablishmentGroup Create(
        EstablishmentGroupIdentifier identifier,
        EstablishmentGroupDetails groupDetails,
        IReadOnlyCollection<EstablishmentOverview> groupEstablishments) =>
            new(identifier, groupDetails, groupEstablishments);

    /// <summary>
    /// Validates that the provided collection of establishments meets
    /// the domain rules for group creation.
    /// </summary>
    /// <param name="groupEstablishments">The collection of establishments to validate.</param>
    /// <exception cref="EstablishmentGroupException">
    /// Thrown when the collection is null or contains no establishments.
    /// </exception>
    private static void ValidateGroupEstablishments(
        IReadOnlyCollection<EstablishmentOverview> groupEstablishments)
    {
        if (groupEstablishments is null)
            throw new EstablishmentGroupException(
                "A collection of 'EstablishmentOverview' objects must be provided for the group.");

        if (groupEstablishments.Count == 0)
            throw new EstablishmentGroupException(
                "A group must contain at least one establishment for the group.");
    }
}
