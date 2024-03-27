namespace Model;

public static class Extentions
{
    public static void AppendBytes(this List<byte> to, byte[] from)
    {
        for (int i = 0; i < from.Length; i++)
            to.Add(from[i]);
    }

    public static int DateSize => sizeof(short) * 3;

    public static DateTime GetDateTime(byte[] bytes)
    {
        int day, month, year;
        
        day = BitConverter.ToInt16(bytes.Take(sizeof(short)).ToArray());
        month = BitConverter.ToInt16(bytes.Skip(sizeof(short)).Take(sizeof(short)).ToArray());
        year = BitConverter.ToInt16(bytes.Skip(sizeof(short) * 2).Take(sizeof(short)).ToArray());

        return new DateTime(year, month, day);
    }

    public static byte[] GetBytesDateTime(DateTime dateTime)
    {
        List<byte> bytes = new List<byte>(DateSize);

        byte[] temp = BitConverter.GetBytes((short)dateTime.Day);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes((short)dateTime.Month);
        bytes.AppendBytes(temp);

        temp = BitConverter.GetBytes((short)dateTime.Year);
        bytes.AppendBytes(temp);

        return bytes.ToArray();
    }
}