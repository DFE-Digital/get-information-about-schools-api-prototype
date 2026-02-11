using DfE.CleanArchitecture.Common.Domain;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Establishments.Application.Model;

/// <summary>
/// Represents the status of an establishment as a domain value object.
/// </summary>
/// <remarks>
/// <para>
/// Although the values resemble an enumeration, they are modelled as a
/// <see cref="ValueObject"/> to allow richer domain meaning, future extension,
/// and behaviour such as validation or transition rules.
/// </para>
/// <para>
/// Equality is based solely on the <see cref="Code"/> property, ensuring that
/// two instances with the same code represent the same conceptual status.
/// </para>
/// </remarks>
public sealed class EstablishmentStatus : ValueObject<EstablishmentStatus>
{
    /// <summary>
    /// Gets the unique numeric code representing the status.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Gets the human‑readable name of the status.
    /// </summary>
    public string Name { get; }

    private EstablishmentStatus(int code, string name)
    {
        Code = code;
        Name = name;
    }

    /// <summary>
    /// The display name used for establishments that are currently open.
    /// </summary>
    private const string OpenName = "Open";

    /// <summary>
    /// The display name used for establishments that are closed.
    /// </summary>
    private const string ClosedName = "Closed";

    /// <summary>
    /// The display name used for establishments that are open but have been
    /// formally proposed for closure.
    /// </summary>
    private const string OpenProposedToCloseName = "Open, but proposed to close";

    /// <summary>
    /// The display name used for establishments that are proposed to open
    /// but are not yet operational.
    /// </summary>
    private const string ProposedToOpenName = "Proposed to open";

    /// <summary>
    /// Represents an establishment that is currently open.
    /// </summary>
    public static readonly EstablishmentStatus Open =
        new(1, OpenName);

    /// <summary>
    /// Represents an establishment that is closed.
    /// </summary>
    public static readonly EstablishmentStatus Closed =
        new(2, ClosedName);

    /// <summary>
    /// Represents an establishment that is open but proposed for closure.
    /// </summary>
    public static readonly EstablishmentStatus OpenProposedToClose =
        new(3, OpenProposedToCloseName);

    /// <summary>
    /// Represents an establishment that is proposed to open.
    /// </summary>
    public static readonly EstablishmentStatus ProposedToOpen =
        new(4, ProposedToOpenName);

    /// <summary>
    /// Returns the <see cref="EstablishmentStatus"/> instance that corresponds
    /// to the supplied status code.
    /// </summary>
    /// <param name="code">The numeric status code (1–4).</param>
    /// <returns>
    /// The matching <see cref="EstablishmentStatus"/> instance.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the supplied code does not correspond to a known status.
    /// </exception>
    public static EstablishmentStatus Create(int code) =>
        code switch
        {
            1 => Open,
            2 => Closed,
            3 => OpenProposedToClose,
            4 => ProposedToOpen,
            _ => throw new ArgumentOutOfRangeException(
                nameof(code),
                $"Unknown establishment status code '{code}'.")
        };

    /// <summary>
    /// Defines equality based on the status code.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Code;
    }
}
