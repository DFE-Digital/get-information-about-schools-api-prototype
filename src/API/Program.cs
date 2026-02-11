using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper;
using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.ViewModels;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.Address;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model.ValidationServices.ContactDetails;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentAddress;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices.EstablishmentContactDetails;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();
builder.Services.AddEstablishmentUseCaseDependencies();
builder.Services.AddEstablishmentInfrastructureDependencies();
builder.Services.AddSingleton<ICsvResponseBuilder, CsvResponseBuilder>();
builder.Services.AddSingleton<IMapper<
    Establishment, EstablishmentViewModel>, EstablishmentModelToViewModelMapper>();
builder.Services
    .Configure<ValidationPatterns>(
        builder.Configuration.GetSection("ValidationPatterns"));
builder.Services.AddSingleton<IRegexValidationService, RegexValidationService>();
builder.Services.AddSingleton<
    IEstablishmentAddressValidator, EstablishmentAddressValidator>();
builder.Services.AddSingleton<
    IEstablishmentContactDetailsValidator, EstablishmentContactDetailsValidator>();

// SQL Server connection string
const string sqlServerConnectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=edubase;Trusted_Connection=True;TrustServerCertificate=True;";

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
