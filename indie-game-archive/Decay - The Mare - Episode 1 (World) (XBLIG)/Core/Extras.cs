using System;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core;

public class Extras
{
	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	protected Game m_game;

	public Extras(Game game, SGSContentLoader CL)
	{
		m_game = game;
		m_font = CL.LoadFont("Fonts/SpriteFont2");
		m_font2 = CL.LoadFont("Fonts/SpriteFont1");
	}

	public virtual void Clear()
	{
		m_game = null;
		m_font = null;
		m_font2 = null;
	}

	public virtual void Reset()
	{
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
