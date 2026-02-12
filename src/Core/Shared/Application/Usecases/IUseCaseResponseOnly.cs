namespace DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;

/// <summary>
/// Represents a use case that does not require an input request object,
/// but produces a response of type <typeparamref name="TUseCaseResponse"/>.
/// </summary>
/// <typeparam name="TUseCaseResponse">
/// The type of the response returned when the use case is executed.
/// Must be a reference type.
/// </typeparam>
/// <remarks>
/// This interface is intended for simple operations where no request data
/// is needed to perform the use case. Implementations should encapsulate
/// all required behaviour internally.
/// </remarks>
public partial interface IUseCaseResponseOnly<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Executes the use case and returns a response of type
    /// <typeparamref name="TUseCaseResponse"/>.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the
    /// <typeparamref name="TUseCaseResponse"/> produced by the use case.
    /// </returns>
    /// <remarks>
    /// Implementations should ensure that all required logic is performed
    /// within this method, as no request object is provided.
    /// </remarks>
    Task<TUseCaseResponse> HandleRequestAsync(
        CancellationToken cancellationToken = default);
}
