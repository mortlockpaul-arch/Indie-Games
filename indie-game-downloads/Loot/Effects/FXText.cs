using System;
using Eyehook.Framework;
using Loot.Core;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public class FXText : FXLocation
{
	private const int poolSize = 16;

	private static int next = 0;

	private static FXText[] pool;

	private TimeSpan timer;

	private char sign;

	private int value;

	private string text;

	private float alpha;

	private Vector2 centerOffset;

	private Particle particle;

	private Color color;

	private Color shadowColor;

	private static TimeSpan duration = TimeSpan.FromSeconds(4.0);

	private Vector2 shadowOffset = new Vector2(2f, 2f);

	private FXText()
	{
	}

	public static FXText GetFX(Location loc, string text, Color color, Color shadowColor)
	{
		return GetFX(loc, '\0', 0, text, color, shadowColor);
	}

	public static FXText GetFX(Location loc, char sign, int value, string text, Color color, Color shadowColor)
	{
		FXText fXText = pool[next];
		next = (next + 1) % 16;
		fXText.Location = loc;
		fXText.timer = TimeSpan.Zero;
		fXText.sign = sign;
		fXText.value = value;
		fXText.text = text;
		fXText.alpha = 255f;
		fXText.particle.pos = Vector2.Zero;
		fXText.particle.vel = new Vector2(0f, -128f);
		fXText.particle.rotation = 0f;
		fXText.color = color;
		fXText.shadowColor = shadowColor;
		int num = text.Length * 24;
		if (sign != 0)
		{
			num = num + 24 + Text.Width(value);
		}
		fXText.centerOffset = new Vector2(12 - num / 2, 0f);
		return fXText;
	}

	public static void Load(ContentManager content)
	{
		pool = new FXText[16];
		for (int i = 0; i < 16; i++)
		{
			pool[i] = new FXText();
		}
	}

	public override void Update(GameTime gameTime)
	{
		timer += gameTime.ElapsedGameTime;
		if (timer >= duration)
		{
			DM.RemoveEffect(this);
			return;
		}
		alpha = (float)(1.0 - timer.TotalSeconds / duration.TotalSeconds);
		particle.pos += particle.vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		if (Profile.Preferences.TextFX)
		{
			base.Draw(spriteBatch);
			float zoom = DungeonView.Camera.Zoom;
			Vector2 v = Position + centerOffset * zoom + particle.pos * zoom;
			Vector2 v2 = v + shadowOffset * zoom;
			Color color = this.color * alpha;
			Color color2 = shadowColor * alpha;
			if (sign != 0)
			{
				Text.Draw(spriteBatch, ref v2, sign, color2, zoom);
				Text.Draw(spriteBatch, ref v, sign, color, zoom);
				Text.Draw(spriteBatch, ref v2, value, color2, zoom);
				Text.Draw(spriteBatch, ref v, value, color, zoom);
			}
			Text.Draw(spriteBatch, ref v2, text, color2, zoom);
			Text.Draw(spriteBatch, ref v, text, color, zoom);
		}
	}
}
