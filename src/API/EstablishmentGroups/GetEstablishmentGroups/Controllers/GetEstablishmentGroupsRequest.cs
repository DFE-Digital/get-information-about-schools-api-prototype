using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers;

public class GetEstablishmentGroupsRequest
{
    [RequestWithRequiredFields("Establishments")]
    public string[] Fields { get; set; } = [];
}