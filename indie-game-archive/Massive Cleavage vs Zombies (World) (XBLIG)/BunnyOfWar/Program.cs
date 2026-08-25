using System;

namespace BunnyOfWar;

internal static class Program
{
	public static BunnyOfWarGame game;

	private static void Main(string[] args)
	{
		try
		{
			using (game = new BunnyOfWarGame())
			{
				game.Run();
			}
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}
}
