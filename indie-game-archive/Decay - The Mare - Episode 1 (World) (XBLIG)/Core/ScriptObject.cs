using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Core;

public class ScriptObject
{
	public string m_name = "";

	protected string m_xml_path = "";

	protected XDocument m_xml_doc;

	protected Game m_game;

	protected Dictionary<string, SoundEffectInstance> m_sounds;

	protected Dictionary<string, List<XElement>> m_events;

	protected Dictionary<SoundEffectInstance, float> m_fade_out_sounds;

	protected Dictionary<SoundEffectInstance, float> m_fade_in_sounds;

	protected List<SoundEffectInstance> m_remove_sounds;

	protected List<DelayedEvent> m_delayed_events;

	protected Dictionary<string, string> m_local_states = new Dictionary<string, string>();

	public ScriptObject(Game game, string xml_path)
	{
		try
		{
			m_game = game;
			m_xml_path = xml_path;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void Clear()
	{
		try
		{
			m_xml_doc = null;
			m_game = null;
			if (m_remove_sounds != null)
			{
				m_remove_sounds.Clear();
				m_remove_sounds = null;
			}
			if (m_fade_in_sounds != null)
			{
				m_fade_in_sounds.Clear();
				m_fade_in_sounds = null;
			}
			if (m_fade_out_sounds != null)
			{
				m_fade_out_sounds.Clear();
				m_fade_out_sounds = null;
			}
			if (m_sounds != null)
			{
				foreach (KeyValuePair<string, SoundEffectInstance> sound in m_sounds)
				{
					if (sound.Value != null)
					{
						sound.Value.Stop();
						sound.Value.Dispose();
					}
				}
				m_sounds.Clear();
				m_sounds = null;
			}
			if (m_events != null)
			{
				foreach (KeyValuePair<string, List<XElement>> @event in m_events)
				{
					if (@event.Value != null)
					{
						@event.Value.Clear();
					}
				}
				m_events.Clear();
				m_events = null;
			}
			if (m_delayed_events != null)
			{
				m_delayed_events.Clear();
				m_delayed_events = null;
			}
			if (m_local_states != null)
			{
				m_local_states.Clear();
				m_local_states = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected void LoadXML()
	{
		if (m_xml_path != "")
		{
			m_xml_doc = XDocument.Load(m_xml_path + ".xml");
		}
	}

	protected virtual SGSContentLoader getContentLoader()
	{
		if (m_game == null)
		{
			return null;
		}
		return m_game.m_CL;
	}

	public string GetLocalState(string state)
	{
		try
		{
			if (!m_local_states.ContainsKey(state))
			{
				ScriptError("Local state for '" + state + "' not defined!");
				return "";
			}
			return m_local_states[state];
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return "";
	}

	public static float ParseFloatValue(string value)
	{
		try
		{
			if (value == "")
			{
				return -1f;
			}
			return float.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return 0f;
	}

	public static string ParseStringFromFloat(float value)
	{
		try
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return "";
	}

	public static double ParseDoubleValue(string value)
	{
		try
		{
			return double.Parse(value.Replace(',', '.'), CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return 0.0;
	}

	protected virtual bool parseElement(XElement element)
	{
		try
		{
			if (element == null)
			{
				return false;
			}
			switch (element.Name.ToString())
			{
			case "LoadContent":
				return parseLoadContent(element);
			case "Action":
				return parseAction(element);
			case "Event":
				return parseEvent(element);
			case "Case":
				return parseCase(element);
			case "HandleEvent":
				return parseHandleEvent(element);
			case "DelayedEvent":
				return parseDelayedEvent(element);
			case "UseEventHandled":
				return parseUseEventHandled(element);
			case "ShowAsk":
				return parseShowAsk(element);
			case "BlockInput":
				return parseBlockInput(element);
			case "UnblockInput":
				return parseUnblockInput(element);
			case "AddItem":
				return parseAddItem(element);
			case "RemoveItem":
				return parseRemoveItem(element);
			case "SetLocalState":
				return parseSetLocalState(element);
			case "HandleLocalState":
				return parseHandleLocalState(element);
			case "Trace":
				return parseTrace(element);
			case "SetGlobalState":
				return parseSetGlobalState(element);
			case "PlayMusic":
				return parsePlayMusic(element);
			case "SetVibration":
				return parseSetVibration(element);
			case "PlayDoorSound":
				return parsePlayDoorSound(element);
			case "SetHUDAlpha":
				return parseSetHUDAlpha(element);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseLoadContent(XElement element)
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
			case "TextureAnimation":
				return parseLoadTextureAnimation(element);
			case "Sound":
				return parseLoadSound(element);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseLoadTextureAnimation(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.FirstAttribute;
			string content_path = "";
			uint frames = 0u;
			bool reverse = false;
			bool flag = false;
			int frame_width = 0;
			int frame_height = 0;
			int total_frames = 0;
			bool random_mode = false;
			bool frame_smoothing = false;
			List<string> list = new List<string>();
			double num = 0.0;
			int size = 2048;
			while (xAttribute != null)
			{
				switch (xAttribute.Name.ToString())
				{
				case "path":
					content_path = xAttribute.Value;
					break;
				case "images":
					frames = uint.Parse(xAttribute.Value);
					break;
				case "reverse":
					reverse = bool.Parse(xAttribute.Value);
					break;
				case "combinedFrames":
					flag = bool.Parse(xAttribute.Value);
					break;
				case "width":
					frame_width = int.Parse(xAttribute.Value);
					break;
				case "height":
					frame_height = int.Parse(xAttribute.Value);
					break;
				case "frames":
					total_frames = int.Parse(xAttribute.Value);
					break;
				case "frameSmoothing":
					frame_smoothing = bool.Parse(xAttribute.Value);
					break;
				case "addAnimation":
					list.Add(xAttribute.Value);
					break;
				case "fps":
					num = ParseDoubleValue(xAttribute.Value);
					break;
				case "size":
					size = int.Parse(xAttribute.Value);
					break;
				case "random_mode":
					random_mode = bool.Parse(xAttribute.Value);
					break;
				}
				xAttribute = xAttribute.NextAttribute;
			}
			if (getContentLoader() == null)
			{
				return false;
			}
			TextureAnimation textureAnimation = null;
			textureAnimation = new TextureAnimation(m_game, getContentLoader(), content_path, frames, reverse);
			textureAnimation.m_random_mode = random_mode;
			if (flag)
			{
				textureAnimation.UseCombinedFrames(frame_width, frame_height, total_frames, size);
			}
			if (num > 0.0)
			{
				textureAnimation.SetFPS(num);
			}
			textureAnimation.m_frame_smoothing = frame_smoothing;
			for (int i = 0; i < list.Count; i++)
			{
				string[] array = list[i].Split(';');
				if (array.Length != 3)
				{
					Console.WriteLine("ScriptObject: TextureAnimation.addAnimation - expected 3 arguments, found " + array.Length);
				}
				else
				{
					textureAnimation.AddAnimation((TextureAnimation)getContentLoader().GetContent(array[0]), int.Parse(array[1]), int.Parse(array[2]));
				}
			}
			list.Clear();
			list = null;
			getContentLoader().AddContent(textureAnimation);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseLoadSound(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.FirstAttribute;
			string path = "";
			while (xAttribute != null)
			{
				string text;
				if ((text = xAttribute.Name.ToString()) != null && text == "path")
				{
					path = xAttribute.Value;
				}
				xAttribute = xAttribute.NextAttribute;
			}
			if (getContentLoader() == null)
			{
				return false;
			}
			getContentLoader().LoadSound(path);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseAction(XElement element)
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
			case "CreateSoundInstance":
				return parseCreateSoundInstance(element);
			case "SoundInstance":
				return parseSoundInstanceAction(element);
			case "AskPickup":
				return parseAskPickup(element);
			case "ShowText":
				return parseShowText(element);
			case "SetState":
				return parseSetState(element);
			case "ChangeArea":
				return parseChangeArea(element);
			case "FadeOutHUD":
				m_game.m_hud.FadeOut();
				return true;
			case "FadeInHUD":
				m_game.m_hud.FadeIn();
				return true;
			case "EnableInput":
				m_game.m_input_enabled = true;
				return true;
			case "DisableInput":
				m_game.m_input_enabled = false;
				return true;
			case "ShowCursor":
				m_game.m_show_cursor = true;
				return true;
			case "HideCursor":
				m_game.m_show_cursor = false;
				return true;
			case "CursorOut":
				m_game.m_cursor.onOut();
				return true;
			case "EnableInventory":
				m_game.m_inventory_enabled = true;
				return true;
			case "DisableInventory":
				m_game.m_inventory_enabled = false;
				return true;
			case "GameDataView":
			{
				XAttribute xAttribute3 = element.Attribute("value");
				if (xAttribute3 != null)
				{
					m_game.m_game_data.m_view = xAttribute3.Value;
					return true;
				}
				xAttribute3 = element.Attribute("local_state");
				if (xAttribute3 != null)
				{
					if (m_local_states != null && m_local_states.ContainsKey(xAttribute3.Value))
					{
						m_game.m_game_data.m_view = m_local_states[xAttribute3.Value];
					}
					return true;
				}
				break;
			}
			case "GameDataArea":
			{
				XAttribute xAttribute2 = element.Attribute("value");
				if (xAttribute2 != null)
				{
					m_game.m_game_data.m_area = xAttribute2.Value;
					return true;
				}
				xAttribute2 = element.Attribute("local_state");
				if (xAttribute2 != null)
				{
					if (m_local_states != null && m_local_states.ContainsKey(xAttribute2.Value))
					{
						m_game.m_game_data.m_area = m_local_states[xAttribute2.Value];
					}
					return true;
				}
				break;
			}
			case "FadeOutMusic":
				m_game.FadeOutMusic();
				break;
			case "FadeInMusic":
				m_game.FadeInMusic();
				break;
			case "StopMusic":
				m_game.StopMusic();
				break;
			case "PauseMusic":
				m_game.PauseMusic();
				break;
			case "ResumeMusic":
				m_game.ResumeMusic();
				break;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseCreateSoundInstance(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.FirstAttribute;
			string path = "";
			string key = "";
			float num = 1f;
			bool isLooped = false;
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
				case "volume":
					num = ParseFloatValue(xAttribute.Value);
					break;
				case "loop":
					isLooped = bool.Parse(xAttribute.Value);
					break;
				}
				xAttribute = xAttribute.NextAttribute;
			}
			if (getContentLoader() == null)
			{
				return false;
			}
			SoundEffect soundEffect = getContentLoader().LoadSound(path);
			if (soundEffect == null)
			{
				return false;
			}
			SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();
			if (soundEffectInstance == null)
			{
				return false;
			}
			soundEffectInstance.Volume = m_game.m_game_settings.m_sound_volume * 0.1f * num;
			soundEffectInstance.IsLooped = isLooped;
			soundEffect = null;
			if (m_sounds == null)
			{
				m_sounds = new Dictionary<string, SoundEffectInstance>();
			}
			m_sounds.Add(key, soundEffectInstance);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSoundInstanceAction(XElement element)
	{
		try
		{
			if (m_sounds == null)
			{
				return false;
			}
			XName name = "name";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			SoundEffectInstance soundEffectInstance = m_sounds[xAttribute.Value];
			if (soundEffectInstance == null)
			{
				return false;
			}
			xAttribute = element.Attribute("volume");
			if (xAttribute != null)
			{
				soundEffectInstance.Volume = ParseFloatValue(xAttribute.Value);
			}
			xAttribute = element.Attribute("loop");
			if (xAttribute != null)
			{
				soundEffectInstance.IsLooped = bool.Parse(xAttribute.Value);
			}
			name = "action";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			switch (xAttribute.Value)
			{
			case "Play":
				soundEffectInstance.Stop();
				soundEffectInstance.Play();
				break;
			case "Pause":
				if (soundEffectInstance.State == SoundState.Playing)
				{
					soundEffectInstance.Pause();
				}
				break;
			case "Resume":
				if (soundEffectInstance.State == SoundState.Paused)
				{
					soundEffectInstance.Resume();
				}
				break;
			case "Stop":
				soundEffectInstance.Stop();
				break;
			case "FadeOut":
				name = "speed";
				xAttribute = element.Attribute(name);
				if (xAttribute != null)
				{
					if (m_fade_out_sounds == null)
					{
						m_fade_out_sounds = new Dictionary<SoundEffectInstance, float>();
					}
					m_fade_out_sounds.Add(soundEffectInstance, ParseFloatValue(xAttribute.Value));
				}
				break;
			case "FadeIn":
				name = "speed";
				xAttribute = element.Attribute(name);
				if (xAttribute != null)
				{
					if (m_fade_in_sounds == null)
					{
						m_fade_in_sounds = new Dictionary<SoundEffectInstance, float>();
					}
					m_fade_in_sounds.Add(soundEffectInstance, ParseFloatValue(xAttribute.Value));
				}
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

	protected virtual bool parseAskPickup(XElement element)
	{
		try
		{
			if (m_game == null || m_game.m_inventory == null)
			{
				return false;
			}
			XName name = "item_id";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			m_game.m_inventory.AskPickup(xAttribute.Value);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseAddItem(XElement element)
	{
		try
		{
			if (m_game == null || m_game.m_inventory == null)
			{
				return false;
			}
			XName name = "item_id";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			m_game.m_inventory.AddItem(xAttribute.Value, loading: false);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseRemoveItem(XElement element)
	{
		try
		{
			if (m_game == null || m_game.m_inventory == null)
			{
				return false;
			}
			XName name = "item_id";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			m_game.m_inventory.RemoveItem(xAttribute.Value);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetLocalState(XElement element)
	{
		try
		{
			if (m_local_states == null)
			{
				m_local_states = new Dictionary<string, string>();
			}
			string text = "";
			string text2 = "";
			XAttribute xAttribute = element.Attribute("state");
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			xAttribute = element.Attribute("value");
			if (xAttribute != null)
			{
				text2 = xAttribute.Value;
				m_local_states[text] = text2;
			}
			xAttribute = element.Attribute("global_state");
			if (xAttribute != null)
			{
				text2 = m_game.m_game_data.GetState(xAttribute.Value);
				m_local_states[text] = text2;
			}
			xAttribute = element.Attribute("add");
			if (xAttribute != null)
			{
				text2 = xAttribute.Value;
				if (!m_local_states.ContainsKey(text))
				{
					ScriptError("Local state for '" + text + "' not defined!");
					return false;
				}
				float num = ParseFloatValue(m_local_states[text]);
				float num2 = ParseFloatValue(text2);
				m_local_states[text] = ParseStringFromFloat(num + num2);
			}
			xAttribute = element.Attribute("random_min");
			if (xAttribute != null)
			{
				int min = int.Parse(xAttribute.Value);
				xAttribute = element.Attribute("random_max");
				if (xAttribute != null)
				{
					int max = int.Parse(xAttribute.Value);
					m_local_states[text] = m_game.GetRandom(min, max).ToString();
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

	protected virtual bool parseHandleLocalState(XElement element)
	{
		try
		{
			string text = "";
			XAttribute xAttribute = element.Attribute("state");
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			if (!m_local_states.ContainsKey(text))
			{
				return false;
			}
			m_game.HandleEvent(m_name + ".Handle" + text + "." + m_local_states[text]);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseTrace(XElement element)
	{
		try
		{
			string text = "";
			XAttribute xAttribute = element.Attribute("text");
			if (xAttribute != null)
			{
				text = xAttribute.Value;
			}
			xAttribute = element.Attribute("local_state");
			if (xAttribute != null)
			{
				if (m_local_states == null)
				{
					ScriptError("No local states defined yet!");
					return false;
				}
				if (!m_local_states.ContainsKey(xAttribute.Value))
				{
					ScriptError("Local state '" + xAttribute.Value + "' not defined!");
					return false;
				}
				string text2 = text;
				text = text2 + "Local state '" + xAttribute.Value + "' = " + m_local_states[xAttribute.Value];
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetGlobalState(XElement element)
	{
		try
		{
			string text = "";
			string text2 = "";
			XAttribute xAttribute = element.Attribute("state");
			if (xAttribute == null)
			{
				ScriptError("SetGlobalState: 'state' attribute not defined!");
				return false;
			}
			text = xAttribute.Value;
			xAttribute = element.Attribute("value");
			if (xAttribute != null)
			{
				text2 = xAttribute.Value;
				m_game.m_game_data.SetState(text, text2);
				return true;
			}
			xAttribute = element.Attribute("local_state");
			if (xAttribute != null)
			{
				text2 = GetLocalState(xAttribute.Value);
				m_game.m_game_data.SetState(text, text2);
				return true;
			}
			ScriptError("SetGlobalState: no value defined!");
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
	}

	protected virtual bool parseShowText(XElement element)
	{
		try
		{
			if (m_game == null || m_game.m_hud == null)
			{
				return false;
			}
			string text = "";
			string text2 = "";
			bool fade = m_game.m_world.GetCurrentView().m_use_text_fade;
			XName name = "text";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = m_game.m_language.GetString(xAttribute.Value);
			name = "text_event";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				text2 = xAttribute.Value;
			}
			name = "fade";
			xAttribute = element.Attribute(name);
			if (xAttribute != null)
			{
				fade = bool.Parse(xAttribute.Value);
			}
			if (text2 != "")
			{
				m_game.m_hud.ShowText(text, text2, fade, m_game.m_world.GetCurrentView().m_no_text_fade);
			}
			else
			{
				m_game.m_hud.ShowText(text, fade, m_game.m_world.GetCurrentView().m_no_text_fade);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetState(XElement element)
	{
		try
		{
			string text = "";
			string text2 = "";
			XName name = "id";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			name = "state";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			m_game.m_game_data.SetState(text, text2);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseChangeArea(XElement element)
	{
		try
		{
			string text = "";
			string text2 = "";
			bool flag = false;
			XName name = "area";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			name = "view";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text2 = xAttribute.Value;
			name = "door_sound";
			xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			flag = bool.Parse(xAttribute.Value);
			m_game.ChangeArea(text, text2, flag);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parsePlayMusic(XElement element)
	{
		try
		{
			int num = 0;
			XAttribute xAttribute = element.Attribute("value");
			if (xAttribute == null)
			{
				return false;
			}
			num = int.Parse(xAttribute.Value);
			m_game.m_game_data.SetState("Music", xAttribute.Value);
			m_game.PlayMusic(num);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetVibration(XElement element)
	{
		try
		{
			float num = 0f;
			XAttribute xAttribute = element.Attribute("value");
			if (xAttribute == null)
			{
				return false;
			}
			num = ParseFloatValue(xAttribute.Value);
			GamePad.SetVibration(Game.PLAYER_INDEX, num, num);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parsePlayDoorSound(XElement element)
	{
		try
		{
			m_game.PlayDoorSound();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseSetHUDAlpha(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("value");
			if (xAttribute == null)
			{
				return false;
			}
			m_game.m_hud.m_alpha = ParseFloatValue(xAttribute.Value);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseEvent(XElement element)
	{
		try
		{
			if (!element.HasAttributes)
			{
				return false;
			}
			XAttribute xAttribute = element.Attribute("name");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			if (m_events == null)
			{
				m_events = new Dictionary<string, List<XElement>>();
			}
			List<XElement> list = new List<XElement>();
			for (XNode xNode = element.FirstNode; xNode != null; xNode = xNode.NextNode)
			{
				if ((object)xNode.GetType() == typeof(XElement))
				{
					list.Add((XElement)xNode);
				}
			}
			if (m_events.ContainsKey(value))
			{
				ScriptError("Event '" + value + "' already exists!");
				return false;
			}
			m_events.Add(value, list);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseCase(XElement element)
	{
		try
		{
			ScriptCase scriptCase = new ScriptCase(this, element);
			if (scriptCase == null)
			{
				return false;
			}
			XNode xNode = scriptCase.Execute(m_game, this);
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
			xNode = null;
			scriptCase.Clear();
			scriptCase = null;
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return false;
	}

	protected virtual bool parseHandleEvent(XElement element)
	{
		try
		{
			string text = "";
			XName name = "event";
			XAttribute xAttribute = element.Attribute(name);
			if (xAttribute == null)
			{
				return false;
			}
			text = xAttribute.Value;
			m_game.HandleEvent(text);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseDelayedEvent(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("event");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			float delay = 0f;
			xAttribute = element.Attribute("delay");
			if (xAttribute != null)
			{
				delay = ParseFloatValue(xAttribute.Value);
			}
			xAttribute = element.Attribute("min_delay");
			if (xAttribute != null)
			{
				float num = ParseFloatValue(xAttribute.Value);
				xAttribute = element.Attribute("max_delay");
				if (xAttribute != null)
				{
					float num2 = ParseFloatValue(xAttribute.Value);
					delay = m_game.GetRandom((int)Math.Round(num), (int)Math.Round(num2));
				}
			}
			if (m_delayed_events == null)
			{
				m_delayed_events = new List<DelayedEvent>();
			}
			m_delayed_events.Add(new DelayedEvent(value, delay));
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseUseEventHandled(XElement element)
	{
		try
		{
			m_game.UseEventHandled();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseShowAsk(XElement element)
	{
		try
		{
			XAttribute xAttribute = element.Attribute("text");
			if (xAttribute == null)
			{
				return false;
			}
			string value = xAttribute.Value;
			xAttribute = element.Attribute("ask1");
			if (xAttribute == null)
			{
				return false;
			}
			string value2 = xAttribute.Value;
			xAttribute = element.Attribute("ask2");
			if (xAttribute == null)
			{
				return false;
			}
			string value3 = xAttribute.Value;
			string a_event = "";
			string b_event = "";
			bool fade = false;
			xAttribute = element.Attribute("ask_event1");
			if (xAttribute != null)
			{
				a_event = xAttribute.Value;
			}
			xAttribute = element.Attribute("ask_event2");
			if (xAttribute != null)
			{
				b_event = xAttribute.Value;
			}
			xAttribute = element.Attribute("fade");
			if (xAttribute != null)
			{
				fade = bool.Parse(xAttribute.Value);
			}
			m_game.m_hud.ShowAsk(value, value2, value3, a_event, b_event, fade, m_game.m_world.GetCurrentView().m_no_text_fade);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseBlockInput(XElement element)
	{
		try
		{
			m_game.m_input_blocked = true;
			m_game.m_input_enabled = false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	protected virtual bool parseUnblockInput(XElement element)
	{
		try
		{
			m_game.m_input_blocked = false;
			m_game.m_input_enabled = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
		return true;
	}

	public virtual void HandleEvent(string s_event)
	{
		if (m_events == null)
		{
			return;
		}
		foreach (KeyValuePair<string, List<XElement>> @event in m_events)
		{
			if (@event.Value != null && @event.Key == s_event)
			{
				for (int i = 0; i < @event.Value.Count; i++)
				{
					parseElement(@event.Value[i]);
				}
			}
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		try
		{
			HandleEvent(m_name + ".onUpdate");
			if (m_fade_out_sounds != null)
			{
				foreach (KeyValuePair<SoundEffectInstance, float> fade_out_sound in m_fade_out_sounds)
				{
					if (fade_out_sound.Key == null)
					{
						continue;
					}
					float volume = fade_out_sound.Key.Volume;
					volume -= (float)elapsed.TotalMilliseconds * 0.001f * fade_out_sound.Value;
					if (volume <= 0f)
					{
						volume = 0f;
						if (m_remove_sounds == null)
						{
							m_remove_sounds = new List<SoundEffectInstance>();
						}
						m_remove_sounds.Add(fade_out_sound.Key);
					}
					fade_out_sound.Key.Volume = volume;
				}
			}
			if (m_remove_sounds != null)
			{
				if (m_fade_out_sounds != null)
				{
					for (int i = 0; i < m_remove_sounds.Count; i++)
					{
						m_fade_out_sounds.Remove(m_remove_sounds[i]);
					}
				}
				m_remove_sounds.Clear();
			}
			if (m_fade_in_sounds != null)
			{
				foreach (KeyValuePair<SoundEffectInstance, float> fade_in_sound in m_fade_in_sounds)
				{
					if (fade_in_sound.Key == null)
					{
						continue;
					}
					float volume2 = fade_in_sound.Key.Volume;
					volume2 += (float)elapsed.TotalMilliseconds * 0.001f * fade_in_sound.Value;
					if (volume2 >= 1f)
					{
						volume2 = 1f;
						if (m_remove_sounds == null)
						{
							m_remove_sounds = new List<SoundEffectInstance>();
						}
						m_remove_sounds.Add(fade_in_sound.Key);
					}
					fade_in_sound.Key.Volume = volume2;
				}
			}
			if (m_remove_sounds != null)
			{
				if (m_fade_in_sounds != null)
				{
					for (int j = 0; j < m_remove_sounds.Count; j++)
					{
						m_fade_in_sounds.Remove(m_remove_sounds[j]);
					}
				}
				m_remove_sounds.Clear();
			}
			if (m_delayed_events == null)
			{
				return;
			}
			for (int k = 0; k < m_delayed_events.Count; k++)
			{
				if (m_delayed_events[k] != null)
				{
					m_delayed_events[k].m_delay -= elapsed.Milliseconds;
					if (m_delayed_events[k].m_delay <= 0f)
					{
						m_game.HandleEvent(m_delayed_events[k].m_event);
						m_delayed_events.Remove(m_delayed_events[k]);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void ScriptError(string message)
	{
	}
}
