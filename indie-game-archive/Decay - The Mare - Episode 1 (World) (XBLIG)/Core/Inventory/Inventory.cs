using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Core.Inventory;

public class Inventory
{
	public enum INVENTORY_STATE
	{
		DISABLED,
		DEFAULT,
		ASK_PICKUP,
		EXAMINE,
		COMBINE
	}

	protected enum INVENTORY_FADE_STATE
	{
		FADE_IN,
		FADE_OUT,
		FADE_IN_EXAMINE,
		FADE_OUT_EXAMINE,
		IDLE
	}

	public INVENTORY_STATE m_state;

	protected INVENTORY_FADE_STATE m_fade_state = INVENTORY_FADE_STATE.IDLE;

	protected Game m_game;

	protected Texture2D m_fade;

	protected Color m_color = Color.White;

	protected float m_alpha;

	protected Color m_pickup_color = Color.White;

	protected float m_pickup_alpha;

	protected Arrow m_left_arrow;

	protected Arrow m_right_arrow;

	protected List<Slot> m_slots = new List<Slot>();

	protected Texture2D m_slot_medium;

	protected Texture2D m_slot_large;

	protected Texture2D m_slot_red;

	protected Texture2D m_slot_green;

	protected Texture2D m_coin;

	protected List<Item> m_preloaded_items = new List<Item>();

	protected List<Item> m_items = new List<Item>();

	protected int m_current_item;

	protected int m_combine_item;

	protected bool m_left_pressed;

	protected bool m_right_pressed;

	protected Item m_pickup_item;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	public Texture2D m_a_button;

	public Texture2D m_b_button;

	public Texture2D m_x_button;

	public Texture2D m_y_button;

	public Texture2D m_LS;

	protected float m_examine_alpha;

	protected SGSModel m_examine_model;

	protected Matrix m_examine_start_rot = Matrix.Identity;

	protected Vector2 m_model_rotation = Vector2.Zero;

	protected float m_anim_rot;

	protected string m_replace_examine_item = "";

	protected string m_change_examine_item = "";

	protected string m_use_event = "";

	protected string m_content_path = "";

	protected ContentManager m_examine_content;

	protected SGSCamera m_camera;

	protected RenderTarget2D m_RT;

	protected SoundEffect m_beep;

	protected SoundEffect m_beep_error;

	public float m_scroll_y;

	private float m_min_scroll_y;

	private float m_max_scroll_y;

	public bool m_rotation_input = true;

