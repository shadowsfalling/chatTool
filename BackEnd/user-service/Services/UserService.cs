using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Services
{
    public class UserService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly UserDbContext _userDbContext;

        public UserService(IConfiguration configuration, UserDbContext context, IServiceScopeFactory scopeFactory)
        {
            _userDbContext = context;
            _scopeFactory = scopeFactory;

            var rabbitMQHost = configuration["RabbitMQ:Host"];
            var rabbitMQPort = int.Parse(configuration["RabbitMQ:Port"]);
            var rabbitMQUsername = configuration["RabbitMQ:Username"];
            var rabbitMQPassword = configuration["RabbitMQ:Password"];

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQHost,
                Port = rabbitMQPort,
                UserName = rabbitMQUsername,
                Password = rabbitMQPassword
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "userQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        BaseMessage baseMessage = JsonConvert.DeserializeObject<BaseMessage>(message);

                        // Dispatcher für bool
                        if (baseMessage.Action == "ValidateUser")
                        {
                            var dispatcher = scope.ServiceProvider.GetRequiredService<MessageDispatcher<bool>>();
                            bool isValid = await dispatcher.DispatchAsync(baseMessage);

                            var response = Encoding.UTF8.GetBytes(isValid.ToString());
                            SendResponse(ea, response);
                        }
                        // Dispatcher für User
                        else if (baseMessage.Action == "CreateUser")
                        {
                            var dispatcher = scope.ServiceProvider.GetRequiredService<MessageDispatcher<User>>();
                            var createdUser = await dispatcher.DispatchAsync(baseMessage);

                            var response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(createdUser));
                            SendResponse(ea, response);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            };

            _channel.BasicConsume(queue: "userQueue", autoAck: true, consumer: consumer);
            Console.WriteLine("UserService is listening for messages...");
        }

        private void SendResponse(BasicDeliverEventArgs ea, byte[] response)
        {
            var props = _channel.CreateBasicProperties();
            props.CorrelationId = ea.BasicProperties.CorrelationId;
            _channel.BasicPublish(exchange: "", routingKey: ea.BasicProperties.ReplyTo, basicProperties: props, body: response);
        }


        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

        // Benutzer-Authentifizierungsmethode
        public User Authenticate(string username, string password)
        {
            if (_userDbContext == null)
            {
                throw new InvalidOperationException("Database context is not initialized.");
            }

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