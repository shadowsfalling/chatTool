using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class CreateUserHandler : IMessageHandler<User>
{
    private readonly UserDbContext _userDbContext;

    public CreateUserHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<User> HandleAsync(string payload)
    {
        var createUserRequest = JsonConvert.DeserializeObject<CreateUserMessage>(payload);

        var user = new User
        {
            Username = createUserRequest.Username,
            Password = createUserRequest.Password
        };

        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();

        Console.WriteLine($"User {createUserRequest.Username} created.");

        return user;
    }
}