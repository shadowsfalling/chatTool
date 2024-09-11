using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RoomService.Services
{

    public class RoomService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RoomService()
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "RoomServiceQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                // Handle the message and notify all users in the room if needed
            };

            _channel.BasicConsume(queue: "RoomServiceQueue", autoAck: true, consumer: consumer);
            Console.WriteLine(" [*] Waiting for messages.");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}