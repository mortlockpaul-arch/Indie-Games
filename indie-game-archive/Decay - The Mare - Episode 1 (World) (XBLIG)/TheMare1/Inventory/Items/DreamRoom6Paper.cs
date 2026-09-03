using System;
using Core;
using Core.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace TheMare1.Inventory.Items;

public class DreamRoom6Paper : Item
{
	public const string ID = "DreamRoom6Paper";

	private Texture2D m_man;

	private SoundEffect m_voice;

	private SoundEffectInstance m_voice_inst;

	private SoundEffect m_man_appears;

	private SoundEffectInstance m_man_appears_inst;

	private bool m_playing_voice;

	private float m_timer;

	private float m_man_alpha;

	private bool m_fade_out_man;

	public DreamRoom6Paper(Core.Game game, ContentManager CM)
		: base(game)
	{
		m_icon = CM.Load<Texture2D>("Inventory/Items/DreamRoom6Paper/the_new_house_thumb");
		m_man = CM.Load<Texture2D>("Inventory/Items/DreamRoom6Paper/document_thenewhouse_man_313x175");
		m_voice = CM.Load<SoundEffect>("Inventory/Items/DreamRoom6Paper/Sound/voice");
		m_voice_inst = m_voice.CreateInstance();
		m_man_appears = CM.Load<SoundEffect>("Inventory/Items/DreamRoom6Paper/Sound/man_appears");
		m_man_appears_inst = m_man_appears.CreateInstance();
		m_man_appears_inst.Pitch = 0.5f;
		onLoadExamine(game.m_CL);
		m_use_scrolling = true;
		if (m_game.GraphicsDevice.DisplayMode.Height < 720)
		{
			m_min_scroll_y = (float)Core.Game.TS_AREA.Bottom - 1630f;
		}
		else
		{
			m_min_scroll_y = (float)Core.Game.TS_AREA.Bottom - 1485f;
		}
		m_name = m_game.m_language.GetString("Document");
		m_desc = "";
		m_id = "DreamRoom6Paper";
	}

	public override void Clear()
	{
		if (m_man != null)
		{
			m_man.Dispose();
			m_man = null;
		}
		if (m_voice_inst != null)
		{
			m_voice_inst.Stop();
			m_voice_inst.Dispose();
			m_voice_inst = null;
		}
		if (m_voice != null)
		{
			m_voice.Dispose();
			m_voice = null;
		}
		if (m_man_appears_inst != null)
		{
			m_man_appears_inst.Stop();
			m_man_appears_inst.Dispose();
			m_man_appears_inst = null;
		}
		if (m_man_appears != null)
		{
			m_man_appears.Dispose();
			m_man_appears = null;
		}
		base.Clear();
	}

	public override void Reset()
	{
		base.Reset();
		m_playing_voice = false;
		m_timer = 0f;
		m_man_alpha = 0f;
		m_fade_out_man = false;
	}

	public override void onLoadExamine(SGSContentLoader CL)
	{
		try
		{
			base.onLoadExamine(CL);
			if (m_examine_image == null)
			{
				m_examine_image = CL.m_CM.Load<Texture2D>("Inventory/Items/DreamRoom6Paper/document_thenewhouse1");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (!m_playing_voice)
		{
			m_playing_voice = true;
			m_voice_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
			m_voice_inst.Play();
			m_timer = 40000f;
		}
		else if (m_timer > 0f)
		{
			m_timer -= (float)elapsed.TotalMilliseconds;
			if (m_timer <= 0f)
			{
				m_timer = 0f;
				m_man_appears_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.1f;
				m_man_appears_inst.Play();
			}
		}
		else if (!m_fade_out_man)
		{
			m_man_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 0.5f;
			if (m_man_alpha > 1f)
			{
				m_man_alpha = 1f;
			}
		}
		else
		{
			m_man_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 5f;
			if (m_man_alpha < 0f)
			{
				m_man_alpha = 0f;
			}
		}
	}

	public override void onCloseExamine()
	{
		base.onCloseExamine();
		if (m_voice_inst != null)
		{
			m_voice_inst.Stop();
		}
		m_fade_out_man = true;
	}

	public override void onExamineGameMenu()
	{
		base.onExamineGameMenu();
		if (m_voice_inst.State == SoundState.Playing)
		{
			m_voice_inst.Pause();
		}
		if (m_man_appears_inst.State == SoundState.Playing)
		{
			m_man_appears_inst.Pause();
		}
	}

	public override void onExamineGameMenuClosed()
	{
		base.onExamineGameMenuClosed();
		if (m_voice_inst != null)
		{
			m_voice_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f;
			if (m_voice_inst.State == SoundState.Paused)
			{
				m_voice_inst.Resume();
			}
		}
		if (m_man_appears_inst != null)
		{
			m_man_appears_inst.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * 0.1f;
			if (m_man_appears_inst.State == SoundState.Paused)
			{
				m_man_appears_inst.Resume();
			}
		}
	}

	public override void DrawExamineImage(SpriteBatch SB, Color color)
	{
		float num = 1f;
		if (m_game.GraphicsDevice.DisplayMode.Height < 720)
		{
			Rectangle destinationRectangle = new Rectangle((int)((float)Core.Game.VIEW_RECT.Width - 1124f * num) / 2, (int)m_game.m_inventory.m_scroll_y, (int)(1124f * num), (int)(1630f * num));
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_examine_image, destinationRectangle, color);
			if (m_man != null)
			{
				SB.Draw(m_man, new Rectangle(destinationRectangle.Left + 409, destinationRectangle.Top + 233, 45, 142), color * m_man_alpha);
			}
		}
		else
		{
			Rectangle destinationRectangle2 = new Rectangle((int)((float)Core.Game.VIEW_RECT.Width - 1024f * num) / 2, (int)m_game.m_inventory.m_scroll_y, (int)(1024f * num), (int)(1485f * num));
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.Draw(m_examine_image, destinationRectangle2, color);
			if (m_man != null)
			{
				SB.Draw(m_man, new Vector2(destinationRectangle2.Left + 373, destinationRectangle2.Top + 212), color * m_man_alpha);
			}
		}
		SB.End();
	}
}
