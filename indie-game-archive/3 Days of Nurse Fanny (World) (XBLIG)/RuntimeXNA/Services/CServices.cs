using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Services;

public class CServices
{
	public const short DT_LEFT = 0;

	public const short DT_TOP = 0;

	public const short DT_CENTER = 1;

	public const short DT_RIGHT = 2;

	public const short DT_BOTTOM = 8;

	public const short DT_VCENTER = 4;

	public const short DT_SINGLELINE = 32;

	public const short DT_CALCRECT = 1024;

	public const short DT_VALIGN = 2048;

	public const int CPTDISPFLAG_INTNDIGITS = 15;

	public const int CPTDISPFLAG_FLOATNDIGITS = 240;

	public const int CPTDISPFLAG_FLOATNDIGITS_SHIFT = 4;

	public const int CPTDISPFLAG_FLOATNDECIMALS = 61440;

	public const int CPTDISPFLAG_FLOATNDECIMALS_SHIFT = 12;

	public const int CPTDISPFLAG_FLOAT_FORMAT = 512;

	public const int CPTDISPFLAG_FLOAT_USENDECIMALS = 1024;

	public const int CPTDISPFLAG_FLOAT_PADD = 2048;

	private Texture2D pixel;

	private Rectangle tempRect;

	private static int[] xPos = null;

	private static Vector2 vector;

	public static int HIWORD(int ul)
	{
		return ul >> 16;
	}

	public static int LOWORD(int ul)
	{
		return ul & 0xFFFF;
	}

	public static int MAKELONG(int lo, int hi)
	{
		return (hi << 16) | (lo & 0xFFFF);
	}

	public static int getRValueJava(int rgb)
	{
		return (rgb >> 16) & 0xFF;
	}

	public static int getGValueJava(int rgb)
	{
		return (rgb >> 8) & 0xFF;
	}

	public static int getBValueJava(int rgb)
	{
		return rgb & 0xFF;
	}

	public static int RGBJava(int r, int g, int b)
	{
		return ((r & 0xFF) << 16) | ((g & 0xFF) << 8) | (b & 0xFF);
	}

	public static int swapRGB(int rgb)
	{
		int num = (rgb >> 16) & 0xFF;
		int num2 = (rgb >> 8) & 0xFF;
		int num3 = rgb & 0xFF;
		return ((num3 & 0xFF) << 16) | ((num2 & 0xFF) << 8) | (num & 0xFF);
	}

	public static Color getColor(int rgb)
	{
		int num = (rgb >> 16) & 0xFF;
		int num2 = (rgb >> 8) & 0xFF;
		int num3 = rgb & 0xFF;
		return new Color((byte)num, (byte)num2, (byte)num3);
	}

	public static Color getColorAlpha(int rgb)
	{
		int r = (rgb >> 16) & 0xFF;
		int g = (rgb >> 8) & 0xFF;
		int b = rgb & 0xFF;
		return new Color(r, g, b, 255);
	}

	public static int clamp(int val, int a, int b)
	{
		return Math.Min(Math.Max(val, a), b);
	}

