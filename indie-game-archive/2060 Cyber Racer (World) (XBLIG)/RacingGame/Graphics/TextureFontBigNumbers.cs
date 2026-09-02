using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Helpers;

namespace RacingGame.Graphics;

public static class TextureFontBigNumbers
{
	private static readonly Rectangle[] BigNumberRects;

	private static int WriteDigit(int x, int y, int digit)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (digit < 0)
		{
			return 0;
		}
		float num = (float)BaseGame.Width / 1600f;
		float num2 = (float)BaseGame.Height / 1200f;
		Rectangle pixelRect = BigNumberRects[digit % BigNumberRects.Length];
		BaseGame.UI.Ingame.RenderOnScreen(new Rectangle(x, y, (int)Math.Round((float)pixelRect.Width * num), (int)Math.Round((float)pixelRect.Height * num2)), pixelRect);
		return (int)Math.Round((float)pixelRect.Width * num);
	}

	private static int WriteDigit(int x, int y, int height, int digit)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (digit < 0)
		{
			return 0;
		}
		float num = (float)BaseGame.Width / 1600f;
		float num2 = (float)BaseGame.Height / 1200f;
		float num3 = (float)height / (float)BigNumberRects[0].Height;
		Rectangle pixelRect = BigNumberRects[digit % BigNumberRects.Length];
		BaseGame.UI.Ingame.RenderOnScreen(new Rectangle(x, y, (int)Math.Round((float)pixelRect.Width * num * num3), (int)Math.Round((float)pixelRect.Height * num2 * num3)), pixelRect);
		return (int)Math.Round((float)pixelRect.Width * num * num3);
	}

	private static int WriteDigit(int x, int y, int digit, float alpha)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)BaseGame.Width / 1600f;
		float num2 = (float)BaseGame.Height / 1200f;
		Rectangle pixelRect = BigNumberRects[digit % BigNumberRects.Length];
		BaseGame.UI.Ingame.RenderOnScreen(new Rectangle(x, y, (int)Math.Round((float)pixelRect.Width * num), (int)Math.Round((float)pixelRect.Height * num2)), pixelRect, ColorHelper.ApplyAlphaToColor(Color.White, alpha));
		return (int)Math.Round((float)pixelRect.Width * num);
	}

	public static int WriteNumber(int x, int y, int number)
	{
		string text = number.ToString();
		int num = 0;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			num += WriteDigit(x + num, y, array[i] - 48);
		}
		return num;
	}

	public static int WriteNumber(int x, int y, int number, float alpha)
	{
		string text = number.ToString();
		int num = 0;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			num += WriteDigit(x + num, y, array[i] - 48, alpha);
		}
		return num;
	}

	public static int WriteNumber(int x, int y, int height, int number)
	{
		string text = number.ToString();
		int num = 0;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			num += WriteDigit(x + num, y, height, array[i] - 48);
		}
		return num;
	}

	public static void WriteNumberCentered(int x, int y, int number)
	{
		WriteNumber((int)((float)x - (float)(number.ToString().Length * BigNumberRects[0].Width / 2) * ((float)BaseGame.Width / 1600f)), y, number);
	}

	public static void WriteNumberCentered(int x, int y, int number, float alpha)
	{
		WriteNumber((int)((float)x - (float)(number.ToString().Length * BigNumberRects[0].Width / 2) * ((float)BaseGame.Width / 1600f)), y, number, alpha);
	}

	static TextureFontBigNumbers()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		BigNumberRects = (Rectangle[])(object)new Rectangle[10]
		{
			new Rectangle(2, 342, 80, 133),
			new Rectangle(84, 342, 80, 133),
			new Rectangle(167, 342, 80, 133),
			new Rectangle(247, 342, 78, 133),
			new Rectangle(330, 342, 80, 133),
			new Rectangle(411, 342, 80, 133),
			new Rectangle(495, 342, 80, 133),
			new Rectangle(578, 342, 80, 133),
			new Rectangle(659, 342, 80, 133),
			new Rectangle(749, 342, 80, 133)
		};
	}
}
