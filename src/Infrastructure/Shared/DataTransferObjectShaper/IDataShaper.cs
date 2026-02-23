namespace DfE.GetInformationAboutSchools.Prototyping.Infrastructure.Shared.DataTransferObjectShaper;

/// <summary>
/// Defines a contract for shaping objects of type <typeparamref name="TDataObject"/> 
/// into new instances of <typeparamref name="TDataObject"/> that contain only the
/// requested fields. Unselected fields are left at their default values.
/// </summary>
/// <typeparam name="TDataObject">
/// The source type being shaped.
/// </typeparam>
public interface IDataShaper<TDataObject>
{
    /// <summary>
    /// Shapes a collection of objects by selecting only the specified fields.
    /// A new instance of <typeparamref name="TDataObject"/> is created for each
    /// source object, populated only with the requested fields.
    /// </summary>
    /// <param name="dataObjects">The source objects to shape.</param>
    /// <param name="fields">
    /// A comma-separated list of field names to include. If null or empty,
    /// all public properties are included.
    /// </param>
    /// <returns>
    /// A sequence of shaped <typeparamref name="TDataObject"/> instances.
    /// </returns>
    Task<IEnumerable<TDataObject>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects,
        string? fields);

    /// <summary>
    /// Shapes a single object by selecting only the specified fields.
    /// A new instance of <typeparamref name="TDataObject"/> is created and
    /// populated only with the requested fields.
    /// </summary>
    /// <param name="dataObject">The source object to shape.</param>
    /// <param name="fields">
    /// A comma-separated list of field names to include. If null or empty,
    /// all public properties are included.
    /// </param>
    /// <returns>
    /// A shaped <typeparamref name="TDataObject"/> instance.
    /// </returns>
    Task<TDataObject> ShapeDataAsync(
        TDataObject dataObject,
        string? fields);
}
