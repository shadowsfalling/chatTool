public interface IMessageHandler<T>
{
    Task<T> HandleAsync(string payload);
}