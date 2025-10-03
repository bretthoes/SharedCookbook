namespace SharedCookbook.Application.Common.Interfaces;

public interface IQuery<out T> : IRequest<T>;
