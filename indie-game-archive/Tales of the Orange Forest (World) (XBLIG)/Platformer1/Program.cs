namespace Platformer1;

internal static class Program
{
	private static void Main(string[] args)
	{
		using PlatformerGame platformerGame = new PlatformerGame();
		platformerGame.Run();
	}
}
