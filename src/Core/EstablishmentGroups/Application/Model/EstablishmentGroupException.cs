namespace DfE.GetInformationAboutSchools.Prototyping.Core.EstablishmentGroups.Application.Model;

/// <summary>
/// Exception type used for group‑related validation or domain errors.
/// </summary>
internal sealed class EstablishmentGroupException : ApplicationException
{
    /// <summary>
    /// Creates a new <see cref="EstablishmentGroupException"/> with a message describing the error.
    /// </summary>
    /// <param name="message">A description of the validation or domain failure.</param>
    public EstablishmentGroupException(string message)
        : base(message){
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentGroupException"/> with a message and an inner exception.
    /// </summary>
    /// <param name="message">A description of the validation or domain failure.</param>
    /// <param name="innerException">The underlying exception that caused this error.</param>
    public EstablishmentGroupException(string message, Exception innerException)
        : base(message, innerException){
    }
}