	public static int drawText(SpriteBatchEffect batch, string s, short flags, CRect rc, int rgb, CFont font, int effect, int effectParam)
	{
		if (s.Length == 0)
		{
			if ((flags & 0x400) != 0)
			{
				rc.right = rc.left;
				rc.bottom = rc.top;
			}
			return 0;
		}
		SpriteFont font2 = font.getFont();
		int num = 0;
		int num2 = s.IndexOf('\n');
		if (num2 >= 0)
		{
			CRect cRect = new CRect();
			cRect.copyRect(rc);
			int num3 = 0;
			int num4 = 0;
			do
			{
				int num5 = -1;
				if (num3 < s.Length)
				{
					num5 = s.IndexOf('\r', num3);
				}
				int num6 = Math.Max(num2, num5);
				if (num5 == num2 - 1)
				{
					num2--;
				}
				string s2 = s.Substring(num3, num2 - num3);
				int num7 = drawIt(batch, font2, s2, (short)(flags | 0x400), cRect, rgb, effect, effectParam);
				num4 = Math.Max(num4, cRect.right - cRect.left);
				num += num7;
				cRect.top += num7;
				cRect.bottom = rc.bottom;
				cRect.right = rc.right;
				num3 = num6 + 1;
				num2 = -1;
				if (num3 < s.Length)
				{
					num2 = s.IndexOf('\n', num3);
				}
			}
			while (num2 >= 0);
			if (num3 < s.Length)
			{
				string s2 = s.Substring(num3);
				int num7 = drawIt(batch, font2, s2, (short)(flags | 0x400), cRect, rgb, effect, effectParam);
				num4 = Math.Max(num4, cRect.right - cRect.left);
				num += num7;
			}
			if ((flags & 0x400) != 0)
			{
				rc.right = rc.left + num4;
				rc.bottom = cRect.bottom;
				return num;
			}
			cRect.copyRect(rc);
			if ((flags & 4) != 0)
			{
				cRect.top = cRect.top + (cRect.bottom - cRect.top) / 2 - num / 2;
			}
			else if ((flags & 8) != 0)
			{
				cRect.top = cRect.bottom - num;
			}
			num = 0;
			num3 = 0;
			num2 = s.IndexOf('\n');
			do
			{
				int num5 = -1;
				if (num3 < s.Length)
				{
					num5 = s.IndexOf('\r', num3);
				}
				int num6 = Math.Max(num2, num5);
				if (num5 == num2 - 1)
				{
					num2--;
				}
				string s2 = s.Substring(num3, num2 - num3);
				int num7 = drawIt(batch, font2, s2, flags, cRect, rgb, effect, effectParam);
				num += num7;
				cRect.top += num7;
				cRect.bottom = rc.bottom;
				cRect.right = rc.right;
				num3 = num6 + 1;
				num2 = -1;
				if (num3 < s.Length)
				{
					num2 = s.IndexOf('\n', num3);
				}
			}
			while (num2 >= 0);
			if (num3 < s.Length)
			{
				string s2 = s.Substring(num3);
				int num7 = drawIt(batch, font2, s2, flags, cRect, rgb, effect, effectParam);
				num += num7;
			}
			return num;
		}
		return drawIt(batch, font2, s, (short)(flags | 0x800), rc, rgb, effect, effectParam);
	}

