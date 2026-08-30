using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class MenuItem
{
	public Vector2 positionOffset;

	public Vector2 positionFinal;

	public Vector2 position;

	public float wide;

	public float angle;

	public float newAngle;

	private SpriteFont font;

	public string text;

	public string desc;

	public string descUnlock;

	public bool selectable;

	public bool selected;

	private Color color;

	private bool active;

	public float value;

	public int loop;

	public float opac = 0f;

	public int Width => (int)font.MeasureString(text).X;

	public int Height => (int)font.MeasureString(text).Y;

	public MenuItem(SpriteFont font, string text, Vector2 position, bool selectable, float value, int loop)
	{
		Initialize(font, text, "", "", position, selectable, value, loop, 100f);
	}

	public MenuItem(SpriteFont font, string text, string desc, Vector2 position, bool selectable, float value, int loop)
	{
		Initialize(font, text, desc, "", position, selectable, value, loop, 100f);
	}

	public MenuItem(SpriteFont font, string text, string desc, string descUnlock, Vector2 position, bool selectable, float value, int loop)
	{
		Initialize(font, text, desc, descUnlock, position, selectable, value, loop, 100f);
	}

	public void Initialize(SpriteFont font, string text, string desc, string descUnlock, Vector2 position, bool selectable, float value, int loop, float wide)
	{
		this.wide = wide;
		this.font = font;
		this.text = text;
		this.desc = desc;
		this.descUnlock = descUnlock;
		this.selectable = selectable;
		this.position = position;
		positionFinal = position;
		angle = 0f;
		this.value = value;
		this.loop = loop;
		active = true;
	}

	public void reset(Vector2 pos)
	{
		position = pos;
		positionFinal = pos;
		positionOffset = pos;
		opac = 0f;
	}

	public void Update(float incAngle)
	{
		opac = MathHelper.Lerp(opac, 1f, 0.05f);
		angle -= incAngle;
		if (angle > (float)Math.PI)
		{
			angle += (float)Math.PI;
		}
		if (angle < 0f)
		{
			angle -= (float)Math.PI;
		}
		angle = MathHelper.WrapAngle(angle);
		positionOffset.X = position.X + (float)(Math.Sin(angle) * (double)wide * 0.5);
		positionOffset.Y = position.Y + (float)(Math.Cos(angle) * (double)wide);
		positionFinal.X = MathHelper.Lerp(positionFinal.X, positionOffset.X, 0.3f);
		positionFinal.Y = MathHelper.Lerp(positionFinal.Y, positionOffset.Y, 0.3f);
		selected = false;
		newAngle = angle - (float)Math.PI / 2f;
		newAngle /= (float)Math.PI / 2f;
		newAngle = Math.Abs(newAngle);
		float num = (0.9f - newAngle) / 1.5f;
		num -= 0.1f;
		if (selectable && selected)
		{
			color = new Color(num, num * 1.5f, num * 2f, opac);
		}
		else
		{
			color = new Color(num, num, num, num * opac);
		}
	}

	public bool isStopped()
	{
		return Vector2.Distance(positionFinal, positionOffset) < 15f;
	}

	public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd)
	{
		string text = "";
		if (value >= 0f)
		{
			switch (loop)
			{
			case 1:
				text = " " + value + "%";
				break;
			case 2:
				text = ((value != 1f) ? " OFF" : " ON");
				break;
			case 3:
				text = " " + value;
				break;
			case 4:
			{
				int num2 = (int)value;
				float num3 = (value - (float)num2) * 10000f;
				int num4 = (int)num3;
				if (num4 % 10 == 9 || num4 % 10 == 7)
				{
					num4++;
				}
				text = " " + num2 + " x " + num4;
				break;
			}
			case 5:
			{
				int num = (int)(value * 100f);
				text = "    Easy";
				if (num > 50)
				{
					text = "    Normal";
				}
				if (num > 100)
				{
					text = "    Hard";
				}
				if (num > 180)
				{
					text = "    Nightmare";
				}
				break;
			}
			default:
				text = "";
				break;
			}
		}
		int bottom = gd.Viewport.TitleSafeArea.Bottom;
		bottom = (int)MathHelper.Clamp(bottom, gd.Viewport.Height - 50, gd.Viewport.Height - 200);
		if (!active || !(opac > 0f))
		{
			return;
		}
		if (selected)
		{
			if (selectable)
			{
				spriteBatch.DrawString(font, this.text + text, positionFinal, Color.LightCyan * opac, 0f, new Vector2(0f, Height / 2), 1.1f, SpriteEffects.None, 0.5f);
				spriteBatch.DrawString(font, desc, new Vector2(gd.Viewport.Width / 2, bottom), Color.LightCyan * opac * 0.8f, 0f, font.MeasureString(desc) / 2f, 0.7f, SpriteEffects.None, 0.5f);
			}
			else
			{
				spriteBatch.DrawString(font, this.text + text, positionFinal, new Color(0.15f, 0.17f, 0.2f, 0.1f * opac), 0f, new Vector2(0f, Height / 2), 1.1f, SpriteEffects.None, 0.5f);
				spriteBatch.DrawString(font, descUnlock, new Vector2(gd.Viewport.Width / 2, bottom), Color.LightCyan * opac * 0.8f, 0f, font.MeasureString(descUnlock) / 2f, 0.7f, SpriteEffects.None, 0.5f);
			}
		}
		else
		{
			spriteBatch.DrawString(font, this.text + text, positionFinal, color, 0f, new Vector2(0f, Height / 2), (float)(int)color.A / 255f + 0.2f, SpriteEffects.None, 0.5f);
		}
	}
}
