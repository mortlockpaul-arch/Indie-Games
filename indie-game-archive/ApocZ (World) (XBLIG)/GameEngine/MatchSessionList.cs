using System;
using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine;

public class MatchSessionList
{
	private const int MAX_ENTRIES = 11;

	private float SearchingDotNum;

	private int DisplayHead;

	private int SelectedEntry;

	private SessionEntryStruct[] SessionEntry = new SessionEntryStruct[11];

	public bool hasFocus;

	public Color diffus;

	public Color shadow;

	public Color texClr;

	public event EventHandler<JoinSessionArgs> JoinSessionDelegate;

	public MatchSessionList()
	{
		for (int i = 0; i < 11; i++)
		{
			SessionEntry[i] = default(SessionEntryStruct);
			SessionEntry[i].valid = false;
			SessionEntry[i].gamerTag = "";
			SessionEntry[i].numPlayers = "";
			SessionEntry[i].matchQuality = "";
		}
	}

	public void Reset()
	{
		DisplayHead = 0;
		SelectedEntry = 0;
		hasFocus = false;
	}

	public bool Update(float eTime)
	{
		return false;
	}

	public void Draw()
	{
		Viewport defualtViewport = EndGameEngine.DefualtViewport;
		Vector2 zero = Vector2.Zero;
		Rectangle a = default(Rectangle);
		zero.X = defualtViewport.TitleSafeArea.Right - 512;
		zero.Y = defualtViewport.TitleSafeArea.Top + 114;
		int num = (int)Menu.defaultFont.MeasureString("M").Y - 2;
		a.X = (int)zero.X - 4;
		a.Y = (int)zero.Y - 2;
		a.Width = 460;
		a.Height = num;
		Menu.spriteBatch.Begin();
		float g = 0.85f;
		Color color = texClr;
		Color d = diffus;
		if (hasFocus)
		{
			g = 1f;
			byte b = (d.B = 211);
			byte r = (d.G = b);
			d.R = r;
		}
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Join Match", zero, shadow, 0f, new Vector2(-2f, -2f), g, SpriteEffects.None, 0);
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Join Match", zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		a.Y += num + 2;
		zero.Y += num + 2;
		Vector2 zero2 = Vector2.Zero;
		Vector2 vector = new Vector2(210f, 0f);
		Vector2 vector2 = new Vector2(290f, 0f);
		for (int i = 0; i < 11; i++)
		{
			float g2 = 0.7f;
			color = texClr;
			d = diffus;
			if (hasFocus && SelectedEntry == i)
			{
				g2 = 0.75f;
				color.R = 0;
				color.G = 0;
				color.B = 0;
				byte b4 = (d.A = diffus.A);
				byte b5 = (d.B = b4);
				byte r2 = (d.G = b5);
				d.R = r2;
			}
			Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, a, color);
			Menu.spriteBatch.DrawString(Menu.defaultFont, SessionEntry[i].gamerTag, zero + zero2, d, 0f, Vector2.Zero, g2, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, SessionEntry[i].numPlayers, zero + vector, d, 0f, Vector2.Zero, g2, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, SessionEntry[i].matchQuality, zero + vector2, d, 0f, Vector2.Zero, g2, SpriteEffects.None, 0);
			a.Y += num + 2;
			zero.Y += num + 2;
		}
		Menu.spriteBatch.End();
	}
}
