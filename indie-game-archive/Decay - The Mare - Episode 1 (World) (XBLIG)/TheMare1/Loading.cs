using System;
using System.Collections.Generic;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1;

public class Loading : Core.Loading
{
	protected Texture2D m_label;

	private List<Image> m_images = new List<Image>();

	private List<int> m_hidden_images = new List<int>();

	private List<int> m_visible_images = new List<int>();

	private float m_image_time = 500f;

	private float m_image_timer;

	public Loading(Core.Game game)
		: base(game)
	{
		try
		{
			m_image_timer = m_image_time;
			for (int i = 1; i <= 12; i++)
			{
				Texture2D texture2D = m_game.m_CL.LoadTexture("Loading/loading" + i);
				int random = m_game.GetRandom(Core.Game.TS_AREA.Left, Core.Game.TS_AREA.Right - texture2D.Width * 2);
				int random2 = m_game.GetRandom(Core.Game.TS_AREA.Top, Core.Game.TS_AREA.Bottom - texture2D.Height);
				m_images.Add(new Image(texture2D, new Rectangle(random, random2, texture2D.Width, texture2D.Height)));
				m_hidden_images.Add(i - 1);
			}
			int num = m_game.GetRandom(1, 5);
			if (m_game.m_state == Core.Game.GAME_STATE.LOADING_GAME)
			{
				num = 0;
			}
			for (int j = 0; j < num; j++)
			{
				ShowImage();
			}
			if (m_game.m_game_data != null)
			{
				if (m_game.m_game_data.GetState("Loading.ForceSuicide") == "1")
				{
					SetSuicide();
					return;
				}
				if (m_game.m_game_data.GetState("Loading.Suicide") == "1" && m_game.GetRandom(0, 100) < 10)
				{
					SetSuicide();
					return;
				}
			}
			m_label = m_game.m_CL.LoadTexture("Loading/loading");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Clear()
	{
		m_label = null;
		m_images.Clear();
		m_images = null;
		base.Clear();
	}

	protected void SetSuicide()
	{
		try
		{
			m_label = m_game.m_CL.LoadTexture("Loading/suicide");
			m_game.m_game_data.SetState("Loading.Suicide", "2");
			m_game.m_game_data.SetState("Loading.ForceSuicide", "");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected void ShowImage()
	{
		try
		{
			if (m_hidden_images.Count > 0)
			{
				int random = m_game.GetRandom(0, m_hidden_images.Count - 1);
				m_visible_images.Add(m_hidden_images[random]);
				m_hidden_images.RemoveAt(random);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			m_image_timer -= elapsed.Milliseconds;
			if (m_image_timer < 0f)
			{
				m_image_timer = m_image_time;
				ShowImage();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Draw(GraphicsDevice device, SpriteBatch SB)
	{
		try
		{
			if (device != null && !device.IsDisposed && SB != null && m_game.m_state != Core.Game.GAME_STATE.LOADING_INTRO)
			{
				device.Clear(Color.Black);
				SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
				SB.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_label, new Vector2(Core.Game.TS_AREA.Right - m_label.Width, Core.Game.TS_AREA.Bottom - m_label.Height), Color.White);
				SB.End();
				for (int i = 0; i < m_visible_images.Count; i++)
				{
					m_images[m_visible_images[i]].Draw(SB, Color.White);
				}
				if (m_game.m_overlay != null)
				{
					m_game.m_overlay.Draw(SB);
				}
				device.Present();
			}
		}
		catch (Exception ex)
		{
			device = null;
			Console.WriteLine(ex.Message);
		}
	}
}
