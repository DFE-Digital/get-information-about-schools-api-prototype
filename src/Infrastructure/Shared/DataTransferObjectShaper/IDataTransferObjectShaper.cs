namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

/// <summary>
/// Defines a contract for shaping objects of type <typeparamref name="TDataObject"/> 
/// into new instances of <typeparamref name="TDataObject"/> that contain only the
/// requested fields. Unselected fields are left at their default values.
/// </summary>
/// <typeparam name="TDataObject">
/// The source type being shaped.
/// </typeparam>
public interface IDataTransferObjectShaper<TDataObject>
{
    /// <summary>
    /// Shapes a collection of objects by selecting only the specified fields.
    /// A new instance of <typeparamref name="TDataObject"/> is created for each
    /// source object, populated only with the requested fields.
    /// </summary>
    /// <param name="dataObjects">The source objects to shape.</param>
    /// <param name="fields">
    /// A <see cref="HashSet{T}"/> containing the field names to include in the shaped output.
    /// If the set is null or contains no values, all public properties of 
    /// <typeparamref name="TDataObject"/> are included. A hash set is used to enable
    /// efficient lookups when determining whether a property should be included.
    /// </param>
    /// <returns>
    /// A sequence of shaped <typeparamref name="TDataObject"/> instances.
    /// </returns>
    Task<IEnumerable<TDataObject>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects,
        HashSet<string> fields);

    /// <summary>
    /// Shapes a single object by selecting only the specified fields.
    /// A new instance of <typeparamref name="TDataObject"/> is created and
    /// populated only with the requested fields.
    /// </summary>
    /// <param name="dataObject">The source object to shape.</param>
    /// <param name="fields">
    /// A <see cref="HashSet{T}"/> containing the field names to include in the shaped output.
    /// If the set is null or contains no values, all public properties of 
    /// <typeparamref name="TDataObject"/> are included. A hash set is used to enable
    /// efficient, case‑insensitive membership checks during shaping.
    /// </param>
    /// <returns>
    /// A shaped <typeparamref name="TDataObject"/> instance.
    /// </returns>
    Task<TDataObject> ShapeDataAsync(
        TDataObject dataObject,
        HashSet<string> fields);
}
