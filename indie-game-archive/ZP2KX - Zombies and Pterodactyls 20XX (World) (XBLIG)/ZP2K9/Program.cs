using System;
using Microsoft.Xna.Framework;

namespace ZP2K9;

internal static class Program
{
	private static void Main(string[] args)
	{
		Game1 game = new Game1();
		try
		{
			((Game)game).Run();
		}
		finally
		{
			((IDisposable)game)?.Dispose();
		}
	}
}
