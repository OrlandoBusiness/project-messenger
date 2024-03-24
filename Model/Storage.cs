using System.ComponentModel.DataAnnotations;

namespace Model;

public class Storage
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public int? MaxCapacity { get; set; }
}
