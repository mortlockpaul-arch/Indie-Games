using System;

namespace Deep_waters;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		using GameStateManagementGame gameStateManagementGame = new GameStateManagementGame();
		gameStateManagementGame.Run();
	}
}
