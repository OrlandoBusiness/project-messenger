using System.ComponentModel.DataAnnotations;

namespace Model;

public class StorageProduct
{
    [Key]
    public int Id { get; set; }

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public int Count { get; set; }
}
