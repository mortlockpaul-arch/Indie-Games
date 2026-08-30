using System.IO;
using Microsoft.Xna.Framework;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.menu;

public class HighScores
{
	public static HighScore[] highScore = new HighScore[10];

	public static void Init()
	{
		for (int i = 0; i < highScore.Length; i++)
		{
			highScore[i] = new HighScore("", 0L);
		}
	}

	public static void Read(BinaryReader reader)
	{
		for (int i = 0; i < 10; i++)
		{
			highScore[i] = new HighScore(reader.ReadString(), reader.ReadInt64());
		}
	}

	public static void Write(BinaryWriter writer)
	{
		for (int i = 0; i < 10; i++)
		{
			writer.Write(highScore[i].name);
			writer.Write(highScore[i].score);
		}
	}

	public static void AddScore(string name, long score)
	{
		int num = 9;
		int num2 = 9;
		while (num2 >= 0 && score >= highScore[num2].score)
		{
			num = num2;
			num2--;
		}
		for (int num3 = 8; num3 >= num; num3--)
		{
			highScore[num3 + 1].name = highScore[num3].name;
			highScore[num3 + 1].score = highScore[num3].score;
		}
		highScore[num].name = name;
		highScore[num].score = score;
		Game1.store.Write();
	}

	public static void DrawScores(Vector2 loc)
	{
		for (int i = 0; i < 10; i++)
		{
			if (highScore[i].score > 0)
			{
				float num = 6f;
				float num2 = 40f;
				Text.DrawScore(i + 1, loc + new Vector2(-50f, (float)i * num2 - 2.5f * num), num, Color.Red, Text.Justify.Right);
				Text.DrawString(highScore[i].name, loc + new Vector2(0f, (float)i * num2), num, Color.White, Text.Justify.Left);
				Text.DrawScore(highScore[i].score, loc + new Vector2(420f, (float)i * num2 - 2.5f * num), num, Color.White, Text.Justify.Right);
			}
		}
	}
}
