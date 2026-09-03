using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core.World;

public class View : ScriptObject
{
	public enum VIEW_DIRECTION
	{
		LEFT,
		RIGHT,
		UP,
		DOWN
	}

	public enum FADE_STATE
	{
		NONE,
		IN,
		OUT,
		TO_BLACK,
		BLACK,
		FROM_BLACK
	}

	public ViewTrigger m_left_trigger;

	public ViewTrigger m_right_trigger;

	public ViewTrigger m_up_trigger;

	public ViewTrigger m_down_trigger;

	protected bool m_navigator_left;

	protected bool m_navigator_right;

	protected bool m_navigator_up;

	protected bool m_navigator_down;

	public ViewTrigger m_back_trigger;

	public ViewTrigger m_proceed_trigger;

	public Animation2D m_left_animation;

	public Animation2D m_right_animation;

	public Animation2D m_up_animation;

	public Animation2D m_down_animation;

	public List<Scene> m_scenes = new List<Scene>();

	public int m_current_scene;

	private Dictionary<string, Trigger> m_triggers = new Dictionary<string, Trigger>();

	public List<ViewItem> m_items = new List<ViewItem>();

	public bool m_enable_navigator = true;

	public HUD.HUD_STATE m_hud_state = HUD.HUD_STATE.NAVIGATOR;

	protected Vector2 m_scene_pos = Vector2.Zero;

	protected Scene m_fade_from_scene;

	protected View m_next_view;

	protected Area m_room;

	protected bool m_fade_scene;

	protected RenderTarget2D m_RT;

	protected bool m_render_to_scene;

	public bool m_use_text_fade;

	public bool m_no_text_fade;

	protected Dictionary<string, Animation2D> m_animations;

	protected Dictionary<Animation2D, string> m_anim_finished_events = new Dictionary<Animation2D, string>();

	protected Dictionary<Animation2D, string> m_add_anim_finished_events = new Dictionary<Animation2D, string>();

	protected Dictionary<string, Image> m_images;

	protected List<Object2D> m_visible_objects = new List<Object2D>();

	public FADE_STATE m_fade_state;

	protected float m_fade_speed;

	protected float m_fade_alpha = 1f;

	protected string m_fade_event = "";

	protected Texture2D m_fade;

