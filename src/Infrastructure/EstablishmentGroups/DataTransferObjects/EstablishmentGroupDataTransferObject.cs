using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjects.Extensions;

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

    private string _establishmentName = DefaultValue;
    private string _groupName = DefaultValue;
    private string _groupTypeName = DefaultValue;
    private string _establishmentUrn = DefaultValue;

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
        set => _groupName = value.Normalise();
    }

    /// <summary>
    /// Gets or sets the descriptive name of the group type.
    /// If the supplied value is null, empty, or whitespace, a default value is used.
    /// </summary>
    public string GroupTypeName
    {
        get => _groupTypeName;
        set => _groupTypeName = value.Normalise();
    }

    /// <summary>
    /// Gets or sets the unique reference number (URN) of the establishment
    /// associated with this group.
    /// </summary>
    public string EstablishmentUrn
    {
        get => _establishmentUrn;
        set => _establishmentUrn = value.Normalise();
    }

    /// <summary>
    /// Gets or sets the official name of the establishment.
    /// If the supplied value is null, empty, or whitespace, a default value is used.
    /// </summary>
    public string EstablishmentName
    {
        get => _establishmentName;
        set => _establishmentName = value.Normalise();
    }
}
