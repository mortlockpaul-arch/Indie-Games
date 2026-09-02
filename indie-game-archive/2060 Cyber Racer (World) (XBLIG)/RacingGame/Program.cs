using System;
using Microsoft.Xna.Framework;

namespace RacingGame;

internal static class Program
{
	private static void Main()
	{
		StartGame();
	}

	public static void StartGame()
	{
		RacingGameManager racingGameManager = new RacingGameManager();
		try
		{
			((Game)racingGameManager).Run();
		}
		finally
		{
			((IDisposable)racingGameManager)?.Dispose();
		}
	}
}
