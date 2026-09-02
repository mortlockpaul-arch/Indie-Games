using Microsoft.Xna.Framework.Graphics;

namespace RacingGame.Helpers;

public static class ColorHelper
{
	public static readonly Color Empty;

	public static readonly Color HalfAlpha;

	private static float StayInRange(float val, float min, float max)
	{
		if (val < min)
		{
			return min;
		}
		if (val > max)
		{
			return max;
		}
		return val;
	}

	public static Color MultiplyColors(Color color1, Color color2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (color1 == Color.White)
		{
			return color2;
		}
		if (color2 == Color.White)
		{
			return color1;
		}
		float num = (float)(int)((Color)(ref color1)).R / 255f;
		float num2 = (float)(int)((Color)(ref color1)).G / 255f;
		float num3 = (float)(int)((Color)(ref color1)).B / 255f;
		float num4 = (float)(int)((Color)(ref color1)).A / 255f;
		float num5 = (float)(int)((Color)(ref color2)).R / 255f;
		float num6 = (float)(int)((Color)(ref color2)).G / 255f;
		float num7 = (float)(int)((Color)(ref color2)).B / 255f;
		float num8 = (float)(int)((Color)(ref color2)).A / 255f;
		return new Color((byte)(StayInRange(num * num5, 0f, 1f) * 255f), (byte)(StayInRange(num2 * num6, 0f, 1f) * 255f), (byte)(StayInRange(num3 * num7, 0f, 1f) * 255f), (byte)(StayInRange(num4 * num8, 0f, 1f) * 255f));
	}

	public static bool SameColor(Color color, Color checkColor)
	{
		if (((Color)(ref color)).R == ((Color)(ref checkColor)).R && ((Color)(ref color)).G == ((Color)(ref checkColor)).G)
		{
			return ((Color)(ref color)).B == ((Color)(ref checkColor)).B;
		}
		return false;
	}

	public static Color InterpolateColor(Color col1, Color col2, float percent)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		return new Color((byte)((float)(int)((Color)(ref col1)).R * (1f - percent) + (float)(int)((Color)(ref col2)).R * percent), (byte)((float)(int)((Color)(ref col1)).G * (1f - percent) + (float)(int)((Color)(ref col2)).G * percent), (byte)((float)(int)((Color)(ref col1)).B * (1f - percent) + (float)(int)((Color)(ref col2)).B * percent), (byte)((float)(int)((Color)(ref col1)).A * (1f - percent) + (float)(int)((Color)(ref col2)).A * percent));
	}

	public static Color ApplyAlphaToColor(Color col, float newAlpha)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (newAlpha < 0f)
		{
			newAlpha = 0f;
		}
		if (newAlpha > 1f)
		{
			newAlpha = 1f;
		}
		return new Color(((Color)(ref col)).R, ((Color)(ref col)).G, ((Color)(ref col)).B, (byte)(newAlpha * 255f));
	}

	public static Color MixAlphaToColor(Color col, float newAlpha)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (newAlpha < 0f)
		{
			newAlpha = 0f;
		}
		if (newAlpha > 1f)
		{
			newAlpha = 1f;
		}
		return new Color((byte)((float)(int)((Color)(ref col)).R * newAlpha), (byte)((float)(int)((Color)(ref col)).G * newAlpha), (byte)((float)(int)((Color)(ref col)).B * newAlpha), (byte)(newAlpha * 255f));
	}

	static ColorHelper()
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Empty = new Color((byte)0, (byte)0, (byte)0, (byte)0);
		HalfAlpha = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)128);
	}
}
