namespace Core;

internal static class Program
{
	private static void Main(string[] args)
	{
		using Game game = new Game();
		game.Run();
		game.Dispose();
	}
}
