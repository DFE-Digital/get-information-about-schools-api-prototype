using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;

/// <summary>
/// Provides extension methods for registering Establishment-related
/// infrastructure dependencies into an <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers Establishment infrastructure services, including the
    /// <see cref="IEstablishmentsRepository"/> and supporting mappers,
    /// into the application's dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the infrastructure
    /// dependencies will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// This method registers <see cref="EstablishmentsRepository"/> as a
    /// <see cref="ServiceLifetime.Singleton"/> implementation of
    /// <see cref="IEstablishmentsRepository"/>.  
    ///
    /// It also registers the DTO-to-domain mapper used by the repository to
    /// convert <see cref="EstablishmentDataTransferObject"/> instances into
    /// domain <see cref="Establishment"/> objects.  
    ///
    /// Both components are stateless and therefore safe to register as singletons.
    /// </remarks>
    public static IServiceCollection AddEstablishmentInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<IEstablishmentsRepository, EstablishmentsRepository>()
            .AddSingleton<IMapper<
                IEnumerable<EstablishmentDataTransferObject>,
                IReadOnlyCollection<Establishment>>, EstablishmentsDtoToModelMapper>();
    }
}
