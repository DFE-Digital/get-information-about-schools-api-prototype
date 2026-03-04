using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Attributes;

namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.GetEstablishmentGroups.Controllers;

public class GetEstablishmentGroupsRequest
{
    [RequestWithRequiredFields("EstablishmentGroups")]
    public string[] Fields { get; set; } = [];
}