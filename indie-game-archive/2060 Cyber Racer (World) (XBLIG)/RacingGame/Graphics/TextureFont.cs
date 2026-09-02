using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RacingGame.Graphics;

internal class TextureFont : IDisposable
{
	internal class FontToRender
	{
		public int x;

		public int y;

		public string text;

		public Color color;

		public float scale;

		public FontToRender(int setX, int setY, string setText, Color setColor)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			x = setX;
			y = setY;
			text = setText;
			color = setColor;
			scale = 1f;
		}

		public FontToRender(int setX, int setY, string setText, Color setColor, float setScale)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			x = setX;
			y = setY;
			text = setText;
			color = setColor;
			scale = setScale;
		}
	}

	private const string GameFontFilename = "peric-0.png";

	private const int FontHeight = 35;

	private const int SubRenderHeight = 5;

	private static Rectangle[] CharRects;

	private Texture fontTexture;

	private SpriteBatch fontSprite;

	private static List<FontToRender> remTexts;

	public static int Height => BaseGame.YToRes1050(30);

	public TextureFont()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		base._002Ector();
		fontTexture = new Texture("peric-0.png");
		fontSprite = new SpriteBatch(BaseGame.Device);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (fontTexture != null)
			{
				fontTexture.Dispose();
			}
			if (fontSprite != null)
			{
				fontSprite.Dispose();
			}
		}
	}

	public static int GetTextWidth(string text)
	{
		int num = 0;
		char[] array = text.ToCharArray();
		foreach (int num2 in array)
		{
			if (num2 >= 32 && num2 - 32 < CharRects.Length)
			{
				num += BaseGame.XToRes1400(CharRects[num2 - 32].Height);
			}
		}
		return num;
	}

	public static void WriteText(int x, int y, string text, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		remTexts.Add(new FontToRender(x, y, text, color));
	}

	public static void WriteText(int x, int y, string text)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		remTexts.Add(new FontToRender(x, y, text, Color.White));
	}

	public static void WriteTextCentered(int x, int y, string text)
	{
		WriteText(x - GetTextWidth(text) / 2, y - Height / 2, text);
	}

	public static void WriteTextCentered(int x, int y, string text, Color color, float scale)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		int textWidth = GetTextWidth(text);
		remTexts.Add(new FontToRender(x - (int)Math.Round((float)textWidth * scale / 2f), y - (int)Math.Round((float)Height * scale / 2f), text, color, scale));
	}

	public static void WriteGameTime(int x, int y, int timeMilliseconds, Color col)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		WriteText(x, y, ((timeMilliseconds < 0) ? "-" : "") + Math.Abs(timeMilliseconds) / 1000 / 60 + ":" + (Math.Abs(timeMilliseconds) / 1000 % 60).ToString("00") + "." + (Math.Abs(timeMilliseconds) / 10 % 100).ToString("00"), col);
	}

	public void WriteAll()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (remTexts.Count == 0)
		{
			return;
		}
		fontSprite.Begin((SpriteBlendMode)1);
		Rectangle val = default(Rectangle);
		for (int i = 0; i < remTexts.Count; i++)
		{
			FontToRender fontToRender = remTexts[i];
			int num = fontToRender.x;
			int y = fontToRender.y;
			Color color = fontToRender.color;
			char[] array = fontToRender.text.ToCharArray();
			foreach (int num2 in array)
			{
				if (num2 >= 32 && num2 - 32 < CharRects.Length)
				{
					Rectangle value = CharRects[num2 - 32];
					value.Y++;
					value.Height = 35;
					((Rectangle)(ref val))._002Ector(num, y - BaseGame.YToRes1050(5), value.Width, value.Height);
					val.Width = BaseGame.XToRes1400((int)Math.Round((float)val.Width * fontToRender.scale));
					val.Height = BaseGame.YToRes1050((int)Math.Round((float)val.Height * fontToRender.scale));
					fontSprite.Draw(fontTexture.XnaTexture, val, (Rectangle?)value, color);
					int height = CharRects[num2 - 32].Height;
					num += BaseGame.XToRes1400((int)Math.Round((float)height * fontToRender.scale));
				}
			}
		}
		fontSprite.End();
		remTexts.Clear();
	}

	static TextureFont()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090f: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092f: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		CharRects = (Rectangle[])(object)new Rectangle[95]
		{
			new Rectangle(0, 0, 1, 8),
			new Rectangle(1, 0, 9, 8),
			new Rectangle(10, 0, 12, 11),
			new Rectangle(22, 0, 19, 17),
			new Rectangle(41, 0, 15, 13),
			new Rectangle(56, 0, 22, 20),
			new Rectangle(78, 0, 26, 24),
			new Rectangle(104, 0, 7, 5),
			new Rectangle(111, 0, 12, 10),
			new Rectangle(123, 0, 12, 10),
			new Rectangle(135, 0, 14, 12),
			new Rectangle(149, 0, 16, 14),
			new Rectangle(165, 0, 8, 7),
			new Rectangle(173, 0, 9, 7),
			new Rectangle(182, 0, 8, 7),
			new Rectangle(190, 0, 17, 16),
			new Rectangle(207, 0, 22, 20),
			new Rectangle(229, 0, 12, 11),
			new Rectangle(0, 35, 18, 16),
			new Rectangle(18, 35, 14, 12),
			new Rectangle(32, 35, 18, 16),
			new Rectangle(50, 35, 15, 13),
			new Rectangle(65, 35, 17, 15),
			new Rectangle(82, 35, 17, 15),
			new Rectangle(99, 35, 17, 15),
			new Rectangle(116, 35, 16, 15),
			new Rectangle(132, 35, 8, 7),
			new Rectangle(140, 35, 8, 7),
			new Rectangle(148, 35, 13, 11),
			new Rectangle(161, 35, 16, 14),
			new Rectangle(177, 35, 13, 11),
			new Rectangle(190, 35, 14, 12),
			new Rectangle(204, 35, 22, 20),
			new Rectangle(226, 35, 22, 20),
			new Rectangle(0, 70, 16, 15),
			new Rectangle(16, 70, 21, 19),
			new Rectangle(37, 70, 23, 21),
			new Rectangle(60, 70, 17, 15),
			new Rectangle(77, 70, 16, 14),
			new Rectangle(93, 70, 23, 21),
			new Rectangle(116, 70, 26, 24),
			new Rectangle(142, 70, 10, 8),
			new Rectangle(152, 70, 10, 9),
			new Rectangle(162, 70, 21, 20),
			new Rectangle(183, 70, 16, 15),
			new Rectangle(199, 70, 27, 25),
			new Rectangle(226, 70, 27, 25),
			new Rectangle(0, 105, 25, 24),
			new Rectangle(25, 105, 17, 15),
			new Rectangle(42, 105, 25, 24),
			new Rectangle(67, 105, 20, 18),
			new Rectangle(87, 105, 15, 13),
			new Rectangle(102, 105, 18, 16),
			new Rectangle(120, 105, 25, 23),
			new Rectangle(145, 105, 22, 20),
			new Rectangle(167, 105, 28, 26),
			new Rectangle(195, 105, 21, 19),
			new Rectangle(216, 105, 18, 16),
			new Rectangle(234, 105, 21, 19),
			new Rectangle(0, 140, 12, 10),
			new Rectangle(12, 140, 17, 16),
			new Rectangle(29, 140, 12, 10),
			new Rectangle(41, 140, 15, 13),
			new Rectangle(56, 140, 15, 13),
			new Rectangle(71, 140, 10, 9),
			new Rectangle(81, 140, 19, 17),
			new Rectangle(100, 140, 17, 15),
			new Rectangle(117, 140, 20, 18),
			new Rectangle(137, 140, 22, 20),
			new Rectangle(159, 140, 17, 15),
			new Rectangle(176, 140, 16, 15),
			new Rectangle(192, 140, 20, 19),
			new Rectangle(212, 140, 23, 22),
			new Rectangle(235, 140, 10, 8),
			new Rectangle(245, 140, 10, 8),
			new Rectangle(0, 175, 19, 17),
			new Rectangle(19, 175, 16, 14),
			new Rectangle(35, 175, 26, 25),
			new Rectangle(61, 175, 25, 23),
			new Rectangle(86, 175, 22, 20),
			new Rectangle(108, 175, 16, 15),
			new Rectangle(124, 175, 22, 21),
			new Rectangle(146, 175, 17, 15),
			new Rectangle(163, 175, 14, 13),
			new Rectangle(177, 175, 17, 15),
			new Rectangle(194, 175, 23, 21),
			new Rectangle(217, 175, 18, 16),
			new Rectangle(0, 210, 27, 25),
			new Rectangle(27, 210, 18, 17),
			new Rectangle(45, 210, 17, 15),
			new Rectangle(62, 210, 18, 17),
			new Rectangle(80, 210, 12, 10),
			new Rectangle(92, 210, 13, 12),
			new Rectangle(105, 210, 12, 10),
			new Rectangle(117, 210, 15, 13)
		};
		remTexts = new List<FontToRender>();
	}
}
