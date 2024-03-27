using System.ComponentModel.DataAnnotations;

namespace Model;

public class Supplie
{
    [Key]
    public int Id { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;

    public int Count { get; set; }

    public double? Price { get; set; }

    public DateTime ShippingDate { get; set; }

    public DateTime ArrivalDate { get; set; }

    public string Show => $"Id: {Id} {Product.Name} {Supplier.Name} {Count}";

    public static int Size => sizeof(int) + Product.Size + Supplier.Size + Storage.Size + sizeof(int) + sizeof(double) + Extentions.DateSize + Extentions.DateSize;

    public static Supplie GetSupplie(byte[] array)
    {
        Supplie supplie = new Supplie();
        int size = 0;

        supplie.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        supplie.Product = Product.GetProduct(array.Skip(size).Take(Product.Size).ToArray());
        size += Product.Size;

        supplie.Supplier = Supplier.GetSupplier(array.Skip(size).Take(Supplier.Size).ToArray());
        size += Supplier.Size;

        supplie.Storage = Storage.GetStorage(array.Skip(size).Take(Storage.Size).ToArray());
        size += Storage.Size;

        supplie.Count = BitConverter.ToInt32(array.Skip(size).Take(sizeof(int)).ToArray());
        size += sizeof(int);

        supplie.Price = BitConverter.ToDouble(array.Skip(size).Take(sizeof(double)).ToArray());
        size += sizeof(double);

        supplie.ShippingDate = Extentions.GetDateTime(array.Skip(size).Take(Extentions.DateSize).ToArray());
        size += Extentions.DateSize;

        supplie.ArrivalDate = Extentions.GetDateTime(array.Skip(size).Take(Extentions.DateSize).ToArray());

        return supplie;
    }

    public static byte[] GetBytes(Supplie supplie)
    {
        List<byte> bytes = new List<byte>(Size);

        byte[] temp = BitConverter.GetBytes(supplie.Id);
        bytes.AppendBytes(temp);

        temp = Product.GetBytes(supplie.Product);
        bytes.AppendBytes(temp);

        temp = Supplier.GetBytes(supplie.Supplier);
        bytes.AppendBytes(temp);

        temp = Storage.GetBytes(supplie.Storage);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(supplie.Count);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes(supplie.Price.Value);
        bytes.AppendBytes(temp);

        temp = Extentions.GetBytesDateTime(supplie.ShippingDate);
        bytes.AppendBytes(temp);

        temp = Extentions.GetBytesDateTime(supplie.ArrivalDate);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"\t\tSupplie Id: {Id}\n{Product}\n{Supplier}\n{Storage}\n\nCount: {Count}\nPrice: {Price}\nShipping Date: {ShippingDate/*ToShortDateString()*/}\nArrival Date: {ArrivalDate/*ToShortDateString()*/}";
}
