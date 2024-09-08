using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Room
{
    [Key] 
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [DefaultValue(true)]
    public bool IsPrivate { get; set; }
}