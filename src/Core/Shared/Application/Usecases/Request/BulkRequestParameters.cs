using DfE.CleanArchitecture.Common.Application;
using DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

/// <summary>
/// Represents the base parameters for a bulk request operation,
/// including validation of required field names and cloning support.
/// </summary>
/// <typeparam name="TResponseObject">
/// The type of the response object returned by the use case.
/// </typeparam>
public abstract partial class BulkRequestParameters<TResponseObject> :
    IUseCaseRequest<UseCaseResponse<TResponseObject>>
{
    /// <summary>
    /// Gets the collection of required field names for the bulk request.
    /// Field names are validated and stored as an immutable array.
    /// </summary>
    public ImmutableArray<string> RequiredFields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BulkRequestParameters{TResponseObject}"/> class.
    /// Validates the provided field names and stores them immutably.
    /// </summary>
    /// <param name="requiredFields">The field names required for the request.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="requiredFields"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the collection is empty or contains invalid field names.
    /// </exception>
    protected BulkRequestParameters(IEnumerable<string> requiredFields)
    {
        ArgumentNullException.ThrowIfNull(requiredFields);

        var validated = new List<string>();

        foreach (var field in requiredFields)
        {
            if (string.IsNullOrWhiteSpace(field)){
                throw new ArgumentException(
                    "Field names cannot be empty or whitespace.");
            }

            if (!IsValidField(field)){
                throw new ArgumentException(
                    $"Invalid field name '{field}'. Only letters, digits, and underscores are allowed, and it must start with a letter.");
            }

            validated.Add(field);
        }

        if (validated.Count == 0){
            throw new ArgumentException(
                "At least one required field must be specified.");
        }

        RequiredFields = [.. validated];
    }

    /// <summary>
    /// Determines whether a field name matches the allowed pattern.
    /// Field names must start with a letter and may contain letters,
    /// digits, or underscores.
    /// </summary>
    /// <param name="field">The field name to validate.</param>
    /// <returns>
    /// <c>true</c> if the field name is valid; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidField(string field) =>
        RequiredFieldsValidation().IsMatch(field);

    /// <summary>
    /// The regular expression pattern used to validate required field names.
    /// Field names must start with a letter and may contain letters,
    /// digits, or underscores.
    /// </summary>
    private const string RequiredFieldsPattern = @"^[A-Za-z][A-Za-z0-9_]*$";

    /// <summary>
    /// Creates a compiled regular expression used to validate required field names.
    /// This method is generated at compile time for performance.
    /// </summary>
    /// <returns>
    /// A compiled <see cref="Regex"/> instance that validates field names.
    /// </returns>
    [GeneratedRegex(RequiredFieldsPattern)]
    private static partial Regex RequiredFieldsValidation();
}
