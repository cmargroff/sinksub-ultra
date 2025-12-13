using System.Text;

public static partial class StringExtensions
{
    public static string Repeat(this string src, int times)
    {
        StringBuilder sb = new();
        for (int x = 0; x < times; x++)
            sb.Append(src);
        return sb.ToString();

    }
}
