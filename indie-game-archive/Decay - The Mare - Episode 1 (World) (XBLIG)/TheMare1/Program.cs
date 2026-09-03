namespace TheMare1;

internal static class Program
{
	private static void Main(string[] args)
	{
		using TheMare1 theMare = new TheMare1();
		theMare.Run();
		theMare.Dispose();
	}
}
