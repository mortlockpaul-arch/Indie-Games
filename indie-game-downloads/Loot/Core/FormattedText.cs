namespace Loot.Core;

public class FormattedText
{
	public char[,] text;

	private int width;

	public int Height => text.GetLength(0) * 40;

	public int Width => width;

	public int MaxWidth => text.GetLength(1) * 24;

	public FormattedText(string s, int maxWidth)
	{
		format(s, maxWidth);
		width = calcWidth();
	}

	private int calcRows(string s, int maxWidth)
	{
		int num = 1;
		int num2 = 0;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (num2 == 0)
			{
				while (c == ' ')
				{
					i++;
					c = s[i];
				}
			}
			if (c == '\n')
			{
				num2 = 0;
				num++;
				continue;
			}
			num2 += 24;
			if (num2 > maxWidth)
			{
				while (c != ' ')
				{
					i--;
					c = s[i];
				}
				num2 = 0;
				num++;
			}
		}
		return num;
	}

	private void format(string s, int maxWidth)
	{
		text = new char[calcRows(s, maxWidth), maxWidth / 24];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (num3 == 0)
			{
				while (c == ' ')
				{
					i++;
					c = s[i];
				}
			}
			if (c == '\n')
			{
				num3 = 0;
				num2 = 0;
				num++;
				continue;
			}
			num3 += 24;
			if (num3 > maxWidth)
			{
				while (c != ' ')
				{
					i--;
					c = s[i];
					num2--;
					text[num, num2] = '\0';
				}
				num3 = 0;
				num2 = 0;
				num++;
			}
			else
			{
				text[num, num2] = c;
				num2++;
			}
		}
	}

	private int calcWidth()
	{
		int num = 0;
		for (int i = 0; i < text.GetLength(0); i++)
		{
			for (int j = 0; j < text.GetLength(1) && text[i, j] != 0; j++)
			{
				int num2 = (j + 1) * 24;
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}
}
