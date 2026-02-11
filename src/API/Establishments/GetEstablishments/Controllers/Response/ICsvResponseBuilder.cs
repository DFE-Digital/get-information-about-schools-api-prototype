using Microsoft.AspNetCore.Mvc;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Establishments.GetEstablishments.Controllers.Response;

/// <summary>
/// Defines a service capable of writing CSV content directly to an HTTP response stream.
/// Implementations handle header configuration, row formatting, escaping, and streaming.
/// </summary>
public interface ICsvResponseBuilder
{
    /// <summary>
    /// Writes a CSV file to the HTTP response stream using the supplied rows and column mapping.
    /// </summary>
    /// <typeparam name="T">The type of the objects being written as CSV rows.</typeparam>
    /// <param name="response">
    /// The <see cref="HttpResponse"/> to which the CSV output will be written.
    /// </param>
    /// <param name="rows">
    /// The collection of data rows to be written to the CSV output.
    /// Each item is transformed into a CSV row using <paramref name="rowSelector"/>.
    /// </param>
    /// <param name="headerColumns">
    /// The column names to write as the first row of the CSV file.
    /// These appear in the order provided.
    /// </param>
    /// <param name="rowSelector">
    /// A function that maps a row object of type <typeparamref name="T"/> into
    /// an ordered array of string fields representing a single CSV row.
    /// </param>
    /// <param name="fileName">
    /// The filename presented to the client when downloading the CSV file.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> used to observe cancellation requests
    /// during the streaming process.
    /// </param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating that the CSV response has been written.
    /// Typically returns an <see cref="EmptyResult"/>.
    /// </returns>
    Task<IActionResult> WriteCsvAsync<T>(
        HttpResponse response,
        IEnumerable<T> rows,
        IEnumerable<string> headerColumns,
        Func<T, string[]> rowSelector,
        string fileName,
        CancellationToken cancellationToken);
}
