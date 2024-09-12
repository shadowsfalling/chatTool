public class MessageDispatcher<T>
{
    private readonly IServiceProvider _serviceProvider;

    public MessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<T> DispatchAsync(BaseMessage message)
    {
        var handler = _serviceProvider.GetService<IMessageHandler<T>>();
        if (handler == null)
        {
            throw new InvalidOperationException("No handler found for action");
        }

        return await handler.HandleAsync(message.Payload);
    }
}