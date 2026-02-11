namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;

public interface IEstablishmentContactDetailsValidator
{
    bool IsValidWebsite(string? website);
    bool IsValidTelephone(string? telephone);
}