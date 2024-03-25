using System.ComponentModel.DataAnnotations;

namespace Model;

public class Measure
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;
}