using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace ErrorReporting;

internal static class Program
{
	private static void Main(string[] args)
	{
		Run<Game1>();
	}

	public static void Run<T>() where T : Game, new()
	{
		if (Debugger.IsAttached)
		{
			using (T val = new T())
			{
				val.Run();
				return;
			}
		}
		try
		{
			using T val2 = new T();
			val2.Run();
		}
		catch (Exception e)
		{
			using ExceptionGame exceptionGame = new ExceptionGame(e);
			exceptionGame.Run();
		}
	}
}
