using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishment.Request;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Usecases.GetEstablishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentAddress;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentContactDetails;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;

/// <summary>
/// Provides extension methods for registering all Establishment-related
/// application services, validators, and use cases into an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Registers all Establishment use case dependencies, validation services,
    /// and supporting components required by the Establishments domain.
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
    /// This method configures all services involved in Establishment-related operations.
    /// It registers the <see cref="GetEstablishmentsUseCase"/> as the implementation of
    /// <see cref="IUseCaseResponseOnly{TUseCaseResponse}"/> for retrieving collections of
    /// <see cref="Establishment"/> instances. In addition to the use case, it also registers
    /// the shared <see cref="IRegexValidationService"/>, which provides reusable regular
    /// expression–based validation logic used throughout the Establishments domain.
    ///
    /// The method also registers the domain-specific validators responsible for validating
    /// Establishment address information and Establishment contact details. These validators
    /// are added as singletons because they are stateless and safe to reuse across requests,
    /// whereas the use case is registered as scoped to align with typical request‑based
    /// application lifetimes.
    ///
    /// Together, these registrations ensure that all Establishment-related functionality is
    /// fully wired up and ready for use by the application.
    /// </remarks>
    public static IServiceCollection AddEstablishmentUseCaseDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddScoped<IUseCaseResponseOnly<
                UseCaseResponse<IReadOnlyCollection<Establishment>>>, GetEstablishmentsUseCase>()
            .AddScoped<IUseCase<
                GetEstablishmentByUrnRequest, UseCaseResponse<Establishment>>, GetEstablishmentByUrnUseCase>()
            .AddSingleton<IRegexValidationService, RegexValidationService>()
            .AddSingleton<IEstablishmentAddressValidator, EstablishmentAddressValidator>()
            .AddSingleton<IEstablishmentContactDetailsValidator, EstablishmentContactDetailsValidator>();
    }
}
