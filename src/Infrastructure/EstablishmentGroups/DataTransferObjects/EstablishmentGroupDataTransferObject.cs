namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;

/// <summary>
/// Represents a data transfer object (DTO) used to transport information
/// about an establishment group between application layers.
/// </summary>
public sealed class EstablishmentGroupDataTransferObject
{
    /// <summary>
    /// Default value assigned to string properties when no data is supplied.
    /// </summary>
    private const string DefaultValue = "UNDEFINED";

    /// <summary>
    /// Backing field for <see cref="EstablishmentName"/> ensuring a non-empty value.
    /// </summary>
    private string _establishmentName = DefaultValue;

    /// <summary>
    /// Backing field for <see cref="GroupName"/> ensuring a non-empty value.
    /// </summary>
    private string _groupName = DefaultValue;

    /// <summary>
    /// Backing field for <see cref="GroupTypeName"/> ensuring a non-empty value.
    /// </summary>
    private string _groupTypeName = DefaultValue;

    /// <summary>
    /// Gets or sets the unique identifier (UID) assigned to the establishment group.
    /// </summary>
    public int UID { get; set; }

    /// <summary>
    /// Gets or sets the official name of the group.
    /// If the supplied value is null, empty, or whitespace, a default value is used.
    /// </summary>
    public string GroupName
    {
        get => _groupName;
        set => _groupName = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the descriptive name of the group type.
    /// If the supplied value is null, empty, or whitespace, a default value is used.
    /// </summary>
    public string GroupTypeName
    {
        get => _groupTypeName;
        set => _groupTypeName = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }

    /// <summary>
    /// Gets or sets the unique reference number (URN) of the establishment
    /// associated with this group.
    /// </summary>
    public int EstablishmentUrn { get; set; }

    /// <summary>
    /// Gets or sets the official name of the establishment.
    /// If the supplied value is null, empty, or whitespace, a default value is used.
    /// </summary>
    public string EstablishmentName
    {
        get => _establishmentName;
        set => _establishmentName = string.IsNullOrWhiteSpace(value) ? DefaultValue : value;
    }
}
