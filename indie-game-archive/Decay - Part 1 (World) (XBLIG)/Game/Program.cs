using System;
using Microsoft.Xna.Framework;

namespace Game;

internal static class Program
{
	private static void Main(string[] args)
	{
		Game game = new Game();
		try
		{
			((Game)game).Run();
			((Game)game).Dispose();
		}
		finally
		{
			((IDisposable)game)?.Dispose();
		}
	}
}
