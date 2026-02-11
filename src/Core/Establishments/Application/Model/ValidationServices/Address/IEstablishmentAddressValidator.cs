namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;

public interface IEstablishmentAddressValidator
{
    bool IsValidStreet(string street);
    bool IsValidTown(string town);
    bool IsValidPostcode(string postcode);
}