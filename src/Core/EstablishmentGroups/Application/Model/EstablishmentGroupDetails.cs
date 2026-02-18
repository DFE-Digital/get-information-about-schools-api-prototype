using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

/// <summary>
/// Represents the descriptive details of an establishment group.
/// This value object encapsulates the group's name and type, enforcing
/// validation rules to ensure both values are always present and meaningful.
/// </summary>
public sealed class EstablishmentGroupDetails : ValueObject<EstablishmentGroupDetails>
{
    /// <summary>
    /// Gets the official name of the group.
    /// Guaranteed to be a non-empty, trimmed string.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the descriptive type of the group (e.g., Trust, Federation).
    /// Guaranteed to be a non-empty, trimmed string.
    /// </summary>
    public string GroupType { get; }

    /// <summary>
    /// Private constructor used internally after validation has been applied.
    /// </summary>
    /// <param name="name">The validated group name.</param>
    /// <param name="groupType">The validated group type.</param>
    private EstablishmentGroupDetails(
        string name,
        string groupType)
    {
        Name = name;
        GroupType = groupType;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentGroupDetails"/> instance after applying
    /// trimming and validation rules to the supplied values.
    /// </summary>
    /// <param name="name">The name of the group.</param>
    /// <param name="groupType">The type of the group.</param>
    /// <returns>A fully validated <see cref="EstablishmentGroupDetails"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when either <paramref name="name"/> or <paramref name="groupType"/> is missing or invalid.
    /// </exception>
    public static EstablishmentGroupDetails Create(
        string? name,
        string? groupType)
    {
        name = name?.Trim();
        groupType = groupType?.Trim();

        Validate(name, groupType);
        return new EstablishmentGroupDetails(name!, groupType!);
    }

    /// <summary>
    /// Validates the supplied group name and type according to domain rules.
    /// </summary>
    /// <param name="name">The group name to validate.</param>
    /// <param name="groupType">The group type to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when either value is null, empty, or whitespace.
    /// </exception>
    private static void Validate(
        string? name,
        string? groupType)
    {
        EnsureNameIsProvided(name);
        EnsureGroupTypeIsProvided(groupType);
    }

    /// <summary>
    /// Ensures that a valid group name has been supplied.
    /// </summary>
    /// <param name="name">The group name to check.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when <paramref name="name"/> is null, empty, or whitespace.
    /// </exception>
    private static void EnsureNameIsProvided(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EstablishmentGroupException("Group name is required.");
    }

    /// <summary>
    /// Ensures that a valid group type has been supplied.
    /// </summary>
    /// <param name="establishmentType">The group type to check.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when <paramref name="establishmentType"/> is null, empty, or whitespace.
    /// </exception>
    private static void EnsureGroupTypeIsProvided(string? establishmentType)
    {
        if (string.IsNullOrWhiteSpace(establishmentType))
            throw new EstablishmentGroupException("Group type is required.");
    }

    /// <summary>
    /// Defines the components used to determine equality between
    /// <see cref="EstablishmentGroupDetails"/> instances.
    /// </summary>
    /// <returns>An enumerable of values that uniquely identify this value object.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return GroupType;
    }
}
