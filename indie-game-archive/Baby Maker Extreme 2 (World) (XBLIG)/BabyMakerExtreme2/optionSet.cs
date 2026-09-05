using System.Collections.Generic;

namespace BabyMakerExtreme2;

public struct optionSet
{
	public int SaveVersion;

	public List<List<int>> HighScores;

	public List<List<int>> HighScoresBabyTypes;

	public List<List<string>> HighScoreNames;

	public List<bool> OutfitUnlocks;

	public List<bool> PowerupUnlocks;

	public List<bool> ModeUnlocks;

	public int TotalDist;
}
