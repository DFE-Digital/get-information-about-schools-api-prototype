using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;

/// <summary>
/// Represents the establishment details returned by the API.  
/// This view model is designed for client consumption and contains only
/// presentation‑ready fields.
/// </summary>
public sealed class EstablishmentViewModel
{
    /// <summary>
    /// Gets or sets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public required string URN { get; set; }

    /// <summary>
    /// Gets or sets the official name of the establishment.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the descriptive type of the establishment
    /// (e.g., Academy, Community School).
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Gets or sets the phase of education delivered by the establishment
    /// (e.g., Primary, Secondary).
    /// </summary>
    public required string PhaseOfEducation { get; set; }

    /// <summary>
    /// Gets or sets the establishment's operational status code as a string
    /// (e.g., "1" = Open, "2" = Closed).
    /// </summary>
    public required string StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the establishment's full postal address.
    /// </summary>
    public required EstablishmentAddress Address { get; set; }
}
