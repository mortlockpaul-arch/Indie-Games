using System.Text;
using System.Xml.Linq;
using Quasar.Global;
using Quasar.Language;

namespace Quasar.GameUtils.Stats;

public class Stat
{
	public enum StatType
	{
		Numeric,
		Time,
		Centesimal,
		Millesimal,
		Percentual
	}

	private string id;

	private string name;

	private StatType statType;

	private bool isMax;

	private bool isAverage;

	public string Id => id;

	public string Name => name;

	public StatType Type => statType;

	public bool IsMax => isMax;

	public bool IsAverage => isAverage;

	public void GetProgressText(StringBuilder builder, long progress, int count)
	{
		if (!IsAverage)
		{
			switch (statType)
			{
			default:
				builder.AppendNumber(progress, AppendNumberOptions.NumberGroup);
				break;
			case StatType.Time:
			{
				long num = progress / 3600000;
				if (num > 0)
				{
					builder.AppendNumber(num);
					builder.Append(':');
				}
				long num2 = progress / 60000 % 60;
				if (num2 < 10)
				{
					builder.Append('0');
				}
				builder.AppendNumber(num2);
				builder.Append(':');
				long num3 = progress / 1000 % 60;
				if (num3 < 10)
				{
					builder.Append('0');
				}
				builder.AppendNumber(num3);
				break;
			}
			case StatType.Centesimal:
				builder.AppendNumber((float)progress * 0.01f, 2, AppendNumberOptions.NumberGroup);
				break;
			case StatType.Millesimal:
				builder.AppendNumber((float)progress * 0.001f, 3, AppendNumberOptions.NumberGroup);
				break;
			}
		}
		else
		{
			float num4 = 0f;
			if (count > 0)
			{
				num4 = (float)progress / (float)count;
			}
			switch (statType)
			{
			case StatType.Numeric:
				builder.AppendNumber(num4, 2, AppendNumberOptions.None);
				break;
			case StatType.Centesimal:
				builder.AppendNumber(num4 * 0.01f, 2, AppendNumberOptions.NumberGroup);
				break;
			case StatType.Millesimal:
				builder.AppendNumber(num4 * 0.001f, 3, AppendNumberOptions.NumberGroup);
				break;
			case StatType.Percentual:
				builder.AppendNumber(num4 * 100f, 2, AppendNumberOptions.None);
				builder.Append("%");
				break;
			case StatType.Time:
				break;
			}
		}
	}

	protected Stat(string id, StatType statType, bool isMax, bool isAverage)
	{
		string text = id.ToUpper();
		name = LanguageManager.Texts["STAT_" + text + "_NAME"];
		this.id = id;
		this.statType = statType;
		this.isMax = isMax;
		this.isAverage = isAverage;
	}

	public static Stat FromXml(XElement xe)
	{
		return new Stat(xe.GetAttribute("id"), (StatType)xe.ParseIntAttribute("type", 0), xe.ParseBoolAttribute("isMax"), xe.ParseBoolAttribute("isAverage"));
	}
}
