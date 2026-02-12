using DfE.CleanArchitecture.Common.Application;

namespace DfE.GetInformationAboutSchools.Prototyping.Core.Shared.Application.Usecases;

/// <summary>
/// Defines a use case that accepts an input request object and produces
/// a response of type <typeparamref name="TUseCaseResponse"/>.
/// </summary>
/// <typeparam name="TUseCaseRequest">
/// The type of the request (input port) passed to the use case. This must
/// implement <see cref="IUseCaseRequest{TUseCaseResponse}"/> to ensure that
/// the request is compatible with the expected response type.
/// </typeparam>
/// <typeparam name="TUseCaseResponse">
/// The type of the response returned when the use case is executed.
/// Must be a reference type.
/// </typeparam>
/// <remarks>
/// This interface represents the standard request–response pattern used
/// throughout the application layer. Implementations should contain all
/// business logic required to process the request and produce a valid
/// response. Validation of the request may be performed internally or
/// delegated to dedicated validators.
/// </remarks>
public interface IUseCase<in TUseCaseRequest, TUseCaseResponse>
    where TUseCaseRequest : IUseCaseRequest<TUseCaseResponse>
    where TUseCaseResponse : class
{
    /// <summary>
    /// Executes the use case using the supplied request and returns a
    /// response of type <typeparamref name="TUseCaseResponse"/>.
    /// </summary>
    /// <param name="request">
    /// The input request containing all data required to perform the use case.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the
    /// <typeparamref name="TUseCaseResponse"/> produced by the use case.
    /// </returns>
    /// <remarks>
    /// Implementations should ensure that all required business logic is
    /// executed within this method. If the request is invalid, implementations
    /// may throw an appropriate exception or return a response indicating
    /// failure, depending on the application's error-handling strategy.
    /// </remarks>
    Task<TUseCaseResponse> HandleRequestAsync(
        TUseCaseRequest request,
        CancellationToken cancellationToken = default);
}
