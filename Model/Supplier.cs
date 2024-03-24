using System.ComponentModel.DataAnnotations;

namespace Model;

public class Supplier
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string Name { get; set; } = null!;

    public float? Rating { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
