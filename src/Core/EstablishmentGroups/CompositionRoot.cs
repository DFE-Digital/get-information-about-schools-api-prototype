using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishment;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetEstablishmentGroups;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Usecases.GetGroup;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Groups;

/// <summary>
/// Provides extension methods for registering all Group-related
/// application services, validators, and use cases into an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    public static IServiceCollection AddGroupUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IUseCaseResponseOnly<
                UseCaseResponse<
                    IReadOnlyCollection<EstablishmentGroup>>>,
                GetEstablishmentGroupsUseCase>()
            .AddScoped<
                IUseCase<
                    GetEstablishmentGroupByUidRequest,
                    UseCaseResponse<EstablishmentGroup>>,
                GetEstablishmentGroupByUidUseCase>();
    }
}
