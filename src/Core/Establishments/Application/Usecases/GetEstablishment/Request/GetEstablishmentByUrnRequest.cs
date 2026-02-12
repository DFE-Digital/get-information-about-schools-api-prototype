using DfE.CleanArchitecture.Common.Application;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment.Request;

/// <summary>
/// Represents a request to retrieve a single <see cref="Establishment"/>
/// identified by its unique reference number (URN).
/// </summary>
/// <remarks>
/// The URN must be a six‑digit numeric value. An exception is thrown if the
/// provided URN does not meet this requirement.
/// </remarks>
public sealed class GetEstablishmentByUrnRequest
    : IUseCaseRequest<UseCaseResponse<Establishment>>
{
    /// <summary>
    /// Gets the six‑digit unique reference number of the establishment to retrieve.
    /// </summary>
    public int Urn { get; }

    /// <summary>
    /// Creates a new <see cref="GetEstablishmentByUrnRequest"/> instance.
    /// </summary>
    /// <param name="urn">The six‑digit URN of the establishment.</param>
    /// <returns>
    /// A validated <see cref="GetEstablishmentByUrnRequest"/> instance.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied URN is not exactly six digits.
    /// </exception>
    public static GetEstablishmentByUrnRequest Create(int urn) => new(urn);

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEstablishmentByUrnRequest"/> class.
    /// </summary>
    /// <param name="urn">The six‑digit URN of the establishment.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied URN is not exactly six digits.
    /// </exception>
    private GetEstablishmentByUrnRequest(int urn)
    {
        // URN must be between 10000 and 999999 inclusive.
        if (urn < 10000 || urn > 999999)
        {
            throw new ArgumentOutOfRangeException(
                nameof(urn),
                "URN must be a five to six‑digit number.");
        }

        Urn = urn;
    }
}
