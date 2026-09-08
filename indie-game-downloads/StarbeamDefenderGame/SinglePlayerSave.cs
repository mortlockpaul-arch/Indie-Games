using System;

namespace StarbeamDefenderGame;

public struct SinglePlayerSave(string name, int score, int rounds)
{
	public string Name = name.Trim();

	public int score = score;

	public string Date = DateTime.Now.Date.ToShortDateString();

	public int rounds = rounds;
}
