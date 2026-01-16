namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Marker interface for requests without a response
/// </summary>
public interface IBaseRequest;

/// <summary>
/// Marker interface for requests with a response
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the handler</typeparam>
public interface IRequest<out TResponse> : IBaseRequest;

/// <summary>
/// Marker interface for requests without a response (returns Unit)
/// </summary>
public interface IRequest : IRequest<Unit>;
