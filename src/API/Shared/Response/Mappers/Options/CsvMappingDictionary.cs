namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers.Options;

/// <summary>
/// A dictionary of CSV mapping configurations, keyed by model type name.
/// </summary>
/// <remarks>
/// This container is typically populated via configuration binding (e.g., appsettings.json)
/// and injected using <see cref="Microsoft.Extensions.Options.IOptions{TOptions}"/>.
/// Each entry maps a model type name (e.g., <c>"School"</c>) to its corresponding
/// <see cref="CsvMappingOptions"/> definition.
/// </remarks>
public sealed class CsvMappingDictionary : Dictionary<string, CsvMappingOptions>{
}
