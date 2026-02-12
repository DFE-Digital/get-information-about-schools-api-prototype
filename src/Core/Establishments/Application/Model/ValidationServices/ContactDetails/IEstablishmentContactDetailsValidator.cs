namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;

/// <summary>
/// Defines validation operations for establishment contact details,
/// including website and telephone number formats.
/// </summary>
/// <remarks>
/// Implementations of this interface encapsulate the domain rules for
/// validating optional contact information supplied for an establishment.
/// </remarks>
public interface IEstablishmentContactDetailsValidator
{
    /// <summary>
    /// Determines whether the supplied website address is valid according
    /// to the domain's formatting rules.
    /// </summary>
    /// <param name="website">The website address to validate.</param>
    /// <returns>
    /// <c>true</c> if the website is considered valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidWebsite(string? website);

    /// <summary>
    /// Determines whether the supplied telephone number is valid according
    /// to the domain's formatting rules.
    /// </summary>
    /// <param name="telephone">The telephone number to validate.</param>
    /// <returns>
    /// <c>true</c> if the telephone number is considered valid; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidTelephone(string? telephone);
}