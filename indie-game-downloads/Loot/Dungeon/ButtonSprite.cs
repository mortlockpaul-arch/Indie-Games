using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public static class ButtonSprite
{
	public static Sprite A;

	public static Sprite B;

	public static Sprite X;

	public static Sprite Y;

	public static Sprite AGray;

	public static Sprite BGray;

	public static Sprite XGray;

	public static Sprite YGray;

	public static Sprite LS;

	public static Sprite RS;

	public static Sprite Back;

	public static Sprite Start;

	public static Sprite LB;

	public static Sprite RB;

	public static Sprite LT;

	public static Sprite RT;

	public static Sprite[] ATimer;

	public static Sprite[] BTimer;

	public static Sprite[] XTimer;

	public static Sprite[] YTimer;

	private static Texture2D Buttons;

	private static Texture2D ButtonTimer;

	public static void LoadContent(ContentManager content)
	{
		Buttons = content.Load<Texture2D>("Sprites\\UI\\Buttons");
		A = new StillSprite(Buttons, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f));
		B = new StillSprite(Buttons, new Rectangle(64, 0, 64, 64), new Vector2(32f, 32f));
		X = new StillSprite(Buttons, new Rectangle(128, 0, 64, 64), new Vector2(32f, 32f));
		Y = new StillSprite(Buttons, new Rectangle(192, 0, 64, 64), new Vector2(32f, 32f));
		AGray = new StillSprite(Buttons, new Rectangle(0, 64, 64, 64), new Vector2(32f, 32f));
		BGray = new StillSprite(Buttons, new Rectangle(64, 64, 64, 64), new Vector2(32f, 32f));
		XGray = new StillSprite(Buttons, new Rectangle(128, 64, 64, 64), new Vector2(32f, 32f));
		YGray = new StillSprite(Buttons, new Rectangle(192, 64, 64, 64), new Vector2(32f, 32f));
		LS = new StillSprite(Buttons, new Rectangle(0, 128, 64, 64), new Vector2(32f, 32f));
		RS = new StillSprite(Buttons, new Rectangle(64, 128, 64, 64), new Vector2(32f, 32f));
		Back = new StillSprite(Buttons, new Rectangle(128, 128, 64, 64), new Vector2(32f, 32f));
		Start = new StillSprite(Buttons, new Rectangle(192, 128, 64, 64), new Vector2(32f, 32f));
		LB = new StillSprite(Buttons, new Rectangle(0, 192, 64, 64), new Vector2(32f, 32f));
		RB = new StillSprite(Buttons, new Rectangle(64, 192, 64, 64), new Vector2(32f, 32f));
		LT = new StillSprite(Buttons, new Rectangle(128, 192, 64, 64), new Vector2(32f, 32f));
		RT = new StillSprite(Buttons, new Rectangle(192, 192, 64, 64), new Vector2(32f, 32f));
		ButtonTimer = content.Load<Texture2D>("Sprites\\HUD\\ButtonTimer");
		ATimer = new Sprite[16];
		for (int i = 0; i < ATimer.Length; i++)
		{
			ATimer[i] = new StillSprite(ButtonTimer, new Rectangle(i * 64, 0, 64, 64), new Vector2(32f, 32f));
		}
		BTimer = new Sprite[16];
		for (int j = 0; j < BTimer.Length; j++)
		{
			BTimer[j] = new StillSprite(ButtonTimer, new Rectangle(j * 64, 64, 64, 64), new Vector2(32f, 32f));
		}
		XTimer = new Sprite[16];
		for (int k = 0; k < XTimer.Length; k++)
		{
			XTimer[k] = new StillSprite(ButtonTimer, new Rectangle(k * 64, 128, 64, 64), new Vector2(32f, 32f));
		}
		YTimer = new Sprite[16];
		for (int l = 0; l < YTimer.Length; l++)
		{
			YTimer[l] = new StillSprite(ButtonTimer, new Rectangle(l * 64, 192, 64, 64), new Vector2(32f, 32f));
		}
	}
}