	public static int drawIt(SpriteBatchEffect batch, SpriteFont f, string s, short flags, CRect rc, int rgb, int effect, int effectParam)
	{
		if (s.Length == 0)
		{
			s = " ";
		}
		int lineSpacing = f.LineSpacing;
		int num = (int)f.MeasureString(" ").X;
		int num2 = rc.right - rc.left;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		if (xPos == null)
		{
			xPos = new int[100];
		}
		bool flag = false;
		bool flag2 = false;
		Color c = Color.Black;
		if ((flags & 0x400) == 0)
		{
			c = getColor(rgb);
		}
		int num8 = rc.top;
		int num9 = lineSpacing;
		if ((num9 & 1) != 0)
		{
			num9++;
		}
		if ((flags & 0x800) != 0)
		{
			if ((flags & 4) != 0)
			{
				num8 = rc.top + (rc.bottom - rc.top) / 2 - num9 / 2;
			}
			else if ((flags & 8) != 0)
			{
				num8 = rc.bottom - lineSpacing;
			}
		}
		int num10 = num8;
		do
		{
			num5 = num3;
			int num11 = 0;
			int num12 = 0;
			num7 += lineSpacing;
			while (true)
			{
				xPos[num11] = num12;
				num11++;
				int num13 = num4;
				num4 = -1;
				if (num5 < s.Length)
				{
					num4 = s.IndexOf(' ', num5);
				}
				if (num4 == -1)
				{
					num4 = s.Length;
				}
				if (num4 < num5)
				{
					num12 -= num;
					break;
				}
				string text = s.Substring(num5, num4 - num5);
				int num14 = (int)f.MeasureString(text).X;
				if (num12 + num14 > num2)
				{
					num11--;
					if (num11 > 0)
					{
						num14 -= num;
						num12 -= num;
						num4 = num13;
						break;
					}
					for (int i = num5; i < num4; i++)
					{
						num14 = (int)f.MeasureString(s.Substring(i, 1)).X;
						if (num12 + num14 >= num2)
						{
							i--;
							if (i > 0)
							{
								num6 = Math.Max(num12, num6);
								if ((flags & 0x400) == 0)
								{
									num12 = (((flags & 1) != 0) ? (rc.left + (rc.right - rc.left) / 2 - num12 / 2) : (((flags & 2) == 0) ? rc.left : (rc.right - num12)));
									text = s.Substring(num5, i - num5);
									vector.X = num12;
									vector.Y = num8;
									batch.DrawString(f, text, vector, c, effect, effectParam);
								}
							}
							num4 = -1;
							if (i < s.Length)
							{
								num4 = s.IndexOf(' ', i);
							}
							flag = true;
							if (num4 >= 0)
							{
								flag2 = true;
							}
							break;
						}
						num12 += num14;
					}
				}
				if (flag)
				{
					break;
				}
				num12 += num14;
				if (num12 + num > num2)
				{
					break;
				}
				num12 += num;
				num5 = num4 + 1;
			}
			if (!flag2)
			{
				if (flag)
				{
					break;
				}
				num6 = Math.Max(num12, num6);
				if ((flags & 0x400) == 0)
				{
					num12 = (((flags & 1) != 0) ? (rc.left + (rc.right - rc.left) / 2 - num12 / 2) : (((flags & 2) == 0) ? rc.left : (rc.right - num12)));
					num5 = num3;
					for (int j = 0; j < num11; j++)
					{
						num4 = -1;
						if (num5 < s.Length)
						{
							num4 = s.IndexOf(' ', num5);
						}
						if (num4 == -1)
						{
							num4 = s.Length;
						}
						if (num4 < num5)
						{
							break;
						}
						string text = s.Substring(num5, num4 - num5);
						vector.X = num12 + xPos[j];
						vector.Y = num8;
						batch.DrawString(f, text, vector, c, effect, effectParam);
						num5 = num4 + 1;
					}
				}
			}
			flag = false;
			flag2 = false;
			num8 += lineSpacing;
			num3 = num4 + 1;
		}
		while (num3 < s.Length);
		if ((flags & 0x400) != 0)
		{
			rc.right = rc.left + num6;
			rc.bottom = num10 + num7;
		}
		return num7;
	}

	public static string intToString(int value, int displayFlags)
	{
		string text = $"{value:D}";
		if ((displayFlags & 0xF) != 0)
		{
			int num = displayFlags & 0xF;
			if (text.Length > num)
			{
				text = text.Substring(0, num);
			}
			else
			{
				while (text.Length < num)
				{
					text = "0" + text;
				}
			}
		}
		return text;
	}

	public static string doubleToString(double value, int displayFlags)
	{
		string text;
		if ((displayFlags & 0x200) == 0)
		{
			text = $"{value:G}";
		}
		else
		{
			int num = ((displayFlags & 0xF0) >> 4) + 1;
			int num2 = -1;
			if ((displayFlags & 0x400) != 0)
			{
				num2 = (displayFlags & 0xF000) >> 12;
			}
			else if (value != 0.0 && value > -1.0 && value < 1.0)
			{
				num2 = num;
			}
			text = ((num2 >= 0) ? string.Format("{0:F" + num2 + "}", value) : string.Format("{0:G" + num + "}", value));
			if ((displayFlags & 0x800) != 0)
			{
				int i = 0;
				foreach (int num3 in text)
				{
					if (num3 != 46 && num3 != 43 && num3 != 45 && num3 != 101 && num3 != 69)
					{
						i++;
					}
				}
				bool flag = false;
				if (text[0] == '-')
				{
					flag = true;
					text = text.Substring(1);
				}
				for (; i < num; i++)
				{
					text = "0" + text;
				}
				if (flag)
				{
					text = "-" + text;
				}
			}
		}
		return text;
	}

	public static int getNextPowerOfTwo(int value)
	{
		uint num = (uint)(value - 1);
		num |= num >> 1;
		num |= num >> 2;
		num |= num >> 4;
		num |= num >> 8;
		num |= num >> 16;
		return (int)(num + 1);
	}

	public void createThePixel(SpriteBatchEffect batch)
	{
		pixel = new Texture2D(batch.GraphicsDevice, 1, 1);
		pixel.SetData(new Color[1] { Color.White });
	}

