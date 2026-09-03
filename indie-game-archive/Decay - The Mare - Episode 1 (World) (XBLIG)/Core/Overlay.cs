using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core;

public class Overlay
{
	private Texture2D m_black_texture;

	private Effect m_shader;

	private SGSContentLoader m_CL;

	private Game m_game;

	public Overlay(Game game)
	{
		m_game = game;
		m_CL = new SGSContentLoader(m_game.Services);
		m_black_texture = m_CL.LoadTexture("HUD/black");
		m_shader = m_CL.m_CM.Load<Effect>("Shader/Brightness");
	}

	public virtual void Clear()
	{
		m_black_texture = null;
		if (m_shader != null)
		{
			m_shader.Dispose();
			m_shader = null;
		}
		if (m_CL != null)
		{
			m_CL.Clear();
			m_CL = null;
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		try
		{
			SB.GraphicsDevice.SetRenderTarget(null);
			float num = (m_game.m_game_settings.m_brightness + 5f) * 0.1f;
			if (num < 1f)
			{
				num *= 0.9f;
			}
			if (num > 1f)
			{
				num *= 1.1f;
			}
			m_shader.Parameters["Brightness"].SetValue(num);
			SB.Begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, m_shader);
			SB.Draw(m_game.m_RT, new Rectangle(0, 0, m_game.m_GDM.PreferredBackBufferWidth, m_game.m_GDM.PreferredBackBufferHeight), Color.White);
			SB.End();
			if (m_black_texture != null)
			{
				int num2 = 4;
				SB.Begin(SpriteSortMode.Deferred, BlendState.Opaque);
				SB.Draw(m_black_texture, new Rectangle(0, 0, m_game.m_GDM.PreferredBackBufferWidth, num2), Color.Black);
				SB.Draw(m_black_texture, new Rectangle(0, 0, num2, m_game.m_GDM.PreferredBackBufferHeight), Color.Black);
				SB.Draw(m_black_texture, new Rectangle(m_game.m_GDM.PreferredBackBufferWidth - num2, 0, num2, m_game.m_GDM.PreferredBackBufferHeight), Color.Black);
				SB.Draw(m_black_texture, new Rectangle(0, m_game.m_GDM.PreferredBackBufferHeight - num2, m_game.m_GDM.PreferredBackBufferWidth, num2), Color.Black);
				SB.End();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Overlay.Draw: " + ex.Message);
		}
	}
}
