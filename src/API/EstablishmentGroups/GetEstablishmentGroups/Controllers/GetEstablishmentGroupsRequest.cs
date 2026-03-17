using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers;

/// <summary>
/// Represents a request to retrieve establishment group data,
/// including the specific fields the caller wishes to include
/// in the response payload.
/// </summary>
public class GetEstablishmentGroupsRequest
{
    /// <summary>
    /// Specifies the fields to be returned for each establishment group.
    /// 
    /// This property is decorated with <see cref="RequestWithRequiredFieldsAttribute"/>,
    /// which enforces that at least one field is supplied and validates the
    /// request against the allowed field set for the <c>EstablishmentGroups</c> context.
    /// </summary>
    [RequestWithRequiredFields("EstablishmentGroups")]
    public string[] Fields { get; set; } = [];
}