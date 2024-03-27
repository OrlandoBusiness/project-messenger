using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model;

public class Product
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public virtual Measure Measure { get; set; } = null!;

    public double? Price { get; set; }

    public int? LifeInDays { get; set; }

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public static int Size = sizeof(int) + 100 + Measure.Size + sizeof(decimal) + sizeof(int);

    public static Product GetProduct(byte[] array)
    {
        Product product = new Product();
        int size = 0;

        product.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        product.Name = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());
        size += 100;

        product.Measure = Measure.GetMeasure(array.Skip(size).Take(Measure.Size).ToArray());
        size += Measure.Size;

        product.Price = BitConverter.ToDouble(array.Skip(size).Take(sizeof(double)).ToArray());
        size += sizeof(double);

        product.LifeInDays = BitConverter.ToInt32(array.Skip(size).Take(sizeof(int)).ToArray());
        size += sizeof(int);

        return product;
    }

    public static byte[] GetBytes(Product product)
    {
        List<byte> bytes = new List<byte>(Size);

        byte[] temp = BitConverter.GetBytes(product.Id);
        bytes.AppendBytes(temp);

        temp = Encoding.UTF8.GetBytes(product.Name);
        Array.Resize(ref temp, 100);
        bytes.AppendBytes(temp);

        temp = Measure.GetBytes(product.Measure);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(product.Price.Value);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(product.LifeInDays.Value);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"{Id}: {Name}\n\tMeasure: {Measure}\nPrice: {Price}\nLifeInDays: {LifeInDays}";
}