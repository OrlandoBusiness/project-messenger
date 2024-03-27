using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model;

public class Supplier
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    public string Password { get; set; } = null!;

    public float? Rating { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public static int Size => sizeof(int) + 100 + 100 + sizeof(float);

    public static Supplier GetSupplier(byte[] array)
    {
        Supplier supplier = new Supplier();
        int size = 0;

        supplier.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        supplier.Name = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());
        size += 100;

        supplier.Password = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());
        size += 100;

        supplier.Rating = BitConverter.ToInt32(array.Skip(size).Take(sizeof(float)).ToArray());

        return supplier;
    }

    public static byte[] GetBytes(Supplier storage)
    {
        List<byte> bytes = new List<byte>(Size);

        byte[] temp = BitConverter.GetBytes(storage.Id);
        bytes.AppendBytes(temp);

        temp = Encoding.UTF8.GetBytes(storage.Name);
        Array.Resize(ref temp, 100);
        bytes.AppendBytes(temp);

        temp = Encoding.UTF8.GetBytes(storage.Password);
        Array.Resize(ref temp, 100);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(storage.Rating.Value);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"{Id}: {Name} | {Password}\nRating: {Rating}";
}
