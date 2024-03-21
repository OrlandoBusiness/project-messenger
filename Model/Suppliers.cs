using System.ComponentModel.DataAnnotations;

namespace Model;

public class Suppliers
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(255)]
    public string Type { get; set; } = null!;

    public int? EmployeeCount { get; set; }
}
