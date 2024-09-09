using Microsoft.EntityFrameworkCore;

public class RoomDbContext : DbContext
{
    public RoomDbContext(DbContextOptions<RoomDbContext> options) : base(options)
    {
        Database.Migrate();
    }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomUser> RoomUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hier die Entitäten und Beziehungen konfigurieren
    }
}