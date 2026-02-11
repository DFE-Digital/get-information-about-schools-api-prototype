namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;

/// <summary>
/// Represents the raw establishment data as retrieved from the data source.
/// This Data Transfer Object (DTO) is used exclusively for persistence and
/// transport concerns and contains no domain logic.
/// </summary>
public sealed class EstablishmentDataTransferObject
{
    /// <summary>
    /// Gets or sets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public int URN { get; set; }

    /// <summary>
    /// Gets or sets the official name of the establishment.
    /// </summary>
    public required string EstablishmentName { get; set; }

    /// <summary>
    /// Gets or sets the website URL associated with the establishment.
    /// </summary>
    public required string SchoolWebsite { get; set; }

    /// <summary>
    /// Gets or sets the establishment's telephone number.
    /// </summary>
    public required string TelephoneNum { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of the establishment type
    /// (e.g., Academy, Community School).
    /// </summary>
    public required string TypeOfEstablishment_name { get; set; }

    /// <summary>
    /// Gets or sets the descriptive name of the establishment's phase of education
    /// (e.g., Primary, Secondary).
    /// </summary>
    public required string PhaseOfEducation_name { get; set; }

    /// <summary>
    /// Gets or sets the first line of the establishment's street address.
    /// </summary>
    public required string Street { get; set; }

    /// <summary>
    /// Gets or sets the town or locality in which the establishment is located.
    /// </summary>
    public required string Town { get; set; }

    /// <summary>
    /// Gets or sets the postcode associated with the establishment's address.
    /// </summary>
    public required string Postcode { get; set; }

    /// <summary>
    /// Gets or sets the numeric status code representing the establishment's
    /// operational status (e.g., 1 = Open, 2 = Closed).
    /// </summary>
    public required int EstablishmentStatus_code { get; set; }
}