	public Inventory(Game game)
	{
		m_game = game;
		m_examine_content = new ContentManager(m_game.Services);
		m_examine_content.RootDirectory = SGSContentLoader.CONTENT_PATH;
		m_content_path = "Inventory/";
		m_font = m_game.Content.Load<SpriteFont>(m_content_path + "../Fonts/SpriteFont1");
		m_font2 = m_game.Content.Load<SpriteFont>(m_content_path + "../Fonts/SpriteFont2");
		m_fade = m_game.Content.Load<Texture2D>(m_content_path + "svart_ruta");
		m_a_button = m_game.Content.Load<Texture2D>(m_content_path + "../HUD/a_button");
		m_b_button = m_game.Content.Load<Texture2D>(m_content_path + "../HUD/b_button");
		m_x_button = m_game.Content.Load<Texture2D>(m_content_path + "../HUD/x_button");
		m_y_button = m_game.Content.Load<Texture2D>(m_content_path + "../HUD/y_button");
		m_LS = m_game.Content.Load<Texture2D>(m_content_path + "../HUD/LS");
		m_coin = m_game.Content.Load<Texture2D>("Inventory/coin");
		m_slot_medium = m_game.Content.Load<Texture2D>(m_content_path + "ruta_medium");
		m_slot_large = m_game.Content.Load<Texture2D>(m_content_path + "ruta_stor");
		m_slot_red = m_game.Content.Load<Texture2D>(m_content_path + "ruta_combine_red");
		m_slot_green = m_game.Content.Load<Texture2D>(m_content_path + "ruta_combine_green");
		Slot item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2(565f - (float)m_slot_medium.Width * 1.75f - (float)m_slot_medium.Width * 1.5f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2(565f - (float)m_slot_medium.Width * 1.75f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_large, Slot.SLOT_TYPE.LARGE)
		{
			m_pos = new Vector2(565f, 288f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2((float)(565 + m_slot_large.Width) + (float)m_slot_medium.Width * 0.75f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2((float)(565 + m_slot_large.Width) + (float)m_slot_medium.Width * 2.25f, 317f)
		};
		m_slots.Add(item);
		m_left_arrow = new Arrow(m_game.Content.Load<Texture2D>(m_content_path + "arrow"), m_game.Content.Load<Texture2D>(m_content_path + "arrow_green"), flip: true);
		m_left_arrow.m_pos = new Vector2(m_slots[0].m_pos.X - m_left_arrow.m_width - 30f, 340f);
		m_right_arrow = new Arrow(m_game.Content.Load<Texture2D>(m_content_path + "arrow"), m_game.Content.Load<Texture2D>(m_content_path + "arrow_green"), flip: false);
		m_right_arrow.m_pos = new Vector2(m_slots[4].m_pos.X + (float)m_slot_medium.Width + 30f, 340f);
		m_camera = new SGSCamera(m_game.GraphicsDevice, Vector3.Zero, 1f, 1000f);
		m_beep = m_game.m_CL.LoadSound("Sound/pipljud");
		m_beep_error = m_game.m_CL.LoadSound("Sound/pipljud_error");
	}

	public virtual void Clear()
	{
		m_beep = null;
		m_beep_error = null;
		m_game = null;
		m_examine_model = null;
		m_font = null;
		m_font2 = null;
		m_fade = null;
		m_left_arrow.Clear();
		m_left_arrow = null;
		m_right_arrow.Clear();
		m_right_arrow = null;
		m_slot_medium.Dispose();
		m_slot_medium = null;
		m_slot_large.Dispose();
		m_slot_large = null;
		m_slot_red.Dispose();
		m_slot_red = null;
		m_slot_green.Dispose();
		m_slot_green = null;
		m_coin = null;
		for (int i = 0; i < m_slots.Count; i++)
		{
			m_slots[i].Clear();
			m_slots[i] = null;
		}
		m_slots.Clear();
		m_slots = null;
		for (int j = 0; j < m_items.Count; j++)
		{
			m_items[j] = null;
		}
		m_items.Clear();
		m_items = null;
		for (int k = 0; k < m_preloaded_items.Count; k++)
		{
			m_preloaded_items[k].Clear();
			m_preloaded_items[k] = null;
		}
		m_preloaded_items.Clear();
		m_preloaded_items = null;
		if (m_pickup_item != null)
		{
			m_pickup_item.Clear();
			m_pickup_item = null;
		}
		if (m_a_button != null)
		{
			m_a_button.Dispose();
			m_a_button = null;
		}
		if (m_b_button != null)
		{
			m_b_button.Dispose();
			m_b_button = null;
		}
		if (m_x_button != null)
		{
			m_x_button.Dispose();
			m_x_button = null;
		}
		if (m_y_button != null)
		{
			m_y_button.Dispose();
			m_y_button = null;
		}
		if (m_LS != null)
		{
			m_LS.Dispose();
			m_LS = null;
		}
		if (m_examine_content != null)
		{
			m_examine_content.Unload();
			m_examine_content.Dispose();
			m_examine_content = null;
		}
		if (m_RT != null)
		{
			m_RT.Dispose();
			m_RT = null;
		}
		m_camera = null;
	}

	public void UpdateSaveData()
	{
		m_game.m_game_data.m_items.Clear();
		for (int i = 0; i < m_items.Count; i++)
		{
			m_game.m_game_data.m_items.Add(m_items[i].m_id);
		}
	}

	public void LoadItems()
	{
		try
		{
			for (int i = 0; i < m_game.m_game_data.m_items.Count; i++)
			{
				AddItem(m_game.m_game_data.m_items[i], loading: true);
			}
		}
		catch
		{
		}
	}

	public bool FindItem(string id)
	{
		for (int i = 0; i < m_items.Count; i++)
		{
			if (m_items[i].m_id == id)
			{
				return true;
			}
		}
		return false;
	}

	public virtual Item GetItem(string item_id)
	{
		if (m_preloaded_items == null)
		{
			return null;
		}
		for (int i = 0; i < m_preloaded_items.Count; i++)
		{
			if (m_preloaded_items[i] != null && m_preloaded_items[i].m_id == item_id)
			{
				return m_preloaded_items[i];
			}
		}
		return null;
	}

	public virtual void AddItem(string id, bool loading)
	{
		Item item = GetItem(id);
		if (item != null)
		{
			m_items.Add(item);
		}
	}

	public void RemoveItem(string id)
	{
		for (int i = 0; i < m_items.Count; i++)
		{
			if (!(m_items[i].m_id == id))
			{
				continue;
			}
			if (m_current_item == i)
			{
				m_current_item--;
				if (m_current_item < 0)
				{
					m_current_item = 0;
				}
			}
			m_items.RemoveAt(i);
		}
	}

	public void FadeOut()
	{
		if (m_fade_state != INVENTORY_FADE_STATE.FADE_OUT)
		{
			m_alpha = 1f;
			m_color = Color.White * m_alpha;
			m_pickup_color = Color.White * m_pickup_alpha;
			m_fade_state = INVENTORY_FADE_STATE.FADE_OUT;
			m_game.m_input_enabled = false;
		}
	}

	public void FadeIn()
	{
		m_alpha = 0f;
		m_color = Color.White * m_alpha;
		m_fade_state = INVENTORY_FADE_STATE.FADE_IN;
		m_game.m_input_enabled = false;
		m_game.m_b_pressed = true;
	}

	protected virtual void onExamine()
	{
		try
		{
			if (m_items.Count <= 0)
			{
				return;
			}
			m_state = INVENTORY_STATE.EXAMINE;
			m_fade_state = INVENTORY_FADE_STATE.FADE_IN_EXAMINE;
			m_examine_alpha = 0f;
			m_game.m_input_enabled = false;
			m_game.m_b_pressed = true;
			m_game.m_a_pressed = true;
			m_examine_model = m_items[m_current_item].m_model;
			if (m_examine_model == null && m_items[m_current_item].m_examine_model_from_item != "")
			{
				Item item = GetItem(m_items[m_current_item].m_examine_model_from_item);
				if (item != null)
				{
					m_examine_model = item.m_model;
					m_items[m_current_item].m_model = item.m_model;
					m_items[m_current_item].m_cam_pos = item.m_cam_pos;
				}
				item = null;
			}
			if (m_examine_model != null)
			{
				m_examine_start_rot = m_examine_model.m_rot_matrix;
				m_camera.m_pos = m_items[m_current_item].m_cam_pos;
				m_camera.m_view_matrix = Matrix.CreateLookAt(m_camera.m_pos, Vector3.Zero, Vector3.Up);
				m_game.m_inventory.m_rotation_input = true;
			}
			if (m_items[m_current_item].m_use_scrolling)
			{
				m_scroll_y = Game.TS_AREA.Top;
				m_min_scroll_y = m_items[m_current_item].m_min_scroll_y;
				m_max_scroll_y = Game.TS_AREA.Top;
			}
			if (m_items[m_current_item].m_examine_anim != null)
			{
				m_items[m_current_item].m_examine_anim.SetFrame(0);
				m_anim_rot = 0f;
			}
			m_items[m_current_item].Reset();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void AskPickup(string item_id)
	{
		m_pickup_item = GetItem(item_id);
		if (m_pickup_item == null)
		{
			return;
		}
		m_examine_model = m_pickup_item.m_model;
		if (m_examine_model == null && m_pickup_item.m_examine_model_from_item != "")
		{
			Item item = GetItem(m_pickup_item.m_examine_model_from_item);
			if (item != null)
			{
				m_examine_model = item.m_model;
				m_pickup_item.m_model = item.m_model;
				m_pickup_item.m_cam_pos = item.m_cam_pos;
			}
			item = null;
		}
		m_state = INVENTORY_STATE.ASK_PICKUP;
		m_game.m_state = Game.GAME_STATE.INVENTORY;
		FadeIn();
		m_game.m_show_cursor = false;
		m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
		m_game.m_y_pressed = true;
		m_pickup_alpha = 0f;
		m_pickup_color = Color.White * m_pickup_alpha;
		if (m_pickup_item != null)
		{
			m_camera.m_pos = m_pickup_item.m_cam_pos;
			m_camera.m_view_matrix = Matrix.CreateLookAt(m_camera.m_pos, Vector3.Zero, Vector3.Up);
		}
		m_game.m_input_enabled = false;
	}

	protected int GetCombineResultIndex(Item item1, Item item2)
	{
		for (int i = 0; i < item1.m_combine_id.Count; i++)
		{
			if (item1.m_combine_id[i] == item2.m_id)
			{
				return i;
			}
		}
		return -1;
	}

	protected virtual void CombineItems()
	{
		if (m_combine_item == m_current_item)
		{
			m_game.PlaySound(m_beep_error, 0.2f);
			return;
		}
		int combineResultIndex = GetCombineResultIndex(m_items[m_combine_item], m_items[m_current_item]);
		if (combineResultIndex == -1)
		{
			m_game.PlaySound(m_beep_error, 0.2f);
			return;
		}
		string id = m_items[m_combine_item].m_id;
		string id2 = m_items[m_current_item].m_id;
		string id3 = m_items[m_combine_item].m_combine_result_id[combineResultIndex];
		bool flag = m_items[m_combine_item].RemoveOnCombine(id2);
		bool flag2 = m_items[m_current_item].RemoveOnCombine(id);
		if (flag)
		{
			RemoveItem(id);
		}
		if (flag2)
		{
			RemoveItem(id2);
		}
		m_combine_item = 0;
		AddItem(id3, loading: false);
		m_current_item = m_items.Count - 1;
		m_slots[2].m_bkg = m_slot_large;
		m_state = INVENTORY_STATE.DEFAULT;
	}

	public virtual void ReplaceExamineItem(string id)
	{
		try
		{
			m_replace_examine_item = id;
			m_fade_state = INVENTORY_FADE_STATE.FADE_OUT_EXAMINE;
			m_examine_alpha = 1f;
			m_game.m_input_enabled = false;
			m_game.m_b_pressed = true;
			m_game.m_a_pressed = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void ChangeExamineItem(string id)
	{
		try
		{
			m_change_examine_item = id;
			m_fade_state = INVENTORY_FADE_STATE.FADE_OUT_EXAMINE;
			m_examine_alpha = 1f;
			m_game.m_input_enabled = false;
			m_game.m_b_pressed = true;
			m_game.m_a_pressed = true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual int GetItemIndex(string item_id)
	{
		try
		{
			for (int i = 0; i < m_items.Count; i++)
			{
				if (m_items[i].m_id == item_id)
				{
					return i;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return -1;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		KeyboardState state = Keyboard.GetState();
		switch (m_state)
		{
		case INVENTORY_STATE.DISABLED:
			if (!m_game.m_input_enabled)
			{
				break;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.Y == ButtonState.Pressed || state.IsKeyDown(Keys.Y))
			{
				if (m_game.m_y_pressed)
				{
					break;
				}
				m_game.m_y_pressed = true;
				if (m_game.m_state != Game.GAME_STATE.SHOW_TEXT && m_items.Count > 0)
				{
					GamePad.SetVibration(Game.PLAYER_INDEX, 0f, 0f);
					m_game.m_state = Game.GAME_STATE.INVENTORY;
					FadeIn();
					m_game.m_show_cursor = false;
					m_game.m_hud.m_state = HUD.HUD_STATE.NONE;
					m_state = INVENTORY_STATE.DEFAULT;
					if (m_game.m_tutorial_state == Tutorial.STATE.INVENTORY)
					{
						m_game.m_tutorial_state = Tutorial.STATE.NONE;
						int tutorial_state = (int)m_game.m_tutorial_state;
						m_game.m_game_data.SetState("TutorialState", tutorial_state.ToString());
					}
				}
			}
			else
			{
				m_game.m_y_pressed = false;
			}
			break;
		case INVENTORY_STATE.DEFAULT:
			if (!m_game.m_input_enabled)
			{
				break;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					FadeOut();
					if (m_game.m_active_trigger != null)
					{
						m_game.m_state = Game.GAME_STATE.ACTIVE_TRIGGER;
					}
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
			{
				if (!m_game.m_a_pressed && m_game.m_active_trigger == null)
				{
					m_game.m_a_pressed = true;
					if (m_items.Count > 0)
					{
						FadeOut();
						m_use_event = m_items[m_current_item].m_id;
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.Y == ButtonState.Pressed || state.IsKeyDown(Keys.Y))
			{
				if (!m_game.m_y_pressed)
				{
					m_game.m_y_pressed = true;
					onExamine();
				}
			}
			else
			{
				m_game.m_y_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.X == ButtonState.Pressed || state.IsKeyDown(Keys.X))
			{
				if (!m_game.m_x_pressed)
				{
					m_game.m_x_pressed = true;
					if (m_items.Count > 0)
					{
						m_state = INVENTORY_STATE.COMBINE;
						m_combine_item = m_current_item;
						m_game.m_a_pressed = true;
						m_game.m_b_pressed = true;
					}
				}
			}
			else
			{
				m_game.m_x_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || state.IsKeyDown(Keys.Right))
			{
				m_right_arrow.SetState(Arrow.ARROW_STATE.ACTIVE);
				if (!m_right_pressed)
				{
					m_game.PlaySound(m_beep, 0.2f);
					m_right_pressed = true;
					if (m_items.Count > 0)
					{
						m_current_item++;
						if (m_current_item >= m_items.Count)
						{
							m_current_item = m_items.Count - 1;
						}
					}
				}
			}
			else
			{
				m_right_arrow.SetState(Arrow.ARROW_STATE.IDLE);
				m_right_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || state.IsKeyDown(Keys.Left))
			{
				m_left_arrow.SetState(Arrow.ARROW_STATE.ACTIVE);
				if (m_left_pressed)
				{
					break;
				}
				m_game.PlaySound(m_beep, 0.2f);
				m_left_pressed = true;
				if (m_items.Count > 0)
				{
					m_current_item--;
					if (m_current_item < 0)
					{
						m_current_item = 0;
					}
				}
			}
			else
			{
				m_left_arrow.SetState(Arrow.ARROW_STATE.IDLE);
				m_left_pressed = false;
			}
			break;
		case INVENTORY_STATE.COMBINE:
			if (m_game.m_input_enabled)
			{
				if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
				{
					if (!m_game.m_a_pressed)
					{
						m_game.m_a_pressed = true;
						CombineItems();
					}
				}
				else
				{
					m_game.m_a_pressed = false;
				}
				if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
				{
					if (!m_game.m_b_pressed)
					{
						m_slots[2].m_bkg = m_slot_large;
						m_current_item = m_combine_item;
						m_game.m_b_pressed = true;
						m_state = INVENTORY_STATE.DEFAULT;
					}
				}
				else
				{
					m_game.m_b_pressed = false;
				}
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Right == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X > 0.2f || state.IsKeyDown(Keys.Right))
			{
				m_right_arrow.SetState(Arrow.ARROW_STATE.ACTIVE);
				if (!m_right_pressed)
				{
					m_game.PlaySound(m_beep, 0.2f);
					m_right_pressed = true;
					if (m_items.Count > 0)
					{
						m_current_item++;
						if (m_current_item >= m_items.Count)
						{
							m_current_item = m_items.Count - 1;
						}
					}
				}
			}
			else
			{
				m_right_arrow.SetState(Arrow.ARROW_STATE.IDLE);
				m_right_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).DPad.Left == ButtonState.Pressed || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X < -0.2f || state.IsKeyDown(Keys.Left))
			{
				m_left_arrow.SetState(Arrow.ARROW_STATE.ACTIVE);
				if (m_left_pressed)
				{
					break;
				}
				m_game.PlaySound(m_beep, 0.2f);
				m_left_pressed = true;
				if (m_items.Count > 0)
				{
					m_current_item--;
					if (m_current_item < 0)
					{
						m_current_item = 0;
					}
				}
			}
			else
			{
				m_left_arrow.SetState(Arrow.ARROW_STATE.IDLE);
				m_left_pressed = false;
			}
			break;
		case INVENTORY_STATE.ASK_PICKUP:
			if (m_pickup_item != null)
			{
				if (m_examine_model != null)
				{
					m_examine_model.Update(elapsed);
				}
				if (m_fade_state == INVENTORY_FADE_STATE.IDLE)
				{
					m_pickup_alpha += (float)elapsed.TotalSeconds * 2f;
					if (m_pickup_alpha >= 1f)
					{
						m_pickup_alpha = 1f;
						m_game.m_input_enabled = true;
					}
					m_pickup_color = Color.White * m_pickup_alpha;
				}
			}
			if (!m_game.m_input_enabled)
			{
				break;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					FadeOut();
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
			{
				if (m_game.m_a_pressed)
				{
					break;
				}
				m_game.m_a_pressed = true;
				FadeOut();
				if (m_pickup_item.m_bundle_ids.Count > 0)
				{
					for (int i = 0; i < m_pickup_item.m_bundle_ids.Count; i++)
					{
						AddItem(m_pickup_item.m_bundle_ids[i], loading: false);
					}
				}
				else
				{
					AddItem(m_pickup_item.m_id, loading: false);
				}
				m_game.HandleEvent("Pickup" + m_pickup_item.m_id);
				m_game.HandleEvent("Pickup." + m_pickup_item.m_id);
				if (m_game.m_tutorial_state == Tutorial.STATE.WAIT_FOR_PICKUP)
				{
					m_game.m_tutorial_state = Tutorial.STATE.INVENTORY;
					int tutorial_state2 = (int)m_game.m_tutorial_state;
					m_game.m_game_data.SetState("TutorialState", tutorial_state2.ToString());
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
		case INVENTORY_STATE.EXAMINE:
			if (m_examine_model != null)
			{
				m_examine_model.Update(elapsed);
			}
			if (m_items[m_current_item] != null)
			{
				m_items[m_current_item].Update(elapsed);
			}
			if (m_fade_state == INVENTORY_FADE_STATE.FADE_IN_EXAMINE)
			{
				m_examine_alpha += (float)elapsed.TotalSeconds * 2f;
				if (m_examine_alpha >= 1f)
				{
					m_examine_alpha = 1f;
					m_fade_state = INVENTORY_FADE_STATE.IDLE;
					m_game.m_input_enabled = true;
				}
			}
			if (m_fade_state == INVENTORY_FADE_STATE.FADE_OUT_EXAMINE)
			{
				m_examine_alpha -= (float)elapsed.TotalSeconds * 2f;
				if (m_examine_alpha <= 0f)
				{
					m_examine_alpha = 0f;
					if (m_examine_model != null)
					{
						m_examine_model.m_rot_matrix = m_examine_start_rot;
						m_examine_model.UpdateTransform();
						m_examine_model = null;
					}
					m_game.m_input_enabled = true;
					m_fade_state = INVENTORY_FADE_STATE.IDLE;
					m_state = INVENTORY_STATE.DEFAULT;
					if (m_replace_examine_item != "")
					{
						Item item = GetItem(m_replace_examine_item);
						if (item != null)
						{
							m_items[m_current_item] = GetItem(m_replace_examine_item);
						}
						m_replace_examine_item = "";
						onExamine();
					}
					if (m_change_examine_item != "")
					{
						int itemIndex = GetItemIndex(m_change_examine_item);
						if (itemIndex != -1)
						{
							m_current_item = itemIndex;
						}
						m_change_examine_item = "";
						onExamine();
					}
				}
			}
			if (!m_game.m_input_enabled)
			{
				break;
			}
			if (m_rotation_input)
			{
				if (m_items[m_current_item].m_examine_anim != null)
				{
					if (GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X <= -0.1f || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X >= 0.1f || state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.Right))
					{
						m_anim_rot += GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X * (float)elapsed.TotalSeconds * 50f;
						if (state.IsKeyDown(Keys.Left))
						{
							m_anim_rot = (0f - (float)elapsed.TotalSeconds) * 50f;
						}
						if (state.IsKeyDown(Keys.Right))
						{
							m_anim_rot = (float)elapsed.TotalSeconds * 50.01f;
						}
					}
					int num = (int)Math.Round(m_anim_rot);
					if (num < 0)
					{
						m_anim_rot = m_items[m_current_item].m_examine_anim.m_end_frame;
						num = m_items[m_current_item].m_examine_anim.m_end_frame;
					}
					if (num > m_items[m_current_item].m_examine_anim.m_end_frame)
					{
						m_anim_rot = 0f;
						num = 0;
					}
					m_items[m_current_item].m_examine_anim.SetFrame(num);
				}
				if (GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X <= -0.1f || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X >= 0.1f || state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.Right))
				{
					m_model_rotation.Y = GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.X * (float)elapsed.TotalMilliseconds * 0.001f * 2f;
					if (state.IsKeyDown(Keys.Left))
					{
						m_model_rotation.Y = (0f - (float)elapsed.TotalMilliseconds) * 0.001f;
					}
					if (state.IsKeyDown(Keys.Right))
					{
						m_model_rotation.Y = (float)elapsed.TotalMilliseconds * 0.001f;
					}
					if (m_examine_model != null)
					{
						m_examine_model.RotateY(m_model_rotation.Y);
					}
				}
				if (GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y <= -0.1f || GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y >= 0.1f || state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.Down))
				{
					m_model_rotation.X = (0f - GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y) * (float)elapsed.TotalMilliseconds * 0.001f * 2f;
					if (state.IsKeyDown(Keys.Up))
					{
						m_model_rotation.X = (0f - (float)elapsed.TotalMilliseconds) * 0.001f;
					}
					if (state.IsKeyDown(Keys.Down))
					{
						m_model_rotation.X = (float)elapsed.TotalMilliseconds * 0.001f;
					}
					if (m_examine_model != null)
					{
						m_examine_model.RotateX(m_model_rotation.X);
					}
					if (m_items[m_current_item].m_use_scrolling)
					{
						m_scroll_y += GamePad.GetState(Game.PLAYER_INDEX).ThumbSticks.Left.Y * (float)elapsed.TotalMilliseconds * 0.001f * 400f;
						if (state.IsKeyDown(Keys.Up))
						{
							m_scroll_y += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
						}
						if (state.IsKeyDown(Keys.Down))
						{
							m_scroll_y -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
						}
						if (m_scroll_y < m_min_scroll_y)
						{
							m_scroll_y = m_min_scroll_y;
						}
						if (m_scroll_y > m_max_scroll_y)
						{
							m_scroll_y = m_max_scroll_y;
						}
					}
				}
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					m_game.m_input_enabled = false;
					m_examine_alpha = 1f;
					m_fade_state = INVENTORY_FADE_STATE.FADE_OUT_EXAMINE;
					m_game.m_y_pressed = true;
					m_game.m_a_pressed = true;
					if (m_items[m_current_item] != null)
					{
						m_items[m_current_item].onCloseExamine();
					}
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			if (GamePad.GetState(Game.PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					if (m_items[m_current_item] != null)
					{
						m_items[m_current_item].onExamineUse();
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
		}
		switch (m_fade_state)
		{
		case INVENTORY_FADE_STATE.FADE_OUT:
			m_alpha -= (float)elapsed.TotalSeconds * 2f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_fade_state = INVENTORY_FADE_STATE.IDLE;
				if (!m_game.m_input_blocked)
				{
					m_game.m_input_enabled = true;
				}
				m_state = INVENTORY_STATE.DISABLED;
				m_pickup_item = null;
				m_game.onCloseInventory();
				if (m_use_event != "")
				{
					m_game.HandleEvent(m_game.m_world.GetCurrentView().m_name + ".Use." + m_use_event);
					m_use_event = "";
				}
			}
			m_color = Color.White * m_alpha;
			m_pickup_color = Color.White * m_alpha * 0.25f;
			break;
		case INVENTORY_FADE_STATE.FADE_IN:
			m_alpha += (float)elapsed.TotalSeconds * 2f;
			if (m_alpha >= 1f)
			{
				m_alpha = 1f;
				m_fade_state = INVENTORY_FADE_STATE.IDLE;
				if (m_state != INVENTORY_STATE.ASK_PICKUP)
				{
					m_game.m_input_enabled = true;
				}
			}
			m_color = Color.White * m_alpha;
			break;
		}
	}

	protected virtual void DrawDefault(SpriteBatch SB)
	{
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color * 0.75f);
		SB.End();
		Color gray = Color.Gray;
		Color color = new Color(64, 64, 64, 255);
		int num = m_current_item - 2;
		m_left_arrow.Draw(SB, m_color * m_alpha);
		m_right_arrow.Draw(SB, m_color * m_alpha);
		for (int i = 0; i < m_slots.Count; i++)
		{
			if (num >= 0 && num < m_items.Count)
			{
				m_slots[i].SetItem(m_items[num]);
				if (i != 2)
				{
					m_slots[i].Draw(SB, gray * m_alpha);
				}
			}
			else
			{
				m_slots[i].SetItem(null);
				m_slots[i].Draw(SB, color * m_alpha);
			}
			num++;
		}
		if (m_state == INVENTORY_STATE.COMBINE)
		{
			m_slots[2].m_bkg = m_slot_green;
			if (m_combine_item == m_current_item)
			{
				m_slots[2].m_bkg = m_slot_red;
			}
		}
		m_slots[2].Draw(SB, m_color);
		if (m_alpha < 1f)
		{
			return;
		}
		Vector2 zero = Vector2.Zero;
		string text = "";
		zero.X = Game.TS_AREA.Left;
		zero.Y = Game.TS_AREA.Bottom - m_coin.Height;
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_coin, zero, m_color * m_alpha);
		text = "x ";
		text = ((m_game.m_game_data == null || !(m_game.m_game_data.GetState("Coins") != "")) ? (text + "0") : (text + m_game.m_game_data.GetState("Coins")));
		text += "/5";
		zero.X += m_coin.Width + 10;
		zero.Y += 2f;
		SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, text, zero, m_color * m_alpha);
		SB.End();
		if (m_items.Count <= 0)
		{
			return;
		}
		if (m_state == INVENTORY_STATE.DEFAULT)
		{
			text = m_game.m_language.GetString("CLOSE");
			Vector2 vector = m_font.MeasureString(text);
			zero = Vector2.Zero;
			zero.X = (float)Game.TS_AREA.Right - vector.X;
			zero.Y = (float)Game.TS_AREA.Bottom - vector.Y;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text, zero, m_color * m_alpha);
			zero.X -= m_b_button.Width + 10;
			SB.Draw(m_b_button, zero, m_color);
			zero = Vector2.Zero;
			text = m_game.m_language.GetString("USE");
			string text2 = m_game.m_language.GetString("EXAMINE");
			string text3 = m_game.m_language.GetString("COMBINE");
			Vector2 vector2 = m_font.MeasureString(text);
			Vector2 vector3 = m_font.MeasureString(text2);
			Vector2 vector4 = m_font.MeasureString(text3);
			float num2 = 10f;
			float num3 = 30f;
			float num4 = (float)m_a_button.Width + num2 + (float)(int)vector2.X + num3 + (float)m_y_button.Width + num2 + (float)(int)vector3.X + num3 + (float)m_x_button.Width + num2 + (float)(int)vector4.X;
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - m_font2.MeasureString(m_items[m_current_item].m_name).X / 2f);
			zero.Y = 210f;
			SB.DrawString(m_font2, m_items[m_current_item].m_name, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black * m_alpha);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, zero, m_color * m_alpha);
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - num4 / 2f);
			zero.Y = 480f;
			if (zero.Y > (float)Game.TS_AREA.Bottom - vector4.Y)
			{
				zero.Y = (float)Game.TS_AREA.Bottom - vector4.Y;
			}
			Color color2 = m_color * m_alpha;
			if (m_game.m_active_trigger != null)
			{
				color2 = m_color * 0.36f;
			}
			SB.Draw(m_a_button, zero, color2);
			zero.X += (float)m_a_button.Width + num2;
			SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text, zero, color2);
			zero.X += (float)(int)vector2.X + num3;
			SB.Draw(m_y_button, zero, m_color * m_alpha);
			zero.X += (float)m_y_button.Width + num2;
			SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text2, zero, m_color * m_alpha);
			zero.X += (float)(int)vector3.X + num3;
			SB.Draw(m_x_button, zero, m_color);
			zero.X += (float)m_x_button.Width + num2;
			SB.DrawString(m_font, text3, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text3, zero, m_color * m_alpha);
			SB.End();
		}
		else if (m_state == INVENTORY_STATE.COMBINE)
		{
			zero = Vector2.Zero;
			string text4 = m_game.m_language.GetString("COMBINE");
			string text5 = m_game.m_language.GetString("CANCEL");
			Vector2 vector5 = m_font.MeasureString(text4);
			Vector2 vector6 = m_font.MeasureString(text5);
			float num5 = 10f;
			float num6 = 30f;
			float num7 = (float)m_a_button.Width + num5 + (float)(int)vector5.X + num6 + (float)m_x_button.Width + num5 + (float)(int)vector6.X;
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - m_font2.MeasureString(m_items[m_current_item].m_name).X / 2f);
			zero.Y = 210f;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, zero, m_color * m_alpha);
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - num7 / 2f);
			zero.Y = 480f;
			if (zero.Y > (float)Game.TS_AREA.Bottom - vector5.Y)
			{
				zero.Y = (float)Game.TS_AREA.Bottom - vector5.Y;
			}
			SB.Draw(m_a_button, zero, m_color * m_alpha);
			zero.X += (float)m_a_button.Width + num5;
			SB.DrawString(m_font, text4, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text4, zero, m_color * m_alpha);
			zero.X += (float)(int)vector5.X + num6;
			SB.Draw(m_b_button, zero, m_color * m_alpha);
			zero.X += (float)m_y_button.Width + num5;
			SB.DrawString(m_font, text5, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text5, zero, m_color * m_alpha);
			SB.End();
		}
	}

	protected virtual void DrawAskPickup(SpriteBatch SB)
	{
		if (m_pickup_alpha > 0f)
		{
			m_pickup_item.RenderExamineModel(SB.GraphicsDevice, m_RT, m_camera);
		}
		if (m_game.m_active_trigger != null)
		{
			m_game.m_active_trigger.Draw(SB);
		}
		else if (m_game.m_world != null && m_game.m_world.GetCurrentView() != null)
		{
			m_game.m_world.GetCurrentView().Draw(SB);
		}
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color * m_alpha * 0.75f);
		SB.End();
		if (m_pickup_alpha > 0f)
		{
			if (m_examine_model != null)
			{
				m_pickup_item.DrawExamineModel(SB, m_RT, m_pickup_color * m_pickup_alpha);
			}
			else
			{
				m_pickup_item.DrawAskPickupImage(SB, m_pickup_color * m_pickup_alpha);
			}
		}
		if (m_pickup_alpha >= 1f && m_alpha >= 1f)
		{
			string text = m_pickup_item.m_name;
			if (m_pickup_item.m_pickup_name != "")
			{
				text = m_pickup_item.m_pickup_name;
			}
			Vector2 zero = Vector2.Zero;
			Vector2 vector = m_font.MeasureString("PICK UP");
			Vector2 vector2 = m_font.MeasureString("CANCEL");
			Vector2 vector3 = m_font2.MeasureString(text);
			float num = 10f;
			float num2 = 40f;
			float num3 = (float)m_a_button.Width + num + (float)(int)vector.X + num2 + (float)m_b_button.Width + num + (float)(int)vector2.X;
			zero.X = (int)((float)Game.VIEW_RECT.Width - vector3.X) / 2;
			zero.Y = (float)Game.TS_AREA.Bottom - vector.Y - 20f - vector3.Y;
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			SB.DrawString(m_font2, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, text, zero, m_color * m_alpha);
			zero.X = (int)((float)Game.VIEW_RECT.Width - num3) / 2;
			zero.Y += vector3.Y + 20f;
			SB.Draw(m_a_button, zero, m_color * m_alpha);
			zero.X += (float)m_a_button.Width + num;
			SB.DrawString(m_font, "PICK UP", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "PICK UP", zero, m_color * m_alpha);
			zero.X += (float)(int)vector.X + num2;
			SB.Draw(m_b_button, zero, m_color * m_alpha);
			zero.X += (float)m_b_button.Width + num;
			SB.DrawString(m_font, "CANCEL", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "CANCEL", zero, m_color * m_alpha);
			SB.End();
		}
	}

	protected virtual void DrawExamine(SpriteBatch SB)
	{
		m_items[m_current_item].RenderExamineModel(SB.GraphicsDevice, m_RT, m_camera);
		if (m_game.m_active_trigger != null)
		{
			m_game.m_active_trigger.Draw(SB);
		}
		else if (m_game.m_world != null && m_game.m_world.GetCurrentView() != null)
		{
			m_game.m_world.GetCurrentView().Draw(SB);
		}
		SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color * m_alpha * 0.75f);
		SB.End();
		if (m_examine_model != null)
		{
			m_items[m_current_item].DrawExamineModel(SB, m_RT, Color.White * m_examine_alpha);
		}
		else
		{
			m_items[m_current_item].DrawExamineImage(SB, Color.White * m_examine_alpha);
		}
		if (!(m_examine_alpha < 1f))
		{
			SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			Vector2 vector = m_font.MeasureString("BACK");
			Vector2 zero = Vector2.Zero;
			zero.X = (float)Game.TS_AREA.Right - vector.X;
			zero.Y = (float)Game.TS_AREA.Bottom - vector.Y;
			SB.DrawString(m_font, "BACK", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "BACK", zero, m_color * m_alpha);
			zero.X -= m_b_button.Width + 10;
			SB.Draw(m_b_button, zero, m_color * m_alpha);
			if (!m_items[m_current_item].m_use_scrolling)
			{
				vector = m_font.MeasureString("ROTATE");
				zero = Vector2.Zero;
				zero.X = Game.TS_AREA.Left;
				zero.Y = Game.TS_AREA.Bottom - m_LS.Height + 7;
				SB.Draw(m_LS, zero, m_color * m_alpha);
				zero.X += m_LS.Width + 10;
				zero.Y = (float)Game.TS_AREA.Bottom - vector.Y;
				SB.DrawString(m_font, "ROTATE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, "ROTATE", zero, m_color * m_alpha);
			}
			else
			{
				Vector2 vector2 = m_font.MeasureString("SCROLL");
				Vector2 zero2 = Vector2.Zero;
				zero2.X = Game.TS_AREA.Left;
				zero2.Y = Game.TS_AREA.Bottom - m_LS.Height + 7;
				SB.Draw(m_LS, zero2, m_color);
				zero2.X += m_LS.Width + 10;
				zero2.Y = (float)Game.TS_AREA.Bottom - vector2.Y;
				SB.DrawString(m_font, "SCROLL", new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
				SB.DrawString(m_font, "SCROLL", zero2, m_color * m_alpha);
			}
			vector = m_font2.MeasureString(m_items[m_current_item].m_desc);
			zero = Vector2.Zero;
			zero.X = ((float)Game.VIEW_RECT.Width - vector.X) / 2f;
			zero.Y = (float)Game.TS_AREA.Bottom - vector.Y - 60f;
			SB.DrawString(m_font2, m_items[m_current_item].m_desc, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, m_items[m_current_item].m_desc, zero, m_color * m_alpha);
			SB.End();
			if (m_items[m_current_item] != null && m_items[m_current_item].m_examine_use_text != "")
			{
				vector = m_font.MeasureString(m_items[m_current_item].m_examine_use_text);
				zero.X = ((float)Game.VIEW_RECT.Width - vector.X - (float)m_a_button.Width - 10f) / 2f;
				zero.Y = Game.TS_AREA.Bottom - m_a_button.Height;
				SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				SB.Draw(m_a_button, zero, Color.White);
				SB.End();
				zero.X += m_a_button.Width + 10;
				m_game.m_hud.DrawText(SB, m_items[m_current_item].m_examine_use_text, zero);
			}
		}
	}

	public virtual void Draw(SpriteBatch SB)
	{
		if (m_state != INVENTORY_STATE.DISABLED && SB != null)
		{
			switch (m_state)
			{
			case INVENTORY_STATE.DEFAULT:
			case INVENTORY_STATE.COMBINE:
				DrawDefault(SB);
				break;
			case INVENTORY_STATE.ASK_PICKUP:
				DrawAskPickup(SB);
				break;
			case INVENTORY_STATE.EXAMINE:
				DrawExamine(SB);
				break;
			}
		}
	}

	public void onGameMenu()
	{
		if (m_state == INVENTORY_STATE.EXAMINE && m_items[m_current_item] != null)
		{
			m_items[m_current_item].onExamineGameMenu();
		}
	}

	public void onGameMenuClosed()
	{
		if (m_state == INVENTORY_STATE.EXAMINE && m_items[m_current_item] != null)
		{
			m_items[m_current_item].onExamineGameMenuClosed();
		}
	}
}
