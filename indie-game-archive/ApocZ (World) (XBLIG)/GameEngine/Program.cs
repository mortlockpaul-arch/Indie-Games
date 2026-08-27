using System;
using EGEngine;
using ErrorReporting;
using Microsoft.Xna.Framework;

namespace GameEngine;

internal static class Program
{
	private static void Main(string[] args)
	{
		Run<Game1>();
	}

	public static void Run<T>() where T : Game, new()
	{
		try
		{
			using GameEngine gameEngine = new GameEngine();
			gameEngine.Run();
		}
		catch (Exception ex)
		{
			EndGameEngine.ThreadExceptionArgument = ((EndGameEngine.ThreadExceptionArgument == null) ? ex : EndGameEngine.ThreadExceptionArgument);
			using ExceptionGame exceptionGame = new ExceptionGame(EndGameEngine.ThreadExceptionArgument);
			exceptionGame.Run();
		}
	}
}