	public View(Game game, Area room, string xml_path)
		: base(game, xml_path)
	{
		m_game = game;
		m_room = room;
		if (m_room != null)
		{
			m_room.AddView(this);
		}
		m_fade = m_room.m_CL.LoadTexture("HUD/black");
		_ = m_game.GraphicsDevice.PresentationParameters;
		m_RT = new RenderTarget2D(m_game.GraphicsDevice, Game.VIEW_RECT.Width, Game.VIEW_RECT.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
		LoadXML();
		if (m_xml_doc == null)
		{
			return;
		}
		XNode xNode = m_xml_doc.Root.FirstNode;
		XElement xElement = null;
		while (xNode != null)
		{
			if ((object)xNode.GetType() == typeof(XElement))
			{
				xElement = (XElement)xNode;
				parseElement(xElement);
			}
			xNode = xNode.NextNode;
		}
		xElement = null;
		xNode = null;
	}

	public override void Clear()
	{
		m_game = null;
		m_room = null;
		if (m_RT != null)
		{
			m_RT.Dispose();
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
		if (m_triggers != null)
		{
			foreach (KeyValuePair<string, Trigger> trigger in m_triggers)
			{
				if (trigger.Value != null)
				{
					trigger.Value.Clear();
				}
			}
			m_triggers.Clear();
			m_triggers = null;
		}
		for (int i = 0; i < m_scenes.Count; i++)
		{
			if (m_scenes[i] != null)
			{
				m_scenes[i].Clear();
				m_scenes[i] = null;
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
		for (int j = 0; j < m_items.Count; j++)
		{
			if (m_items[j] != null)
			{
				m_items[j].Clear();
				m_items[j] = null;
			}
		}
		m_items.Clear();
		m_items = null;
		if (m_animations != null)
		{
			foreach (KeyValuePair<string, Animation2D> animation in m_animations)
			{
				if (animation.Value != null)
				{
					animation.Value.Clear();
				}
			}
			m_animations.Clear();
			m_animations = null;
		}
		if (m_anim_finished_events != null)
		{
			m_anim_finished_events.Clear();
			m_anim_finished_events = null;
		}
		if (m_add_anim_finished_events != null)
		{
			m_add_anim_finished_events.Clear();
			m_add_anim_finished_events = null;
		}
		if (m_images != null)
		{
			foreach (KeyValuePair<string, Image> image in m_images)
			{
				if (image.Value != null)
				{
					image.Value.Clear();
				}
			}
			m_images.Clear();
			m_images = null;
		}
		if (m_fade != null)
		{
			m_fade.Dispose();
			m_fade = null;
		}
		base.Clear();
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
		foreach (KeyValuePair<string, Trigger> trigger in m_triggers)
		{
			if (trigger.Value != null)
			{
				trigger.Value.m_state = Trigger.TRIGGER_STATE.IDLE;
			}
		}
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i] != null && m_items[i].m_state != ViewItem.VIEWITEM_STATE.REMOVED)
			{
				m_items[i].m_state = ViewItem.VIEWITEM_STATE.SCENE;
			}
		}
		m_game.m_hud.m_navigator.Setup(m_navigator_left, m_navigator_right, m_navigator_up, m_navigator_down);
		if (m_delayed_events != null)
		{
			m_delayed_events.Clear();
		}
		m_game.HandleEvent(m_name + ".onReset");
	}

	public virtual void ResetCursorTriggers()
	{
		CursorTrigger cursorTrigger = null;
		foreach (KeyValuePair<string, Trigger> trigger in m_triggers)
		{
			if (trigger.Value != null)
			{
				cursorTrigger = null;
				if (trigger.Value.m_class_name == "CursorTrigger")
				{
					cursorTrigger = (CursorTrigger)trigger.Value;
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

	public virtual void AddTrigger(Trigger trigger, string name)
	{
		try
		{
			m_triggers.Add(name, trigger);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual Trigger GetTrigger(string name)
	{
		try
		{
			Trigger value = null;
			if (m_triggers.TryGetValue(name, out value))
			{
				return value;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return null;
	}

	public virtual void EnableTrigger(string name, bool enable)
	{
		try
		{
			Trigger trigger = GetTrigger(name);
			if (trigger != null)
			{
				trigger.m_enabled = enable;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void RemoveTrigger(Trigger trigger)
	{
		if (trigger == null)
		{
			return;
		}
		string text = "";
		foreach (KeyValuePair<string, Trigger> trigger2 in m_triggers)
		{
			if (trigger2.Value == trigger)
			{
				text = trigger2.Key;
			}
		}
		if (text != "")
		{
			m_triggers.Remove(text);
		}
		trigger.Clear();
	}

	protected virtual void RemoveTrigger(string name)
	{
		if (name == "")
		{
			return;
		}
		Trigger trigger = null;
		foreach (KeyValuePair<string, Trigger> trigger2 in m_triggers)
		{
			if (trigger2.Key == name)
			{
				trigger = trigger2.Value;
			}
		}
		if (trigger != null)
		{
			m_triggers.Remove(name);
			trigger.Clear();
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
				m_game.HandleEvent(m_name + ".onLeft");
				m_game.ActivateTrigger(m_left_trigger);
				break;
			case VIEW_DIRECTION.RIGHT:
				m_game.HandleEvent(m_name + ".onRight");
				m_game.ActivateTrigger(m_right_trigger);
				break;
			case VIEW_DIRECTION.UP:
				m_game.HandleEvent(m_name + ".onUp");
				m_game.ActivateTrigger(m_up_trigger);
				break;
			case VIEW_DIRECTION.DOWN:
				m_game.HandleEvent(m_name + ".onDown");
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

	public override void HandleEvent(string s_event)
	{
		string text;
		if ((text = s_event) != null && text == "View.onDown" && m_game.m_world.GetCurrentView() == this)
		{
			m_game.ActivateTrigger(GetTrigger("TriggerDown"));
		}
		base.HandleEvent(s_event);
	}

	public virtual void FadeFromScene(int scene)
	{
		FadeFromScene(scene, 1.25f);
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

	public virtual void UpdateScript(TimeSpan elapsed)
	{
		try
		{
			UpdateObjects(elapsed);
			if (m_anim_finished_events != null)
			{
				List<Animation2D> list = new List<Animation2D>();
				foreach (KeyValuePair<Animation2D, string> anim_finished_event in m_anim_finished_events)
				{
					if (anim_finished_event.Key != null && anim_finished_event.Key.m_state == Animation2D.ANIM_STATE.ANIM_STATE_STOPPED)
					{
						m_game.HandleEvent(anim_finished_event.Value);
						list.Add(anim_finished_event.Key);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					m_anim_finished_events.Remove(list[i]);
				}
			}
			if (m_add_anim_finished_events != null)
			{
				foreach (KeyValuePair<Animation2D, string> add_anim_finished_event in m_add_anim_finished_events)
				{
					if (add_anim_finished_event.Key != null)
					{
						m_anim_finished_events.Add(add_anim_finished_event.Key, add_anim_finished_event.Value);
					}
				}
				m_add_anim_finished_events.Clear();
			}
			switch (m_fade_state)
			{
			case FADE_STATE.TO_BLACK:
				m_fade_alpha += (float)elapsed.TotalSeconds * m_fade_speed;
				if (m_fade_alpha >= 1f)
				{
					m_fade_alpha = 1f;
					m_fade_state = FADE_STATE.BLACK;
					m_game.HandleEvent(m_name + ".onFadeToBlack");
					if (m_fade_event != "")
					{
						m_game.HandleEvent(m_fade_event);
					}
				}
				break;
			case FADE_STATE.FROM_BLACK:
				m_fade_alpha -= (float)elapsed.TotalSeconds * m_fade_speed;
				if (m_fade_alpha <= 0f)
				{
					m_fade_alpha = 1f;
					m_fade_state = FADE_STATE.NONE;
					m_game.HandleEvent(m_name + ".onFadeFromBlack");
					if (m_fade_event != "")
					{
						m_game.HandleEvent(m_fade_event);
					}
				}
				break;
			case FADE_STATE.IN:
				m_fade_alpha += (float)elapsed.TotalSeconds * m_fade_speed;
				if (m_fade_alpha >= 1f)
				{
					m_fade_state = FADE_STATE.NONE;
					m_game.HandleEvent(m_name + ".onFadeIn");
				}
				break;
			case FADE_STATE.OUT:
				m_fade_alpha -= (float)elapsed.TotalSeconds * m_fade_speed;
				if (m_fade_alpha <= 0f)
				{
					m_fade_alpha = 1f;
					m_fade_state = FADE_STATE.NONE;
					m_game.HandleEvent(m_name + ".onFadeOut");
				}
				break;
			case FADE_STATE.BLACK:
				break;
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
		UpdateScript(elapsed);
		if (m_scenes != null && m_scenes.Count > 0 && m_scenes[m_current_scene] != null)
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
		m_game.m_over_trigger = null;
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
		foreach (KeyValuePair<string, Trigger> trigger in m_triggers)
		{
			if (trigger.Value != null)
			{
				trigger.Value.Update(elapsed);
			}
		}
		if (m_game.m_over_trigger != null && m_game.m_state == Game.GAME_STATE.SCENE)
		{
			if (m_game.m_cursor.m_state != Cursor.CURSOR_STATE.OVER && m_game.m_cursor.m_state != Cursor.CURSOR_STATE.OVER_ANIM)
			{
				m_game.m_cursor.onOver(m_game.m_over_trigger.m_type);
			}
			else if (m_game.m_cursor.m_trigger_type != m_game.m_over_trigger.m_type)
			{
				m_game.m_cursor.onOver(m_game.m_over_trigger.m_type);
			}
		}
		else if (m_game.m_cursor.m_state == Cursor.CURSOR_STATE.OVER)
		{
			m_game.onCursorOut();
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

	public virtual void UpdateObjects(TimeSpan elapsed)
	{
		try
		{
			for (int i = 0; i < m_visible_objects.Count; i++)
			{
				if (m_visible_objects[i] != null)
				{
					m_visible_objects[i].Update(elapsed);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual void onFadeFromSceneFinished()
	{
	}

	public virtual void UpdateEffect(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
		try
		{
			if (SB == null)
			{
				return;
			}
			SB.GraphicsDevice.SetRenderTarget(m_RT);
			SB.GraphicsDevice.Clear(ClearOptions.Target, new Color(0, 0, 0, 0), 0f, 0);
			if (m_scenes != null && m_scenes.Count > 0 && m_scenes[m_current_scene] != null)
			{
				m_scenes[m_current_scene].Draw(SB);
			}
			if (m_fade_scene && m_fade_from_scene != null)
			{
				m_fade_from_scene.Draw(SB);
			}
			float num = 1f;
			try
			{
				if (m_scenes != null && m_scenes.Count > 0)
				{
					num = m_scenes[m_current_scene].GetAlpha();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			for (int i = 0; i < m_items.Count; i++)
			{
				if (m_items[i] != null)
				{
					m_items[i].Draw(SB, Color.White * num);
				}
			}
			for (int j = 0; j < m_visible_objects.Count; j++)
			{
				if (m_visible_objects[j] != null)
				{
					m_visible_objects[j].Draw(SB, Color.White * num);
				}
			}
			if (m_fade_state == FADE_STATE.TO_BLACK || m_fade_state == FADE_STATE.FROM_BLACK || m_fade_state == FADE_STATE.BLACK)
			{
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_fade, Game.VIEW_RECT, Color.White * m_fade_alpha);
				SB.End();
				SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_RT, Game.VIEW_RECT, Color.White);
				SB.End();
			}
			else
			{
				SB.GraphicsDevice.SetRenderTarget(m_game.m_RT);
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_RT, Game.VIEW_RECT, Color.White * m_fade_alpha);
				SB.End();
			}
		}
		catch (Exception ex2)
		{
			Console.WriteLine(ex2.Message);
		}
	}

	public virtual void DrawEffect(SpriteBatch SB)
	{
	}

	public virtual void RenderToScene(SpriteBatch SB)
	{
	}

	protected override bool parseElement(XElement element)
	{
		switch (element.Name.ToString())
		{
		case "Setup":
			parseSetup(element);
			break;
		case "Navigator":
			parseNavigator(element);
			break;
		case "AddScene":
			parseAddScene(element);
			break;
		case "AddImage":
			parseAddImage(element);
			break;
		case "ShowImage":
			parseShowImage(element);
			break;
		case "HideImage":
			parseHideImage(element);
			break;
		case "SetImageAlpha":
			parseSetImageAlpha(element);
			break;
		case "FadeOutImage":
			parseFadeOutImage(element);
			break;
		case "FadeInImage":
			parseFadeInImage(element);
			break;
		case "SetImageRotation":
			parseSetImageRotation(element);
			break;
		case "RotateImage":
			parseRotateImage(element);
			break;
		case "SetTurnTrigger":
			parseSetTurnTrigger(element);
			break;
		case "ViewTrigger":
			return parseViewTrigger(element);
		case "CursorTrigger":
			parseCursorTrigger(element);
			break;
		case "RemoveTrigger":
			parseRemoveTrigger(element);
			break;
		case "EnableTrigger":
			parseEnableTrigger(element);
			break;
		case "DisableTrigger":
			parseDisableTrigger(element);
			break;
		case "HandleState":
			parseHandleState(element);
			break;
		case "ChangeScene":
			parseChangeScene(element);
			break;
		case "FadeFromScene":
			parseFadeFromScene(element);
			break;
		case "SetTurnAnimation":
			parseSetTurnAnimation(element);
			break;
		}
		return base.parseElement(element);
	}

	protected override bool parseAction(XElement element)
	{
		try
		{
			if (!element.HasAttributes)
			{
				return false;
			}
			XAttribute xAttribute = element.Attribute("type");
			if (xAttribute == null)
			{
				return false;
			}
			switch (xAttribute.Value)
			{
			case "CreateAnimationInstance":
				return parseCreateAnimationInstance(element);
			case "AnimationInstance":
				return parseAnimationInstanceAction(element);
			case "FadeToBlack":
				return parseFadeToBlack(element);
			case "FadeFromBlack":
				return parseFadeFromBlack(element);
			case "ActivateTrigger":
				return parseActivateTrigger(element);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return base.parseAction(element);
	}

	protected virtual bool parseSetup(XElement element)
	{
		try
		{
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			m_name = xAttribute.Value;
			xAttribute = element.Attribute("navigator");
			if (xAttribute != null)
			{
				m_enable_navigator = bool.Parse(xAttribute.Value);
			}
			xAttribute = element.Attribute("hud_state");
			if (xAttribute != null)
			{
				switch (xAttribute.Value)
				{
				case "back":
					m_hud_state = HUD.HUD_STATE.BACK;
					break;
				case "cancel":
					m_hud_state = HUD.HUD_STATE.CANCEL;
					break;
				case "move_forward":
					m_hud_state = HUD.HUD_STATE.MOVE_FORWARD;
					break;
				case "navigator":
					m_hud_state = HUD.HUD_STATE.NAVIGATOR;
					break;
				case "proceed":
					m_hud_state = HUD.HUD_STATE.PROCEED;
					break;
				default:
					m_hud_state = HUD.HUD_STATE.NONE;
					break;
				}
			}
			xAttribute = element.Attribute("text_fade");
			if (xAttribute != null)
			{
				m_use_text_fade = bool.Parse(xAttribute.Value);
			}
			xAttribute = element.Attribute("no_text_fade");
			if (xAttribute != null)
			{
				m_no_text_fade = bool.Parse(xAttribute.Value);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseNavigator(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("left");
			if (xAttribute != null)
			{
				m_navigator_left = bool.Parse(xAttribute.Value);
			}
			xAttribute = element.Attribute("right");
			if (xAttribute != null)
			{
				m_navigator_right = bool.Parse(xAttribute.Value);
			}
			xAttribute = element.Attribute("up");
			if (xAttribute != null)
			{
				m_navigator_up = bool.Parse(xAttribute.Value);
			}
			xAttribute = element.Attribute("down");
			if (xAttribute != null)
			{
				m_navigator_down = bool.Parse(xAttribute.Value);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseAddScene(XElement element)
	{
		try
		{
			string text = "";
			XName name = "image_path";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			m_scenes.Add(new Scene(m_room.m_CL.LoadTexture(text)));
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetTurnTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "direction";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value.ToLower();
			string text2 = "";
			name = "next_view";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			string text3 = "";
			name = "anim_path";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text3 = xAttribute.Value;
			bool flag = false;
			name = "reverse_anim";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			flag = bool.Parse(xAttribute.Value);
			ViewTrigger viewTrigger = null;
			string name2 = "";
			switch (text)
			{
			case "left":
				m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(text3);
				name2 = "TriggerLeft";
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT));
				break;
			case "right":
				m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(text3);
				name2 = "TriggerRight";
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT));
				break;
			case "up":
				m_up_animation = (TextureAnimation)m_room.m_CL.GetContent(text3);
				name2 = "TriggerUp";
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP));
				break;
			case "down":
				m_down_animation = (TextureAnimation)m_room.m_CL.GetContent(text3);
				name2 = "TriggerDown";
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN));
				break;
			}
			if (viewTrigger == null)
			{
				return false;
			}
			AddTrigger(viewTrigger, name2);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseViewTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			string text2 = "";
			name = "next_view";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			string text3 = "";
			name = "type";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text3 = xAttribute.Value.ToLower();
			bool flag = false;
			name = "reverse";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				flag = bool.Parse(xAttribute.Value);
			}
			ViewTrigger viewTrigger = null;
			switch (text3)
			{
			case "left":
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT));
				break;
			case "right":
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT));
				break;
			case "up":
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP));
				break;
			case "down":
				viewTrigger = (flag ? new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE) : new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN));
				break;
			case "fade_out":
				viewTrigger = new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT);
				break;
			case "fade_to_black":
				viewTrigger = new ViewTrigger(m_game, m_name, text2, ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK);
				break;
			}
			if (viewTrigger == null)
			{
				return false;
			}
			AddTrigger(viewTrigger, text);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseCursorTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			ViewTrigger trigger = null;
			string text2 = "bitmap";
			name = "collision";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				text2 = xAttribute.Value;
			}
			string content_path = "";
			name = "bitmap_path";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				content_path = xAttribute.Value;
			}
			string rect = "";
			name = "rect";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				rect = xAttribute.Value.ToLower();
			}
			string style = "zoom";
			name = "cursor_style";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				style = xAttribute.Value.ToLower();
			}
			string text3 = "view";
			name = "trigger_type";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				text3 = xAttribute.Value.ToLower();
			}
			string next_view_name = "";
			name = "trigger_view";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				next_view_name = xAttribute.Value;
			}
			string type = "";
			name = "viewtrigger_type";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				type = xAttribute.Value.ToLower();
			}
			bool reverse = false;
			name = "reverse_anim";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				reverse = bool.Parse(xAttribute.Value.ToLower());
			}
			if (text3 == "view")
			{
				trigger = new ViewTrigger(m_game, m_name, next_view_name, getViewTriggerAnimType(type, reverse));
			}
			string s_event = "";
			name = "trigger_event";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				s_event = xAttribute.Value;
			}
			bool activate_own = false;
			name = "activate_own";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				activate_own = bool.Parse(xAttribute.Value);
			}
			bool enabled = true;
			name = "enabled";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				enabled = bool.Parse(xAttribute.Value);
			}
			CursorTrigger cursorTrigger = null;
			if (text3 == "view")
			{
				if (text2 == "bitmap")
				{
					cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, content_path, trigger, getCursorStyle(style));
				}
				if (text2 == "rect")
				{
					cursorTrigger = new CursorTrigger(m_game, parseRect(rect), trigger, getCursorStyle(style));
				}
			}
			if (text3 == "event")
			{
				if (text2 == "bitmap")
				{
					cursorTrigger = new CursorTrigger(m_game, m_room.m_CL, content_path, s_event, getCursorStyle(style));
				}
				if (text2 == "rect")
				{
					cursorTrigger = new CursorTrigger(m_game, parseRect(rect), s_event, getCursorStyle(style));
				}
			}
			if (cursorTrigger != null)
			{
				cursorTrigger.m_activate_own = activate_own;
				cursorTrigger.m_enabled = enabled;
				AddTrigger(cursorTrigger, text);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseRemoveTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			RemoveTrigger(text);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseEnableTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			EnableTrigger(text, enable: true);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseDisableTrigger(XElement element)
	{
		try
		{
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			EnableTrigger(text, enable: false);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseHandleState(XElement element)
	{
		try
		{
			string text = "";
			XName name = "state";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			string text2 = "";
			name = "event";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			string state = m_game.m_game_data.GetState(text);
			m_game.HandleEvent(text2 + "." + state);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseChangeScene(XElement element)
	{
		try
		{
			int num = 0;
			XName name = "scene";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			num = int.Parse(xAttribute.Value);
			ChangeScene(num);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseFadeFromScene(XElement element)
	{
		try
		{
			int num = 0;
			XAttribute xAttribute = element.Attribute("scene");
			if (xAttribute == null)
			{
				return false;
			}
			num = int.Parse(xAttribute.Value);
			FadeFromScene(num);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetTurnAnimation(XElement element)
	{
		try
		{
			string text = "";
			XName name = "direction";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value.ToLower();
			string text2 = "";
			name = "animation_path";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			switch (text)
			{
			case "left":
				m_left_animation = (TextureAnimation)m_room.m_CL.GetContent(text2);
				break;
			case "right":
				m_right_animation = (TextureAnimation)m_room.m_CL.GetContent(text2);
				break;
			case "up":
				m_up_animation = (TextureAnimation)m_room.m_CL.GetContent(text2);
				break;
			case "down":
				m_down_animation = (TextureAnimation)m_room.m_CL.GetContent(text2);
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseAddImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.FirstAttribute;
			string path = "";
			string key = "";
			Rectangle dest_rect = Game.VIEW_RECT;
			Vector2 center = Vector2.Zero;
			bool flag = false;
			while (xAttribute != null)
			{
				switch (xAttribute.Name.ToString())
				{
				case "path":
					path = xAttribute.Value;
					break;
				case "name":
					key = xAttribute.Value;
					break;
				case "dest_rect":
					dest_rect = parseRect(xAttribute.Value);
					break;
				case "center":
					center = parseVector2(xAttribute.Value);
					break;
				case "show":
					flag = bool.Parse(xAttribute.Value);
					break;
				}
				xAttribute = xAttribute.NextAttribute;
			}
			if (getContentLoader() == null)
			{
				return false;
			}
			Texture2D texture2D = getContentLoader().LoadTexture(path);
			if (texture2D == null)
			{
				return false;
			}
			Image image = new Image(texture2D, dest_rect);
			if (image == null)
			{
				return false;
			}
			image.m_center = center;
			if (m_images == null)
			{
				m_images = new Dictionary<string, Image>();
			}
			m_images.Add(key, image);
			if (flag)
			{
				m_visible_objects.Add(image);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseShowImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			if (m_images == null)
			{
				return false;
			}
			Image image = m_images[value];
			if (image == null)
			{
				return false;
			}
			if (m_visible_objects == null)
			{
				return false;
			}
			if (!m_visible_objects.Contains(image))
			{
				m_visible_objects.Add(image);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseHideImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			if (m_images == null)
			{
				return false;
			}
			Image image = m_images[value];
			if (image == null)
			{
				return false;
			}
			if (m_visible_objects == null)
			{
				return false;
			}
			if (m_visible_objects.Contains(image))
			{
				m_visible_objects.Remove(image);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetImageAlpha(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			if (m_images == null)
			{
				return false;
			}
			Image image = m_images[value];
			if (image == null)
			{
				return false;
			}
			xAttribute = element.Attribute("value");
			if (xAttribute == null)
			{
				return false;
			}
			image.m_alpha = ScriptObject.ParseFloatValue(xAttribute.Value);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseFadeOutImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			float speed = 1f;
			xAttribute = element.Attribute("speed");
			if (xAttribute != null)
			{
				speed = ScriptObject.ParseFloatValue(xAttribute.Value);
			}
			if (m_images == null)
			{
				return false;
			}
			Image image = m_images[value];
			if (image == null)
			{
				return false;
			}
			image.FadeOut(speed);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseFadeInImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			float speed = 1f;
			xAttribute = element.Attribute("speed");
			if (xAttribute != null)
			{
				speed = ScriptObject.ParseFloatValue(xAttribute.Value);
			}
			if (m_images == null)
			{
				return false;
			}
			Image image = m_images[value];
			if (image == null)
			{
				return false;
			}
			image.FadeIn(speed);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetImageRotation(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			float rotation = 0f;
			xAttribute = element.Attribute("rotation");
			if (xAttribute != null)
			{
				rotation = ScriptObject.ParseFloatValue(xAttribute.Value);
			}
			xAttribute = element.Attribute("local_state");
			if (xAttribute != null)
			{
				rotation = ScriptObject.ParseFloatValue(GetLocalState(xAttribute.Value));
			}
			if (m_images == null)
			{
				ScriptError("SetImageRotation: no images defined!");
				return false;
			}
			if (!m_images.ContainsKey(value))
			{
				ScriptError("SetImageRotation: no image defined with name '" + value + "'!");
				return false;
			}
			m_images[value].m_rotation = rotation;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseRotateImage(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			float num = 0f;
			xAttribute = element.Attribute("rotate");
			if (xAttribute == null)
			{
				ScriptError("SetImageRotation: rotate attribute not defined!");
				return false;
			}
			num = ScriptObject.ParseFloatValue(xAttribute.Value);
			if (m_images == null)
			{
				ScriptError("SetImageRotation: no images defined!");
				return false;
			}
			if (!m_images.ContainsKey(value))
			{
				ScriptError("SetImageRotation: no image defined with name '" + value + "'!");
				return false;
			}
			m_images[value].m_rotation += num;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseCreateAnimationInstance(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.FirstAttribute;
			string path = "";
			string key = "";
			Rectangle rectangle = Rectangle.Empty;
			while (xAttribute != null)
			{
				switch (xAttribute.Name.ToString())
				{
				case "path":
					path = xAttribute.Value;
					break;
				case "name":
					key = xAttribute.Value;
					break;
				case "dest_rect":
					rectangle = parseRect(xAttribute.Value);
					break;
				}
				xAttribute = xAttribute.NextAttribute;
			}
			if (getContentLoader() == null)
			{
				return false;
			}
			Animation2D animation2D = (Animation2D)getContentLoader().GetContent(path);
			if (animation2D == null)
			{
				return false;
			}
			if (rectangle != Rectangle.Empty)
			{
				TextureAnimation textureAnimation = (TextureAnimation)animation2D;
				if (textureAnimation != null)
				{
					textureAnimation.m_positioned = true;
					textureAnimation.m_dest_rect = rectangle;
				}
			}
			if (m_animations == null)
			{
				m_animations = new Dictionary<string, Animation2D>();
			}
			m_animations.Add(key, animation2D);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseAnimationInstanceAction(XElement element)
	{
		try
		{
			if (m_animations == null)
			{
				return false;
			}
			string text = "";
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			if (!m_animations.ContainsKey(xAttribute.Value))
			{
				ScriptError("Failed to get animation with name '" + xAttribute.Value + "'");
				return false;
			}
			Animation2D animation2D = m_animations[xAttribute.Value];
			if (animation2D == null)
			{
				return false;
			}
			name = "action";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			name = "fps";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				double fPS = ScriptObject.ParseDoubleValue(xAttribute.Value);
				animation2D.SetFPS(fPS);
			}
			Animation2D.LOOP_TYPE loop = Animation2D.LOOP_TYPE.NO_LOOP;
			name = "loop";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				switch (xAttribute.Value)
				{
				case "no_loop":
					loop = Animation2D.LOOP_TYPE.NO_LOOP;
					break;
				case "cycle":
					loop = Animation2D.LOOP_TYPE.CYCLE;
					break;
				case "ping_pong":
					loop = Animation2D.LOOP_TYPE.PING_PONG;
					break;
				}
			}
			name = "reverse_anim";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				animation2D.m_reverse = bool.Parse(xAttribute.Value);
			}
			name = "action";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				switch (xAttribute.Value.ToLower())
				{
				case "play":
					animation2D.Play(loop);
					break;
				case "show":
					if (!m_visible_objects.Contains(animation2D))
					{
						m_visible_objects.Add(animation2D);
					}
					break;
				case "hide":
					if (m_visible_objects.Contains(animation2D))
					{
						m_visible_objects.Remove(animation2D);
					}
					break;
				case "set_frame":
				{
					TextureAnimation textureAnimation2 = (TextureAnimation)animation2D;
					if (textureAnimation2 != null)
					{
						xAttribute = element.Attribute("value");
						if (xAttribute != null)
						{
							textureAnimation2.SetFrame(int.Parse(xAttribute.Value));
						}
					}
					break;
				}
				case "set_end_frame":
				{
					TextureAnimation textureAnimation3 = (TextureAnimation)animation2D;
					if (textureAnimation3 != null)
					{
						xAttribute = element.Attribute("value");
						if (xAttribute != null)
						{
							textureAnimation3.m_end_frame = int.Parse(xAttribute.Value);
						}
					}
					break;
				}
				case "play_from_frame":
				{
					animation2D.Play(loop);
					TextureAnimation textureAnimation = (TextureAnimation)animation2D;
					if (textureAnimation != null)
					{
						xAttribute = element.Attribute("value");
						if (xAttribute != null)
						{
							textureAnimation.SetFrame(int.Parse(xAttribute.Value));
						}
					}
					break;
				}
				}
			}
			name = "finished_event";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				string value = xAttribute.Value;
				if (value != "")
				{
					if (m_add_anim_finished_events == null)
					{
						m_add_anim_finished_events = new Dictionary<Animation2D, string>();
					}
					if (m_add_anim_finished_events.ContainsKey(animation2D))
					{
						ScriptError("Finished event '" + value + "' for animation '" + text + "' already defined!");
						return false;
					}
					m_add_anim_finished_events.Add(animation2D, value);
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseFadeToBlack(XElement element)
	{
		try
		{
			m_fade_speed = 1f;
			m_fade_event = "";
			XAttribute xAttribute = element.Attribute("speed");
			if (xAttribute != null)
			{
				m_fade_speed = ScriptObject.ParseFloatValue(xAttribute.Value);
			}
			xAttribute = element.Attribute("event");
			if (xAttribute != null)
			{
				m_fade_event = xAttribute.Value;
			}
			m_fade_alpha = 0f;
			m_fade_state = FADE_STATE.TO_BLACK;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseFadeFromBlack(XElement element)
	{
		try
		{
			m_fade_speed = 1f;
			m_fade_event = "";
			XAttribute xAttribute = element.Attribute("speed");
			if (xAttribute != null)
			{
				m_fade_speed = ScriptObject.ParseFloatValue(xAttribute.Value);
			}
			xAttribute = element.Attribute("event");
			if (xAttribute != null)
			{
				m_fade_event = xAttribute.Value;
			}
			m_fade_alpha = 1f;
			m_fade_state = FADE_STATE.FROM_BLACK;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseActivateTrigger(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			Trigger trigger = GetTrigger(xAttribute.Value);
			if (trigger == null)
			{
				return false;
			}
			m_game.ActivateTrigger(trigger);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	private ViewTrigger.VIEWTRIGGER_ANIM_TYPE getViewTriggerAnimType(string type, bool reverse)
	{
		switch (type)
		{
		case "left":
			if (!reverse)
			{
				return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT;
			}
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.LEFT_REVERSE;
		case "right":
			if (!reverse)
			{
				return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT;
			}
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.RIGHT_REVERSE;
		case "up":
			if (!reverse)
			{
				return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP;
			}
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UP_REVERSE;
		case "down":
			if (!reverse)
			{
				return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN;
			}
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.DOWN_REVERSE;
		case "fade_out":
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_OUT;
		case "fade_to_black":
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.FADE_TO_BLACK;
		default:
			return ViewTrigger.VIEWTRIGGER_ANIM_TYPE.UNKNOWN;
		}
	}

	private Trigger.TRIGGER_TYPE getCursorStyle(string style)
	{
		return style switch
		{
			"zoom" => Trigger.TRIGGER_TYPE.ZOOM, 
			"zoom_small" => Trigger.TRIGGER_TYPE.ZOOM_SMALL, 
			"use" => Trigger.TRIGGER_TYPE.USE, 
			"use_small" => Trigger.TRIGGER_TYPE.USE_SMALL, 
			"view" => Trigger.TRIGGER_TYPE.VIEW, 
			_ => Trigger.TRIGGER_TYPE.UNKNOWN, 
		};
	}

	private Rectangle parseRect(string rect)
	{
		string[] array = rect.Split(',');
		if (array.Length != 4)
		{
			return default(Rectangle);
		}
		return new Rectangle(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
	}

	private Vector2 parseVector2(string rect)
	{
		string[] array = rect.Split(',');
		if (array.Length != 2)
		{
			return Vector2.Zero;
		}
		return new Vector2(int.Parse(array[0]), int.Parse(array[1]));
	}

	protected override SGSContentLoader getContentLoader()
	{
		try
		{
			return m_room.m_CL;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return null;
	}

	public void FadeOut(float speed)
	{
		m_fade_alpha = 1f;
		m_fade_speed = speed;
		m_fade_state = FADE_STATE.OUT;
	}

	public void FadeIn(float speed)
	{
		m_fade_alpha = 0f;
		m_fade_speed = speed;
		m_fade_state = FADE_STATE.IN;
	}
}
