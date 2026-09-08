using System;

namespace StarbeamDefenderGame;

public struct MultiplayerSave(string name, string nametwo, int score, int rounds)
{
	public string Name = name.Trim();

	public string NameTwo = nametwo;

	public int score = score;

	public string Date = DateTime.Now.Date.ToShortDateString();

	public int rounds = rounds;
}
