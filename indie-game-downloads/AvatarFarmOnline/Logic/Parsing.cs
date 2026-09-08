using System.Text;
using Quasar.Global;
using Quasar.Language;

namespace AvatarFarmOnline.Logic;

internal static class Parsing
{
	public const char ClockChar = '\u0bba';

	public const char AccumulatedChar = '\u0bbb';

	public const char FuelChar = '\u0bbc';

	public const char XpChar = '\u0bbd';

	public const char WinterChar = '\u0bbe';

	public const char SpringChar = '\u0bbf';

	public const char SummerChar = '\u0bc0';

	public const char FallChar = '\u0bc1';

	public const char FeedChar = '\u0bc2';

	public const char HarvestChar = '\u0bc3';

	private const int TICKS_PER_DAY = 86400;

	private const int TICKS_PER_HOUR = 3600;

	private const int TICKS_PER_MINUTE = 60;

	public static AvatarFarmOnline.Logic.Seasons ParseSeasons(string value)
	{
		switch (value)
		{
		case "Winter":
			return AvatarFarmOnline.Logic.Seasons.Winter;
		case "Summer":
			return AvatarFarmOnline.Logic.Seasons.Summer;
		case "Fall":
			return AvatarFarmOnline.Logic.Seasons.Fall;
		case "Spring":
			return AvatarFarmOnline.Logic.Seasons.Spring;
		case "Any":
			return AvatarFarmOnline.Logic.Seasons.Any;
		default:
			if (value.Length == 4)
			{
				AvatarFarmOnline.Logic.Seasons seasons = AvatarFarmOnline.Logic.Seasons.None;
				if (value[0] == '1')
				{
					seasons |= AvatarFarmOnline.Logic.Seasons.Winter;
				}
				if (value[1] == '1')
				{
					seasons |= AvatarFarmOnline.Logic.Seasons.Spring;
				}
				if (value[2] == '1')
				{
					seasons |= AvatarFarmOnline.Logic.Seasons.Summer;
				}
				if (value[3] == '1')
				{
					seasons |= AvatarFarmOnline.Logic.Seasons.Fall;
				}
				return seasons;
			}
			return (AvatarFarmOnline.Logic.Seasons)GameMath.ParseInt(value);
		}
	}

	public static void GetToolTypeText(ToolTypes toolType, StringBuilder sb)
	{
		if ((toolType & ToolTypes.Harvest) != ToolTypes.None)
		{
			sb.Append("Harvest");
		}
		if ((toolType & ToolTypes.Plant) != ToolTypes.None)
		{
			sb.Append("Plant");
		}
		if ((toolType & ToolTypes.Tree) != ToolTypes.None)
		{
			sb.Append("Harvest trees");
		}
		if ((toolType & ToolTypes.Plow) != ToolTypes.None)
		{
			sb.Append("Plow");
		}
		if ((toolType & ToolTypes.Water) != ToolTypes.None)
		{
			sb.Append("Water");
		}
	}

	public static void GetSeasonsText(AvatarFarmOnline.Logic.Seasons value, StringBuilder sb)
	{
		if ((value & AvatarFarmOnline.Logic.Seasons.Winter) != AvatarFarmOnline.Logic.Seasons.None)
		{
			sb.Append('\u0bbe');
		}
		if ((value & AvatarFarmOnline.Logic.Seasons.Spring) != AvatarFarmOnline.Logic.Seasons.None)
		{
			sb.Append('\u0bbf');
		}
		if ((value & AvatarFarmOnline.Logic.Seasons.Summer) != AvatarFarmOnline.Logic.Seasons.None)
		{
			sb.Append('\u0bc0');
		}
		if ((value & AvatarFarmOnline.Logic.Seasons.Fall) != AvatarFarmOnline.Logic.Seasons.None)
		{
			sb.Append('\u0bc1');
		}
	}

	public static char GetSeasonChar(AvatarFarmOnline.Logic.Seasons value)
	{
		return value switch
		{
			AvatarFarmOnline.Logic.Seasons.Winter => '\u0bbe', 
			AvatarFarmOnline.Logic.Seasons.Summer => '\u0bc0', 
			AvatarFarmOnline.Logic.Seasons.Fall => '\u0bc1', 
			_ => '\u0bbf', 
		};
	}

	public static string GetText(AvatarFarmOnline.Logic.Seasons value)
	{
		return value switch
		{
			AvatarFarmOnline.Logic.Seasons.Summer => "SUMMER".Translate(), 
			AvatarFarmOnline.Logic.Seasons.Winter => "WINTER".Translate(), 
			AvatarFarmOnline.Logic.Seasons.Fall => "FALL".Translate(), 
			_ => "SPRING".Translate(), 
		};
	}