	public void drawFilledRectangle(CRunApp app, int x, int y, int width, int height, int rgb, int thickness, int borderColor, int effect, int effectParam)
	{
		Color color = getColor(rgb);
		drawFilledRectangleSub(app.spriteBatch, x, y, width, height, color, effect, effectParam);
		if (thickness > 0)
		{
			color = getColor(borderColor);
			drawFilledRectangleSub(app.spriteBatch, x, y, width, thickness, color, effect, effectParam);
			drawFilledRectangleSub(app.spriteBatch, x, y + height - thickness, width, thickness, color, effect, effectParam);
			drawFilledRectangleSub(app.spriteBatch, x, y, thickness, height, color, effect, effectParam);
			drawFilledRectangleSub(app.spriteBatch, x + width - thickness, y, thickness, height, color, effect, effectParam);
		}
	}

	public void drawRect(SpriteBatchEffect batch, CRect rc, int rgb, int effect, int effectParam)
	{
		int width = rc.right - rc.left;
		int height = rc.bottom - rc.top;
		Color color = getColor(rgb);
		drawFilledRectangleSub(batch, rc.left, rc.top, width, 1, color, effect, effectParam);
		drawFilledRectangleSub(batch, rc.left, rc.bottom - 1, width, 1, color, effect, effectParam);
		drawFilledRectangleSub(batch, rc.left, rc.top, 1, height, color, effect, effectParam);
		drawFilledRectangleSub(batch, rc.right - 1, rc.top, 1, height, color, effect, effectParam);
	}

	public void drawRect(SpriteBatchEffect batch, int x1, int y1, int width, int height, int rgb, int effect, int effectParam)
	{
		Color color = getColor(rgb);
		drawFilledRectangleSub(batch, x1, y1, width, 1, color, effect, effectParam);
		drawFilledRectangleSub(batch, x1, y1 + height - 1, width, 1, color, effect, effectParam);
		drawFilledRectangleSub(batch, x1, y1, 1, height, color, effect, effectParam);
		drawFilledRectangleSub(batch, x1 + width - 1, y1, 1, height, color, effect, effectParam);
	}

	public void fillRect(SpriteBatchEffect batch, CRect rc, int rgb, int effect, int effectParam)
	{
		Color color = getColor(rgb);
		drawFilledRectangleSub(batch, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, color, effect, effectParam);
	}

	public void fillRect(SpriteBatchEffect batch, int x1, int y1, int width, int height, int rgb, int effect, int effectParam)
	{
		Color color = getColor(rgb);
		drawFilledRectangleSub(batch, x1, y1, width, height, color, effect, effectParam);
	}

	public void drawFilledRectangleSub(SpriteBatchEffect batch, int x, int y, int width, int height, Color color, int effect, int effectParam)
	{
		if (pixel == null)
		{
			createThePixel(batch);
		}
		tempRect.X = x;
		tempRect.Y = y;
		tempRect.Width = width;
		tempRect.Height = height;
		batch.Draw(pixel, tempRect, null, color, effect, effectParam);
	}

	private static void drawColorLine(Color[] colors, int x1, int y1, int x2, int width, Color color)
	{
		int num = y1 * width;
		for (int i = x1; i < x2; i++)
		{
			colors[i + num] = color;
		}
	}

	public static Texture2D createUpArrow(CRunApp app, int width, int height, int rgb)
	{
		if (width == 0 || height == 0)
		{
			return null;
		}
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		Color color = getColor(rgb);
		for (double num = 0.0; num < (double)height; num++)
		{
			double num2 = (double)(width / 2) - num / (double)height * (double)(width / 2);
			double num3 = (double)(width / 2) + num / (double)height * (double)(width / 2);
			drawColorLine(array, (int)num2, (int)num, (int)num3, nextPowerOfTwo, color);
		}
		texture2D.SetData(array);
		return texture2D;
	}

	public static Texture2D createDownArrow(CRunApp app, int width, int height, int rgb)
	{
		if (width == 0 || height == 0)
		{
			return null;
		}
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		Color color = getColor(rgb);
		for (double num = 0.0; num < (double)height; num++)
		{
			double num2 = num / (double)height * (double)(width / 2);
			double num3 = (double)width - num / (double)height * (double)(width / 2);
			drawColorLine(array, (int)num2, (int)num, (int)num3, nextPowerOfTwo, color);
		}
		texture2D.SetData(array);
		return texture2D;
	}

