using System.ComponentModel.DataAnnotations;

public class CreateRoomDto
{
    [Required]
    public string Name { get; set; }

    public CreateRoomDto(string name)
    {
        Name = name;
    }
}