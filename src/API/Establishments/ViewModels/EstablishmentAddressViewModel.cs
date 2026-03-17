namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;

/// <summary>
/// Represents the address details of an establishment as exposed by the API.
/// This view model is used to return structured, human‑readable address
/// information to clients.
/// </summary>
public sealed class EstablishmentAddressViewModel
{
    /// <summary>
    /// The first line of the establishment's address, typically containing
    /// the building number and street name.
    /// </summary>
    public required string Street { get; set; }

    /// <summary>
    /// The town or city in which the establishment is located.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// The postal code associated with the establishment's address.
    /// </summary>
    public required string Postcode { get; set; }
}