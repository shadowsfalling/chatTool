using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace RoomService.Services
{
    public class RoomService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RoomDbContext _context;

        public RoomService(IConnection connection, RoomDbContext roomDbContext)
        {
            _connection = connection;
            _context = roomDbContext;

            // Stelle sicher, dass die Queue existiert
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "RoomServiceQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public async Task<bool> AddUserToRoom(int roomId, int userId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return false; // Raum nicht gefunden
            }

            // Sende Benutzervalidierung an den UserService
            bool isValidUser = await SendUserValidationMessage(userId);
            if (!isValidUser)
            {
                return false; // Benutzer ungültig
            }

            // Benutzer zum Raum hinzufügen
            var roomUser = new RoomUser
            {
                RoomId = roomId,
                UserId = userId
            };

            _context.RoomUsers.Add(roomUser);
            await _context.SaveChangesAsync();

            return true; // Erfolgreich hinzugefügt
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received: {message}");
            };

            _channel.BasicConsume(queue: "RoomServiceQueue", autoAck: true, consumer: consumer);
            Console.WriteLine("RoomService is listening for messages...");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        private async Task<bool> SendUserValidationMessage(int userId)
        {
            var tcs = new TaskCompletionSource<bool>();

            var message = new UserValidationMessage
            {
                Action = "ValidateUser",
                Payload = JsonConvert.SerializeObject(new { UserId = userId }) 
            };

            var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = _channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = _channel.QueueDeclare().QueueName;

            _channel.BasicPublish(exchange: "", routingKey: "userQueue", basicProperties: props, body: messageBytes);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId == props.CorrelationId)
                {
                    var response = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var isValid = bool.Parse(response);
                    tcs.SetResult(isValid);
                }
            };

            _channel.BasicConsume(queue: props.ReplyTo, autoAck: true, consumer: consumer);
            Console.WriteLine("Send message");

            return await tcs.Task;
        }
    }
}