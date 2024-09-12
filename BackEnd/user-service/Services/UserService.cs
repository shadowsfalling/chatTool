using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace UserService.Services
{
    public class UserService
    {
        private readonly UserDbContext _userDbContext;
        private readonly IModel _channel;
        private readonly IConnection _connection;
         private readonly IServiceScopeFactory _scopeFactory;

        public UserService(IConfiguration configuration, UserDbContext context, IServiceScopeFactory scopeFactory)
        {
            _userDbContext = context;
            _scopeFactory = scopeFactory;

            // Hole RabbitMQ-Konfigurationswerte aus der appsettings.json
            var rabbitMQHost = configuration["RabbitMQ:Host"];
            var rabbitMQPort = int.Parse(configuration["RabbitMQ:Port"]);
            var rabbitMQUsername = configuration["RabbitMQ:Username"];
            var rabbitMQPassword = configuration["RabbitMQ:Password"];

            // Erstelle die RabbitMQ-Verbindung
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQHost,
                Port = rabbitMQPort,
                UserName = rabbitMQUsername,
                Password = rabbitMQPassword
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "userValidationQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        // StartListening Methode für den RabbitMQ-Consumer
       public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var userDbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var validationRequest = JsonConvert.DeserializeObject<UserValidationMessage>(message);

                    // Benutzer validieren
                    var user = await userDbContext.Users.FindAsync(validationRequest.UserId);
                    bool isValid = user != null;

                    var responseBytes = Encoding.UTF8.GetBytes(isValid.ToString());
                    var replyProps = _channel.CreateBasicProperties();
                    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                    _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: replyProps, body: responseBytes);
                }
            };

            _channel.BasicConsume(queue: "userValidationQueue", autoAck: true, consumer: consumer);
            Console.WriteLine("UserService is listening for validation requests...");
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        // Benutzer-Authentifizierungsmethode
        public User Authenticate(string username, string password)
        {
            var user = _userDbContext.Users.SingleOrDefault(u => u.Username == username);
            if (user == null || user.Password != HashPassword(password))
            {
                return null;  // Authentication failed
            }
            return user;  // Authentication successful
        }

        // Hashing-Methode für Passwörter
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}