using RabbitMQ.Client;
using System.Text;

namespace ChatService.Services
{
    public class ChatService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ChatService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "RoomServiceQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessageToRoomService(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "RoomServiceQueue", basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}