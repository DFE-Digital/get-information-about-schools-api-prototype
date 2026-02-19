using DfE.CleanArchitecture.Common.CrossCutting.Mapper;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response.Mappers;

/// <summary>
/// Defines a contract for mapping a model of type <typeparamref name="TModel"/>
/// into one or more CSV rows.
/// </summary>
/// <typeparam name="TModel">
/// The source model type being transformed into CSV output.
/// </typeparam>
/// <remarks>
/// Implementations of this interface are responsible for converting a model
/// into a sequence of string arrays, where each array represents a single CSV row.
/// This allows support for both simple one‑row mappings and expanded multi‑row
/// mappings (e.g., when the model contains collections).
/// </remarks>
public interface ICsvMapper<TModel> : IMapper<TModel, IEnumerable<string[]>>
{
    /// <summary>
    /// Gets the ordered set of column headers that should appear
    /// at the top of the generated CSV output.
    /// </summary>
    /// <remarks>
    /// The number and order of headers must match the number and order
    /// of values produced in each mapped CSV row.
    /// </remarks>
    string[] Headers { get; }
}
