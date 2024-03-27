using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace Model;

public class Order
{
    [Key]
    public int Id { get; set; }

    public virtual Product Product { get; set; } = null!;

    public int Count { get; set; }

    public bool IsActive { get; set; } = true;

    public override string ToString() => $"{Product.Name}: {Count}";
  
    public string Show => $"Id: {Id} {Product.Name} {Count}";

    public static int Size => sizeof(int) + Product.Size + sizeof(int) + sizeof(bool);

    public static Order GetOrder(byte[] array)
    {
        Order order = new Order();
        int size = 0;

        order.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        order.Product = Product.GetProduct(array.Skip(size).Take(Product.Size).ToArray());
        size += Product.Size;

        order.Count = BitConverter.ToInt32(array.Skip(size).Take(sizeof(int)).ToArray());
        size += sizeof(int);

        order.IsActive = BitConverter.ToBoolean(array.Skip(size).Take(sizeof(bool)).ToArray());

        return order;
    }

    public static byte[] GetBytes(Order order)
    {
        List<byte> bytes = new List<byte>(Size);

        byte[] temp = BitConverter.GetBytes(order.Id);
        bytes.AppendBytes(temp);

        temp = Product.GetBytes(order.Product);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(order.Count);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(order.IsActive);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"{Id}: {Product}\nCount: {Count}\nActive: {IsActive}";
}