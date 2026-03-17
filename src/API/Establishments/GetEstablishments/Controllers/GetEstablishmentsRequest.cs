using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;

namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.GetEstablishmentGroups.Controllers;

/// <summary>
/// Represents a request to retrieve establishment data,
/// specifying which fields the caller wants included in the response.
/// </summary>
public class GetEstablishmentsRequest
{
    /// <summary>
    /// The list of fields to be returned for each establishment.
    ///
    /// This property is decorated with <see cref="RequestWithRequiredFieldsAttribute"/>,
    /// which ensures that at least one field is supplied and validates the request
    /// against the allowed field set for the <c>Establishments</c> context.
    /// </summary>
    [RequestWithRequiredFields("Establishments")]
    public string[] Fields { get; set; } = [];
}
