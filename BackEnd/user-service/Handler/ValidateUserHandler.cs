using Newtonsoft.Json;

public class ValidateUserHandler : IMessageHandler<bool>
{
    private readonly UserDbContext _userDbContext;

    public ValidateUserHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }

    public async Task<bool> HandleAsync(string payload)
    {
        Console.WriteLine("ValidateUserHandler: payload", payload);

        var validationRequest = JsonConvert.DeserializeObject<UserValidationMessage>(payload);
        var user = await _userDbContext.Users.FindAsync(validationRequest.UserId);

        bool isValid = user != null;
        Console.WriteLine($"User validation result: {isValid}");

        return isValid;
    }
}