using System.ComponentModel.DataAnnotations;

public class Message
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Content { get; set; }

    [Required]
    public int RoomId {get; set;}

    [Required]
    public DateTime Timestamp { get; set; }
}