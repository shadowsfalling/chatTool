using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RoomService.Services
{

    public class RoomService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly RoomDbContext _context;

        public RoomService(IConfiguration configuration, RoomDbContext roomDbContext)
        {
            _context = roomDbContext;
            _configuration = configuration;
            var rabbitMQHost = _configuration["RabbitMQ:Host"];
            var rabbitMQPort = int.Parse(_configuration["RabbitMQ:Port"]!);
            var rabbitMQUsername = _configuration["RabbitMQ:Username"];
            var rabbitMQPassword = _configuration["RabbitMQ:Password"];

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQHost,
                Port = rabbitMQPort,
                UserName = rabbitMQUsername,
                Password = rabbitMQPassword
            };

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

        public async Task<bool> AddUserToRoom(int roomId, int userId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return false; // Raum nicht gefunden
            }

            // Benutzervalidierung über RabbitMQ anfordern
            bool isValidUser = await ValidateUserAsync(userId);
            if (!isValidUser)
            {
                return false; // Benutzer ist ungültig
            }

            var roomUser = new RoomUser
            {
                RoomId = roomId,
                UserId = userId
            };

            _context.RoomUsers.Add(roomUser);
            await _context.SaveChangesAsync();

            return true; // Erfolgreich hinzugefügt
        }

        private async Task<bool> ValidateUserAsync(int userId)
        {
            var tcs = new TaskCompletionSource<bool>();

            // Nachricht an den User-Service senden
            var correlationId = Guid.NewGuid().ToString();
            var replyQueueName = _channel.QueueDeclare().QueueName;
            var consumer = new EventingBasicConsumer(_channel);

            // Antwort-Handler
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var isValid = bool.Parse(response);
                    tcs.SetResult(isValid);
                }
            };

            _channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);

            var message = new { UserId = userId };
            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            _channel.BasicPublish(exchange: "", routingKey: "userValidationQueue", basicProperties: props, body: messageBytes);

            return await tcs.Task;
        }
    }
}