using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;

/// <summary>
/// Maps a single <see cref="EstablishmentDataTransferObject"/> into a fully
/// constructed domain <see cref="Establishment"/> instance.
/// </summary>
/// <remarks>
/// This mapper applies all domain validation rules via the injected validators.
/// It is responsible only for transformation and does not perform any persistence
/// or repository operations.
/// </remarks>
public sealed class EstablishmentDtoToModelMapper :
    IMapper<EstablishmentDataTransferObject, Establishment>
{
    private readonly IEstablishmentContactDetailsValidator _contactValidator;
    private readonly IEstablishmentAddressValidator _addressValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentDtoToModelMapper"/> class.
    /// </summary>
    /// <param name="contactValidator">
    /// Validator used to enforce domain rules for contact details.
    /// </param>
    /// <param name="addressValidator">
    /// Validator used to enforce domain rules for establishment addresses.
    /// </param>
    public EstablishmentDtoToModelMapper(
        IEstablishmentContactDetailsValidator contactValidator,
        IEstablishmentAddressValidator addressValidator)
    {
        _contactValidator = contactValidator;
        _addressValidator = addressValidator;
    }

    /// <summary>
    /// Maps the supplied <see cref="EstablishmentDataTransferObject"/> into a corresponding
    /// <see cref="Establishment"/> domain model.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing establishment information.
    /// </param>
    /// <returns>
    /// A fully constructed <see cref="Establishment"/> domain object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dto"/> is <c>null</c>.
    /// </exception>
    public Establishment Map(EstablishmentDataTransferObject dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // Construct identifier
        var identifier = new EstablishmentIdentifier(dto.URN);

        // Construct establishment details
        var details = EstablishmentDetails.Create(
            dto.EstablishmentName,
            dto.EstablishmentType,
            dto.EducationPhase,
            dto.EstablishmentStatus);

        // Construct validated contact details
        var contactDetails = EstablishmentContactDetails.Create(
            dto.SchoolWebsite,
            dto.TelephoneNum,
            _contactValidator);

        // Construct validated address
        var address = EstablishmentAddress.Create(
            dto.Street,
            dto.Town,
            dto.Postcode,
            _addressValidator);

        // Construct final aggregate
        return new Establishment(identifier, details, contactDetails, address);
    }
}
