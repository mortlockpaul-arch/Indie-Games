namespace Defenders;

internal static class Program
{
	public static string[] arguments;

	private static void Main(string[] args)
	{
		arguments = args;
		using Game1 game = new Game1();
		game.Run();
	}
}