	public static Texture2D createRoundedRect(CRunApp app, int width, int height, int colorRect1, int colorRect2, int colorFill1, int colorFill2)
	{
		if (width == 0 || height == 0)
		{
			return null;
		}
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		Color color = new Color(0, 0, 0, 0);
		for (int i = 0; i < height; i++)
		{
			int num = nextPowerOfTwo * i;
			for (int j = width; j < nextPowerOfTwo; j++)
			{
				array[num + j] = color;
			}
		}
		float num2 = (colorRect1 >> 16) & 0xFF;
		float num3 = (colorRect1 >> 8) & 0xFF;
		float num4 = colorRect1 & 0xFF;
		float num5 = (colorRect2 >> 16) & 0xFF;
		float num6 = (colorRect2 >> 8) & 0xFF;
		float num7 = colorRect2 & 0xFF;
		float num8 = (colorFill1 >> 16) & 0xFF;
		float num9 = (colorFill1 >> 8) & 0xFF;
		float num10 = colorFill1 & 0xFF;
		float num11 = (colorFill2 >> 16) & 0xFF;
		float num12 = (colorFill2 >> 8) & 0xFF;
		float num13 = colorFill2 & 0xFF;
		int num14 = height / 6;
		double num15 = Math.PI / 100.0;
		float num16 = (num11 - num8) / (float)height;
		float num17 = (num12 - num9) / (float)height;
		float num18 = (num13 - num10) / (float)height;
		float num19 = (num5 - num2) / (float)height;
		float num20 = (num6 - num3) / (float)height;
		float num21 = (num7 - num4) / (float)height;
		int num22 = -1;
		for (double num23 = 0.0; num23 < Math.PI / 2.0; num23 += num15)
		{
			int num24 = (int)((double)num14 - (double)num14 * Math.Cos(num23));
			int num25 = (int)((double)(width - num14) + (double)num14 * Math.Cos(num23));
			int num26 = (int)((double)num14 - (double)num14 * Math.Sin(num23));
			if (num26 != num22)
			{
				num22 = num26;
				float num27 = num8 + num16 * (float)num26;
				float num28 = num9 + num17 * (float)num26;
				float num29 = num10 + num18 * (float)num26;
				color = new Color((byte)num27, (byte)num28, (byte)num29);
				drawColorLine(array, num24, num26, num25, nextPowerOfTwo, color);
				float num30 = num2 + num19 * (float)num26;
				float num31 = num3 + num20 * (float)num26;
				float num32 = num4 + num21 * (float)num26;
				color = new Color((byte)num30, (byte)num31, (byte)num32);
				if (num26 == 0)
				{
					drawColorLine(array, num24, num26, num25, nextPowerOfTwo, color);
					continue;
				}
				array[num24 + num26 * nextPowerOfTwo] = color;
				array[num25 + num26 * nextPowerOfTwo] = color;
			}
		}
		for (int num26 = num14; num26 < height - num14; num26++)
		{
			float num27 = num8 + num16 * (float)num26;
			float num28 = num9 + num17 * (float)num26;
			float num29 = num10 + num18 * (float)num26;
			color = new Color((byte)num27, (byte)num28, (byte)num29);
			drawColorLine(array, 0, num26, width, nextPowerOfTwo, color);
			float num30 = num2 + num19 * (float)num26;
			float num31 = num3 + num20 * (float)num26;
			float num32 = num4 + num21 * (float)num26;
			color = new Color((byte)num30, (byte)num31, (byte)num32);
			array[num26 * nextPowerOfTwo] = color;
			array[width - 1 + num26 * nextPowerOfTwo] = color;
		}
		for (double num23 = num15; num23 < Math.PI / 2.0; num23 += num15)
		{
			int num24 = (int)((double)num14 - (double)num14 * Math.Cos(num23));
			int num25 = (int)((double)(width - num14) + (double)num14 * Math.Cos(num23));
			int num26 = (int)((double)(height - num14) + (double)num14 * Math.Sin(num23));
			if (num26 != num22)
			{
				num22 = num26;
				float num27 = num8 + num16 * (float)num26;
				float num28 = num9 + num17 * (float)num26;
				float num29 = num10 + num18 * (float)num26;
				color = new Color((byte)num27, (byte)num28, (byte)num29);
				drawColorLine(array, num24, num26, num25, nextPowerOfTwo, color);
				float num30 = num2 + num19 * (float)num26;
				float num31 = num3 + num20 * (float)num26;
				float num32 = num4 + num21 * (float)num26;
				color = new Color((byte)num30, (byte)num31, (byte)num32);
				array[num24 + num26 * nextPowerOfTwo] = color;
				array[num25 + num26 * nextPowerOfTwo] = color;
				if (num26 == height - 1)
				{
					drawColorLine(array, num24, num26, num25, nextPowerOfTwo, color);
				}
			}
		}
		texture2D.SetData(array);
		return texture2D;
	}

