#if DEBUG
using Microsoft.AspNetCore.Mvc;
using UserService.Services;

[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    private readonly SeederService _seederService;

    public DatabaseController(SeederService seederService)
    {
        _seederService = seederService;
    }

    // Endpunkt zum Seedern der Datenbank
    [HttpPost("seed")]
    public IActionResult SeedDatabase()
    {
        try
        {
            _seederService.Seed();
            return Ok("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while seeding the database: {ex.Message}");
        }
    }

    // Endpunkt zum Zurücksetzen der Datenbank
    [HttpPost("reset")]
    public IActionResult ResetDatabase()
    {
        try
        {
            _seederService.ResetDatabase();
            return Ok("Database reset successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred while resetting the database: {ex.Message}");
        }
    }
}
#endif