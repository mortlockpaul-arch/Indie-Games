using System;
using Microsoft.Xna.Framework;

namespace amProject;

internal static class Program
{
	private static void Main(string[] args)
	{
		AmbientMachine ambientMachine = new AmbientMachine();
		try
		{
			((Game)ambientMachine).Run();
		}
		finally
		{
			((IDisposable)ambientMachine)?.Dispose();
		}
	}
}
