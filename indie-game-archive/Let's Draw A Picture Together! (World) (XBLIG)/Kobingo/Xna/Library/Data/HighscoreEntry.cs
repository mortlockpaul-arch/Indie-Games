using System;

namespace Kobingo.Xna.Library.Data;

public class HighscoreEntry
{
	public const int MAX_VALUES = 5;

	public string Name { get; set; }

	public DateTime Created { get; set; }

	public int Type { get; set; }

	public int Score { get; set; }

	public int[] Values { get; private set; }

	public HighscoreEntry()
	{
		Values = new int[5];
	}
}
