using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.DataShapingRules;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper.Factories;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure;

public static class CompositionRoot
{
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<ITypeFactory, TypeFactory>()
            .AddSingleton<ICollectionFactory, CollectionFactory>()
            .AddSingleton<IDataShapingRule, NullValueShapingRule>()
            .AddSingleton<IDataShapingRule, PrimitiveValueShapingRule>()
            .AddSingleton<IDataShapingRule, CollectionShapingRule>()
            .AddSingleton<IDataShapingRule, ComplexObjectShapingRule>()
            .AddSingleton(typeof(IDataTransferObjectShaper<>), typeof(DataTransferObjectShaper<>))
            // Establishment dependencies.
            .AddScoped<ISqlReader, SqlReader>()
            .AddScoped<IEstablishmentsRepository, EstablishmentsRepository>()
            .AddSingleton<ICollectionFactory, CollectionFactory>()
            .AddSingleton<ITypeFactory, TypeFactory>()
            .AddSingleton<IMapper<
                IEnumerable<EstablishmentDataTransferObject>,
                IReadOnlyCollection<Establishment>>,
                    EstablishmentsDtoToModelMapper>()
            .AddSingleton<IMapper<
                EstablishmentDataTransferObject, Establishment>,
                    EstablishmentDtoToModelMapper>()
            // Establishment Group dependencies.
            .AddScoped<ISqlReader, SqlReader>()
            .AddScoped<IEstablishmentGroupsRepository, EstablishmentGroupsRepository>()
            .AddSingleton<IMapper<
                IEnumerable<EstablishmentGroupDataTransferObject>,
                IReadOnlyCollection<EstablishmentGroup>>,
                    EstablishmentGroupsDtoToModelMapper>();
    }
}






