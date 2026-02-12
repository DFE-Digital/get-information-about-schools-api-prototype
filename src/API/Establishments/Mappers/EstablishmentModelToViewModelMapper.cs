using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;

public sealed class EstablishmentModelToViewModelMapper : IMapper<Establishment, EstablishmentViewModel>
{
    public EstablishmentViewModel Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(nameof(input));

        return new EstablishmentViewModel()
        {
            URN = input.Identifier.Urn,
            Name = input.BasicDetails.Name,
            Type = input.BasicDetails.EstablishmentType,
            PhaseOfEducation = input.BasicDetails.PhaseOfEducation,
            Address = input.Address,
            StatusCode = input.BasicDetails.Status
        };
    }
}
