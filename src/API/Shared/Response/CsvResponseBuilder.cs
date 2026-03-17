using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DfE.GetInformationAboutSchools.Prototyping.API.Shared.Response;

/// <summary>
/// Provides functionality for writing CSV responses directly to an HTTP response stream.
/// Encapsulates header configuration, row writing, escaping, and streaming behaviour.
/// </summary>
public class CsvResponseBuilder : ICsvResponseBuilder
{
    /// <summary>
    /// The MIME content type used for CSV responses.
    /// Includes UTF‑8 charset to ensure correct encoding across all clients.
    /// </summary>
    private const string ContentType = "text/csv; charset=utf-8";

    /// <summary>
    /// The HTTP header key used to instruct the browser to download the file
    /// rather than attempt to display it inline.
    /// </summary>
    private const string ContentDispositionHeader = "Content-Disposition";

    /// <summary>
    /// The delimiter used between CSV fields.
    /// RFC 4180 specifies a comma as the standard delimiter.
    /// </summary>
    private const char Delimiter = ',';

    /// <summary>
    /// The character used to wrap fields that contain special characters
    /// such as commas, quotes, or newline characters.
    /// </summary>
    private const char Quote = '"';

    /// <summary>
    /// Sentinel value used across DTOs to represent missing or undefined data.
    /// When encountered, it is treated as an empty CSV field.
    /// </summary>
    private const string Undefined = "UNDEFINED";

    /// <summary>
    /// Writes a CSV file to the HTTP response stream using the supplied rows and column mapping.
    /// </summary>
    /// <typeparam name="TRowType">The type of the row model.</typeparam>
    /// <param name="response">The HTTP response to write the CSV output to.</param>
    /// <param name="rows">The collection of rows to output.</param>
    /// <param name="headerColumns">The CSV header column names.</param>
    /// <param name="rowSelector">
    /// A function that maps a row model to one or more arrays of string fields
    /// representing CSV rows.
    /// </param>
    /// <param name="fileName">The filename to present to the client when downloading.</param>
    /// <param name="cancellationToken">A cancellation token for cooperative cancellation.</param>
    /// <returns>An <see cref="IActionResult"/> indicating completion.</returns>
    public async Task<IActionResult> WriteCsvAsync<TRowType>(
        HttpResponse response,
        IEnumerable<TRowType> rows,
        IEnumerable<string> headerColumns,
        Func<TRowType, IEnumerable<string[]>> rowSelector,
        string fileName,
        CancellationToken cancellationToken)
    {
        response.ContentType = ContentType;
        response.Headers[ContentDispositionHeader] = $"attachment; filename=\"{fileName}\"";

        // Materialise all row data so we can inspect columns before writing
        List<string[]> rowData = rows
            .SelectMany(rowSelector)
            .ToList();

        // Determine which columns contain at least one meaningful value
        HashSet<int> includedColumnIndexes = GetIncludedColumnIndexes(rowData);

        // Filter header columns based on included indexes
        List<string> filteredHeaders = headerColumns
            .Where((_, index) => includedColumnIndexes.Contains(index))
            .ToList();

        StreamWriter writer = new(response.Body, Encoding.UTF8, leaveOpen: false);

        await using (writer)
        {
            // Write header row
            string headerLine = string.Join(Delimiter, filteredHeaders);
            await writer.WriteLineAsync(headerLine);

            // Write each filtered data row
            foreach (string[] fields in rowData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string[] filteredFields = fields
                    .Where((_, index) => includedColumnIndexes.Contains(index))
                    .ToArray();

                string line = BuildLine(filteredFields);
                await writer.WriteLineAsync(line);
            }

            await writer.FlushAsync(cancellationToken);
        }

        return new EmptyResult();
    }

    /// <summary>
    /// Determines which column indexes contain at least one meaningful value.
    /// A column is excluded if all values are null, whitespace, or "UNDEFINED".
    /// </summary>
    /// <param name="rows">The materialised CSV rows.</param>
    /// <returns>A set of column indexes that should be included in the output.</returns>
    private static HashSet<int> GetIncludedColumnIndexes(List<string[]> rows)
    {
        HashSet<int> included = new();

        if (rows.Count == 0)
        {
            return included;
        }

        int columnCount = rows[0].Length;

        for (int col = 0; col < columnCount; col++)
        {
            bool hasMeaningfulValue = rows.Any(row =>
                !string.IsNullOrWhiteSpace(row[col]) &&
                !string.Equals(row[col], Undefined, StringComparison.OrdinalIgnoreCase));

            if (hasMeaningfulValue)
            {
                included.Add(col);
            }
        }

        return included;
    }

    /// <summary>
    /// Builds a CSV line from the supplied fields, applying escaping rules as required.
    /// </summary>
    /// <param name="fields">The fields to include in the CSV row.</param>
    /// <returns>A correctly formatted CSV line.</returns>
    private static string BuildLine(params string?[] fields)
    {
        StringBuilder builder = new();

        for (int i = 0; i < fields.Length; i++)
        {
            if (i > 0)
            {
                builder.Append(Delimiter);
            }

            builder.Append(Escape(fields[i]));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Escapes a CSV field according to RFC 4180 rules.
    /// Also treats the sentinel value <c>UNDEFINED</c> as an empty field.
    /// </summary>
    /// <param name="value">The field value to escape.</param>
    /// <returns>A safely escaped CSV field.</returns>
    private static string Escape(string? value)
    {
        // Treat null, whitespace, or UNDEFINED as empty
        if (string.IsNullOrWhiteSpace(value) ||
            string.Equals(value, Undefined, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        // Escape embedded quotes
        string escaped = value.Replace(Quote.ToString(), $"{Quote}{Quote}");

        // Wrap in quotes if required
        return (escaped.Contains(Delimiter) ||
                escaped.Contains(Quote) ||
                escaped.Contains('\n'))
            ? $"{Quote}{escaped}{Quote}"
            : escaped;
    }
}
