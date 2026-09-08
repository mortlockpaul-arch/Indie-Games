using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class PostDraw : DrawableGameComponent
{
	private ScreenManager m_Screen;

	private List<Texture2D> m_Texture = new List<Texture2D>();

	private List<Vector2> m_TexPos = new List<Vector2>();

	private List<Color> m_TexColor = new List<Color>();

	private List<SpriteEffects> m_TextureEffect = new List<SpriteEffects>();

	private List<Rectangle> m_TextureRect = new List<Rectangle>();

	private List<string> m_TextToDraw = new List<string>();

	private List<Vector3> m_TextPosition = new List<Vector3>();

	private List<Color> m_TextColor = new List<Color>();

	private List<SpriteFont> m_TextFont = new List<SpriteFont>();

	public PostDraw(GameState state)
		: base(state.ScreenManager.Game)
	{
		m_Screen = state.ScreenManager;
		if (base.Game == null)
		{
			throw new ArgumentNullException("game");
		}
	}

	public void AddText(string text, Vector3 textposition, Color textcolor, SpriteFont font)
	{
		m_TextToDraw.Add(text);
		m_TextColor.Add(textcolor);
		m_TextPosition.Add(textposition);
		m_TextFont.Add(font);
	}

	public void AddTexture(Texture2D tex, Vector2 position, Color color, Texture2D border)
	{
		if (border != null)
		{
			int num = border.Width - tex.Width;
			Vector2 item = new Vector2(position.X - (float)(num / 2), position.Y - (float)(num / 2));
			m_Texture.Add(border);
			m_TexPos.Add(item);
			m_TexColor.Add(Color.White);
			m_TextureRect.Add(new Rectangle(0, 0, tex.Width + num, tex.Height + num));
			m_TextureEffect.Add(SpriteEffects.None);
		}
		m_TextureRect.Add(new Rectangle(0, 0, tex.Width, tex.Height));
		m_TextureEffect.Add(SpriteEffects.None);
		m_Texture.Add(tex);
		m_TexPos.Add(position);
		m_TexColor.Add(color);
	}

	public void AddTexture(Texture2D tex, Vector2 position, Color color, Rectangle rect, SpriteEffects spriteEffect)
	{
		m_TextureEffect.Add(spriteEffect);
		m_TextureRect.Add(rect);
		m_Texture.Add(tex);
		m_TexPos.Add(position);
		m_TexColor.Add(color);
	}

	public override void Draw(GameTime gameTime)
	{
		m_Screen.SpriteBatch.Begin();
		for (int i = 0; i < m_Texture.Count; i++)
		{
			m_Screen.SpriteBatch.Draw(m_Texture[i], m_TexPos[i], m_TextureRect[i], m_TexColor[i], 0f, Vector2.Zero, 1f, m_TextureEffect[i], 1f);
		}
		for (int j = 0; j < m_TextToDraw.Count; j++)
		{
			m_Screen.SpriteBatch.DrawString(m_TextFont[j], m_TextToDraw[j], new Vector2(m_TextPosition[j].X, m_TextPosition[j].Y), m_TextColor[j], m_TextPosition[j].Z, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}
		m_Screen.SpriteBatch.End();
		base.Draw(gameTime);
	}
}
