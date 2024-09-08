using System.ComponentModel.DataAnnotations;

public class RoomUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }
    
    [Required]
    public int UserId { get; set; }
}