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
			T val = new T();
			try
			{
				val.Run();
				return;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		try
		{
			T val2 = new T();
			try
			{
				val2.Run();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		catch (Exception e)
		{
			using ExceptionGame exceptionGame = new ExceptionGame(e);
			exceptionGame.Run();
		}
	}
}
