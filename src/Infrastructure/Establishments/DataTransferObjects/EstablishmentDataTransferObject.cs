namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;

public sealed class EstablishmentDataTransferObject
{
    public int URN { get; set; }
    public required string EstablishmentName { get; set; }
    public required string SchoolWebsite { get; set; }
    public required string TelephoneNum { get; set; }
}