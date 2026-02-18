using DfE.CleanArchitecture.Common.Domain;
using System.Text.RegularExpressions;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

/// <summary>
/// Represents a lightweight overview of an establishment belonging to a group.
/// This value object contains only the essential identifying information:
/// the establishment URN and its official name.
/// </summary>
public sealed partial class EstablishmentOverview : ValueObject<EstablishmentOverview>
{
    /// <summary>
    /// Gets the unique reference number (URN) assigned to the establishment.
    /// Guaranteed to be a valid 5–7 digit numeric value.
    /// </summary>
    public int URN { get; }

    /// <summary>
    /// Gets the official name of the establishment.
    /// Guaranteed to be a non‑empty, trimmed string.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Private constructor used after validation has been applied.
    /// </summary>
    /// <param name="urn">The validated establishment URN.</param>
    /// <param name="name">The validated establishment name.</param>
    private EstablishmentOverview(int urn, string name)
    {
        URN = urn;
        Name = name;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentOverview"/> instance after applying
    /// trimming and validation rules to the supplied values.
    /// </summary>
    /// <param name="urn">The establishment URN.</param>
    /// <param name="name">The establishment name.</param>
    /// <returns>A fully validated <see cref="EstablishmentOverview"/> value object.</returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when the URN or name is missing or invalid.
    /// </exception>
    public static EstablishmentOverview Create(
        int urn,
        string? name)
    {
        name = name?.Trim();

        Validate(urn, name);
        return new EstablishmentOverview(urn, name!);
    }

    /// <summary>
    /// Validates the supplied URN and name according to domain rules.
    /// </summary>
    /// <param name="urn">The URN to validate.</param>
    /// <param name="name">The name to validate.</param>
    private static void Validate(
        int urn,
        string? name)
    {
        EnsureNameIsProvided(name);
        EnsureValidUrn(urn);
    }

    /// <summary>
    /// Ensures that the establishment name is present and non‑empty.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the name is null, empty, or whitespace.
    /// </exception>
    private static void EnsureNameIsProvided(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new EstablishmentGroupException("Establishment name is required.");
    }

    /// <summary>
    /// Validates that the supplied URN matches the required numeric pattern.
    /// </summary>
    /// <param name="urn">The URN to validate.</param>
    /// <returns><c>true</c> if the URN is valid; otherwise, <c>false</c>.</returns>
    private static bool EnsureValidUrn(int urn) =>
        UrnValidation().IsMatch(urn.ToString());

    /// <summary>
    /// Regular expression pattern for validating a 5 to 7‑digit URN.
    /// </summary>
    private const string UrnPattern = @"^\d{5,7}$";

    /// <summary>
    /// Compiled regular expression for URN validation.
    /// </summary>
    [GeneratedRegex(UrnPattern)]
    private static partial Regex UrnValidation();

    /// <summary>
    /// Defines the components used to determine equality between
    /// <see cref="EstablishmentOverview"/> instances.
    /// </summary>
    /// <returns>An enumerable containing the URN and name.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return URN;
        yield return Name;
    }
}