	public static Texture2D createGradientRectangle(CRunApp app, int width, int height, int color1, int color2, bool bVertical, int thickness, int borderColor)
	{
		if (width == 0 || height == 0)
		{
			return null;
		}
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		float num = (color1 >> 16) & 0xFF;
		float num2 = (color1 >> 8) & 0xFF;
		float num3 = color1 & 0xFF;
		float num4 = (color2 >> 16) & 0xFF;
		float num5 = (color2 >> 8) & 0xFF;
		float num6 = color2 & 0xFF;
		Color color3 = new Color(num, num2, num3);
		float num7 = num;
		float num8 = num2;
		float num9 = num3;
		if (bVertical)
		{
			float num10 = (num4 - num) / (float)height;
			float num11 = (num5 - num2) / (float)height;
			float num12 = (num6 - num3) / (float)height;
			for (int i = 0; i < height; i++)
			{
				int num13 = i * nextPowerOfTwo;
				if (num != num7 || num2 != num8 || num3 != num9)
				{
					color3 = new Color((byte)num, (byte)num2, (byte)num3);
				}
				for (int j = 0; j < width; j++)
				{
					array[num13 + j] = color3;
				}
				num += num10;
				num2 += num11;
				num3 += num12;
			}
		}
		else
		{
			float num10 = (num4 - num) / (float)width;
			float num11 = (num5 - num2) / (float)width;
			float num12 = (num6 - num3) / (float)width;
			for (int j = 0; j < width; j++)
			{
				if (num != num7 || num2 != num8 || num3 != num9)
				{
					color3 = new Color((byte)num, (byte)num2, (byte)num3);
				}
				for (int i = 0; i < height; i++)
				{
					array[i * nextPowerOfTwo + j] = color3;
				}
				num += num10;
				num2 += num11;
				num3 += num12;
			}
		}
		color3 = new Color(0, 0, 0, 0);
		for (int i = 0; i < height; i++)
		{
			int num13 = nextPowerOfTwo * i;
			for (int j = width; j < nextPowerOfTwo; j++)
			{
				array[num13 + j] = color3;
			}
		}
		if (thickness > 0)
		{
			color3 = getColor(borderColor);
			fillRectangle(array, nextPowerOfTwo, 0, 0, width, thickness, color3);
			fillRectangle(array, nextPowerOfTwo, 0, height - thickness, width, height, color3);
			fillRectangle(array, nextPowerOfTwo, 0, 0, thickness, height, color3);
			fillRectangle(array, nextPowerOfTwo, width - thickness, 0, width, height, color3);
		}
		texture2D.SetData(array);
		return texture2D;
	}

	private static void fillRectangle(Color[] colors, int textureWidth, int x1, int y1, int x2, int y2, Color color)
	{
		for (int i = y1; i < y2; i++)
		{
			int num = i * textureWidth;
			for (int j = x1; j < x2; j++)
			{
				colors[num + j] = color;
			}
		}
	}

