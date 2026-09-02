using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Game.Inventory;
using Game.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using SGSCore;

namespace Game;

public class Game : Game
{
	public enum GAME_STATE
	{
		SELECT_CONTROLLER,
		START_MENU,
		ASK_TUTORIAL,
		TUTORIAL,
		SCENE,
		ACTIVE_TRIGGER,
		INVENTORY,
		SHOW_TEXT,
		SHOW_ASK,
		FADE_IN_AREA,
		FADE_OUT_AREA,
		LOADING_AREA,
		START_LOADING_GAME,
		LOADING_GAME,
		START_LOADING_INTRO,
		LOADING_INTRO,
		SHOW_INTRO,
		CHECK_TRIALMODE
	}

	public enum TUTORIAL_STATE
	{
		MOVE_CURSOR,
		DECREASE_SPEED,
		INCREASE_SPEED,
		CHANGE_VIEW,
		CHANGING_VIEW,
		USE,
		WAIT_FOR_PICKUP,
		INVENTORY,
		NONE
	}

	protected enum MUSIC_STATE
	{
		PLAYING,
		STOPPED,
		PAUSED,
		FADE_IN,
		FADE_OUT
	}

	public static string STORAGE_LOCATION;

	public static string STORAGE_SETTINGS_FILE;

	public static string STORAGE_SAVE_FILE;

	public static Game INST;

	public static Rectangle VIEW_RECT;

	public static Rectangle TS_AREA;

	public static float MUSIC_VOL_DEC_MULTI;

	public static PlayerIndex PLAYER_INDEX;

	public TUTORIAL_STATE m_tutorial_state;

	private GraphicsDeviceManager m_GDM;

	public SpriteBatch m_SB;

	public GAME_STATE m_state;

	public SGSContentLoader m_CL;

	public Intro m_intro;

	public bool m_a_pressed;

	public bool m_b_pressed;

	public bool m_x_pressed;

	public bool m_y_pressed;

	public bool m_left_pressed;

	public bool m_right_pressed;

	public bool m_up_pressed;

	public bool m_down_pressed;

	public bool m_d_down_pressed;

	public bool m_d_up_pressed;

	public bool m_d_left_pressed;

	public bool m_d_right_pressed;

	public Cursor m_cursor;

	public bool m_show_cursor;

	public bool m_input_enabled;

	public bool m_update_cursor;

	public bool m_inventory_enabled;

	public Trigger m_over_trigger;

	public Trigger m_active_trigger;

	public HUD m_hud;

	public global::Game.Inventory.Inventory m_inventory;

	private Animation2D m_noise_effect;

	private Color m_noise_color;

	public Texture2D m_fade_texture;

	private float m_fade_alpha;

	public Effect m_shader;

	protected object m_SD_state;

	protected StorageDevice m_SD;

	public GameSettings m_game_settings;

	public GameData m_game_data;

	private bool m_load_game_data;

	private bool m_save_game_data;

	private bool m_save_settings;

	public bool m_game_data_found;

	public global::Game.World.World m_world;

	private string m_next_area;

	private string m_next_view;

	public Loading m_loading;

	protected bool m_new_game;

	public StartMenu m_start_menu;

	public GameMenu m_game_menu;

	public bool m_show_game_menu;

	public bool m_game_menu_enabled;

	public Song m_music;

	public Song m_music1;

	public Song m_music2;

	public Song m_music3;

	protected MUSIC_STATE m_music_state;

	protected string m_current_music;

	public bool m_play_door_sound;

	public SoundEffect m_door_open;

	public bool m_freeze;

	protected Random m_rand;

	private Texture2D m_start_bkg;

	private SpriteFont m_font;

	private bool m_show_not_signed_in;

	private bool m_show_SD_not_connected;

	private bool m_show_storage_failed;

	public bool m_volume_changed;

	private bool m_in_pause;

