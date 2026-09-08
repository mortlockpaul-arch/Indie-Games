using System;
using System.IO;
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
			GameEngine gameEngine = new GameEngine();
			try
			{
				gameEngine.Run();
			}
			finally
			{
				try
				{
					((IDisposable)gameEngine)?.Dispose();
				}
				catch (Exception ex)
				{
					File.AppendAllText("ApocZ-dispose-error.log", ex.ToString());
				}
			}
		}
		catch (Exception ex2)
		{
			EndGameEngine.ThreadExceptionArgument = ((EndGameEngine.ThreadExceptionArgument == null) ? ex2 : EndGameEngine.ThreadExceptionArgument);
			try
			{
				File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-crash.log"), EndGameEngine.ThreadExceptionArgument.ToString());
			}
			catch
			{
			}
			using ExceptionGame exceptionGame = new ExceptionGame(EndGameEngine.ThreadExceptionArgument);
			exceptionGame.Run();
		}
	}
}
