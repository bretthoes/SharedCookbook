namespace SharedCookbook.Application.Common.Interfaces;

public interface ICommand<out T> : IRequest<T>;
