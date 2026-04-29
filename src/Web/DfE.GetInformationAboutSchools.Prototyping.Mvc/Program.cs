using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper;
using DfE.CleanArchitecture.Common.CrossCutting.Mapper;
using DfE.GetInformationAboutSchools.Prototyping.API.EstablishmentGroups.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.DynamicViewModelConverters.ConversionRules;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;
using DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups;
using DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.ValidationServices;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.DataTransferObjects;
using DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Establishments.Mappers;
using Npgsql;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;

var builder = WebApplication.CreateBuilder(args);

// NEEDED FOR DOCKER AS STYLES WEREN'T SHOWING
StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

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

var postgresConnectionString = builder.Configuration.GetConnectionString("Edubase");

builder.Services.AddDatabase(
    "edubase",
    _ => new NpgsqlConnection(postgresConnectionString),
    options => options.DefaultTimeout = 5
);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
