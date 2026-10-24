namespace CrackmeDemo;

public class Program
{
    private static string SecretKey = "Sharphy-Secret-2026";

    public static void Main(string[] args)
    {
        Console.WriteLine($"CrackmeDemo loaded. Secret: {GetSecret()}");
    }

    private static string GetSecret()
    {
        return SecretKey;
    }
}
