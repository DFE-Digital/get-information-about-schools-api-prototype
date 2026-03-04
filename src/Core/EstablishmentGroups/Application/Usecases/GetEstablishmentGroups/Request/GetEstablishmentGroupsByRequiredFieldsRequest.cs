using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishments.Request;

/// <summary>
/// Represents a request to retrieve establishment groups using a specified
/// set of required field names. Field validation is performed by the
/// base <see cref="BulkRequestParameters{TResponseObject}"/> class.
/// </summary>
public sealed class GetEstablishmentGroupsByRequiredFieldsRequest(
       string[] requiredFields)
       : BulkRequestParameters<IReadOnlyCollection<EstablishmentGroup>>(requiredFields)
{
    /// <summary>
    /// Creates a new <see cref="GetEstablishmentGroupsByRequiredFieldsRequest"/> instance
    /// using the specified required field names. This static factory method provides
    /// a convenient way to construct the request without referencing the constructor.
    /// </summary>
    /// <param name="requiredFields">The required field names to include in the request.</param>
    /// <returns>
    /// A new <see cref="GetEstablishmentGroupsByRequiredFieldsRequest"/> instance containing
    /// the provided required field list.
    /// </returns>
    public static GetEstablishmentGroupsByRequiredFieldsRequest Create(
        string[] requiredFields) =>
            new(requiredFields);
}
