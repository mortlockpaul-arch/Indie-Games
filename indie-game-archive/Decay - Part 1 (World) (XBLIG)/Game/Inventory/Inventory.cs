using System;
using System.Collections.Generic;
using Game.Inventory.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SGSCore;

namespace Game.Inventory;

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

	private enum INVENTORY_FADE_STATE
	{
		FADE_IN,
		FADE_OUT,
		FADE_IN_EXAMINE,
		FADE_OUT_EXAMINE,
		IDLE
	}

	public INVENTORY_STATE m_state;

	private INVENTORY_FADE_STATE m_fade_state;

	private Game m_game;

	private Texture2D m_fade;

	private Texture2D m_bag;

	private Texture2D m_red_stripe;

	private Texture2D m_rost;

	private Color m_color;

	private float m_alpha;

	private Color m_pickup_color;

	private float m_pickup_alpha;

	private Arrow m_left_arrow;

	private Arrow m_right_arrow;

	private List<Slot> m_slots;

	private Texture2D m_slot_medium;

	private Texture2D m_slot_large;

	private Texture2D m_slot_red;

	private Texture2D m_slot_green;

	private List<Item> m_preloaded_items;

	private List<Item> m_items;

	private int m_current_item;

	private int m_combine_item;

	private bool m_left_pressed;

	private bool m_right_pressed;

	private Item m_pickup_item;

	private SpriteFont m_font;

	private SpriteFont m_font2;

	public Texture2D m_a_button;

	public Texture2D m_b_button;

	public Texture2D m_x_button;

	public Texture2D m_y_button;

	public Texture2D m_LS;

	private float m_examine_alpha;

	private SGSModel m_examine_model;

	private Matrix m_examine_start_rot;

	private Vector2 m_model_rotation;

	private string m_use_event;

	private string m_content_path;

	private ContentManager m_examine_content;

	private SGSCamera m_camera;

	private RenderTarget2D m_RT;

	private SoundEffect m_beep;

	private SoundEffect m_beep_error;

	public float m_scroll_y;

	private float m_min_scroll_y;

	private float m_max_scroll_y;

	public bool m_rotation_input;

	public Inventory(Game game)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Expected O, but got Unknown
		m_fade_state = INVENTORY_FADE_STATE.IDLE;
		m_color = Color.White;
		m_pickup_color = Color.White;
		m_slots = new List<Slot>();
		m_preloaded_items = new List<Item>();
		m_items = new List<Item>();
		m_examine_start_rot = Matrix.Identity;
		m_model_rotation = Vector2.Zero;
		m_use_event = "";
		m_content_path = "";
		m_rotation_input = true;
		base._002Ector();
		m_game = game;
		m_examine_content = new ContentManager((IServiceProvider)((Game)m_game).Services);
		m_examine_content.RootDirectory = "Content/";
		m_content_path = "Inventory/";
		m_font = ((Game)m_game).Content.Load<SpriteFont>(m_content_path + "../Fonts/SpriteFont1");
		m_font2 = ((Game)m_game).Content.Load<SpriteFont>(m_content_path + "../Fonts/SpriteFont2");
		m_fade = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "svart_ruta");
		m_bag = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "bag");
		m_red_stripe = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "red_stripe");
		m_rost = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "rost");
		m_a_button = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "../HUD/a_button");
		m_b_button = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "../HUD/b_button");
		m_x_button = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "../HUD/x_button");
		m_y_button = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "../HUD/y_button");
		m_LS = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "../HUD/LS");
		((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
		m_slot_medium = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "ruta_medium");
		m_slot_large = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "ruta_stor");
		m_slot_red = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "ruta_combine_red");
		m_slot_green = ((Game)m_game).Content.Load<Texture2D>(m_content_path + "ruta_combine_green");
		Slot item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2(395f - (float)m_slot_medium.Width * 1.75f - (float)m_slot_medium.Width * 1.5f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2(395f - (float)m_slot_medium.Width * 1.75f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_large, Slot.SLOT_TYPE.LARGE)
		{
			m_pos = new Vector2(395f, 288f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2((float)(395 + m_slot_large.Width) + (float)m_slot_medium.Width * 0.75f, 317f)
		};
		m_slots.Add(item);
		item = new Slot(m_slot_medium, Slot.SLOT_TYPE.MEDIUM)
		{
			m_pos = new Vector2((float)(395 + m_slot_large.Width) + (float)m_slot_medium.Width * 2.25f, 317f)
		};
		m_slots.Add(item);
		m_left_arrow = new Arrow(((Game)m_game).Content.Load<Texture2D>(m_content_path + "arrow"), ((Game)m_game).Content.Load<Texture2D>(m_content_path + "arrow_green"), flip: true);
		m_left_arrow.m_pos = new Vector2(m_slots[0].m_pos.X - m_left_arrow.m_width - 30f, 340f);
		m_right_arrow = new Arrow(((Game)m_game).Content.Load<Texture2D>(m_content_path + "arrow"), ((Game)m_game).Content.Load<Texture2D>(m_content_path + "arrow_green"), flip: false);
		m_right_arrow.m_pos = new Vector2(m_slots[4].m_pos.X + (float)m_slot_medium.Width + 30f, 340f);
		m_preloaded_items.Add(new Key01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Pincett01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Polygrip01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Flashlight01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Frame01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Remote01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Battery01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Note01(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Note02(m_game, ((Game)m_game).Content));
		m_preloaded_items.Add(new Flashlight02(m_game, ((Game)m_game).Content));
		m_camera = new SGSCamera(((Game)m_game).GraphicsDevice, Vector3.Zero, 1f, 1000f);
		PresentationParameters presentationParameters = ((Game)m_game).GraphicsDevice.PresentationParameters;
		m_RT = new RenderTarget2D(((Game)m_game).GraphicsDevice, Game.VIEW_RECT.Width, Game.VIEW_RECT.Height, 1, (SurfaceFormat)1, presentationParameters.MultiSampleType, presentationParameters.MultiSampleQuality, (RenderTargetUsage)0);
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
		m_bag = null;
		m_red_stripe = null;
		m_rost = null;
		m_left_arrow.Clear();
		m_left_arrow = null;
		m_right_arrow.Clear();
		m_right_arrow = null;
		((GraphicsResource)m_slot_medium).Dispose();
		m_slot_medium = null;
		((GraphicsResource)m_slot_large).Dispose();
		m_slot_large = null;
		((GraphicsResource)m_slot_red).Dispose();
		m_slot_red = null;
		((GraphicsResource)m_slot_green).Dispose();
		m_slot_green = null;
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
			((GraphicsResource)m_a_button).Dispose();
			m_a_button = null;
		}
		if (m_b_button != null)
		{
			((GraphicsResource)m_b_button).Dispose();
			m_b_button = null;
		}
		if (m_x_button != null)
		{
			((GraphicsResource)m_x_button).Dispose();
			m_x_button = null;
		}
		if (m_y_button != null)
		{
			((GraphicsResource)m_y_button).Dispose();
			m_y_button = null;
		}
		if (m_LS != null)
		{
			((GraphicsResource)m_LS).Dispose();
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
			((RenderTarget)m_RT).Dispose();
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
		for (int i = 0; i < m_game.m_game_data.m_items.Count; i++)
		{
			AddItem(m_game.m_game_data.m_items[i]);
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

	public void AddItem(string id)
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
			m_alpha = 255f;
			m_fade_state = INVENTORY_FADE_STATE.FADE_OUT;
			((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
			((Color)(ref m_pickup_color)).A = ((Color)(ref m_color)).A;
			m_game.m_input_enabled = false;
		}
	}

	public void FadeIn()
	{
		m_alpha = 0f;
		m_fade_state = INVENTORY_FADE_STATE.FADE_IN;
		((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
		m_game.m_input_enabled = false;
		m_game.m_b_pressed = true;
	}

	public virtual void AskPickup(string item_id)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
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
		m_game.m_hud.FadeOut();
		m_game.m_y_pressed = true;
		m_pickup_color = Color.White;
		m_pickup_alpha = 0f;
		((Color)(ref m_pickup_color)).A = (byte)Math.Round(m_pickup_alpha);
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
		AddItem(id3);
		m_current_item = m_items.Count - 1;
		m_slots[2].m_bkg = m_slot_large;
		m_state = INVENTORY_STATE.DEFAULT;
	}

	public virtual void Update(TimeSpan elapsed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Invalid comparison between Unknown and I4
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Invalid comparison between Unknown and I4
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Invalid comparison between Unknown and I4
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Invalid comparison between Unknown and I4
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Invalid comparison between Unknown and I4
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Invalid comparison between Unknown and I4
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Invalid comparison between Unknown and I4
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Invalid comparison between Unknown and I4
		//IL_0f4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6b: Invalid comparison between Unknown and I4
		//IL_0c41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Invalid comparison between Unknown and I4
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1003: Unknown result type (might be due to invalid IL or missing references)
		//IL_1008: Unknown result type (might be due to invalid IL or missing references)
		//IL_100c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1011: Unknown result type (might be due to invalid IL or missing references)
		//IL_1015: Unknown result type (might be due to invalid IL or missing references)
		//IL_101b: Invalid comparison between Unknown and I4
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Invalid comparison between Unknown and I4
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d69: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Invalid comparison between Unknown and I4
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9d: Unknown result type (might be due to invalid IL or missing references)
		KeyboardState state = Keyboard.GetState();
		GamePadState state15;
		GamePadDPad dPad3;
		GamePadState state18;
		GamePadButtons buttons8;
		GamePadState state19;
		GamePadButtons buttons9;
		GamePadState state20;
		GamePadThumbSticks thumbSticks7;
		ref Vector2 model_rotation2;
		GamePadState state22;
		GamePadThumbSticks thumbSticks9;
		GamePadState state24;
		GamePadDPad dPad4;
		switch (m_state)
		{
		case INVENTORY_STATE.DISABLED:
		{
			GamePadState state17 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons7 = ((GamePadState)(ref state17)).Buttons;
			if ((int)((GamePadButtons)(ref buttons7)).Y == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)89))
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
					m_game.m_hud.FadeOut();
					m_state = INVENTORY_STATE.DEFAULT;
					if (m_game.m_tutorial_state == Game.TUTORIAL_STATE.INVENTORY)
					{
						m_game.m_tutorial_state = Game.TUTORIAL_STATE.NONE;
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
		}
		case INVENTORY_STATE.DEFAULT:
		{
			if (!m_game.m_input_enabled)
			{
				break;
			}
			GamePadState state9 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons3 = ((GamePadState)(ref state9)).Buttons;
			if ((int)((GamePadButtons)(ref buttons3)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					FadeOut();
					m_game.onCloseInventory();
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			GamePadState state10 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons4 = ((GamePadState)(ref state10)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				if (!m_game.m_a_pressed && m_game.m_active_trigger == null)
				{
					m_game.m_a_pressed = true;
					if (m_items.Count > 0)
					{
						FadeOut();
						m_game.onCloseInventory();
						m_use_event = m_items[m_current_item].m_id;
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			GamePadState state11 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons5 = ((GamePadState)(ref state11)).Buttons;
			if ((int)((GamePadButtons)(ref buttons5)).Y == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)89))
			{
				if (!m_game.m_y_pressed)
				{
					m_game.m_y_pressed = true;
					if (m_items.Count > 0)
					{
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
							m_items[m_current_item].Reset();
							m_game.m_inventory.m_rotation_input = true;
						}
						if (m_items[m_current_item].m_use_scrolling)
						{
							m_scroll_y = ((Rectangle)(ref Game.TS_AREA)).Top;
							m_min_scroll_y = m_items[m_current_item].m_min_scroll_y;
							m_max_scroll_y = ((Rectangle)(ref Game.TS_AREA)).Top;
						}
					}
				}
			}
			else
			{
				m_game.m_y_pressed = false;
			}
			GamePadState state12 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons6 = ((GamePadState)(ref state12)).Buttons;
			if ((int)((GamePadButtons)(ref buttons6)).X == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)88))
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
			GamePadState state13 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadDPad dPad2 = ((GamePadState)(ref state13)).DPad;
			if ((int)((GamePadDPad)(ref dPad2)).Right != 1)
			{
				GamePadState state14 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref state14)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks5)).Left.X > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
				{
					m_right_arrow.SetState(Arrow.ARROW_STATE.IDLE);
					m_right_pressed = false;
					goto IL_05dd;
				}
			}
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
			goto IL_05dd;
		}
		case INVENTORY_STATE.COMBINE:
		{
			if (m_game.m_input_enabled)
			{
				GamePadState state5 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadButtons buttons = ((GamePadState)(ref state5)).Buttons;
				if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
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
				GamePadState state6 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadButtons buttons2 = ((GamePadState)(ref state6)).Buttons;
				if ((int)((GamePadButtons)(ref buttons2)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
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
			GamePadState state7 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadDPad dPad = ((GamePadState)(ref state7)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Right != 1)
			{
				GamePadState state8 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state8)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks4)).Left.X > 0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
				{
					m_right_arrow.SetState(Arrow.ARROW_STATE.IDLE);
					m_right_pressed = false;
					goto IL_086e;
				}
			}
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
			goto IL_086e;
		}
		case INVENTORY_STATE.ASK_PICKUP:
		{
			if (m_pickup_item != null)
			{
				if (m_examine_model != null)
				{
					m_examine_model.Update(elapsed);
				}
				if (m_fade_state == INVENTORY_FADE_STATE.IDLE)
				{
					m_pickup_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
					if (m_pickup_alpha >= 255f)
					{
						m_pickup_alpha = 255f;
						m_game.m_input_enabled = true;
					}
					((Color)(ref m_pickup_color)).A = (byte)Math.Round(m_pickup_alpha);
				}
			}
			if (!m_game.m_input_enabled)
			{
				break;
			}
			GamePadState state26 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons10 = ((GamePadState)(ref state26)).Buttons;
			if ((int)((GamePadButtons)(ref buttons10)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					FadeOut();
					m_game.onCloseInventory();
				}
			}
			else
			{
				m_game.m_b_pressed = false;
			}
			GamePadState state27 = GamePad.GetState(Game.PLAYER_INDEX);
			GamePadButtons buttons11 = ((GamePadState)(ref state27)).Buttons;
			if ((int)((GamePadButtons)(ref buttons11)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
			{
				if (!m_game.m_a_pressed)
				{
					m_game.m_a_pressed = true;
					FadeOut();
					m_game.onCloseInventory();
					m_game.HandleEvent("Pickup" + m_pickup_item.m_id);
					if (m_game.m_tutorial_state == Game.TUTORIAL_STATE.WAIT_FOR_PICKUP)
					{
						m_game.m_tutorial_state = Game.TUTORIAL_STATE.INVENTORY;
						int tutorial_state2 = (int)m_game.m_tutorial_state;
						m_game.m_game_data.SetState("TutorialState", tutorial_state2.ToString());
					}
				}
			}
			else
			{
				m_game.m_a_pressed = false;
			}
			break;
		}
		case INVENTORY_STATE.EXAMINE:
			{
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
					m_examine_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
					if (m_examine_alpha >= 255f)
					{
						m_examine_alpha = 255f;
						m_fade_state = INVENTORY_FADE_STATE.IDLE;
						m_game.m_input_enabled = true;
					}
				}
				if (m_fade_state == INVENTORY_FADE_STATE.FADE_OUT_EXAMINE)
				{
					m_examine_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
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
					}
				}
				if (!m_game.m_input_enabled)
				{
					break;
				}
				if (m_rotation_input)
				{
					GamePadState state2 = GamePad.GetState(Game.PLAYER_INDEX);
					GamePadThumbSticks thumbSticks = ((GamePadState)(ref state2)).ThumbSticks;
					if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.X <= -0.1f))
					{
						GamePadState state3 = GamePad.GetState(Game.PLAYER_INDEX);
						GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state3)).ThumbSticks;
						if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.X >= 0.1f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37) && !((KeyboardState)(ref state)).IsKeyDown((Keys)39))
						{
							goto IL_0d52;
						}
					}
					ref Vector2 model_rotation = ref m_model_rotation;
					GamePadState state4 = GamePad.GetState(Game.PLAYER_INDEX);
					GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state4)).ThumbSticks;
					model_rotation.Y = ((GamePadThumbSticks)(ref thumbSticks3)).Left.X * (float)elapsed.TotalMilliseconds * 0.001f * 2f;
					if (((KeyboardState)(ref state)).IsKeyDown((Keys)37))
					{
						m_model_rotation.Y = (0f - (float)elapsed.TotalMilliseconds) * 0.001f;
					}
					if (((KeyboardState)(ref state)).IsKeyDown((Keys)39))
					{
						m_model_rotation.Y = (float)elapsed.TotalMilliseconds * 0.001f;
					}
					if (m_examine_model != null)
					{
						m_examine_model.RotateY(m_model_rotation.Y);
					}
					goto IL_0d52;
				}
				goto IL_0f4e;
			}
			IL_086e:
			state15 = GamePad.GetState(Game.PLAYER_INDEX);
			dPad3 = ((GamePadState)(ref state15)).DPad;
			if ((int)((GamePadDPad)(ref dPad3)).Left != 1)
			{
				GamePadState state16 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref state16)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks6)).Left.X < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
				{
					m_left_arrow.SetState(Arrow.ARROW_STATE.IDLE);
					m_left_pressed = false;
					break;
				}
			}
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
			break;
			IL_0f4e:
			state18 = GamePad.GetState(Game.PLAYER_INDEX);
			buttons8 = ((GamePadState)(ref state18)).Buttons;
			if ((int)((GamePadButtons)(ref buttons8)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
			{
				if (!m_game.m_b_pressed)
				{
					m_game.m_b_pressed = true;
					m_game.m_input_enabled = false;
					m_examine_alpha = 255f;
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
			state19 = GamePad.GetState(Game.PLAYER_INDEX);
			buttons9 = ((GamePadState)(ref state19)).Buttons;
			if ((int)((GamePadButtons)(ref buttons9)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
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
			IL_0d52:
			state20 = GamePad.GetState(Game.PLAYER_INDEX);
			thumbSticks7 = ((GamePadState)(ref state20)).ThumbSticks;
			if (!(((GamePadThumbSticks)(ref thumbSticks7)).Left.Y <= -0.1f))
			{
				GamePadState state21 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks8 = ((GamePadState)(ref state21)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks8)).Left.Y >= 0.1f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)38) && !((KeyboardState)(ref state)).IsKeyDown((Keys)40))
				{
					goto IL_0f4e;
				}
			}
			model_rotation2 = ref m_model_rotation;
			state22 = GamePad.GetState(Game.PLAYER_INDEX);
			thumbSticks9 = ((GamePadState)(ref state22)).ThumbSticks;
			model_rotation2.X = (0f - ((GamePadThumbSticks)(ref thumbSticks9)).Left.Y) * (float)elapsed.TotalMilliseconds * 0.001f * 2f;
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)38))
			{
				m_model_rotation.X = (0f - (float)elapsed.TotalMilliseconds) * 0.001f;
			}
			if (((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				m_model_rotation.X = (float)elapsed.TotalMilliseconds * 0.001f;
			}
			if (m_examine_model != null)
			{
				m_examine_model.RotateX(m_model_rotation.X);
			}
			if (m_items[m_current_item].m_use_scrolling)
			{
				float scroll_y = m_scroll_y;
				GamePadState state23 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks10 = ((GamePadState)(ref state23)).ThumbSticks;
				m_scroll_y = scroll_y + ((GamePadThumbSticks)(ref thumbSticks10)).Left.Y * (float)elapsed.TotalMilliseconds * 0.001f * 400f;
				if (((KeyboardState)(ref state)).IsKeyDown((Keys)38))
				{
					m_scroll_y += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
				}
				if (((KeyboardState)(ref state)).IsKeyDown((Keys)40))
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
			goto IL_0f4e;
			IL_05dd:
			state24 = GamePad.GetState(Game.PLAYER_INDEX);
			dPad4 = ((GamePadState)(ref state24)).DPad;
			if ((int)((GamePadDPad)(ref dPad4)).Left != 1)
			{
				GamePadState state25 = GamePad.GetState(Game.PLAYER_INDEX);
				GamePadThumbSticks thumbSticks11 = ((GamePadState)(ref state25)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks11)).Left.X < -0.2f) && !((KeyboardState)(ref state)).IsKeyDown((Keys)37))
				{
					m_left_arrow.SetState(Arrow.ARROW_STATE.IDLE);
					m_left_pressed = false;
					break;
				}
			}
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
			break;
		}
		switch (m_fade_state)
		{
		case INVENTORY_FADE_STATE.FADE_OUT:
			m_alpha -= (float)elapsed.TotalMilliseconds * 0.001f * 400f;
			if (m_alpha <= 0f)
			{
				m_alpha = 0f;
				m_fade_state = INVENTORY_FADE_STATE.IDLE;
				m_game.m_input_enabled = true;
				m_state = INVENTORY_STATE.DISABLED;
				m_pickup_item = null;
				if (m_use_event != "")
				{
					m_game.HandleUseEvent(m_use_event);
					m_use_event = "";
				}
			}
			((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
			((Color)(ref m_pickup_color)).A = ((Color)(ref m_color)).A;
			break;
		case INVENTORY_FADE_STATE.FADE_IN:
			m_alpha += (float)elapsed.TotalMilliseconds * 0.001f * 400f;
			if (m_alpha >= 255f)
			{
				m_alpha = 255f;
				m_fade_state = INVENTORY_FADE_STATE.IDLE;
				if (m_state != INVENTORY_STATE.ASK_PICKUP)
				{
					m_game.m_input_enabled = true;
				}
			}
			((Color)(ref m_color)).A = (byte)Math.Floor(m_alpha);
			break;
		}
	}

	protected virtual void DrawDefault(SpriteBatch SB)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color);
		SB.Draw(m_red_stripe, new Vector2(0f, (float)((Game.VIEW_RECT.Height - m_red_stripe.Height) / 2)), m_color);
		SB.Draw(m_bag, new Vector2((float)(Game.VIEW_RECT.Width - m_bag.Width + 60), 0f), m_color);
		SB.End();
		SB.Begin((SpriteBlendMode)1, (SpriteSortMode)2, (SaveStateMode)0);
		((Game)m_game).GraphicsDevice.RenderState.SourceBlend = (Blend)1;
		((Game)m_game).GraphicsDevice.RenderState.DestinationBlend = (Blend)3;
		SB.Draw(m_rost, Game.VIEW_RECT, new Color(((Color)(ref m_color)).R, ((Color)(ref m_color)).G, ((Color)(ref m_color)).B, (byte)(((Color)(ref m_color)).A / 4)));
		SB.End();
		Color gray = Color.Gray;
		((Color)(ref gray)).A = ((Color)(ref m_color)).A;
		Color color = default(Color);
		((Color)(ref color))._002Ector((byte)16, (byte)16, (byte)16, ((Color)(ref m_color)).A);
		int num = m_current_item - 2;
		m_left_arrow.Draw(SB, m_color);
		m_right_arrow.Draw(SB, m_color);
		for (int i = 0; i < m_slots.Count; i++)
		{
			if (num >= 0 && num < m_items.Count)
			{
				m_slots[i].SetItem(m_items[num]);
				if (i != 2)
				{
					m_slots[i].Draw(SB, gray);
				}
			}
			else
			{
				m_slots[i].SetItem(null);
				m_slots[i].Draw(SB, color);
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
		SB.Begin((SpriteBlendMode)1);
		Vector2 zero = Vector2.Zero;
		string text = "COLLECTED ITEMS";
		zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - m_font2.MeasureString(text).X / 2f);
		zero.Y = 160f;
		SB.DrawString(m_font2, text, zero, m_color);
		SB.End();
		if (m_alpha < 255f || m_items.Count <= 0)
		{
			return;
		}
		if (m_state == INVENTORY_STATE.DEFAULT)
		{
			Vector2 val = m_font.MeasureString("CLOSE");
			zero = Vector2.Zero;
			zero.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - val.X;
			zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y;
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font, "CLOSE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "CLOSE", zero, m_color);
			zero.X -= (float)(m_b_button.Width + 10);
			SB.Draw(m_b_button, zero, m_color);
			zero = Vector2.Zero;
			Vector2 val2 = m_font.MeasureString("USE");
			Vector2 val3 = m_font.MeasureString("EXAMINE");
			Vector2 val4 = m_font.MeasureString("COMBINE");
			float num2 = 10f;
			float num3 = 30f;
			float num4 = (float)m_a_button.Width + num2 + (float)(int)val2.X + num3 + (float)m_y_button.Width + num2 + (float)(int)val3.X + num3 + (float)m_x_button.Width + num2 + (float)(int)val4.X;
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - m_font2.MeasureString(m_items[m_current_item].m_name).X / 2f);
			zero.Y = 530f;
			SB.DrawString(m_font2, m_items[m_current_item].m_name, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, zero, m_color);
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - num4 / 2f);
			zero.Y += val4.Y + num3;
			if (zero.Y > (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val4.Y)
			{
				zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val4.Y;
			}
			Color color2 = m_color;
			if (m_game.m_active_trigger != null)
			{
				((Color)(ref color2)).A = 92;
			}
			SB.Draw(m_a_button, zero, color2);
			zero.X += (float)m_a_button.Width + num2;
			SB.DrawString(m_font, "USE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "USE", zero, color2);
			zero.X += (float)(int)val2.X + num3;
			SB.Draw(m_y_button, zero, m_color);
			zero.X += (float)m_y_button.Width + num2;
			SB.DrawString(m_font, "EXAMINE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "EXAMINE", zero, m_color);
			zero.X += (float)(int)val3.X + num3;
			SB.Draw(m_x_button, zero, m_color);
			zero.X += (float)m_x_button.Width + num2;
			SB.DrawString(m_font, "COMBINE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "COMBINE", zero, m_color);
			SB.End();
		}
		else if (m_state == INVENTORY_STATE.COMBINE)
		{
			zero = Vector2.Zero;
			string text2 = "COMBINE";
			string text3 = "CANCEL";
			Vector2 val5 = m_font.MeasureString(text2);
			Vector2 val6 = m_font.MeasureString(text3);
			float num5 = 10f;
			float num6 = 30f;
			float num7 = (float)m_a_button.Width + num5 + (float)(int)val5.X + num6 + (float)m_x_button.Width + num5 + (float)(int)val6.X;
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - m_font2.MeasureString(m_items[m_current_item].m_name).X / 2f);
			zero.Y = 530f;
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, m_items[m_current_item].m_name, zero, m_color);
			zero.X = (int)(m_slots[2].m_pos.X + (float)(m_slots[2].m_bkg.Width / 2) - num7 / 2f);
			zero.Y += val5.Y + num6;
			if (zero.Y > (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val5.Y)
			{
				zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val5.Y;
			}
			SB.Draw(m_a_button, zero, m_color);
			zero.X += (float)m_a_button.Width + num5;
			SB.DrawString(m_font, text2, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text2, zero, m_color);
			zero.X += (float)(int)val5.X + num6;
			SB.Draw(m_b_button, zero, m_color);
			zero.X += (float)m_y_button.Width + num5;
			SB.DrawString(m_font, text3, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, text3, zero, m_color);
			SB.End();
		}
	}

	protected virtual void DrawAskPickup(SpriteBatch SB)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
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
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color);
		SB.Draw(m_red_stripe, Game.VIEW_RECT, m_color);
		SB.End();
		SB.Begin((SpriteBlendMode)1, (SpriteSortMode)2, (SaveStateMode)0);
		((Game)m_game).GraphicsDevice.RenderState.SourceBlend = (Blend)1;
		((Game)m_game).GraphicsDevice.RenderState.DestinationBlend = (Blend)3;
		SB.Draw(m_rost, Game.VIEW_RECT, new Color(((Color)(ref m_color)).R, ((Color)(ref m_color)).G, ((Color)(ref m_color)).B, (byte)(((Color)(ref m_color)).A / 4)));
		SB.End();
		if (m_pickup_alpha > 0f)
		{
			if (m_examine_model != null)
			{
				m_pickup_item.DrawExamineModel(SB, m_RT, m_pickup_color);
			}
			else
			{
				m_pickup_item.DrawAskPickupImage(SB, m_pickup_color);
			}
		}
		if (((Color)(ref m_pickup_color)).A >= byte.MaxValue)
		{
			Vector2 zero = Vector2.Zero;
			Vector2 val = m_font.MeasureString("PICK UP");
			Vector2 val2 = m_font.MeasureString("CANCEL");
			Vector2 val3 = m_font2.MeasureString(m_pickup_item.m_name);
			float num = 10f;
			float num2 = 40f;
			float num3 = (float)m_a_button.Width + num + (float)(int)val.X + num2 + (float)m_b_button.Width + num + (float)(int)val2.X;
			zero.X = (int)((float)Game.VIEW_RECT.Width - val3.X) / 2;
			zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y - 20f - val3.Y;
			SB.Begin((SpriteBlendMode)1);
			SB.DrawString(m_font2, m_pickup_item.m_name, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font2, m_pickup_item.m_name, zero, m_color);
			zero.X = (int)((float)Game.VIEW_RECT.Width - num3) / 2;
			zero.Y += val3.Y + 20f;
			SB.Draw(m_a_button, zero, m_color);
			zero.X += (float)m_a_button.Width + num;
			SB.DrawString(m_font, "PICK UP", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "PICK UP", zero, m_color);
			zero.X += (float)(int)val.X + num2;
			SB.Draw(m_b_button, zero, m_color);
			zero.X += (float)m_b_button.Width + num;
			SB.DrawString(m_font, "CANCEL", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			SB.DrawString(m_font, "CANCEL", zero, m_color);
			SB.End();
		}
	}

	protected virtual void DrawExamine(SpriteBatch SB)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		m_items[m_current_item].RenderExamineModel(SB.GraphicsDevice, m_RT, m_camera);
		if (m_game.m_active_trigger != null)
		{
			m_game.m_active_trigger.Draw(SB);
		}
		else if (m_game.m_world != null && m_game.m_world.GetCurrentView() != null)
		{
			m_game.m_world.GetCurrentView().Draw(SB);
		}
		SB.Begin((SpriteBlendMode)1);
		SB.Draw(m_fade, Game.VIEW_RECT, m_color);
		SB.Draw(m_red_stripe, Game.VIEW_RECT, m_color);
		SB.End();
		SB.Begin((SpriteBlendMode)1, (SpriteSortMode)2, (SaveStateMode)0);
		((Game)m_game).GraphicsDevice.RenderState.SourceBlend = (Blend)1;
		((Game)m_game).GraphicsDevice.RenderState.DestinationBlend = (Blend)3;
		Color color = m_color;
		((Color)(ref color)).A = (byte)(((Color)(ref m_color)).A / 4);
		SB.Draw(m_rost, Game.VIEW_RECT, color);
		SB.End();
		if (m_examine_model != null)
		{
			m_items[m_current_item].DrawExamineModel(SB, m_RT, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)m_examine_alpha));
		}
		else
		{
			m_items[m_current_item].DrawExamineImage(SB, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)m_examine_alpha));
		}
		if (m_examine_alpha < 255f)
		{
			return;
		}
		SB.Begin((SpriteBlendMode)1);
		Vector2 val = m_font.MeasureString("BACK");
		Vector2 zero = Vector2.Zero;
		zero.X = (float)((Rectangle)(ref Game.TS_AREA)).Right - val.X;
		zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y;
		SB.DrawString(m_font, "BACK", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font, "BACK", zero, m_color);
		zero.X -= (float)(m_b_button.Width + 10);
		SB.Draw(m_b_button, zero, m_color);
		if (!m_items[m_current_item].m_use_scrolling)
		{
			val = m_font.MeasureString("ROTATE");
			zero = Vector2.Zero;
			zero.X = ((Rectangle)(ref Game.TS_AREA)).Left;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_LS.Height + 7;
			if (m_examine_model != null)
			{
				SB.Draw(m_LS, zero, m_color);
			}
			zero.X += (float)(m_LS.Width + 10);
			zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y;
			if (m_examine_model != null)
			{
				SB.DrawString(m_font, "ROTATE", new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
				SB.DrawString(m_font, "ROTATE", zero, m_color);
			}
		}
		else
		{
			Vector2 val2 = m_font.MeasureString("SCROLL");
			Vector2 zero2 = Vector2.Zero;
			zero2.X = ((Rectangle)(ref Game.TS_AREA)).Left;
			zero2.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_LS.Height + 7;
			SB.Draw(m_LS, zero2, m_color);
			zero2.X += (float)(m_LS.Width + 10);
			zero2.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val2.Y;
			SB.DrawString(m_font, "SCROLL", new Vector2(zero2.X + 1f, zero2.Y + 2f), Color.Black);
			SB.DrawString(m_font, "SCROLL", zero2, m_color);
		}
		val = m_font2.MeasureString(m_items[m_current_item].m_desc);
		zero = Vector2.Zero;
		zero.X = ((float)Game.VIEW_RECT.Width - val.X) / 2f;
		zero.Y = (float)((Rectangle)(ref Game.TS_AREA)).Bottom - val.Y - 60f;
		SB.DrawString(m_font2, m_items[m_current_item].m_desc, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
		SB.DrawString(m_font2, m_items[m_current_item].m_desc, zero, m_color);
		SB.End();
		if (m_items[m_current_item] != null && m_items[m_current_item].m_examine_use_text != "")
		{
			val = m_font.MeasureString(m_items[m_current_item].m_examine_use_text);
			zero.X = ((float)Game.VIEW_RECT.Width - val.X - (float)m_a_button.Width - 10f) / 2f;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_a_button.Height;
			SB.Begin((SpriteBlendMode)1);
			SB.Draw(m_a_button, zero, Color.White);
			SB.End();
			zero.X += (float)(m_a_button.Width + 10);
			m_game.m_hud.DrawText(SB, m_items[m_current_item].m_examine_use_text, zero);
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
