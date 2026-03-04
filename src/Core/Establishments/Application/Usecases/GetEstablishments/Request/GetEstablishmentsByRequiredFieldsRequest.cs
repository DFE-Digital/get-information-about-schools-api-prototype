using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishments.Request;

/// <summary>
/// Represents a request to retrieve establishments using a specified
/// set of required field names. Field validation is performed by the
/// base <see cref="BulkRequestParameters{TResponseObject}"/> class.
/// </summary>
public sealed class GetEstablishmentsByRequiredFieldsRequest(
       string[] requiredFields)
       : BulkRequestParameters<IReadOnlyCollection<Establishment>>(requiredFields)
{
    /// <summary>
    /// Creates a new <see cref="GetEstablishmentsByRequiredFieldsRequest"/> instance
    /// using the specified required field names. This static factory method provides
    /// a convenient way to construct the request without referencing the constructor.
    /// </summary>
    /// <param name="requiredFields">The required field names to include in the request.</param>
    /// <returns>
    /// A new <see cref="GetEstablishmentsByRequiredFieldsRequest"/> instance containing
    /// the provided required field list.
    /// </returns>
    public static GetEstablishmentsByRequiredFieldsRequest Create(
        string[] requiredFields) =>
            new(requiredFields);
}
