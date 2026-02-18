using System.Text.RegularExpressions;
using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

/// <summary>
/// Represents the unique identifier (UID) for a group of establishments.
/// This value object ensures that the UID conforms to the required
/// 4–5 digit numeric format.
/// </summary>
public sealed partial class EstablishmentGroupIdentifier : ValueObject<EstablishmentGroupIdentifier>
{
    /// <summary>
    /// Gets the validated UID value associated with the group.
    /// </summary>
    public int UID { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentGroupIdentifier"/> class.
    /// Ensures that the supplied UID meets the required numeric format.
    /// </summary>
    /// <param name="uid">The numeric UID value to validate and assign.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="uid"/> does not match the required 4–5 digit pattern.
    /// </exception>
    public EstablishmentGroupIdentifier(int uid)
    {
        if (!IsValidUid(uid))
            throw new ArgumentException(
                "UID must be a valid 4 to 5-digit numeric value.", nameof(uid));

        UID = uid;
    }

    /// <summary>
    /// Returns the UID as a string representation.
    /// </summary>
    public override string ToString() => UID.ToString();

    /// <summary>
    /// Defines the components used to determine equality between
    /// <see cref="EstablishmentGroupIdentifier"/> instances.
    /// </summary>
    /// <returns>An enumerable containing the UID value.</returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UID;
    }

    /// <summary>
    /// Determines whether the supplied UID matches the required validation pattern.
    /// </summary>
    /// <param name="urn">The UID value to validate.</param>
    /// <returns><c>true</c> if the UID is valid; otherwise, <c>false</c>.</returns>
    private static bool IsValidUid(int urn) =>
        UidValidation().IsMatch(urn.ToString());

    /// <summary>
    /// Regular expression pattern for validating a 4 to 5‑digit UID.
    /// </summary>
    private const string UidPattern = @"^\d{4,5}$";

    /// <summary>
    /// Compiled regular expression used to validate UID values.
    /// </summary>
    [GeneratedRegex(UidPattern)]
    private static partial Regex UidValidation();
}
