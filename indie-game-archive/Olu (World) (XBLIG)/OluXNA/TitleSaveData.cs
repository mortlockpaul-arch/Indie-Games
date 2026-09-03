using System;

namespace OluXNA;

[Serializable]
public struct TitleSaveData
{
	public int[][] topScores = new int[4][];

	public string[][] topNames = new string[4][];

	public TitleSaveData(int q)
	{
		for (int i = 0; i < topScores.Length; i++)
		{
			topScores[i] = new int[10];
			topNames[i] = new string[10];
		}
	}
}
