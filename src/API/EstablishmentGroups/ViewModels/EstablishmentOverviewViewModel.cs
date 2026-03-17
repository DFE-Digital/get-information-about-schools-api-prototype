namespace DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;

/// <summary>
/// Represents a lightweight API view model describing an establishment
/// that belongs to a group. This model exposes only the essential
/// identifying information required by API consumers.
/// </summary>
public sealed class EstablishmentOverviewViewModel
{
    /// <summary>
    /// Gets the unique reference number (URN) assigned to the establishment.
    /// </summary>
    public required string Urn { get; init; }

    /// <summary>
    /// Gets the official name of the establishment.
    /// </summary>
    public required string Name { get; init; }
}
