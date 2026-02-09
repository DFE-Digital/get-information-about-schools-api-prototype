using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;
using System.Buffers;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;

/// <summary>
/// Maps a collection of <see cref="EstablishmentDataTransferObject"/> instances
/// into a read-only collection of domain <see cref="Establishment"/> objects.
/// </summary>
/// <remarks>
/// This mapper is optimised to minimise allocations by using <see cref="ArrayPool{T}"/>
/// when constructing the intermediate buffer.
/// </remarks>
public sealed class EstablishmentsDtoToModelMapper :
    IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>>
{
    /// <summary>
    /// Maps the supplied DTO collection into a corresponding collection of domain models.
    /// </summary>
    /// <param name="input">
    /// The sequence of <see cref="EstablishmentDataTransferObject"/> instances to map.
    /// </param>
    /// <returns>
    /// A read-only collection of fully constructed <see cref="Establishment"/> domain objects.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public IReadOnlyCollection<Establishment> Map(
        IEnumerable<EstablishmentDataTransferObject> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Materialise once to avoid multiple enumeration.
        ICollection<EstablishmentDataTransferObject> dtoList =
            input as ICollection<EstablishmentDataTransferObject> ?? [.. input];

        int count = dtoList.Count;

        ArrayPool<Establishment> pool = ArrayPool<Establishment>.Shared;
        Establishment[] buffer = pool.Rent(count);

        int index = 0;

        try
        {
            foreach (EstablishmentDataTransferObject dto in dtoList)
            {
                EstablishmentIdentifier identifier = new(dto.URN);

                EstablishmentDetails details =
                    EstablishmentDetails.Create(
                        dto.EstablishmentName,
                        dto.SchoolWebsite,
                        dto.TelephoneNum);

                buffer[index++] = new Establishment(identifier, details);
            }

            // Copy only the populated portion into a new array.
            Establishment[] result = new Establishment[index];
            Array.Copy(buffer, result, index);

            return result;
        }
        finally
        {
            // Clear only the used portion before returning to the pool.
            Array.Clear(buffer, 0, index);
            pool.Return(buffer);
        }
    }
}
