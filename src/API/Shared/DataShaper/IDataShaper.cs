namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.DataShaper;

/// <summary>
/// Provides functionality for shaping objects of type <typeparamref name="TDataObject"/>
/// into dynamic projections that include only the requested fields.
/// </summary>
/// <typeparam name="TDataObject">
/// The source data type from which shaped projections are created.
/// </typeparam>
public interface IDataShaper<TDataObject>
{
    /// <summary>
    /// Shapes a collection of data objects by selecting only the specified fields.
    /// </summary>
    /// <param name="dataObjects">
    /// The collection of source objects to shape.
    /// </param>
    /// <param name="fields">
    /// A comma-separated list of field names to include in the shaped output.
    /// If <c>null</c> or empty, all public properties are included.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a sequence
    /// of <see cref="ShapedEntity"/> instances with only the requested fields.
    /// </returns>
    Task<IEnumerable<ShapedEntity>> ShapeDataAsync(
        IEnumerable<TDataObject> dataObjects, string? fields);

    /// <summary>
    /// Shapes a single data object by selecting only the specified fields.
    /// </summary>
    /// <param name="dataObject">
    /// The source object to shape.
    /// </param>
    /// <param name="fields">
    /// A comma-separated list of field names to include in the shaped output.
    /// If <c>null</c> or empty, all public properties are included.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing a
    /// <see cref="ShapedEntity"/> with only the requested fields.
    /// </returns>
    Task<ShapedEntity> ShapeDataAsync(
        TDataObject dataObject, string? fields);
}
