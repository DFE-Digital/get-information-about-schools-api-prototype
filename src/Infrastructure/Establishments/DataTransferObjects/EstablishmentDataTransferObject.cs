namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;

/// <summary>
/// Represents the raw establishment data as retrieved from the data source.
/// This Data Transfer Object (DTO) is used exclusively for persistence and
/// transport concerns and contains no domain logic.
/// </summary>
public sealed class EstablishmentDataTransferObject
{
    /// <summary>
    /// Default value assigned to string properties when no data is supplied.
    /// </summary>
    private const string DefaultValue = "UNDEFINED";

    private string _establishmentName = DefaultValue;
    private string _schoolWebsite = DefaultValue;
    private string _telephoneNum = DefaultValue;
    private string _establishmentType = DefaultValue;
    private string _educationPhase = DefaultValue;
    private string _street = DefaultValue;
    private string _town = DefaultValue;
    private string _postcode = DefaultValue;
    private string _establishmentStatus = DefaultValue;

    /// <summary>
    /// Gets or sets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public int URN { get; set; }

    /// <summary>
    /// Gets or sets the official name of the establishment.
    /// </summary>
    public string EstablishmentName
    {
        get => _establishmentName;
        set => _establishmentName = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the website URL associated with the establishment.
    /// </summary>
    public string SchoolWebsite
    {
        get => _schoolWebsite;
        set => _schoolWebsite = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the establishment's telephone number.
    /// </summary>
    public string TelephoneNum
    {
        get => _telephoneNum;
        set => _telephoneNum = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the descriptive name of the establishment type.
    /// </summary>
    public string EstablishmentType
    {
        get => _establishmentType;
        set => _establishmentType = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the descriptive name of the establishment's phase of education.
    /// </summary>
    public string EducationPhase
    {
        get => _educationPhase;
        set => _educationPhase = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the first line of the establishment's street address.
    /// </summary>
    public string Street
    {
        get => _street;
        set => _street = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the town or locality in which the establishment is located.
    /// </summary>
    public string Town
    {
        get => _town;
        set => _town = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the postcode associated with the establishment's address.
    /// </summary>
    public string Postcode
    {
        get => _postcode;
        set => _postcode = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the descriptive status of the establishment.
    /// </summary>
    public string EstablishmentStatus
    {
        get => _establishmentStatus;
        set => _establishmentStatus = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }
}
