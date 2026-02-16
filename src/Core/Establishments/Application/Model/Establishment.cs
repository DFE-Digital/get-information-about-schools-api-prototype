using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Aggregate root representing an establishment within the domain.
/// </summary>
/// <remarks>
/// This aggregate is immutable and enforces structural completeness only.
/// All supplied value objects are assumed to be pre‑validated by their
/// respective factories or validators.
/// </remarks>
public sealed class Establishment : AggregateRoot<EstablishmentIdentifier>
{
    /// <summary>
    /// Gets the core descriptive details of the establishment, such as
    /// name, type, phase of education, and operational status.
    /// </summary>
    public EstablishmentDetails BasicDetails { get; }

    /// <summary>
    /// Gets the validated contact details for the establishment, including
    /// website and telephone information.
    /// </summary>
    public EstablishmentContactDetails ContactDetails { get; }

    /// <summary>
    /// Gets the validated physical address of the establishment.
    /// </summary>
    public EstablishmentAddress Address { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Establishment"/> class.
    /// </summary>
    /// <param name="identifier">
    /// The unique identifier assigned to this establishment.
    /// </param>
    /// <param name="basicDetails">
    /// A valid <see cref="EstablishmentDetails"/> instance describing the
    /// establishment’s core characteristics.
    /// </param>
    /// <param name="contactDetails">
    /// A valid <see cref="EstablishmentContactDetails"/> instance containing
    /// communication details for the establishment.
    /// </param>
    /// <param name="address">
    /// A valid <see cref="EstablishmentAddress"/> instance representing the
    /// establishment’s physical location.
    /// </param>
    /// <exception cref="EstablishmentException">
    /// Thrown when any supplied value object is <c>null</c>.
    /// </exception>
    public Establishment(
        EstablishmentIdentifier identifier,
        EstablishmentDetails basicDetails,
        EstablishmentContactDetails contactDetails,
        EstablishmentAddress address)
        : base(identifier)
    {
        BasicDetails = basicDetails
            ?? throw new EstablishmentException(
                "An initialised 'EstablishmentDetails' object must be provided.");

        ContactDetails = contactDetails
            ?? throw new EstablishmentException(
                "An initialised 'EstablishmentContactDetails' object must be provided.");

        Address = address
            ?? throw new EstablishmentException(
                "An initialised 'EstablishmentAddress' object must be provided.");
    }

    /// <summary>
    /// Creates a new <see cref="Establishment"/> aggregate instance.
    /// </summary>
    /// <param name="identifier">The establishment’s unique identifier.</param>
    /// <param name="basicDetails">Validated establishment details.</param>
    /// <param name="contactDetails">Validated contact details.</param>
    /// <param name="address">Validated establishment address.</param>
    /// <returns>
    /// A fully constructed <see cref="Establishment"/> aggregate.
    /// </returns>
    public static Establishment Create(
        EstablishmentIdentifier identifier,
        EstablishmentDetails basicDetails,
        EstablishmentContactDetails contactDetails,
        EstablishmentAddress address) =>
        new(identifier, basicDetails, contactDetails, address);
}
