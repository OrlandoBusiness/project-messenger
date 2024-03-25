using System.ComponentModel.DataAnnotations;

namespace Model;

public class Order
{
    [Key]
    public int Id { get; set; }

    public virtual Product Product { get; set; } = null!;

    public int Count { get; set; }

    public bool IsActive { get; set; } = true;
}