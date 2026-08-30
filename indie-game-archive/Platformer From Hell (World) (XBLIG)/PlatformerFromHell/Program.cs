namespace PlatformerFromHell;

internal static class Program
{
	public static PlatformerGame game;

	private static void Main(string[] args)
	{
		using (game = new PlatformerGame())
		{
			game.IsFixedTimeStep = false;
			game.Run();
		}
	}
}
