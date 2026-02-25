using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;

public sealed class EstablishmentModelToViewModelMapper
    : IMapper<Establishment, object?>
{
    private readonly DynamicViewModelConverter _converter;

    public EstablishmentModelToViewModelMapper(DynamicViewModelConverter converter)
    {
        _converter = converter;
    }

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