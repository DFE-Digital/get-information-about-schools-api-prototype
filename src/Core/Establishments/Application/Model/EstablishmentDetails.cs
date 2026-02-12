using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the core identifying details of an establishment as an immutable
/// domain value object.
/// </summary>
/// <remarks>
/// <para>
/// This value object enforces all domain invariants at creation time. Optional
/// fields may be omitted, but when supplied must conform to the rules of the
/// domain.
/// </para>
/// <para>
/// Because this is a value object, equality is determined by the values of its
/// components rather than by identity.
/// </para>
/// </remarks>
public sealed class EstablishmentDetails : ValueObject<EstablishmentDetails>
{
    /// <summary>
    /// Gets the establishment's official name. This value is always required.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the establishment's type (e.g., Academy, Community School).
    /// This value is required.
    /// </summary>
    public string EstablishmentType { get; }

    /// <summary>
    /// Gets the establishment's phase of education (e.g., Primary, Secondary).
    /// This value is required.
    /// </summary>
    public string PhaseOfEducation { get; }

    /// <summary>
    /// Gets the establishment's operational status (e.g., Open, Closed).
    /// </summary>
    public string Status { get; }

    private EstablishmentDetails(
        string name,
        string establishmentType,
        string phaseOfEducation,
        string status)
    {
        Name = name;
        EstablishmentType = establishmentType;
        PhaseOfEducation = phaseOfEducation;
        Status = status;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentDetails"/> instance after validating
    /// all supplied values against domain rules.
    /// </summary>
    /// <param name="name">The establishment's official name.</param>
    /// <param name="establishmentType">The establishment's type.</param>
    /// <param name="phaseOfEducation">The establishment's phase of education.</param>
    /// <param name="status">The establishment's operational status.</param>
    /// <returns>
    /// A fully validated <see cref="EstablishmentDetails"/> value object.
    /// </returns>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value violates a domain invariant.
    /// </exception>
    public static EstablishmentDetails Create(
        string? name,
        string? establishmentType,
        string? phaseOfEducation,
        string? status)
    {
        name = name?.Trim();
        establishmentType = establishmentType?.Trim();
        phaseOfEducation = phaseOfEducation?.Trim();
        status = status?.Trim();

        Validate(name, establishmentType, phaseOfEducation, status);
        return new EstablishmentDetails(name!, establishmentType!, phaseOfEducation!, status!);
    }

    /// <summary>
    /// Validates all supplied establishment details by enforcing the domain
    /// invariants for each required field.
    /// </summary>
    /// <param name="name">The establishment's official name.</param>
    /// <param name="establishmentType">The establishment's type.</param>
    /// <param name="phaseOfEducation">The establishment's phase of education.</param>
    /// <param name="status">The establishment's operational status.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value fails its corresponding validation rule.
    /// </exception>
    private static void Validate(
        string? name,
        string? establishmentType,
        string? phaseOfEducation,
        string? status)
    {
        EnsureNameIsProvided(name);
        EnsureEstablishmentTypeIsProvided(establishmentType);
        EnsurePhaseOfEducationIsProvided(phaseOfEducation);
        EnsureStatusIsProvided(status);
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
            throw new EstablishmentException("Establishment name is required.");
    }

    /// <summary>
    /// Ensures that the establishment type is present and non‑empty.
    /// </summary>
    /// <param name="establishmentType">The type to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the type is null, empty, or whitespace.
    /// </exception>
    private static void EnsureEstablishmentTypeIsProvided(string? establishmentType)
    {
        if (string.IsNullOrWhiteSpace(establishmentType))
            throw new EstablishmentException("Establishment type is required.");
    }

    /// <summary>
    /// Ensures that the phase of education is present and non‑empty.
    /// </summary>
    /// <param name="phaseOfEducation">The phase to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the phase is null, empty, or whitespace.
    /// </exception>
    private static void EnsurePhaseOfEducationIsProvided(string? phaseOfEducation)
    {
        if (string.IsNullOrWhiteSpace(phaseOfEducation))
            throw new EstablishmentException("Establishment phase of education is required.");
    }

    /// <summary>
    /// Ensures that the establishment's operational status is present and non‑empty.
    /// </summary>
    /// <param name="status">The operational status to validate.</param>
    /// <exception cref="EstablishmentException">
    /// Thrown when the status is null, empty, or whitespace.
    /// </exception>
    private static void EnsureStatusIsProvided(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new EstablishmentException("Establishment status is required.");
    }

    /// <summary>
    /// Defines equality based on all component values.
    /// </summary>
    /// <returns>
    /// An enumeration of the components that define equality.
    /// </returns>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return EstablishmentType;
        yield return PhaseOfEducation;
        yield return Status;
    }
}
