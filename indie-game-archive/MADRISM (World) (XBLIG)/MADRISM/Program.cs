namespace MADRISM
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			using (Madrism madrism = new Madrism())
			{
				madrism.Run();
			}
		}
	}
}
