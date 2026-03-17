using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;

/// <summary>
/// Maps an <see cref="Establishment"/> domain model into a dynamic view model
/// suitable for API responses.
/// 
/// This mapper constructs a strongly typed <see cref="EstablishmentViewModel"/> and then
/// converts it into a dynamic object using <see cref="DynamicViewModelConverter"/>,
/// allowing field‑level shaping based on client requests.
/// </summary>
public sealed class EstablishmentModelToViewModelMapper
    : IMapper<Establishment, object?>
{
    private readonly DynamicViewModelConverter _converter;

    /// <summary>
    /// Creates a new instance of <see cref="EstablishmentModelToViewModelMapper"/>.
    /// </summary>
    /// <param name="converter">
    /// The converter responsible for transforming view models into dynamic,
    /// field‑selectable representations.
    /// </param>
    public EstablishmentModelToViewModelMapper(DynamicViewModelConverter converter)
    {
        _converter = converter;
    }

    /// <summary>
    /// Maps an <see cref="Establishment"/> domain model into a dynamic view model.
    /// </summary>
    /// <param name="input">The establishment domain model to map.</param>
    /// <returns>
    /// A dynamic object representing the establishment, shaped according to
    /// the active field selection rules.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public object Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(nameof(input));

        EstablishmentViewModel establishmentViewModel = new()
        {
            URN = input.Identifier.Urn,
            Name = input.BasicDetails.Name,
            Type = input.BasicDetails.EstablishmentType,
            PhaseOfEducation = input.BasicDetails.PhaseOfEducation,
            Address = input.Address,
            StatusCode = input.BasicDetails.Status
        };

        return _converter.ToDynamic(establishmentViewModel)!;
    }
}