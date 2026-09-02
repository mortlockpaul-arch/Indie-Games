using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Properties;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class Highscores : IGameScreen
{
	private struct HighscoreInLevel(string setName, int setTimeMs)
	{
		public string name = setName;

		public int timeMilliseconds = setTimeMs;

		public override string ToString()
		{
			return name + ":" + timeMilliseconds;
		}
	}

	private const int NumOfHighscores = 10;

	private const int NumOfHighscoreLevels = 3;

	private static HighscoreInLevel[,] highscores;

	private int selectedLevel = 1;

	private static void WriteHighscoresToSettings()
	{
		string text = "";
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				text = text + ((text.Length == 0) ? "" : ",") + highscores[i, j];
			}
		}
		GameSettings.Default.Highscores = text;
		ThreadPool.QueueUserWorkItem(SaveSettings, null);
	}

	private static void SaveSettings(object state)
	{
		GameSettings.Save();
	}

	private static bool ReadHighscoresFromSettings()
	{
		if (string.IsNullOrEmpty(GameSettings.Default.Highscores))
		{
			return false;
		}
		string text = GameSettings.Default.Highscores;
		string[] array = text.Split(',');
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 10 && i * 10 + j < array.Length; j++)
			{
				string[] array2 = array[i * 10 + j].Split(':');
				ref HighscoreInLevel reference = ref highscores[i, j];
				reference = new HighscoreInLevel(array2[0], Convert.ToInt32(array2[1]));
			}
		}
		return true;
	}

	public static void Initialize()
	{
		highscores = new HighscoreInLevel[3, 10];
		if (ReadHighscoresFromSettings())
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				ref HighscoreInLevel reference = ref highscores[i, j];
				reference = new HighscoreInLevel("Player " + (j + 1), (75000 + j * 5000) * (i + 1));
			}
		}
		WriteHighscoresToSettings();
	}

	public static float GetTopLapTime(int level)
	{
		return (float)highscores[level, 0].timeMilliseconds / 1000f;
	}

	public static int[] GetTop5LapTimes(int level)
	{
		return new int[5]
		{
			highscores[level, 0].timeMilliseconds,
			highscores[level, 1].timeMilliseconds,
			highscores[level, 2].timeMilliseconds,
			highscores[level, 3].timeMilliseconds,
			highscores[level, 4].timeMilliseconds
		};
	}

	public static int GetRankFromCurrentTime(int level, int timeMilliseconds)
	{
		if (timeMilliseconds < 1000)
		{
			return 10;
		}
		for (int i = 0; i < 10; i++)
		{
			if (timeMilliseconds <= highscores[level, i].timeMilliseconds)
			{
				return i;
			}
		}
		return 10;
	}

	public static void SubmitHighscore(int level, int timeMilliseconds)
	{
		for (int i = 0; i < 10; i++)
		{
			if (timeMilliseconds <= highscores[level, i].timeMilliseconds)
			{
				for (int num = 9; num > i; num--)
				{
					ref HighscoreInLevel reference = ref highscores[level, num];
					reference = highscores[level, num - 1];
				}
				highscores[level, i].name = GameSettings.Default.PlayerName;
				highscores[level, i].timeMilliseconds = timeMilliseconds;
				WriteHighscoresToSettings();
				break;
			}
		}
	}

	public bool Render()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		BaseGame.UI.RenderBlackBar(160, 338);
		int num = 10;
		int num2 = 18;
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			num += 36;
			num2 += 26;
		}
		BaseGame.UI.Headers.RenderOnScreenRelative1600(num, num2, UIRenderer.HeaderHighscoresGfxRect);
		int num3 = BaseGame.XToRes(297);
		int num4 = BaseGame.YToRes(182);
		int num5 = BaseGame.YToRes(27);
		bool flag = Input.MouseInBox(new Rectangle(num3, num4, BaseGame.XToRes(125), num5));
		TextureFont.WriteText(num3, num4, "Beginner", (selectedLevel == 0) ? Color.Yellow : (flag ? Color.White : Color.LightGray));
		if (flag && Input.MouseLeftButtonJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedLevel = 0;
		}
		num3 += BaseGame.XToRes(168);
		flag = Input.MouseInBox(new Rectangle(num3, num4, BaseGame.XToRes(125), num5));
		TextureFont.WriteText(num3, num4, "Advanced", (selectedLevel == 1) ? Color.Yellow : (flag ? Color.White : Color.LightGray));
		if (flag && Input.MouseLeftButtonJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedLevel = 1;
		}
		num3 += BaseGame.XToRes(182);
		flag = Input.MouseInBox(new Rectangle(num3, num4, BaseGame.XToRes(125), num5));
		TextureFont.WriteText(num3, num4, "Expert", (selectedLevel == 2) ? Color.Yellow : (flag ? Color.White : Color.LightGray));
		if (flag && Input.MouseLeftButtonJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedLevel = 2;
		}
		if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedLevel = (selectedLevel + 2) % 3;
		}
		else if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedLevel = (selectedLevel + 1) % 3;
		}
		int num6 = BaseGame.XToRes(300);
		int x = BaseGame.XToRes(350);
		int num7 = BaseGame.XToRes(640);
		num4 = BaseGame.YToRes(208);
		BaseGame.DrawLine(new Point(num6, num4), new Point(num7 + TextureFont.GetTextWidth("5:67:89"), num4), new Color((byte)192, (byte)192, (byte)192, (byte)128));
		BaseGame.DrawLine(new Point(num6, num4 + 1), new Point(num7 + TextureFont.GetTextWidth("5:67:89"), num4 + 1), new Color((byte)192, (byte)192, (byte)192, (byte)128));
		num4 = BaseGame.YToRes(220);
		Rectangle rect = default(Rectangle);
		for (int i = 0; i < 10; i++)
		{
			((Rectangle)(ref rect))._002Ector(0, num4, BaseGame.Width, num5);
			Color color = (Color)(Input.MouseInBox(rect) ? Color.White : new Color((byte)200, (byte)200, (byte)200));
			TextureFont.WriteText(num6, num4, 1 + i + ".", color);
			TextureFont.WriteText(x, num4, highscores[selectedLevel, i].name, color);
			TextureFont.WriteGameTime(num7, num4, highscores[selectedLevel, i].timeMilliseconds, Color.Yellow);
			num4 += num5;
		}
		BaseGame.UI.RenderBottomButtons(onlyBack: true);
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBJustPressed || Input.GamePadBackJustPressed || (Input.MouseLeftButtonJustPressed && Input.MousePos.Y > num4))
		{
			return true;
		}
		return false;
	}
}
