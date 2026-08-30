namespace Billard3;

internal static class Program
{
	private static void Main(string[] args)
	{
		using BillardGame billardGame = new BillardGame();
		billardGame.Run();
	}
}
