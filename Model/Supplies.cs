using System.ComponentModel.DataAnnotations;

namespace Model;

public class Supplies
{
    [Key]
    public int Id { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;

    public int Count { get; set; }

    public decimal? Price { get; set; }

    public DateTime ShippingDate { get; set; }

    public DateTime ArrivalDate { get; set; }
}
