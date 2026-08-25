using System.Collections.Generic;

namespace JamSouls.Core;

internal class Option
{
	public enum OptionType
	{
		Time,
		Score,
		BotNumber,
		BotLevel
	}

	public const int MAX_BOT_LEVEL = 5;

	public string Title;

	public List<string> Caption;

	public List<int> Value = new List<int>();

	public int currentValueIdx;

	public OptionType type;

	public int Limit;

	public int Minimum;

	public bool bLocked;

	public Option(OptionType otype)
	{
		type = otype;
		switch (type)
		{
		case OptionType.Score:
		{
			Title = "Score : ";
			currentValueIdx = 3;
			string text = TextManager.GetText(TextID.FRAGS);
			switch (GameContext.GameMode)
			{
			case GAME_MODE.DEATHMATCH:
				text = TextManager.GetText(TextID.FRAGS);
				break;
			case GAME_MODE.CAPTURE_THE_JAM:
				text = TextManager.GetText(TextID.FLAGS);
				break;
			case GAME_MODE.JAM_BALL:
				text = TextManager.GetText(TextID.GOAL);
				break;
			}
			Caption = new List<string>(new string[11]
			{
				"5 " + text,
				" 10 " + text,
				" 20 " + text,
				"30 " + text,
				"40 " + text,
				"50 " + text,
				"60 " + text,
				"70 " + text,
				"80 " + text,
				"90 " + text,
				"100 " + text
			});
			Value = new List<int>(new int[11]
			{
				5, 10, 20, 30, 40, 50, 60, 70, 80, 90,
				100
			});
			break;
		}
		case OptionType.Time:
		{
			Title = "Time : ";
			currentValueIdx = 9;
			Caption = new List<string>(new string[20]
			{
				"1 Min", " 1:30 Min", " 2 Min", "2:30 Min", "3 Min", "3:30 Min", "4 Min", "4:30 Min", "5 Min", "5:30 Min",
				"6 Min", "6:30 Min", "7 Min", "7:30 Min", "8 Min", "8:30 Min", "9 Min", "9:30 Min", "10 Min", "00"
			});
			int num = 30000;
			for (int i = 0; i < Caption.Count - 1; i++)
			{
				num += 30000;
				Value.Add(num);
			}
			Value.Add(0);
			break;
		}
		case OptionType.BotNumber:
			Title = "Bots :";
			currentValueIdx = 0;
			Caption = new List<string>(new string[4]
			{
				"    " + TextManager.GetText(TextID.NO_BOT),
				"    " + TextManager.GetText(TextID.ONE_BOT),
				"    " + TextManager.GetText(TextID.TWO_BOT),
				"   " + TextManager.GetText(TextID.THREE_BOT)
			});
			Value = new List<int>(new int[4] { 0, 1, 2, 3 });
			break;
		case OptionType.BotLevel:
			currentValueIdx = 1;
			Title = "Bot Level : ";
			Caption = new List<string>(new string[3]
			{
				"      " + TextManager.GetText(TextID.DIFFICULTY_ONE),
				"      " + TextManager.GetText(TextID.DIFFICULTY_TWO),
				"      " + TextManager.GetText(TextID.DIFFICULTY_THREE)
			});
			Value = new List<int>(new int[3] { 0, 1, 2 });
			break;
		}
	}

	public void SetLimit(int limit)
	{
		Limit = limit;
	}

	public string GetCaption()
	{
		return Caption[currentValueIdx];
	}

	public int GetValue()
	{
		return Value[currentValueIdx];
	}

	public void IncrementeValue()
	{
		if (!bLocked)
		{
			currentValueIdx++;
			if (currentValueIdx >= Caption.Count - Limit)
			{
				currentValueIdx = Minimum;
			}
		}
	}

	public void DecrementValue()
	{
		if (!bLocked)
		{
			currentValueIdx--;
			if (currentValueIdx < Minimum)
			{
				currentValueIdx = Caption.Count - Limit - 1;
			}
		}
	}
}
