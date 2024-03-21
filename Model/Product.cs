using System.ComponentModel.DataAnnotations;

namespace Model;

public class Product
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public int Count { get; set; }

    public decimal Cost { get; set; }
}