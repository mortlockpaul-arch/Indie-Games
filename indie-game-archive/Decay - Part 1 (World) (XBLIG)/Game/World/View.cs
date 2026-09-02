using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.World;

public class View
{
	public enum VIEW_DIRECTION
	{
		LEFT,
		RIGHT,
		UP,
		DOWN
	}

	public string m_name;

	public ViewTrigger m_left_trigger;

	public ViewTrigger m_right_trigger;

	public ViewTrigger m_up_trigger;

	public ViewTrigger m_down_trigger;

	public ViewTrigger m_back_trigger;

	public ViewTrigger m_proceed_trigger;

	public Animation2D m_left_animation;

	public Animation2D m_right_animation;

	public Animation2D m_up_animation;

	public Animation2D m_down_animation;

	public List<Scene> m_scenes;

	public int m_current_scene;

	public List<Trigger> m_triggers;

	public List<ViewItem> m_items;

	public bool m_enable_navigator;

	public HUD.HUD_STATE m_hud_state;

	public Game m_game;

	protected Vector2 m_scene_pos;

	protected Scene m_fade_from_scene;

	protected View m_next_view;

	protected Area m_room;

	protected bool m_fade_scene;

	protected RenderTarget2D m_RT;

	protected bool m_render_to_scene;

	public bool m_use_text_fade;

