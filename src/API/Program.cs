using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper;
using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;
using DfE.GetInformationAboutSchools.Prototyping.Core.Groups;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Model;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.EstablishmentGroups;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddEstablishmentUseCaseDependencies();
builder.Services.AddEstablishmentInfrastructureDependencies();
builder.Services.AddGroupInfrastructureDependencies();
builder.Services.AddGroupUseCaseDependencies();
builder.Services.AddSingleton<ICsvResponseBuilder, CsvResponseBuilder>();
builder.Services.AddSingleton<IMapper<
    Establishment, EstablishmentViewModel>, EstablishmentModelToViewModelMapper>();
builder.Services.AddSingleton<IMapper<
    EstablishmentDataTransferObject, Establishment>, EstablishmentDtoToModelMapper>();
builder.Services.Configure<CsvMappingDictionary>(
    builder.Configuration.GetSection("CsvMappings"));
builder.Services.AddSingleton<
    ICsvMapper<EstablishmentGroup>, ModelToCsvMapper<EstablishmentGroup>>();
builder.Services.AddSingleton<
    ICsvMapper<Establishment>, ModelToCsvMapper<Establishment>>();
builder.Services.AddSingleton<
    IMapper<EstablishmentGroup, EstablishmentGroupViewModel>, EstablishmentGroupModelToViewModelMapper>();
builder.Services
    .Configure<ValidationPatterns>(
        builder.Configuration.GetSection("ValidationPatterns"));

// SQL Server connection string
var sqlServerConnectionString = builder.Configuration.GetConnectionString("Edubase");

// Register database using the new string-based overload
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
    options.SerializerOptions.WriteIndented = false; // important
    options.SerializerOptions.MaxDepth = 0; // unlimited
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
