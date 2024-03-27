using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Model;

public class Measure
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public static int Size = sizeof(int) + 100;

    public static Measure GetMeasure(byte[] array)
    {
        Measure measure = new Measure();
        int size = 0;

        measure.Id = BitConverter.ToInt32(array.Take(sizeof(int)).ToArray());
        size += sizeof(int);

        measure.Name = Encoding.UTF8.GetString(array.Skip(size).Take(100).Where(b => b != 0).ToArray());

        return measure;
    }

    public static byte[] GetBytes(Measure measure)
    {
        List<byte> bytes = new List<byte>(Size);

        byte[] temp = BitConverter.GetBytes(measure.Id);
        bytes.AppendBytes(temp);

        temp = Encoding.UTF8.GetBytes(measure.Name);
        Array.Resize(ref temp, 100);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }

    public override string ToString() => $"{Id}: {Name}";
}