using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Games.Painter;

internal static class Program
{
	private static void Main(string[] args)
	{
		PainterGame painterGame = new PainterGame();
		try
		{
			((Game)painterGame).Run();
		}
		finally
		{
			((IDisposable)painterGame)?.Dispose();
		}
	}
}
