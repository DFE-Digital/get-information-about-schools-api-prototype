using Dfe.Data.Common.Infrastructure.Persistence.Sql.Dapper;
using DfE.GetInformationAboutSchools.Prototyping.API.Establishments.Controllers.Response;
using DfE.GetInformationAboutSchools.Prototyping.Core.Establishments;
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