	public View(Game game, Area room)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_name = "";
		m_scenes = new List<Scene>();
		m_triggers = new List<Trigger>();
		m_items = new List<ViewItem>();
		m_enable_navigator = true;
		m_hud_state = HUD.HUD_STATE.NAVIGATOR;
		m_scene_pos = Vector2.Zero;
		base._002Ector();
		m_game = game;
		m_room = room;
		if (m_room != null)
		{
			m_room.AddView(this);
		}
	}

	public virtual void Clear()
	{
		m_game = null;
		m_room = null;
		if (m_RT != null)
		{
			((RenderTarget)m_RT).Dispose();
			m_RT = null;
		}
		if (m_left_trigger != null)
		{
			m_left_trigger.Clear();
			m_left_trigger = null;
		}
		if (m_right_trigger != null)
		{
			m_right_trigger.Clear();
			m_right_trigger = null;
		}
		if (m_up_trigger != null)
		{
			m_up_trigger.Clear();
			m_up_trigger = null;
		}
		if (m_down_trigger != null)
		{
			m_down_trigger.Clear();
			m_down_trigger = null;
		}
		if (m_back_trigger != null)
		{
			m_back_trigger.Clear();
			m_back_trigger = null;
		}
		if (m_proceed_trigger != null)
		{
			m_proceed_trigger.Clear();
			m_proceed_trigger = null;
		}
		for (int i = 0; i < m_triggers.Count; i++)
		{
			if (m_triggers[i] != null)
			{
				m_triggers[i].Clear();
				m_triggers[i] = null;
			}
		}
		m_triggers.Clear();
		m_triggers = null;
		for (int j = 0; j < m_scenes.Count; j++)
		{
			if (m_scenes[j] != null)
			{
				m_scenes[j].Clear();
				m_scenes[j] = null;
			}
		}
		m_scenes.Clear();
		m_scenes = null;
		m_fade_from_scene = null;
		if (m_left_animation != null)
		{
			m_left_animation.Clear();
			m_left_animation = null;
		}
		if (m_right_animation != null)
		{
			m_right_animation.Clear();
			m_right_animation = null;
		}
		if (m_up_animation != null)
		{
			m_up_animation.Clear();
			m_up_animation = null;
		}
		if (m_down_animation != null)
		{
			m_down_animation.Clear();
			m_down_animation = null;
		}
		for (int k = 0; k < m_items.Count; k++)
		{
			if (m_items[k] != null)
			{
				m_items[k].Clear();
				m_items[k] = null;
			}
		}
		m_items.Clear();
		m_items = null;
	}

	public virtual void Reset()
	{
		if (m_left_trigger != null)
		{
			m_left_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		if (m_right_trigger != null)
		{
			m_right_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		if (m_up_trigger != null)
		{
			m_up_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		if (m_down_trigger != null)
		{
			m_down_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		if (m_back_trigger != null)
		{
			m_back_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		if (m_proceed_trigger != null)
		{
			m_proceed_trigger.m_state = Trigger.TRIGGER_STATE.IDLE;
		}
		for (int i = 0; i < m_triggers.Count; i++)
		{
			if (m_triggers[i] != null)
			{
				m_triggers[i].m_state = Trigger.TRIGGER_STATE.IDLE;
			}
		}
		for (int j = 0; j < m_items.Count; j++)
		{
			if (m_items[j] != null && m_items[j].m_state != ViewItem.VIEWITEM_STATE.REMOVED)
			{
				m_items[j].m_state = ViewItem.VIEWITEM_STATE.SCENE;
			}
		}
	}

	public virtual void ResetCursorTriggers()
	{
		CursorTrigger cursorTrigger = null;
		for (int i = 0; i < m_triggers.Count; i++)
		{
			if (m_triggers[i] != null)
			{
				cursorTrigger = null;
				if (m_triggers[i].m_class_name == "CursorTrigger")
				{
					cursorTrigger = (CursorTrigger)m_triggers[i];
				}
				if (cursorTrigger != null)
				{
					cursorTrigger.m_state = Trigger.TRIGGER_STATE.IDLE;
				}
			}
		}
		cursorTrigger = null;
	}

	public void RemoveItem(string name)
	{
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i] != null && m_items[i].m_name == name)
			{
				m_items[i].Remove();
			}
		}
	}

	protected virtual void RemoveTrigger(Trigger trigger)
	{
		if (trigger != null)
		{
			trigger.Clear();
			m_triggers[m_triggers.IndexOf(trigger)] = null;
		}
	}

	protected virtual void ChangeScene(int scene)
	{
		if (scene >= 0 && scene < m_scenes.Count)
		{
			m_current_scene = scene;
		}
	}

	public virtual void Setup()
	{
	}

	public virtual void onDirection(VIEW_DIRECTION dir)
	{
		if (m_game.m_state != Game.GAME_STATE.ACTIVE_TRIGGER)
		{
			switch (dir)
			{
			case VIEW_DIRECTION.LEFT:
				m_game.ActivateTrigger(m_left_trigger);
				break;
			case VIEW_DIRECTION.RIGHT:
				m_game.ActivateTrigger(m_right_trigger);
				break;
			case VIEW_DIRECTION.UP:
				m_game.ActivateTrigger(m_up_trigger);
				break;
			case VIEW_DIRECTION.DOWN:
				m_game.ActivateTrigger(m_down_trigger);
				break;
			}
		}
	}

	public virtual void onBack()
	{
		if (m_back_trigger != null)
		{
			m_game.ActivateTrigger(m_back_trigger);
		}
	}

	public virtual void onProceed()
	{
		if (m_proceed_trigger != null)
		{
			m_game.ActivateTrigger(m_proceed_trigger);
		}
	}

	public virtual void HandleEvent(string s_event)
	{
	}

	public virtual bool HandleUseEvent(string s_event)
	{
		return false;
	}

	public virtual void FadeFromScene(int scene)
	{
		FadeFromScene(scene, 255f);
	}

	public virtual void FadeFromScene(int scene, float speed)
	{
		if (scene < 0 || scene >= m_scenes.Count)
		{
			return;
		}
		m_fade_from_scene = m_scenes[scene];
		m_fade_from_scene.FadeOut(speed);
		m_fade_scene = true;
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i] != null)
			{
				m_items[i].FadeOut();
			}
		}
		m_game.m_input_enabled = false;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		if (m_scenes[m_current_scene] != null)
		{
			m_scenes[m_current_scene].Update(elapsed);
		}
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i] != null && (m_items[i].m_state == ViewItem.VIEWITEM_STATE.FADE_OUT || m_items[i].m_state == ViewItem.VIEWITEM_STATE.FADE_OUT_REMOVE))
			{
				m_items[i].Update(elapsed);
			}
		}
		if (m_left_trigger != null)
		{
			m_left_trigger.Update(elapsed);
		}
		if (m_right_trigger != null)
		{
			m_right_trigger.Update(elapsed);
		}
		if (m_up_trigger != null)
		{
			m_up_trigger.Update(elapsed);
		}
		if (m_down_trigger != null)
		{
			m_down_trigger.Update(elapsed);
		}
		for (int j = 0; j < m_triggers.Count; j++)
		{
			if (m_triggers[j] != null)
			{
				m_triggers[j].Update(elapsed);
			}
		}
		if (m_fade_scene && m_fade_from_scene != null)
		{
			m_fade_from_scene.Update(elapsed);
			if (m_fade_from_scene.m_fade_state == Scene.FADE_STATE.IDLE)
			{
				m_fade_from_scene = null;
				m_fade_scene = false;
				m_game.m_input_enabled = true;
				onFadeFromSceneFinished();
			}
		}
	}

	public virtual void UpdateLayerAnimations(TimeSpan elapsed)
	{
	}

	protected virtual void onFadeFromSceneFinished()
	{
	}

	public virtual void UpdateEffect(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (SB == null)
		{
			return;
		}
		if (m_scenes[m_current_scene] != null)
		{
			m_scenes[m_current_scene].Draw(SB);
		}
		if (m_fade_scene && m_fade_from_scene != null)
		{
			m_fade_from_scene.Draw(SB);
		}
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i] != null)
			{
				m_items[i].Draw(SB, Color.White);
			}
		}
	}

	public virtual void DrawEffect(SpriteBatch SB)
	{
	}

	public virtual void RenderToScene(SpriteBatch SB)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		Console.WriteLine("View.RenderToScene");
		if (m_RT == null)
		{
			PresentationParameters presentationParameters = ((Game)m_game).GraphicsDevice.PresentationParameters;
			m_RT = new RenderTarget2D(((Game)m_game).GraphicsDevice, Game.VIEW_RECT.Width, Game.VIEW_RECT.Height, 1, (SurfaceFormat)1, presentationParameters.MultiSampleType, presentationParameters.MultiSampleQuality, (RenderTargetUsage)0);
		}
		SB.GraphicsDevice.SetRenderTarget(0, m_RT);
		SB.GraphicsDevice.Clear((ClearOptions)1, new Color((byte)0, (byte)0, (byte)0, (byte)0), 0f, 0);
		Draw(SB);
		SB.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		m_scenes[m_current_scene].m_texture = m_RT.GetTexture();
		ChangeScene(m_current_scene);
	}
}
