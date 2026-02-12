namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;

/// <summary>
/// Defines validation operations for establishment address components,
/// including street name, town, and postcode formats.
/// </summary>
/// <remarks>
/// Implementations of this interface encapsulate the domain rules for
/// validating address information supplied for an establishment.
/// </remarks>
public interface IEstablishmentAddressValidator
{
    /// <summary>
    /// Determines whether the supplied street value is valid according
    /// to the domain's formatting rules.
    /// </summary>
    /// <param name="street">The street value to validate.</param>
    /// <returns>
    /// <c>true</c> if the street value is considered valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidStreet(string street);

    /// <summary>
    /// Determines whether the supplied town value is valid according
    /// to the domain's formatting rules.
    /// </summary>
    /// <param name="town">The town value to validate.</param>
    /// <returns>
    /// <c>true</c> if the town value is considered valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidTown(string town);

    /// <summary>
    /// Determines whether the supplied postcode is valid according
    /// to the domain's formatting rules.
    /// </summary>
    /// <param name="postcode">The postcode to validate.</param>
    /// <returns>
    /// <c>true</c> if the postcode is considered valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidPostcode(string postcode);
}