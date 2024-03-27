using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Model;

public class Storage
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(100)]
    public string Password { get; set; } = null!;

    public int? MaxCapacity { get; set; }

    public static int Size => sizeof(int) + 100 + 100 + sizeof(int);

    public static Storage GetStorage(byte[] array)
    {
        Storage storage = new Storage();
        int size = 0;

        storage.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        storage.Name = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());
        size += 100;

        storage.Password = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());
        size += 100;

        storage.MaxCapacity = BitConverter.ToInt32(array.Skip(size).Take(sizeof(int)).ToArray());

        return storage;
    }

    public static byte[] GetBytes(Storage storage)
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

        temp = BitConverter.GetBytes(storage.MaxCapacity.Value);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"{Id}: {Name} | {Password}\nMax Capacity: {MaxCapacity}";
}
