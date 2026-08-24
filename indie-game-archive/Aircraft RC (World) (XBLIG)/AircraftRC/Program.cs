using System;

namespace AircraftRC;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		using CustomPhysicsGame customPhysicsGame = new CustomPhysicsGame();
		customPhysicsGame.Run();
	}
}
