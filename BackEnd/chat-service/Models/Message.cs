using System.ComponentModel.DataAnnotations;

public class Message
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    // todo: currently the name will be saved here, not the id. Changing the parameter will fix it
    public string UserId { get; set; }

    [Required]
    public DateTime Timestamp { get; set; }
}