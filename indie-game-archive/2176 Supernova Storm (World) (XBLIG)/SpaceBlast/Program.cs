using System;
using Microsoft.Xna.Framework;

namespace SpaceBlast;

internal static class Program
{
	private static void Main(string[] args)
	{
		MainGame mainGame = new MainGame();
		try
		{
			((Game)mainGame).Run();
		}
		finally
		{
			((IDisposable)mainGame)?.Dispose();
		}
	}
}
