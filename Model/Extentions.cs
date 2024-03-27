namespace Model;

public static class Extentions
{
    public static void AppendBytes(this List<byte> to, byte[] from)
    {
        for (int i = 0; i < from.Length; i++)
            to.Add(from[i]);
    }
}