namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;

/// <summary>
/// Represents the API-facing view model for a group,
/// exposing the key identifying and descriptive information
/// required by clients of the Groups API. This model also includes
/// a lightweight overview of the establishments that belong to the group.
/// </summary>
public sealed class EstablishmentGroupViewModel
{
    /// <summary>
    /// Gets or sets the unique identifier (UID) assigned to the group.
    /// This value corresponds to the group's persistent identity within
    /// the data source.
    /// </summary>
    public required int UID { get; set; }

    /// <summary>
    /// Gets or sets the official name of the group.
    /// This is the human-readable name presented to API consumers.
    /// </summary>
    public required string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the descriptive type name of the group
    /// (for example, Trust, Federation, or Local Authority Group).
    /// </summary>
    public required string GroupTypeName { get; set; }

    /// <summary>
    /// Gets or sets the collection of establishments that belong to the group.
    /// Each item provides a lightweight overview containing only the
    /// essential establishment details required by API consumers.
    /// </summary>
    public required IEnumerable<EstablishmentOverviewViewModel> Establishments { get; set; }
}