	public Game()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		m_show_cursor = true;
		m_input_enabled = true;
		m_update_cursor = true;
		m_inventory_enabled = true;
		m_noise_color = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)92);
		m_game_settings = new GameSettings();
		m_next_area = "";
		m_next_view = "";
		m_new_game = true;
		m_game_menu_enabled = true;
		m_music_state = MUSIC_STATE.STOPPED;
		m_current_music = "";
		m_play_door_sound = true;
		((Game)this)._002Ector();
		INST = null;
		INST = this;
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		m_GDM = new GraphicsDeviceManager((Game)(object)this);
		m_GDM.PreferredBackBufferWidth = 1280;
		m_GDM.PreferredBackBufferHeight = 720;
		m_GDM.PreferMultiSampling = true;
		m_GDM.PreferredDepthStencilFormat = (DepthFormat)48;
		((Game)this).Content.RootDirectory = "Content/";
		m_rand = new Random(DateTime.Now.Millisecond);
	}

	protected override void Initialize()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		PresentationParameters presentationParameters = ((Game)this).GraphicsDevice.PresentationParameters;
		presentationParameters.MultiSampleType = (MultiSampleType)4;
		((Game)this).GraphicsDevice.Reset(presentationParameters);
		Viewport viewport = ((Game)this).GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((Game)this).GraphicsDevice.Viewport;
		VIEW_RECT = new Rectangle(0, 0, width, ((Viewport)(ref viewport2)).Height);
		((Game)this).Initialize();
	}

	protected override void BeginRun()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		DisplayMode displayMode = ((Game)this).GraphicsDevice.DisplayMode;
		double num = (double)VIEW_RECT.Width / (double)((DisplayMode)(ref displayMode)).Width;
		double num2 = (double)VIEW_RECT.Height / (double)((DisplayMode)(ref displayMode)).Height;
		TS_AREA = new Rectangle((int)Math.Round((double)((DisplayMode)(ref displayMode)).TitleSafeArea.X * num), (int)Math.Round((double)((DisplayMode)(ref displayMode)).TitleSafeArea.Y * num2), (int)Math.Round((double)((DisplayMode)(ref displayMode)).TitleSafeArea.Width * num), (int)Math.Round((double)((DisplayMode)(ref displayMode)).TitleSafeArea.Height * num2));
		if (TS_AREA.X == 0)
		{
			TS_AREA.X += (int)((float)VIEW_RECT.Width * 0.1f);
			TS_AREA.Width -= (int)((float)VIEW_RECT.Width * 0.2f);
		}
		if (TS_AREA.Y == 0)
		{
			TS_AREA.Y += (int)((float)VIEW_RECT.Height * 0.1f);
			TS_AREA.Height -= (int)((float)VIEW_RECT.Height * 0.2f);
		}
		((Game)this).BeginRun();
	}

	public int GetRandom(int min, int max)
	{
		if (m_rand == null)
		{
			return -1;
		}
		return m_rand.Next(min, max + 1);
	}

	public void onIntroFinished()
	{
		m_state = GAME_STATE.START_LOADING_GAME;
	}

	public void onCreditsClosed()
	{
		if (m_start_menu != null)
		{
			m_start_menu.m_state = StartMenu.STARTMENU_STATE.MAIN;
		}
	}

	public void onNewGame()
	{
		m_new_game = true;
		m_tutorial_state = TUTORIAL_STATE.MOVE_CURSOR;
		m_state = GAME_STATE.START_LOADING_INTRO;
	}

	public void onContinueGame()
	{
		if (!CheckSignedIn())
		{
			if (m_start_menu != null)
			{
				m_start_menu.m_selection = StartMenu.STARTMENU_SELECTION.NEW_GAME;
			}
		}
		else if (m_SD == null || !m_SD.IsConnected)
		{
			if (m_start_menu != null)
			{
				m_start_menu.m_selection = StartMenu.STARTMENU_SELECTION.NEW_GAME;
			}
			m_SD = null;
			m_game_data_found = false;
			m_show_SD_not_connected = true;
		}
		else
		{
			m_new_game = false;
			m_state = GAME_STATE.START_LOADING_GAME;
		}
	}

	public void onOptionsClosed()
	{
		if (m_state == GAME_STATE.START_MENU)
		{
			if (m_start_menu != null)
			{
				m_start_menu.m_state = StartMenu.STARTMENU_STATE.MAIN;
			}
		}
		else
		{
			m_game_menu.m_state = GameMenu.GAMEMENU_STATE.MENU;
		}
	}

	public void onExtrasClosed()
	{
		if (m_start_menu != null)
		{
			m_start_menu.m_state = StartMenu.STARTMENU_STATE.MAIN;
		}
	}

	public void onExitGame()
	{
		Clear();
		m_CL = new SGSContentLoader((IServiceProvider)((Game)this).Services);
		m_show_game_menu = false;
		m_start_menu = new StartMenu(this);
		m_state = GAME_STATE.START_MENU;
		m_input_enabled = true;
		m_inventory_enabled = true;
		m_update_cursor = true;
		m_game_menu_enabled = true;
	}

	protected void LoadInterface()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		m_fade_texture = ((Game)this).Content.Load<Texture2D>("HUD/black");
		m_noise_effect = new TextureAnimation(this, ((Game)this).Content, "Effects/Noise/", 5u, reverse: false);
		m_noise_effect.SetFPS(15.0);
		m_noise_effect.m_random_mode = true;
		m_noise_effect.Play(Animation2D.LOOP_TYPE.CYCLE);
		m_cursor = new Cursor(this, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
		m_hud = new HUD(this);
		m_inventory = new global::Game.Inventory.Inventory(this);
		m_game_menu = new GameMenu(this);
	}

	protected void LoadIntro()
	{
		if (m_intro != null)
		{
			m_intro.Clear();
			m_intro = null;
		}
		m_intro = new Intro(this, m_CL);
	}

	protected void LoadWorld()
	{
		if (m_world != null)
		{
			m_world.Clear();
			m_world = null;
		}
		m_music1 = ((Game)this).Content.Load<Song>("Music/ambient");
		m_music2 = ((Game)this).Content.Load<Song>("Music/right_room");
		m_door_open = m_CL.LoadSound("Sound/door_open");
		m_world = new global::Game.World.World(this);
		if (m_game_data.GetState("TutorialState") != "")
		{
			m_tutorial_state = (TUTORIAL_STATE)int.Parse(m_game_data.GetState("TutorialState"));
			if (m_tutorial_state == TUTORIAL_STATE.CHANGING_VIEW)
			{
				m_tutorial_state = TUTORIAL_STATE.CHANGE_VIEW;
			}
		}
		if (m_game_data.m_area != "")
		{
			m_world.ChangeArea(m_game_data.m_area, m_game_data.m_view, fade_in: true);
		}
		else
		{
			m_world.ChangeArea("Room1", "View1", fade_in: true);
		}
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		m_SB = new SpriteBatch(((Game)this).GraphicsDevice);
		m_CL = new SGSContentLoader((IServiceProvider)((Game)this).Services);
		m_start_bkg = m_CL.LoadTexture("StartMenu/bkg");
		m_font = m_CL.LoadFont("Fonts/SpriteFont2");
		m_state = GAME_STATE.SELECT_CONTROLLER;
	}

	protected override void UnloadContent()
	{
		Clear();
		m_rand = null;
		m_SD_state = null;
		m_SD = null;
		if (m_game_data != null)
		{
			m_game_data.Clear();
			m_game_data = null;
		}
		if (m_game_settings != null)
		{
			m_game_settings.Clear();
			m_game_settings = null;
		}
		m_SB = null;
		m_GDM = null;
	}

	public void Clear()
	{
		m_start_bkg = null;
		m_font = null;
		m_door_open = null;
		m_shader = null;
		m_over_trigger = null;
		m_active_trigger = null;
		if (m_game_menu != null)
		{
			m_game_menu.Clear();
			m_game_menu = null;
		}
		if (m_loading != null)
		{
			m_loading.Clear();
			m_loading = null;
		}
		if (m_start_menu != null)
		{
			m_start_menu.Clear();
			m_start_menu = null;
		}
		if (m_world != null)
		{
			m_world.Clear();
			m_world = null;
		}
		if (m_cursor != null)
		{
			m_cursor.Clear();
			m_cursor = null;
		}
		if (m_hud != null)
		{
			m_hud.Clear();
			m_hud = null;
		}
		if (m_inventory != null)
		{
			m_inventory.Clear();
			m_inventory = null;
		}
		if (m_fade_texture != null)
		{
			((GraphicsResource)m_fade_texture).Dispose();
			m_fade_texture = null;
		}
		if (m_noise_effect != null)
		{
			m_noise_effect.Clear();
			m_noise_effect = null;
		}
		m_music = null;
		if (m_music1 != (Song)null)
		{
			m_music1.Dispose();
			m_music1 = null;
		}
		if (m_music2 != (Song)null)
		{
			m_music2.Dispose();
			m_music2 = null;
		}
		if (m_music3 != (Song)null)
		{
			m_music3.Dispose();
			m_music3 = null;
		}
		if (m_CL != null)
		{
			m_CL.Clear();
			m_CL = null;
		}
		((Game)this).Content.Unload();
	}

	protected virtual void SelectStorageDevice()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			m_freeze = true;
			m_SD_state = "GetDevice for Player";
			Guide.BeginShowStorageDeviceSelector(PLAYER_INDEX, (AsyncCallback)StorageDeviceSelected, m_SD_state);
		}
		catch
		{
		}
	}

	protected void onMessageFinished(IAsyncResult res)
	{
		try
		{
			Guide.EndShowMessageBox(res);
		}
		catch
		{
		}
	}

	protected bool CheckSignedIn()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			for (int i = 0; i < ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count; i++)
			{
				if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i].PlayerIndex == PLAYER_INDEX)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		m_SD = null;
		m_game_data_found = false;
		m_show_not_signed_in = true;
		return false;
	}

	protected virtual void StorageDeviceSelected(IAsyncResult res)
	{
		StorageContainer val = null;
		try
		{
			m_SD = Guide.EndShowStorageDeviceSelector(res);
			if (!CheckSignedIn())
			{
				if (m_game_data == null)
				{
					m_game_data = new GameData();
				}
				m_freeze = false;
				return;
			}
			if (m_SD != null && m_SD.IsConnected)
			{
				val = m_SD.OpenContainer(STORAGE_LOCATION);
				string path = Path.Combine(val.Path, STORAGE_SAVE_FILE);
				if (File.Exists(path))
				{
					m_game_data_found = true;
				}
				else
				{
					m_game_data_found = false;
				}
				path = Path.Combine(val.Path, STORAGE_SETTINGS_FILE);
				if (File.Exists(path))
				{
					val.Dispose();
					val = null;
					m_game_settings = GameSettings.Load(m_SD);
					if (m_game_settings == null)
					{
						m_game_settings = new GameSettings();
						throw new NullReferenceException();
					}
					if (m_start_menu != null && m_start_menu.m_options_menu != null)
					{
						m_start_menu.m_options_menu.SetGamma(m_game_settings.m_brightness);
					}
				}
				else
				{
					val.Dispose();
					val = null;
					m_game_settings = new GameSettings();
				}
			}
			if (m_load_game_data)
			{
				if (m_SD != null)
				{
					LoadGameData();
				}
				else
				{
					m_state = GAME_STATE.START_MENU;
				}
				m_a_pressed = true;
				m_load_game_data = false;
				m_freeze = false;
				return;
			}
			if (m_save_game_data)
			{
				if (m_SD != null)
				{
					SaveGameData();
				}
				m_save_game_data = false;
			}
			if (m_save_settings)
			{
				if (m_SD != null)
				{
					SaveSettings();
				}
				m_save_settings = false;
			}
			if (m_game_data_found && !m_SD.IsConnected)
			{
				m_game_data_found = false;
				if (m_game_data == null)
				{
					m_game_data = new GameData();
				}
				if (m_game_settings == null)
				{
					m_game_settings = new GameSettings();
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Game.StorageDeviceSelected: " + ex.Message);
			if (m_load_game_data)
			{
				m_load_game_data = false;
				if (m_game_settings != null)
				{
					m_game_settings.Clear();
					m_game_settings = null;
				}
				if (m_game_data != null)
				{
					m_game_data.Clear();
					m_game_data = null;
				}
			}
			m_game_settings = new GameSettings();
			m_game_data = new GameData();
			m_save_game_data = false;
			m_game_data_found = false;
			if (val != null)
			{
				val.Dispose();
				val = null;
			}
			m_show_storage_failed = true;
		}
		m_freeze = false;
	}

	public virtual void SaveSettings()
	{
		try
		{
			if (CheckSignedIn())
			{
				if (m_SD == null || !m_SD.IsConnected)
				{
					m_save_settings = true;
					SelectStorageDevice();
				}
				else if (!GameSettings.Save(m_game_settings, m_SD))
				{
					m_show_storage_failed = true;
					m_SD = null;
					m_game_data_found = false;
				}
			}
		}
		catch
		{
		}
	}

	public virtual void SaveGameData()
	{
		try
		{
			if (!CheckSignedIn())
			{
				return;
			}
			if (m_SD == null || !m_SD.IsConnected)
			{
				m_save_game_data = true;
				SelectStorageDevice();
				return;
			}
			if (m_inventory != null)
			{
				m_inventory.UpdateSaveData();
			}
			if (GameData.Save(m_game_data, m_SD))
			{
				m_game_data_found = true;
				return;
			}
			m_show_storage_failed = true;
			m_SD = null;
			m_game_data_found = false;
		}
		catch
		{
		}
	}

	protected virtual void LoadGameData()
	{
		try
		{
			if (!CheckSignedIn())
			{
				return;
			}
			if (m_SD == null || !m_SD.IsConnected)
			{
				if (m_state == GAME_STATE.LOADING_GAME)
				{
					m_show_storage_failed = true;
					m_SD = null;
					m_game_data_found = false;
					if (m_loading != null)
					{
						m_loading.Stop();
						m_loading.Clear();
						m_loading = null;
					}
					onExitGame();
				}
				else
				{
					m_load_game_data = true;
					SelectStorageDevice();
				}
				return;
			}
			m_game_data = GameData.Load(m_SD);
			if (m_game_data == null)
			{
				m_game_data = new GameData();
				m_game_data_found = false;
				m_show_storage_failed = true;
				m_SD = null;
				if (m_loading != null)
				{
					m_loading.Stop();
					m_loading.Clear();
					m_loading = null;
				}
				onExitGame();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Game.LoadGameData: " + ex.Message);
			if (m_game_data == null)
			{
				m_game_data = new GameData();
				m_game_data_found = false;
			}
		}
	}

	public void PlaySoundRandomPan(SoundEffect sound)
	{
		float num = GetRandom(-10, 10);
		num *= 0.1f;
		PlaySound(sound, 1f, num, 0f);
	}

	public void PlaySoundRandomPitch(SoundEffect sound)
	{
		float num = GetRandom(-10, 10);
		num *= 0.1f;
		PlaySound(sound, 1f, 0f, num);
	}

	public void PlaySoundRandomPanPitch(SoundEffect sound)
	{
		PlaySoundRandomPanPitch(sound, 1f);
	}

	public void PlaySoundRandomPanPitch(SoundEffect sound, float vol)
	{
		float num = GetRandom(-10, 10);
		num *= 0.1f;
		float num2 = GetRandom(-10, 10);
		num2 *= 0.1f;
		PlaySound(sound, vol, num, num2);
	}

	public void PlaySound(SoundEffect sound)
	{
		PlaySound(sound, 1f, 0f, 0f);
	}

	public void PlaySound(SoundEffect sound, float vol)
	{
		PlaySound(sound, vol, 0f, 0f);
	}

	public void PlaySound(SoundEffect sound, float vol, float pan)
	{
		PlaySound(sound, vol, pan, 0f);
	}

	public void PlaySound(SoundEffect sound, float vol, float pan, float pitch)
	{
		if (sound != null)
		{
			sound.Play(m_game_settings.m_sound_volume * 0.1f * vol, pitch, pan);
		}
	}

	public void ChangeArea(string area, string view, bool door_sound)
	{
		m_next_area = area;
		m_next_view = view;
		m_input_enabled = false;
		m_a_pressed = true;
		m_b_pressed = true;
		m_y_pressed = true;
		m_cursor.onOut();
		m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
		m_hud.FadeOut();
		FadeOutArea();
		if (door_sound)
		{
			PlaySound(m_door_open, 0.2f);
		}
	}

	public void FadeOutArea()
	{
		m_show_cursor = false;
		m_state = GAME_STATE.FADE_OUT_AREA;
		m_fade_alpha = 0f;
	}

	public void FadeInArea()
	{
		m_state = GAME_STATE.FADE_IN_AREA;
		m_fade_alpha = 510f;
		if (m_game_data != null && m_game_data.GetState("Music") != "")
		{
			switch (m_game_data.GetState("Music"))
			{
			case "1":
				PlayMusic(m_music1);
				break;
			case "2":
				PlayMusic(m_music2);
				break;
			case "3":
				PlayMusic(m_music3);
				break;
			}
		}
	}

	public void ActivateTrigger(Trigger trigger)
	{
		if (trigger != null && trigger.m_enabled)
		{
			m_over_trigger = null;
			m_active_trigger = trigger;
			m_active_trigger.Activate();
			m_input_enabled = false;
			if (m_cursor != null && m_cursor.m_state != Cursor.CURSOR_STATE.IDLE)
			{
				m_cursor.onOut();
			}
			m_state = GAME_STATE.ACTIVE_TRIGGER;
			if (m_hud != null)
			{
				m_hud.FadeOut();
			}
		}
	}

	public void ClearTrigger()
	{
		m_over_trigger = null;
		m_active_trigger = null;
		m_input_enabled = true;
		m_state = GAME_STATE.SCENE;
		if (m_hud != null)
		{
			if (m_world != null && m_world.GetCurrentView() != null)
			{
				m_hud.m_state = m_world.GetCurrentView().m_hud_state;
			}
			m_hud.FadeIn();
		}
		if (m_tutorial_state == TUTORIAL_STATE.CHANGING_VIEW)
		{
			m_tutorial_state = TUTORIAL_STATE.USE;
			int tutorial_state = (int)m_tutorial_state;
			m_game_data.SetState("TutorialState", tutorial_state.ToString());
		}
	}

	public void onCursorOver(Trigger trigger)
	{
		m_over_trigger = trigger;
		if (m_cursor != null && m_update_cursor)
		{
			m_cursor.onOver(trigger.m_type);
		}
	}

	public void onCursorOut()
	{
		if (m_update_cursor)
		{
			m_over_trigger = null;
			if (m_cursor != null)
			{
				m_cursor.onOut();
			}
		}
	}

	public void onCloseInventory()
	{
		if (m_active_trigger != null)
		{
			m_state = GAME_STATE.ACTIVE_TRIGGER;
		}
		else
		{
			m_state = GAME_STATE.SCENE;
			if (m_world != null && m_world.GetCurrentView() != null)
			{
				m_hud.m_state = m_world.GetCurrentView().m_hud_state;
			}
			m_hud.FadeIn();
		}
		m_show_cursor = true;
	}

	public void PlayMusic(Song music)
	{
		if (music == (Song)null)
		{
			return;
		}
		if (music == m_music1)
		{
			MUSIC_VOL_DEC_MULTI = 0.3f;
		}
		else if (music == m_music2)
		{
			MUSIC_VOL_DEC_MULTI = 0.3f;
		}
		else if (music == m_music3)
		{
			MUSIC_VOL_DEC_MULTI = 0.1f;
		}
		if (m_game_data != null)
		{
			if (m_game_data.GetState("Music") == m_current_music)
			{
				return;
			}
			m_current_music = m_game_data.GetState("Music");
		}
		m_music = music;
		m_music_state = MUSIC_STATE.PLAYING;
		MediaPlayer.IsRepeating = true;
		MediaPlayer.Volume = m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
		MediaPlayer.Play(m_music);
	}

	public void FadeOutMusic()
	{
		if (!(m_music == (Song)null))
		{
			m_music_state = MUSIC_STATE.FADE_OUT;
			MediaPlayer.Volume = m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
			if (m_game_data != null)
			{
				m_game_data.SetState("Music", "");
				m_current_music = "";
			}
		}
	}

	public void FadeInMusic()
	{
		m_music_state = MUSIC_STATE.FADE_IN;
		MediaPlayer.Volume = 0f;
	}

	public void StopMusic()
	{
		MediaPlayer.Pause();
		m_music = null;
		m_current_music = "";
	}

	public virtual void HandleEvent(string s_event)
	{
		if (m_world != null)
		{
			m_world.HandleEvent(s_event);
		}
	}

	public virtual void HandleUseEvent(string s_event)
	{
		if (m_world != null)
		{
			m_world.HandleUseEvent(s_event);
		}
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Invalid comparison between Unknown and I4
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Invalid comparison between Unknown and I4
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Invalid comparison between Unknown and I4
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Invalid comparison between Unknown and I4
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Invalid comparison between Unknown and I4
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Invalid comparison between Unknown and I4
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1e: Invalid comparison between Unknown and I4
		//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0def: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Invalid comparison between Unknown and I4
		//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6f: Invalid comparison between Unknown and I4
		//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed7: Invalid comparison between Unknown and I4
		//IL_0f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Invalid comparison between Unknown and I4
		//IL_0f8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa7: Invalid comparison between Unknown and I4
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_1068: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1071: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076: Unknown result type (might be due to invalid IL or missing references)
		//IL_107a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1080: Invalid comparison between Unknown and I4
		try
		{
			((Game)this).Update(gameTime);
			if (!((Game)this).IsActive || Guide.IsVisible)
			{
				if (!m_in_pause)
				{
					m_in_pause = true;
					HandleEvent("Game.Pause");
				}
				GamePad.SetVibration(PLAYER_INDEX, 0f, 0f);
				m_a_pressed = true;
				if (m_state == GAME_STATE.SHOW_INTRO && m_intro != null)
				{
					m_intro.Update(gameTime.ElapsedGameTime);
				}
				return;
			}
			m_in_pause = false;
			if (m_freeze)
			{
				return;
			}
			if (m_show_not_signed_in)
			{
				m_show_not_signed_in = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Not signed in", "Failed to load/save data. A signed in profile is required for this operation.", (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)2, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
				return;
			}
			if (m_show_SD_not_connected)
			{
				m_show_SD_not_connected = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Device not connected", "Failed to load/save data. The selected device is not connected.", (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)2, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
				return;
			}
			if (m_show_storage_failed)
			{
				m_show_storage_failed = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Failed", "Failed to load/save data. Check that the selected storage device is connected and that a valid profile is signed in. If the problem remains, restart the game and try again.", (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)2, (AsyncCallback)onMessageFinished, (object)object.Equals(0, 0));
				return;
			}
			if (m_state == GAME_STATE.CHECK_TRIALMODE)
			{
				if (!Guide.IsTrialMode)
				{
					SelectStorageDevice();
				}
				m_start_menu = new StartMenu(this);
				m_state = GAME_STATE.START_MENU;
				return;
			}
			if (m_volume_changed)
			{
				m_volume_changed = false;
				if (m_world != null)
				{
					m_world.HandleEvent("VolumeChanged");
				}
			}
			KeyboardState state = Keyboard.GetState();
			if (m_state == GAME_STATE.SELECT_CONTROLLER)
			{
				for (PlayerIndex val = (PlayerIndex)0; (int)val <= 3; val = (PlayerIndex)(val + 1))
				{
					GamePadState state2 = GamePad.GetState(val);
					GamePadButtons buttons = ((GamePadState)(ref state2)).Buttons;
					if ((int)((GamePadButtons)(ref buttons)).Start == 1)
					{
						PLAYER_INDEX = val;
						m_state = GAME_STATE.CHECK_TRIALMODE;
					}
				}
				return;
			}
			switch (m_music_state)
			{
			case MUSIC_STATE.FADE_IN:
				MediaPlayer.Volume += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 0.2f;
				if (MediaPlayer.Volume >= m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI)
				{
					MediaPlayer.Volume = m_game_settings.m_sound_volume * 0.1f * MUSIC_VOL_DEC_MULTI;
					m_music_state = MUSIC_STATE.PLAYING;
				}
				break;
			case MUSIC_STATE.FADE_OUT:
				MediaPlayer.Volume -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 0.2f;
				if (MediaPlayer.Volume <= 0f)
				{
					MediaPlayer.Volume = 0f;
					MediaPlayer.Pause();
				}
				break;
			}
			if (m_game_menu != null)
			{
				GamePadState state3 = GamePad.GetState(PLAYER_INDEX);
				GamePadButtons buttons2 = ((GamePadState)(ref state3)).Buttons;
				if (((int)((GamePadButtons)(ref buttons2)).Start == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)27)) && m_game_menu_enabled)
				{
					GamePad.SetVibration(PLAYER_INDEX, 0f, 0f);
					m_show_game_menu = true;
					HandleEvent("Game.Pause");
					if (m_inventory != null)
					{
						m_inventory.onGameMenu();
					}
				}
			}
			if (m_show_game_menu)
			{
				if (m_game_menu != null)
				{
					m_game_menu.Update(gameTime.ElapsedGameTime);
				}
			}
			else if (m_state == GAME_STATE.START_LOADING_INTRO)
			{
				StopMusic();
				m_state = GAME_STATE.LOADING_INTRO;
				Clear();
				m_CL = new SGSContentLoader((IServiceProvider)((Game)this).Services);
				m_loading = new Loading(this);
				m_loading.Start(gameTime);
				LoadIntro();
				m_loading.Stop();
				m_loading.Clear();
				m_loading = null;
				if (m_intro != null)
				{
					m_intro.Start();
				}
				m_state = GAME_STATE.SHOW_INTRO;
			}
			else if (m_state == GAME_STATE.START_LOADING_GAME)
			{
				StopMusic();
				m_state = GAME_STATE.LOADING_GAME;
				Clear();
				m_CL = new SGSContentLoader((IServiceProvider)((Game)this).Services);
				m_loading = new Loading(this);
				m_loading.Start(gameTime);
				m_shader = ((Game)this).Content.Load<Effect>("Shader");
				LoadInterface();
				if (m_game_data != null)
				{
					m_game_data.Clear();
					m_game_data = null;
				}
				if (!m_new_game)
				{
					LoadGameData();
					m_inventory.LoadItems();
				}
				else
				{
					m_game_data = new GameData();
				}
				LoadWorld();
				m_loading.Stop();
				m_loading.Clear();
				m_loading = null;
			}
			else if (m_state == GAME_STATE.SHOW_INTRO)
			{
				if (m_intro != null)
				{
					m_intro.Update(gameTime.ElapsedGameTime);
				}
			}
			else if (m_state == GAME_STATE.START_MENU)
			{
				if (m_start_menu != null)
				{
					m_start_menu.Update(gameTime.ElapsedGameTime);
				}
			}
			else
			{
				if (m_state == GAME_STATE.LOADING_AREA)
				{
					return;
				}
				if (m_state == GAME_STATE.FADE_IN_AREA || m_state == GAME_STATE.FADE_OUT_AREA)
				{
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					if (m_state == GAME_STATE.FADE_IN_AREA)
					{
						m_fade_alpha -= (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 400f;
						if (m_world != null)
						{
							m_world.Update(gameTime.ElapsedGameTime);
						}
						if (!(m_fade_alpha <= 0f))
						{
							return;
						}
						if (m_new_game)
						{
							m_new_game = false;
							m_state = GAME_STATE.ASK_TUTORIAL;
							m_game_data.SetState("AskTutorial", "1");
						}
						else
						{
							m_state = GAME_STATE.SCENE;
							if (m_game_data.GetState("AskTutorial") == "1")
							{
								m_state = GAME_STATE.ASK_TUTORIAL;
							}
							if (m_game_data.GetState("TutorialState") != "" && (m_tutorial_state == TUTORIAL_STATE.MOVE_CURSOR || m_tutorial_state == TUTORIAL_STATE.DECREASE_SPEED || m_tutorial_state == TUTORIAL_STATE.INCREASE_SPEED || m_tutorial_state == TUTORIAL_STATE.CHANGE_VIEW))
							{
								m_state = GAME_STATE.TUTORIAL;
							}
						}
						m_world.m_current_area.Init();
					}
					else
					{
						if (m_state != GAME_STATE.FADE_OUT_AREA)
						{
							return;
						}
						m_hud.Update(gameTime.ElapsedGameTime);
						m_fade_alpha += (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f * 400f;
						if (m_world != null)
						{
							m_world.Update(gameTime.ElapsedGameTime);
						}
						if (m_fade_alpha >= 255f)
						{
							if (m_next_area != "")
							{
								m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
								m_state = GAME_STATE.LOADING_AREA;
								m_loading = new Loading(this);
								m_loading.m_draw_background = false;
								m_loading.Start(gameTime);
								m_world.ChangeArea(m_next_area, m_next_view, fade_in: true);
								m_loading.Stop();
								m_loading.Clear();
								m_loading = null;
								m_next_area = "";
							}
							else
							{
								FadeInArea();
							}
						}
					}
					return;
				}
				if (m_state == GAME_STATE.ASK_TUTORIAL)
				{
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					GamePadState state4 = GamePad.GetState(PLAYER_INDEX);
					GamePadButtons buttons3 = ((GamePadState)(ref state4)).Buttons;
					if ((int)((GamePadButtons)(ref buttons3)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
					{
						if (!m_a_pressed)
						{
							m_a_pressed = true;
							m_state = GAME_STATE.TUTORIAL;
							m_hud.m_state = HUD.HUD_STATE.NONE;
							m_tutorial_state = TUTORIAL_STATE.MOVE_CURSOR;
							int tutorial_state = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state.ToString());
							m_game_data.SetState("AskTutorial", "0");
						}
					}
					else
					{
						m_a_pressed = false;
					}
					GamePadState state5 = GamePad.GetState(PLAYER_INDEX);
					GamePadButtons buttons4 = ((GamePadState)(ref state5)).Buttons;
					if ((int)((GamePadButtons)(ref buttons4)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
					{
						if (!m_b_pressed)
						{
							m_b_pressed = true;
							m_state = GAME_STATE.SCENE;
							m_tutorial_state = TUTORIAL_STATE.NONE;
							int tutorial_state2 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state2.ToString());
							m_game_data.SetState("AskTutorial", "0");
						}
					}
					else
					{
						m_b_pressed = false;
					}
					return;
				}
				if (m_show_cursor && m_cursor != null && m_update_cursor)
				{
					m_cursor.Update(gameTime.ElapsedGameTime);
				}
				if (m_hud != null)
				{
					m_hud.Update(gameTime.ElapsedGameTime);
				}
				if ((m_state != GAME_STATE.TUTORIAL || m_tutorial_state == TUTORIAL_STATE.INVENTORY) && m_inventory != null)
				{
					if (m_inventory_enabled)
					{
						m_inventory.Update(gameTime.ElapsedGameTime);
					}
					else
					{
						m_inventory.m_state = global::Game.Inventory.Inventory.INVENTORY_STATE.DISABLED;
					}
				}
				switch (m_state)
				{
				case GAME_STATE.TUTORIAL:
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					switch (m_tutorial_state)
					{
					case TUTORIAL_STATE.MOVE_CURSOR:
					{
						GamePadState state20 = GamePad.GetState(PLAYER_INDEX);
						GamePadThumbSticks thumbSticks5 = ((GamePadState)(ref state20)).ThumbSticks;
						if (((GamePadThumbSticks)(ref thumbSticks5)).Left.X == 0f)
						{
							GamePadState state21 = GamePad.GetState(PLAYER_INDEX);
							GamePadThumbSticks thumbSticks6 = ((GamePadState)(ref state21)).ThumbSticks;
							if (((GamePadThumbSticks)(ref thumbSticks6)).Left.Y == 0f)
							{
								break;
							}
						}
						m_tutorial_state = TUTORIAL_STATE.DECREASE_SPEED;
						int tutorial_state7 = (int)m_tutorial_state;
						m_game_data.SetState("TutorialState", tutorial_state7.ToString());
						break;
					}
					case TUTORIAL_STATE.DECREASE_SPEED:
					{
						GamePadState state17 = GamePad.GetState(PLAYER_INDEX);
						GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref state17)).ThumbSticks;
						if (((GamePadThumbSticks)(ref thumbSticks3)).Left.X == 0f)
						{
							GamePadState state18 = GamePad.GetState(PLAYER_INDEX);
							GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref state18)).ThumbSticks;
							if (((GamePadThumbSticks)(ref thumbSticks4)).Left.Y == 0f)
							{
								break;
							}
						}
						GamePadState state19 = GamePad.GetState(PLAYER_INDEX);
						GamePadTriggers triggers2 = ((GamePadState)(ref state19)).Triggers;
						if (((GamePadTriggers)(ref triggers2)).Left != 0f)
						{
							m_tutorial_state = TUTORIAL_STATE.INCREASE_SPEED;
							int tutorial_state6 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state6.ToString());
						}
						break;
					}
					case TUTORIAL_STATE.INCREASE_SPEED:
					{
						GamePadState state14 = GamePad.GetState(PLAYER_INDEX);
						GamePadThumbSticks thumbSticks = ((GamePadState)(ref state14)).ThumbSticks;
						if (((GamePadThumbSticks)(ref thumbSticks)).Left.X == 0f)
						{
							GamePadState state15 = GamePad.GetState(PLAYER_INDEX);
							GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref state15)).ThumbSticks;
							if (((GamePadThumbSticks)(ref thumbSticks2)).Left.Y == 0f)
							{
								break;
							}
						}
						GamePadState state16 = GamePad.GetState(PLAYER_INDEX);
						GamePadTriggers triggers = ((GamePadState)(ref state16)).Triggers;
						if (((GamePadTriggers)(ref triggers)).Right != 0f)
						{
							m_tutorial_state = TUTORIAL_STATE.CHANGE_VIEW;
							int tutorial_state5 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state5.ToString());
							m_hud.m_navigator.FadeIn();
						}
						break;
					}
					case TUTORIAL_STATE.CHANGE_VIEW:
					{
						if (!m_input_enabled)
						{
							break;
						}
						GamePadState state13 = GamePad.GetState(PLAYER_INDEX);
						GamePadDPad dPad5 = ((GamePadState)(ref state13)).DPad;
						if ((int)((GamePadDPad)(ref dPad5)).Down == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)98))
						{
							if (!m_d_down_pressed)
							{
								m_d_down_pressed = true;
								m_hud.m_state = HUD.HUD_STATE.NAVIGATOR;
								m_tutorial_state = TUTORIAL_STATE.CHANGING_VIEW;
								int tutorial_state4 = (int)m_tutorial_state;
								m_game_data.SetState("TutorialState", tutorial_state4.ToString());
								if (m_world != null && m_world.GetCurrentView() != null)
								{
									m_world.GetCurrentView().onDirection(View.VIEW_DIRECTION.DOWN);
								}
							}
						}
						else
						{
							m_d_down_pressed = false;
						}
						break;
					}
					}
					break;
				case GAME_STATE.SHOW_TEXT:
				{
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					if (!m_input_enabled)
					{
						break;
					}
					GamePadState state12 = GamePad.GetState(PLAYER_INDEX);
					GamePadButtons buttons7 = ((GamePadState)(ref state12)).Buttons;
					if ((int)((GamePadButtons)(ref buttons7)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
					{
						if (!m_a_pressed)
						{
							m_a_pressed = true;
							m_hud.HideText();
						}
					}
					else
					{
						m_a_pressed = false;
					}
					break;
				}
				case GAME_STATE.ACTIVE_TRIGGER:
					if (m_active_trigger != null)
					{
						m_active_trigger.Update(gameTime.ElapsedGameTime);
					}
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					break;
				case GAME_STATE.SCENE:
				case GAME_STATE.SHOW_ASK:
				{
					if (m_noise_effect != null)
					{
						if (m_world != null)
						{
							m_world.UpdateEffect(gameTime.ElapsedGameTime);
						}
						m_noise_effect.Update(gameTime.ElapsedGameTime);
					}
					if (m_world != null)
					{
						m_world.Update(gameTime.ElapsedGameTime);
					}
					if (!m_input_enabled)
					{
						break;
					}
					GamePadState state6 = GamePad.GetState(PLAYER_INDEX);
					GamePadDPad dPad = ((GamePadState)(ref state6)).DPad;
					if ((int)((GamePadDPad)(ref dPad)).Down == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)98))
					{
						if (!m_d_down_pressed)
						{
							m_d_down_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onDirection(View.VIEW_DIRECTION.DOWN);
							}
						}
					}
					else
					{
						m_d_down_pressed = false;
					}
					GamePadState state7 = GamePad.GetState(PLAYER_INDEX);
					GamePadDPad dPad2 = ((GamePadState)(ref state7)).DPad;
					if ((int)((GamePadDPad)(ref dPad2)).Up == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)104))
					{
						if (!m_d_up_pressed)
						{
							m_d_up_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onDirection(View.VIEW_DIRECTION.UP);
							}
						}
					}
					else
					{
						m_d_up_pressed = false;
					}
					GamePadState state8 = GamePad.GetState(PLAYER_INDEX);
					GamePadDPad dPad3 = ((GamePadState)(ref state8)).DPad;
					if ((int)((GamePadDPad)(ref dPad3)).Left == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)100))
					{
						if (!m_d_left_pressed)
						{
							m_d_left_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onDirection(View.VIEW_DIRECTION.LEFT);
							}
						}
					}
					else
					{
						m_d_left_pressed = false;
					}
					GamePadState state9 = GamePad.GetState(PLAYER_INDEX);
					GamePadDPad dPad4 = ((GamePadState)(ref state9)).DPad;
					if ((int)((GamePadDPad)(ref dPad4)).Right == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)102))
					{
						if (!m_d_right_pressed)
						{
							m_d_right_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onDirection(View.VIEW_DIRECTION.RIGHT);
							}
						}
					}
					else
					{
						m_d_right_pressed = false;
					}
					GamePadState state10 = GamePad.GetState(PLAYER_INDEX);
					GamePadButtons buttons5 = ((GamePadState)(ref state10)).Buttons;
					if ((int)((GamePadButtons)(ref buttons5)).A == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)65))
					{
						if (!m_a_pressed)
						{
							m_a_pressed = true;
							if (m_over_trigger != null)
							{
								if (m_tutorial_state == TUTORIAL_STATE.USE)
								{
									m_tutorial_state = TUTORIAL_STATE.WAIT_FOR_PICKUP;
									int tutorial_state3 = (int)m_tutorial_state;
									m_game_data.SetState("TutorialState", tutorial_state3.ToString());
								}
								if (!m_over_trigger.m_activate_own)
								{
									ActivateTrigger(m_over_trigger);
								}
								else
								{
									m_over_trigger.Activate();
									m_over_trigger = null;
								}
							}
							if (!m_show_cursor && m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onProceed();
							}
						}
					}
					else
					{
						m_a_pressed = false;
					}
					GamePadState state11 = GamePad.GetState(PLAYER_INDEX);
					GamePadButtons buttons6 = ((GamePadState)(ref state11)).Buttons;
					if ((int)((GamePadButtons)(ref buttons6)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)66))
					{
						if (!m_b_pressed)
						{
							m_b_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onBack();
							}
						}
					}
					else
					{
						m_b_pressed = false;
					}
					break;
				}
				case GAME_STATE.INVENTORY:
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Game.Update: " + ex.Message);
		}
	}

	protected void DrawSelectController(TimeSpan elapsed)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			m_SB.Begin();
			m_SB.Draw(m_start_bkg, VIEW_RECT, Color.White);
			string text = "PRESS START";
			Vector2 val = m_font.MeasureString(text);
			m_SB.DrawString(m_font, text, new Vector2(((float)VIEW_RECT.Width - val.X) / 2f, ((float)VIEW_RECT.Height - val.Y) / 2f + 100f), Color.White);
			m_SB.End();
		}
		catch
		{
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (((Game)this).GraphicsDevice.IsDisposed || (int)((Game)this).GraphicsDevice.GraphicsDeviceStatus != 0)
			{
				return;
			}
			((Game)this).GraphicsDevice.Clear(Color.Black);
			if (m_freeze)
			{
				return;
			}
			if (m_state == GAME_STATE.SELECT_CONTROLLER)
			{
				DrawSelectController(gameTime.ElapsedGameTime);
				return;
			}
			if (m_state == GAME_STATE.SHOW_INTRO && m_intro != null)
			{
				m_intro.Draw(m_SB);
			}
			if (m_show_game_menu)
			{
				if (m_game_menu != null)
				{
					if (m_active_trigger != null)
					{
						m_active_trigger.Draw(m_SB);
					}
					else if (m_world != null && m_world.GetCurrentView() != null)
					{
						m_world.GetCurrentView().Draw(m_SB);
					}
					m_game_menu.Draw(m_SB);
				}
				if (m_fade_texture != null)
				{
					int num = 4;
					m_SB.Begin((SpriteBlendMode)1);
					m_SB.Draw(m_fade_texture, new Rectangle(0, 0, VIEW_RECT.Width, num), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(0, 0, num, VIEW_RECT.Height), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(VIEW_RECT.Width - num, 0, num, VIEW_RECT.Height), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(0, VIEW_RECT.Height - num, VIEW_RECT.Width, num), Color.Black);
					m_SB.End();
				}
				return;
			}
			switch (m_state)
			{
			case GAME_STATE.START_LOADING_GAME:
			case GAME_STATE.LOADING_GAME:
				return;
			case GAME_STATE.START_MENU:
				if (m_start_menu != null)
				{
					m_start_menu.Draw(m_SB);
				}
				return;
			case GAME_STATE.LOADING_AREA:
				return;
			case GAME_STATE.FADE_IN_AREA:
			case GAME_STATE.FADE_OUT_AREA:
			{
				if (m_world != null)
				{
					m_world.Draw(m_SB);
				}
				if (m_noise_effect != null)
				{
					if (m_world != null)
					{
						m_world.DrawEffect(m_SB);
					}
					m_noise_effect.DrawMultiply(m_SB, m_noise_color);
				}
				Color white = Color.White;
				if (m_fade_alpha <= 255f)
				{
					((Color)(ref white)).A = (byte)Math.Round(m_fade_alpha);
				}
				else
				{
					((Color)(ref white)).A = byte.MaxValue;
				}
				if (m_fade_texture != null)
				{
					m_SB.Begin((SpriteBlendMode)1);
					m_SB.Draw(m_fade_texture, VIEW_RECT, white);
					m_SB.End();
				}
				break;
			}
			case GAME_STATE.ACTIVE_TRIGGER:
				if (m_active_trigger != null)
				{
					m_active_trigger.Draw(m_SB);
				}
				if (m_noise_effect != null)
				{
					if (m_world != null)
					{
						m_world.DrawEffect(m_SB);
					}
					m_noise_effect.DrawMultiply(m_SB, m_noise_color);
				}
				break;
			case GAME_STATE.SCENE:
			case GAME_STATE.SHOW_TEXT:
			case GAME_STATE.SHOW_ASK:
				if (m_world != null)
				{
					m_world.Draw(m_SB);
				}
				if (m_noise_effect != null)
				{
					if (m_world != null)
					{
						m_world.DrawEffect(m_SB);
					}
					m_noise_effect.DrawMultiply(m_SB, m_noise_color);
				}
				break;
			case GAME_STATE.ASK_TUTORIAL:
				if (m_world != null)
				{
					m_world.Draw(m_SB);
				}
				if (m_noise_effect != null)
				{
					if (m_world != null)
					{
						m_world.DrawEffect(m_SB);
					}
					m_noise_effect.DrawMultiply(m_SB, m_noise_color);
				}
				break;
			case GAME_STATE.TUTORIAL:
				if (m_world != null)
				{
					m_world.Draw(m_SB);
				}
				if (m_noise_effect != null)
				{
					if (m_world != null)
					{
						m_world.DrawEffect(m_SB);
					}
					m_noise_effect.DrawMultiply(m_SB, m_noise_color);
				}
				break;
			case GAME_STATE.INVENTORY:
				if (m_active_trigger != null)
				{
					m_active_trigger.Draw(m_SB);
				}
				else if (m_world != null && m_world.GetCurrentView() != null)
				{
					m_world.GetCurrentView().Draw(m_SB);
				}
				break;
			}
			if (m_inventory != null)
			{
				m_inventory.Draw(m_SB);
			}
			if (m_hud != null)
			{
				m_hud.Draw(m_SB);
			}
			if (m_state == GAME_STATE.ASK_TUTORIAL)
			{
				if (m_fade_texture != null)
				{
					int num2 = 4;
					m_SB.Begin((SpriteBlendMode)1);
					m_SB.Draw(m_fade_texture, new Rectangle(0, 0, VIEW_RECT.Width, num2), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(0, 0, num2, VIEW_RECT.Height), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(VIEW_RECT.Width - num2, 0, num2, VIEW_RECT.Height), Color.Black);
					m_SB.Draw(m_fade_texture, new Rectangle(0, VIEW_RECT.Height - num2, VIEW_RECT.Width, num2), Color.Black);
					m_SB.End();
				}
				return;
			}
			if (m_show_cursor && m_cursor != null)
			{
				m_cursor.Draw(m_SB);
			}
			if (m_fade_texture != null)
			{
				int num3 = 4;
				m_SB.Begin((SpriteBlendMode)1);
				m_SB.Draw(m_fade_texture, new Rectangle(0, 0, VIEW_RECT.Width, num3), Color.Black);
				m_SB.Draw(m_fade_texture, new Rectangle(0, 0, num3, VIEW_RECT.Height), Color.Black);
				m_SB.Draw(m_fade_texture, new Rectangle(VIEW_RECT.Width - num3, 0, num3, VIEW_RECT.Height), Color.Black);
				m_SB.Draw(m_fade_texture, new Rectangle(0, VIEW_RECT.Height - num3, VIEW_RECT.Width, num3), Color.Black);
				m_SB.End();
			}
			((Game)this).Draw(gameTime);
		}
		catch
		{
			try
			{
				m_SB.End();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Game.Draw: " + ex.Message);
			}
		}
	}

	static Game()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		STORAGE_LOCATION = "Decay";
		STORAGE_SETTINGS_FILE = "Settings.sav";
		STORAGE_SAVE_FILE = "Part1.sav";
		INST = null;
		MUSIC_VOL_DEC_MULTI = 1f;
		PLAYER_INDEX = (PlayerIndex)0;
	}
}
