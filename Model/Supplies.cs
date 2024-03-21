using System.ComponentModel.DataAnnotations;

namespace Model;

public class Supplies
{
    [Key]
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public int Volume { get; set; }

    public decimal SaleCost { get; set; }
}
