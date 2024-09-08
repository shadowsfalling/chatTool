using Microsoft.EntityFrameworkCore;

public class RoomDbContext : DbContext
{
    public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options) {}

    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomUser> RoomUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entitäten konfigurieren
    }
}