	public void drawPatternRectangle(SpriteBatchEffect batch, CImage image, int xx, int yy, int width, int height, int thickness, int borderColor, int effect, int effectParam)
	{
		int num = (width + image.width - 1) / image.width;
		int num2 = (height + image.height - 1) / image.height;
		tempRect.Width = image.width;
		tempRect.Height = image.height;
		Texture2D texture = image.image;
		Rectangle? sourceRectangle = null;
		if (image.mosaic != 0)
		{
			texture = image.app.imageBank.mosaics[image.mosaic];
			sourceRectangle = image.mosaicRectangle;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				tempRect.X = xx + i * image.width;
				tempRect.Y = yy + j * image.height;
				batch.Draw(texture, tempRect, sourceRectangle, Color.White, effect, effectParam);
			}
		}
		if (thickness > 0)
		{
			num *= image.width;
			num2 *= image.height;
			Color color = getColor(borderColor);
			drawFilledRectangleSub(batch, xx, yy, num, thickness, color, effect, effectParam);
			drawFilledRectangleSub(batch, xx, yy + num2 - thickness, num, thickness, color, effect, effectParam);
			drawFilledRectangleSub(batch, xx, yy, thickness, num2, color, effect, effectParam);
			drawFilledRectangleSub(batch, xx + num - thickness, yy, thickness, num2, color, effect, effectParam);
		}
	}

	public static Texture2D createEllipse(CRunApp app, int width, int height, int borderWidth, int borderColor)
	{
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		_ = width / 2;
		_ = height / 2;
		Color color = new Color(0, 0, 0, 0);
		int num = nextPowerOfTwo * height;
		for (int i = 0; i < num; i++)
		{
			array[i] = color;
		}
		color = getColor(borderColor);
		createEllipse(array, nextPowerOfTwo, width, height, borderWidth, color);
		texture2D.SetData(array);
		return texture2D;
	}

	public static Texture2D createFilledEllipse(CRunApp app, int width, int height, int rgb, int borderWidth, int borderColor)
	{
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		int num = width / 2 - 1;
		int num2 = height / 2 - 1;
		Color color = new Color(0, 0, 0, 0);
		int num3 = nextPowerOfTwo * height;
		for (int i = 0; i < num3; i++)
		{
			array[i] = color;
		}
		double num4 = Math.PI / 1000.0;
		int num5 = -1;
		color = getColor(rgb);
		for (double num6 = 0.0; num6 < Math.PI; num6 += num4)
		{
			int num7 = (int)((double)num2 - (double)num2 * Math.Sin(Math.PI / 2.0 + num6));
			if (num7 != num5)
			{
				int num8 = (int)((double)num + (double)num * Math.Cos(Math.PI / 2.0 + num6));
				int num9 = (int)((double)num + (double)num * Math.Cos(Math.PI / 2.0 - num6));
				int num10 = num7 * nextPowerOfTwo;
				for (int j = num8; j < num9; j++)
				{
					array[num10 + j] = color;
				}
				num5 = num7;
			}
		}
		if (borderWidth > 0)
		{
			color = getColor(borderColor);
			createEllipse(array, nextPowerOfTwo, width, height, borderWidth, color);
		}
		texture2D.SetData(array);
		return texture2D;
	}

	public static Texture2D createGradientEllipse(CRunApp app, int width, int height, int color1, int color2, bool bVertical, int borderWidth, int borderColor)
	{
		int nextPowerOfTwo = getNextPowerOfTwo(width);
		Texture2D texture2D = new Texture2D(app.spriteBatch.GraphicsDevice, nextPowerOfTwo, height);
		Color[] array = new Color[nextPowerOfTwo * height];
		int num = width / 2 - 1;
		int num2 = height / 2 - 1;
		Color color3 = new Color(0, 0, 0, 0);
		int num3 = nextPowerOfTwo * height;
		for (int i = 0; i < num3; i++)
		{
			array[i] = color3;
		}
		float num4 = (color1 >> 16) & 0xFF;
		float num5 = (color1 >> 8) & 0xFF;
		float num6 = color1 & 0xFF;
		float num7 = (color2 >> 16) & 0xFF;
		float num8 = (color2 >> 8) & 0xFF;
		float num9 = color2 & 0xFF;
		color3 = new Color(num4, num5, num6);
		float num10 = num4;
		float num11 = num5;
		float num12 = num6;
		double num13 = Math.PI / 1000.0;
		int num14 = -1;
		int num15 = -1;
		if (bVertical)
		{
			float num16 = (num7 - num4) / (float)height;
			float num17 = (num8 - num5) / (float)height;
			float num18 = (num9 - num6) / (float)height;
			for (double num19 = 0.0; num19 < Math.PI; num19 += num13)
			{
				int num20 = (int)((double)num2 - (double)num2 * Math.Sin(Math.PI / 2.0 + num19));
				if (num20 != num15)
				{
					if (num4 != num10 || num5 != num11 || num6 != num12)
					{
						color3 = new Color((byte)num4, (byte)num5, (byte)num6);
					}
					int num21 = (int)((double)num + (double)num * Math.Cos(Math.PI / 2.0 + num19));
					int num22 = (int)((double)num + (double)num * Math.Cos(Math.PI / 2.0 - num19));
					int num23 = num20 * nextPowerOfTwo;
					for (int j = num21; j < num22; j++)
					{
						array[num23 + j] = color3;
					}
					num15 = num20;
					num4 += num16;
					num5 += num17;
					num6 += num18;
				}
			}
		}
		else
		{
			float num16 = (num7 - num4) / (float)width;
			float num17 = (num8 - num5) / (float)width;
			float num18 = (num9 - num6) / (float)width;
			for (double num19 = 0.0; num19 < Math.PI; num19 += num13)
			{
				int j = (int)((double)num + (double)num * Math.Cos(Math.PI - num19));
				if (j != num14)
				{
					if (num4 != num10 || num5 != num11 || num6 != num12)
					{
						color3 = new Color((byte)num4, (byte)num5, (byte)num6);
					}
					int num24 = (int)((double)num2 - (double)num2 * Math.Sin(Math.PI - num19));
					int num25 = (int)((double)num2 - (double)num2 * Math.Sin(Math.PI + num19));
					for (int num20 = num24; num20 < num25; num20++)
					{
						array[num20 * nextPowerOfTwo + j] = color3;
					}
					num14 = j;
					num4 += num16;
					num5 += num17;
					num6 += num18;
				}
			}
		}
		if (borderWidth > 0)
		{
			color3 = getColor(borderColor);
			createEllipse(array, nextPowerOfTwo, width, height, borderWidth, color3);
		}
		texture2D.SetData(array);
		return texture2D;
	}

	private static void createEllipse(Color[] colors, int textureWidth, int width, int height, int thickness, Color color)
	{
		int num = width / 2 - 1;
		int num2 = height / 2 - 1;
		int num3 = num;
		int num4 = num2;
		double num5 = Math.PI / 1000.0;
		for (int i = 0; i < thickness; i++)
		{
			for (double num6 = 0.0; num6 < Math.PI * 2.0; num6 += num5)
			{
				int num7 = (int)((double)num3 + (double)num * Math.Cos(Math.PI / 2.0 + num6));
				int num8 = (int)((double)num4 - (double)num2 * Math.Sin(Math.PI / 2.0 + num6));
				colors[num8 * textureWidth + num7] = color;
			}
			num--;
			num2--;
		}
	}

	public void drawLine(SpriteBatchEffect batch, int x1, int y1, int x2, int y2, int rgb, int thickness, int effect, int effectParam)
	{
		if (pixel == null)
		{
			createThePixel(batch);
		}
		Vector2 vector = new Vector2(x1, y1);
		Vector2 value = new Vector2(x2, y2);
		float x3 = Vector2.Distance(vector, value);
		float rotation = (float)Math.Atan2(value.Y - vector.Y, value.X - vector.X);
		Color color = getColor(rgb);
		batch.Draw(pixel, vector, null, color, rotation, Vector2.Zero, new Vector2(x3, thickness), SpriteEffects.None, 0f, effect, effectParam);
	}

	public static void replaceColor(CRunApp app, Color[] pixels, int width, int height, int oldColor, int newColor)
	{
		Color color = getColor(newColor);
		byte b = (byte)getRValueJava(oldColor);
		byte b2 = (byte)getGValueJava(oldColor);
		byte b3 = (byte)getBValueJava(oldColor);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Color color2 = pixels[i * width + j];
				if (color2.R == b && color2.G == b2 && color2.B == b3)
				{
					if (newColor != 0)
					{
						color.A = color2.A;
					}
					else
					{
						color.A = 0;
					}
					pixels[i * width + j] = color;
				}
			}
		}
	}
}
