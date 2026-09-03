using System;
using Core;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheMare1.World.Views.DreamMainRoom;

internal class Zoom_Door1 : View
{
	private enum STATE
	{
		WAIT,
		SHOW_TEXT,
		FADE_OUT_TEXT
	}

	private STATE m_state;

	protected float m_alpha;

	private SpriteFont m_font;

	protected float m_timer;

	protected string m_text = "";

	public Zoom_Door1(Core.Game game, Area room, string xml_path)
		: base(game, room, xml_path)
	{
		m_state = STATE.WAIT;
		m_font = getContentLoader().LoadFont("Fonts/SpriteFont2");
	}

	public override void Clear()
	{
		m_font = null;
		base.Clear();
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "Zoom_Door1.onMareAnim")
		{
			m_game.m_game_menu_enabled = false;
			m_game.m_a_pressed = true;
			m_state = STATE.SHOW_TEXT;
			m_alpha = 1f;
			m_timer = 3f;
			m_text = m_game.m_language.GetString("To be continued ...");
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		try
		{
			base.Update(elapsed);
			if (m_state == STATE.WAIT)
			{
				return;
			}
			switch (m_state)
			{
			case STATE.SHOW_TEXT:
				m_timer -= (float)elapsed.TotalSeconds;
				if (m_timer <= 0f)
				{
					m_timer = 0f;
					m_state = STATE.FADE_OUT_TEXT;
				}
				break;
			case STATE.FADE_OUT_TEXT:
				m_alpha -= (float)elapsed.TotalSeconds * 1f;
				if (m_alpha <= 0f)
				{
					m_alpha = 0f;
					m_game.HandleEvent("Game.Finished");
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		base.Draw(SB);
		switch (m_state)
		{
		case STATE.SHOW_TEXT:
		case STATE.FADE_OUT_TEXT:
			if (m_text != "")
			{
				Vector2 vector = m_font.MeasureString(m_text);
				Vector2 zero = Vector2.Zero;
				zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
				zero.Y = ((float)Core.Game.VIEW_RECT.Height - vector.Y) / 2f;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.DrawString(m_font, m_text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
				SB.DrawString(m_font, m_text, zero, Color.White * m_alpha);
				SB.End();
			}
			break;
		}
	}
}
