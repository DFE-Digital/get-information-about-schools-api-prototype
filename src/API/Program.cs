using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper;
using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.ModelBinding.Options;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Required fields configuration
builder.Services.Configure<DefaultRequiredFields>(
    builder.Configuration.GetSection("DefaultRequiredFields"));

// Domain registrations
builder.Services.AddEstablishmentUseCaseDependencies();
builder.Services.AddInfrastructureDependencies();
builder.Services.AddEstablishmentGroupUseCaseDependencies();
builder.Services.AddSingleton<ICsvResponseBuilder, CsvResponseBuilder>();
builder.Services.AddSingleton<IMapper<Establishment, object?>, EstablishmentModelToViewModelMapper>();
builder.Services.AddSingleton<IMapper<EstablishmentDataTransferObject, Establishment>, EstablishmentDtoToModelMapper>();
builder.Services.Configure<CsvMappingDictionary>(
    builder.Configuration.GetSection("CsvMappings"));
builder.Services.AddSingleton<ICsvMapper<EstablishmentGroup>, ModelToCsvMapper<EstablishmentGroup>>();
builder.Services.AddSingleton<ICsvMapper<Establishment>, ModelToCsvMapper<Establishment>>();
builder.Services.AddSingleton<IMapper<EstablishmentGroup, object?>, EstablishmentGroupModelToViewModelMapper>();
builder.Services.Configure<ValidationPatterns>(
    builder.Configuration.GetSection("ValidationPatterns"));
builder.Services.AddSingleton<IDynamicConversionRule, UndefinedStringConversionRule>();
builder.Services.AddSingleton<IDynamicConversionRule, StringConversionRule>();
builder.Services.AddSingleton<IDynamicConversionRule, ValueTypeConversionRule>();
builder.Services.AddSingleton<IDynamicConversionRule, DictionaryConversionRule>();
builder.Services.AddSingleton<IDynamicConversionRule, EnumerableConversionRule>();
builder.Services.AddSingleton<IDynamicConversionRule, ObjectConversionRule>();
builder.Services.AddSingleton<DynamicViewModelConverter>();

// SQL Server connection
var sqlServerConnectionString = builder.Configuration.GetConnectionString("Edubase");

builder.Services.AddDatabase(
    "edubase",
    _ => new SqlConnection(sqlServerConnectionString),
    options => options.DefaultTimeout = 5
);

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.WriteIndented = false;
    options.SerializerOptions.MaxDepth = 0;
});

builder.Services.AddControllers(options =>
{
    var requiredFields = builder.Configuration.GetSection("DefaultRequiredFields").Get<DefaultRequiredFields>()
        ?? new DefaultRequiredFields { RequiredFields = [] };
    options.ModelBinderProviders.Insert(0,
        new RequestWithRequiredFieldsModelBinderProvider(requiredFields));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
