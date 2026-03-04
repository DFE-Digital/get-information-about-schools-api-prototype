using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishment;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishmentGroups;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishments.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetGroup;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups;

/// <summary>
/// Provides extension methods for registering all Establishment Group–related
/// application services, validators, and use cases into an <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all use cases required for querying establishment groups,
    /// including retrieval by required fields and by UID.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> into which the dependencies will be registered.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddEstablishmentGroupUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<
                IUseCase<
                    GetEstablishmentGroupsByRequiredFieldsRequest,
                    UseCaseResponse<IReadOnlyCollection<EstablishmentGroup>>>,
                GetEstablishmentGroupsUseCase>()
            .AddScoped<
                IUseCase<
                    GetEstablishmentGroupByUidRequest,
                    UseCaseResponse<EstablishmentGroup>>,
                GetEstablishmentGroupByUidUseCase>();
    }
}