	public static AvatarFarmOnline.Logic.Seasons NextSeason(AvatarFarmOnline.Logic.Seasons value)
	{
		return value switch
		{
			AvatarFarmOnline.Logic.Seasons.Fall => AvatarFarmOnline.Logic.Seasons.Winter, 
			AvatarFarmOnline.Logic.Seasons.Spring => AvatarFarmOnline.Logic.Seasons.Summer, 
			AvatarFarmOnline.Logic.Seasons.Summer => AvatarFarmOnline.Logic.Seasons.Fall, 
			_ => AvatarFarmOnline.Logic.Seasons.Spring, 
		};
	}

	public static void SetTimeTextShort(int time, StringBuilder sb)
	{
		int num = time / 86400;
		bool flag = false;
		if (num > 0)
		{
			sb.AppendNumber(num);
			sb.Append("d");
			time -= num * 86400;
			flag = true;
		}
		int num2 = time / 3600;
		if (num2 > 0)
		{
			if (flag)
			{
				sb.Append(' ');
			}
			if (flag)
			{
				sb.AppendNumber(num2, 2, AppendNumberOptions.FixedSize);
			}
			else
			{
				sb.AppendNumber(num2);
			}
			sb.Append("h");
			time -= num2 * 3600;
			flag = true;
		}
		int num3 = time / 60;
		if (num == 0 && num3 > 0)
		{
			if (flag)
			{
				sb.Append(' ');
			}
			if (flag)
			{
				sb.AppendNumber(num3, 2, AppendNumberOptions.FixedSize);
			}
			else
			{
				sb.AppendNumber(num3);
			}
			sb.Append("m");
			time -= num3 * 60;
			flag = true;
		}
		int num4 = time;
		if (num == 0 && num2 == 0 && num4 > 0)
		{
			if (flag)
			{
				sb.Append(' ');
			}
			if (flag)
			{
				sb.AppendNumber(num4, 2, AppendNumberOptions.FixedSize);
			}
			else
			{
				sb.AppendNumber(num4);
			}
			sb.Append("s");
			flag = true;
		}
	}

	public static void SetTimeText(int time, StringBuilder sb)
	{
		int num = time / 86400;
		time -= num * 86400;
		int num2 = time / 3600;
		time -= num2 * 3600;
		int num3 = time / 60;
		time -= num3 * 60;
		int num4 = time;
		bool flag = false;
		if (num > 0)
		{
			sb.AppendNumber(num);
			sb.Append(' ');
			if (num == 1)
			{
				sb.Append("DAY_DIM".Translate());
			}
			else
			{
				sb.Append("DAYS_DIM".Translate());
			}
			flag = true;
		}
		if (num2 > 0)
		{
			if (flag)
			{
				sb.Append(", ");
			}
			sb.AppendNumber(num2);
			sb.Append(' ');
			if (num2 == 1)
			{
				sb.Append("HOUR_DIM".Translate());
			}
			else
			{
				sb.Append("HOURS_DIM".Translate());
			}
			flag = true;
		}
		if (num3 > 0)
		{
			if (flag)
			{
				sb.Append(", ");
			}
			sb.AppendNumber(num3);
			sb.Append(' ');
			if (num3 == 1)
			{
				sb.Append("MINUTE_DIM".Translate());
			}
			else
			{
				sb.Append("MINUTES_DIM".Translate());
			}
			flag = true;
		}
		if (num4 > 0)
		{
			if (flag)
			{
				sb.Append(", ");
			}
			sb.AppendNumber(num4);
			sb.Append(' ');
			if (num4 == 1)
			{
				sb.Append("SECOND_DIM".Translate());
			}
			else
			{
				sb.Append("SECONDS_DIM".Translate());
			}
			flag = true;
		}
	}

	public static ToolTypes ParseToolTypes(string value)
	{
		switch (value)
		{
		case "Plow":
			return ToolTypes.Plow;
		case "Harvest":
			return ToolTypes.Harvest;
		case "Plant":
			return ToolTypes.Plant;
		case "Water":
			return ToolTypes.Water;
		case "Tree":
			return ToolTypes.Tree;
		default:
			if (value.Length == 5)
			{
				ToolTypes toolTypes = ToolTypes.None;
				if (value[0] == '1')
				{
					toolTypes |= ToolTypes.Plow;
				}
				if (value[1] == '1')
				{
					toolTypes |= ToolTypes.Harvest;
				}
				if (value[2] == '1')
				{
					toolTypes |= ToolTypes.Plant;
				}
				if (value[3] == '1')
				{
					toolTypes |= ToolTypes.Water;
				}
				if (value[4] == '1')
				{
					toolTypes |= ToolTypes.Tree;
				}
				return toolTypes;
			}
			return (ToolTypes)GameMath.ParseInt(value);
		}
	}
}
