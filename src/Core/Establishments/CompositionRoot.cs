using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;

/// <summary>
/// Provides extension methods for registering Establishment-related
/// use case dependencies into an <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all Establishment use case dependencies required by the application.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> into which the dependencies will be registered.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance, enabling fluent chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// This method registers the <see cref="GetEstablishmentsUseCase"/> as the implementation
    /// for the <see cref="IUseCaseResponseOnly{TUseCaseResponse}"/> interface, specifically
    /// for returning a <see cref="UseCaseResponse{T}"/> containing a collection of
    /// <see cref="Establishment"/> instances.
    /// </remarks>
    public static IServiceCollection AddEstablishmentUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.AddScoped<
            IUseCaseResponseOnly<UseCaseResponse<IReadOnlyCollection<Establishment>>>,
            GetEstablishmentsUseCase>();
    }
}
