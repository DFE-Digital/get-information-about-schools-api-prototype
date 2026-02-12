using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;

/// <summary>
/// Provides extension methods for registering Establishment‑related
/// infrastructure dependencies into an <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers Establishment infrastructure services, including:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="ISqlReader"/> – a scoped, read‑only SQL query service.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="IEstablishmentsRepository"/> – the repository responsible for
    /// retrieving Establishment domain models.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// DTO‑to‑domain mappers used by the repository.
    /// </description>
    /// </item>
    /// </list>
    /// These services are registered with <see cref="ServiceLifetime.Scoped"/>,
    /// ensuring they participate correctly in the per‑request lifetime of the
    /// application's data access pipeline.
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
    /// <para>
    /// The <see cref="ISqlReader"/> abstraction centralises SQL query execution,
    /// transaction handling, and parameter binding. It is registered as scoped
    /// because it depends on <c>IDbContextProvider</c>, which is also scoped.
    /// </para>
    /// <para>
    /// The <see cref="IEstablishmentsRepository"/> is likewise registered as scoped,
    /// ensuring each HTTP request receives its own repository instance with its
    /// own SQL reader and mappers.
    /// </para>
    /// </remarks>
    public static IServiceCollection AddEstablishmentInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<ISqlReader, SqlReader>()
            .AddScoped<IEstablishmentsRepository, EstablishmentsRepository>()
            .AddScoped<IMapper<
                IEnumerable<EstablishmentDataTransferObject>,
                IReadOnlyCollection<Establishment>>, EstablishmentsDtoToModelMapper>();
    }
}
