using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Screens;

public class ControlsScreen : Screen
{
	private const string title = "Controls";

	private const string ls = "\u0088\u0089 Move/Attack";

	private const string rs = "\u008a\u008b Zoom In/Out";

	private const string a = "\u0080\u0081 Poison Skill";

	private const string b = "\u0082\u0083 Frenzy Skill";

	private const string x = "\u0084\u0085 Freeze Skill";

	private const string y = "\u0086\u0087 Orb Skill";

	private const string back = "\u008c\u008d Inventory";

	private const string start = "\u008e\u008f Pause Menu";

	private const string lb = "\u0090\u0091 Stats";

	private const string rb = "\u0092\u0093 Skills";

	private const string lt = "\u0094\u0095 Interact";

	private const string rt = "\u0096\u0097 Drink Potion";

	private static Texture2D pixel;

	private static Sprite background;

	private bool open = true;

	private bool close;

	private float delta;

	private Vector2 titlePos = new Vector2(640f, 120f);

	private Vector2 shadowOffset = new Vector2(2f, 2f);

	private Color titleTextColor = new Color(151, 100, 0);

	private Color titleTextShadowColor = new Color(255, 255, 0);

	private Vector2 tl = new Vector2(240f, 200f);

	private Vector2 tm = new Vector2(652f, 200f);

	private Vector2 lineOffset = new Vector2(0f, 64f);

	private Vector2 bgPos = new Vector2(640f, 360f);

	private Color descColor = Color.Black;

	private Color bgColor = MC.ScreenManager.BackgroundColor;

	public ControlsScreen()
		: base(modal: true)
	{
	}

	public static void Load(ContentManager content)
	{
		pixel = content.Load<Texture2D>("Sprites\\Pixel");
		background = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\ControlsBackground"));
	}

	public override void update(GameTime gameTime)
	{
		if (MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B))
		{
			open = false;
			close = true;
		}
		if (open)
		{
			delta += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (!((double)delta < 1.0))
			{
				delta = 1f;
				open = false;
			}
		}
		else if (close)
		{
			delta -= (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (!((double)delta > 0.0))
			{
				MC.ScreenManager.removeScreen(this);
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		Vector2 zero = Vector2.Zero;
		Color color = bgColor;
		if (open || close)
		{
			color *= delta;
			zero.Y = (float)(-720.0 * (1.0 - (double)delta));
		}
		base.spriteBatch.Begin();
		base.spriteBatch.Draw(pixel, base.viewportRect, color);
		background.Draw(base.spriteBatch, zero + bgPos);
		Text.DrawCentered(base.spriteBatch, zero + titlePos + shadowOffset, "Controls", titleTextShadowColor);
		Text.DrawCentered(base.spriteBatch, zero + titlePos, "Controls", titleTextColor);
		Vector2 vector = zero + tl;
		Vector2 vector2 = zero + tm;
		Text.Draw(base.spriteBatch, vector, "\u0088\u0089 Move/Attack", descColor);
		Text.Draw(base.spriteBatch, vector + lineOffset, "\u008a\u008b Zoom In/Out", descColor);
		Text.Draw(base.spriteBatch, vector + lineOffset * 2f, "\u0080\u0081 Poison Skill", descColor);
		Text.Draw(base.spriteBatch, vector + lineOffset * 3f, "\u0082\u0083 Frenzy Skill", descColor);
		Text.Draw(base.spriteBatch, vector + lineOffset * 4f, "\u0084\u0085 Freeze Skill", descColor);
		Text.Draw(base.spriteBatch, vector + lineOffset * 5f, "\u0086\u0087 Orb Skill", descColor);
		Text.Draw(base.spriteBatch, vector2, "\u008c\u008d Inventory", descColor);
		Text.Draw(base.spriteBatch, vector2 + lineOffset, "\u008e\u008f Pause Menu", descColor);
		Text.Draw(base.spriteBatch, vector2 + lineOffset * 2f, "\u0090\u0091 Stats", descColor);
		Text.Draw(base.spriteBatch, vector2 + lineOffset * 3f, "\u0092\u0093 Skills", descColor);
		Text.Draw(base.spriteBatch, vector2 + lineOffset * 4f, "\u0094\u0095 Interact", descColor);
		Text.Draw(base.spriteBatch, vector2 + lineOffset * 5f, "\u0096\u0097 Drink Potion", descColor);
		base.spriteBatch.End();
	}
}
