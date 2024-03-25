using System.ComponentModel.DataAnnotations;

namespace Model;

public class Product
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public virtual Measure Measure { get; set; } = null!;

    public decimal? Price { get; set; }

    public int? LifeInDays { get; set; }

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
}