using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World.Views.Room1;

internal class View1_1_1 : View
{
	private SoundEffect m_sound_poly;

	private SoundEffect m_sound_water;

	private TextureAnimation m_polygrip_anim;

	private bool m_play_poly_anim;

	private bool m_fade_to_black;

	private float m_fade_alpha;

	private Texture2D m_fade;

	private float m_fade_timer;

	private float m_fade_timeout = 2f;

	public View1_1_1(Game game, Area room)
		: base(game, room)
	{
		m_name = "View1_1_1";
		m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(m_room.m_content_path + "vattenblandare")));
		m_fade = m_room.m_CL.LoadTexture("HUD/black");
		m_scenes.Add(new Scene(m_fade));
		m_polygrip_anim = (TextureAnimation)m_room.m_CL.GetContent(m_room.m_content_path + "Animations/Polygrip/");
		m_sound_poly = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_polygrip");
		m_sound_water = m_room.m_CL.LoadSound(m_room.m_content_path + "Sound/bathroom_water");
		m_enable_navigator = false;
		m_hud_state = HUD.HUD_STATE.BACK;
		m_use_text_fade = true;
	}

	public override void Clear()
	{
		m_sound_poly = null;
		m_sound_water = null;
		if (m_polygrip_anim != null)
		{
			m_polygrip_anim.Clear();
			m_polygrip_anim = null;
		}
		if (m_fade != null)
		{
			((GraphicsResource)m_fade).Dispose();
			m_fade = null;
		}
		base.Clear();
	}

	public override void Setup()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		m_back_trigger = new ViewTrigger(m_game, this, m_room.GetView("View1_1"), ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
		CursorTrigger cursorTrigger = new CursorTrigger(m_game, Game.VIEW_RECT, "BlandareStuck", Trigger.TRIGGER_TYPE.ZOOM);
		cursorTrigger.m_activate_own = true;
		m_triggers.Add(cursorTrigger);
	}

	public override bool HandleUseEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "Polygrip01")
		{
			m_play_poly_anim = true;
			m_polygrip_anim.Play();
			m_game.m_hud.FadeOut();
			m_game.m_input_enabled = false;
			m_game.m_show_cursor = false;
			m_game.m_inventory_enabled = false;
			m_game.m_game_data.m_view = "View1_1";
			m_game.m_game_data.SetState("Room1.GateState", "4");
			m_room.HandleEvent("View1_1_1.UsePolygrip");
			m_game.PlaySound(m_sound_poly, 0.2f);
			return true;
		}
		return base.HandleUseEvent(s_event);
	}

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "BlandareStuck")
		{
			m_game.m_hud.ShowText("The handle is missing ...", m_use_text_fade);
		}
		base.HandleEvent(s_event);
	}

	public override void Update(TimeSpan elapsed)
	{
		base.Update(elapsed);
		if (m_play_poly_anim && m_polygrip_anim != null)
		{
			m_polygrip_anim.Update(elapsed);
			if (m_polygrip_anim.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
			{
				m_play_poly_anim = false;
				m_fade_to_black = true;
				m_game.PlaySound(m_sound_water, 0.2f);
			}
		}
		if (m_fade_to_black && m_fade_alpha < 255f)
		{
			m_fade_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 200f;
			if (m_fade_alpha >= 255f)
			{
				m_fade_alpha = 255f;
				ChangeScene(1);
				m_fade_timer = m_fade_timeout;
			}
		}
		if (m_fade_timer > 0f)
		{
			m_fade_timer -= (float)elapsed.TotalMilliseconds * 0.001f;
			if (m_fade_timer <= 0f)
			{
				m_fade_timer = 0f;
				m_game.m_input_enabled = true;
				m_game.m_show_cursor = true;
				m_game.m_inventory_enabled = true;
				m_game.ActivateTrigger(m_back_trigger);
			}
		}
	}

	public override void Draw(SpriteBatch SB)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(SB);
		if (m_play_poly_anim)
		{
			m_polygrip_anim.Draw(SB);
		}
		if (m_fade_to_black)
		{
			m_polygrip_anim.m_state = Animation2D.ANIM_STATE.ANIM_STATE_PAUSED;
			m_polygrip_anim.SetFrame(28);
			m_polygrip_anim.Draw(SB);
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_fade, Game.VIEW_RECT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Math.Round(m_fade_alpha)));
			SB.End();
		}
	}
}
