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

        StreamWriter writer =
            new(response.Body, Encoding.UTF8, leaveOpen: false);

        await using (writer)
        {
            // Write header row
            string headerLine = string.Join(Delimiter, headerColumns);
            await writer.WriteLineAsync(headerLine);

            // Write each data row (may be multiple rows per model)
            foreach (TRowType row in rows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (string[] fields in rowSelector(row))
                {
                    string line = BuildLine(fields);
                    await writer.WriteLineAsync(line);
                }

                await writer.FlushAsync(cancellationToken);
            }
        }

        return new EmptyResult();
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
    /// Wraps fields in quotes if they contain commas, quotes, or newline characters.
    /// </summary>
    /// <param name="value">The field value to escape.</param>
    /// <returns>A safely escaped CSV field.</returns>
    private static string Escape(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        string escaped =
            value.Replace(Quote.ToString(), $"{Quote}{Quote}");

        return (escaped.Contains(Delimiter) ||
            escaped.Contains(Quote) ||
            escaped.Contains('\n')) ?
                $"{Quote}{escaped}{Quote}" : escaped;
    }
}
