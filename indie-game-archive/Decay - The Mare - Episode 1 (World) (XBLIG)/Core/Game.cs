using System;
using Core.Inventory;
using Core.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using SGSCore;

namespace Core;

public class Game : Microsoft.Xna.Framework.Game
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

	public static string STORAGE_LOCATION = "";

	public static string STORAGE_SETTINGS_FILE = "";

	public static string STORAGE_SAVE_FILE = "";

	public static Rectangle VIEW_RECT;

	public static Rectangle TS_AREA;

	public static PlayerIndex PLAYER_INDEX = PlayerIndex.One;

	public Tutorial.STATE m_tutorial_state = Tutorial.STATE.NONE;

	public GraphicsDeviceManager m_GDM;

	public SpriteBatch m_SB;

	public GAME_STATE m_state;

	public SGSContentLoader m_CL;

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

	public bool m_show_cursor = true;

	public bool m_input_enabled = true;

	public bool m_update_cursor = true;

	public bool m_inventory_enabled = true;

	public bool m_input_blocked;

	public Trigger m_over_trigger;

	public Trigger m_active_trigger;

	public HUD m_hud;

	public Core.Inventory.Inventory m_inventory;

	private Animation2D m_noise_effect;

	private Color m_noise_color = Color.White * 0.5f;

	public Texture2D m_fade_texture;

	private float m_fade_alpha;

	public Effect m_shader;

	protected object m_SD_state;

	protected StorageDevice m_SD;

	public GameSettings m_game_settings = new GameSettings();

	public GameData m_game_data;

	private bool m_load_game_data;

	private bool m_save_game_data;

	private bool m_save_settings;

	public bool m_game_data_found;

	public Intro m_intro;

	public Core.World.World m_world;

	private string m_next_area = "";

	private string m_next_view = "";

	public Loading m_loading;

	protected bool m_new_game = true;

	public StartMenu m_start_menu;

	public GameMenu m_game_menu;

	public bool m_show_game_menu;

	public bool m_game_menu_enabled = true;

	public bool m_play_door_sound = true;

	public SoundEffect m_door_open1;

	public SoundEffect m_door_open2;

	public SoundEffect m_door_open3;

	public SoundEffect m_door_open4;

	public bool m_freeze;

	protected Random m_rand;

	protected SpriteFont m_font;

	protected SpriteFont m_font2;

	private bool m_show_not_signed_in;

	private bool m_show_SD_not_connected;

	private bool m_show_storage_failed;

	public Overlay m_overlay;

	public RenderTarget2D m_RT;

	public bool m_use_event_handled;

	public Language m_language;

	private bool m_in_pause;

	public Game()
	{
		m_GDM = new GraphicsDeviceManager(this);
		base.Components.Add(new GamerServicesComponent(this));
		m_GDM.PreferredBackBufferWidth = 1280;
		m_GDM.PreferredBackBufferHeight = 720;
		m_GDM.PreferMultiSampling = true;
		m_GDM.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		base.Content.RootDirectory = SGSContentLoader.CONTENT_PATH;
		m_rand = new Random(DateTime.Now.Millisecond);
	}

	protected override void Initialize()
	{
		PresentationParameters presentationParameters = base.GraphicsDevice.PresentationParameters;
		presentationParameters.MultiSampleCount = 4;
		base.GraphicsDevice.Reset(presentationParameters);
		VIEW_RECT = new Rectangle(0, 0, 1280, 720);
		base.Initialize();
	}

	protected override void BeginRun()
	{
		DisplayMode displayMode = base.GraphicsDevice.DisplayMode;
		double num = (double)VIEW_RECT.Width / (double)displayMode.Width;
		double num2 = (double)VIEW_RECT.Height / (double)displayMode.Height;
		TS_AREA = new Rectangle((int)Math.Round((double)displayMode.TitleSafeArea.X * num), (int)Math.Round((double)displayMode.TitleSafeArea.Y * num2), (int)Math.Round((double)displayMode.TitleSafeArea.Width * num), (int)Math.Round((double)displayMode.TitleSafeArea.Height * num2));
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
		m_overlay = new Overlay(this);
		base.BeginRun();
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
		m_intro.Clear();
		m_intro = null;
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
		m_tutorial_state = Tutorial.STATE.MOVE_CURSOR;
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
		m_CL = new SGSContentLoader(base.Services);
		m_show_game_menu = false;
		m_state = GAME_STATE.START_MENU;
		m_start_menu = CreateStartMenu();
		m_input_blocked = false;
		m_input_enabled = true;
		m_inventory_enabled = true;
		m_update_cursor = true;
		m_game_menu_enabled = true;
		m_tutorial_state = Tutorial.STATE.NONE;
	}

	protected virtual StartMenu CreateStartMenu()
	{
		return null;
	}

	protected virtual GameMenu CreateGameMenu()
	{
		return null;
	}

	protected virtual Loading CreateLoading()
	{
		return null;
	}

	protected virtual void LoadIntro()
	{
		try
		{
			if (m_intro != null)
			{
				m_intro.Clear();
				m_intro = null;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual void LoadInterface()
	{
		try
		{
			m_font = m_CL.LoadFont("Fonts/SpriteFont2");
			m_fade_texture = base.Content.Load<Texture2D>("HUD/black");
			m_noise_effect = new TextureAnimation(this, base.Content, "Effects/Noise/", 5u, reverse: false);
			m_noise_effect.SetFPS(15.0);
			m_noise_effect.m_random_mode = true;
			m_noise_effect.Play(Animation2D.LOOP_TYPE.CYCLE);
			m_cursor = new Cursor(this, Color.White);
			m_hud = new HUD(this);
			m_game_menu = CreateGameMenu();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual void LoadWorld()
	{
		try
		{
			m_show_cursor = false;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void LoadContent()
	{
		_ = base.GraphicsDevice.PresentationParameters;
		m_SB = new SpriteBatch(base.GraphicsDevice);
		m_RT = new RenderTarget2D(base.GraphicsDevice, VIEW_RECT.Width, VIEW_RECT.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
		m_CL = new SGSContentLoader(base.Services);
		m_font = m_CL.LoadFont("Fonts/SpriteFont2");
		m_font2 = m_CL.LoadFont("Fonts/SpriteFont1");
		string text = "en";
		m_language = new Language(this, "XMLContent/Language/" + text);
		m_state = GAME_STATE.SELECT_CONTROLLER;
	}

	protected override void UnloadContent()
	{
		try
		{
			Clear();
			if (m_overlay != null)
			{
				m_overlay.Clear();
				m_overlay = null;
			}
			if (m_RT != null)
			{
				m_RT.Dispose();
				m_RT = null;
			}
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
			if (m_SB != null)
			{
				m_SB.Dispose();
				m_SB = null;
			}
			m_GDM = null;
			if (m_language != null)
			{
				m_language.Clear();
				m_language = null;
			}
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
			m_font = null;
			m_font2 = null;
			m_door_open1 = null;
			m_door_open2 = null;
			m_door_open3 = null;
			m_door_open4 = null;
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
				m_fade_texture.Dispose();
				m_fade_texture = null;
			}
			if (m_noise_effect != null)
			{
				m_noise_effect.Clear();
				m_noise_effect = null;
			}
			if (m_CL != null)
			{
				m_CL.Clear();
				m_CL = null;
			}
			base.Content.Unload();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected virtual void SelectStorageDevice()
	{
		try
		{
			m_freeze = true;
			m_SD_state = "GetDevice for Player";
			StorageDevice.BeginShowSelector(PLAYER_INDEX, StorageDeviceSelected, m_SD_state);
		}
		catch (Exception ex)
		{
			Console.WriteLine("Game.SelectStorageDevice: " + ex.Message);
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
		try
		{
			for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
			{
				if (Gamer.SignedInGamers[i].PlayerIndex == PLAYER_INDEX)
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
		StorageContainer storageContainer = null;
		try
		{
			m_SD = StorageDevice.EndShowSelector(res);
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
				IAsyncResult asyncResult = m_SD.BeginOpenContainer(STORAGE_LOCATION, null, null);
				asyncResult.AsyncWaitHandle.WaitOne();
				storageContainer = m_SD.EndOpenContainer(asyncResult);
				asyncResult.AsyncWaitHandle.Close();
				string sTORAGE_SAVE_FILE = STORAGE_SAVE_FILE;
				if (storageContainer.FileExists(sTORAGE_SAVE_FILE))
				{
					m_game_data_found = true;
				}
				else
				{
					m_game_data_found = false;
				}
				sTORAGE_SAVE_FILE = STORAGE_SETTINGS_FILE;
				if (storageContainer.FileExists(sTORAGE_SAVE_FILE))
				{
					storageContainer.Dispose();
					storageContainer = null;
					m_game_settings = GameSettings.Load(m_SD);
					if (m_game_settings == null)
					{
						m_game_settings = new GameSettings();
						throw new NullReferenceException();
					}
					SoundEffect.MasterVolume = m_game_settings.m_sound_volume * 0.1f;
					if (m_start_menu != null && m_start_menu.m_options_menu != null)
					{
						m_start_menu.m_options_menu.SetGamma(m_game_settings.m_brightness);
					}
				}
				else
				{
					storageContainer.Dispose();
					storageContainer = null;
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
			if (storageContainer != null)
			{
				storageContainer.Dispose();
				storageContainer = null;
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
		sound?.Play(m_game_settings.m_sound_volume * 0.1f * vol, pitch, pan);
	}

	public virtual void ChangeArea(string area, string view, bool door_sound)
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
			PlayDoorSound();
		}
	}

	public void PlayDoorSound()
	{
		switch (GetRandom(1, 4))
		{
		case 1:
			PlaySound(m_door_open1, 0.75f);
			break;
		case 2:
			PlaySound(m_door_open2, 0.75f);
			break;
		case 3:
			PlaySound(m_door_open3, 0.5f);
			break;
		case 4:
			PlaySound(m_door_open4, 0.5f);
			break;
		}
	}

	public void FadeOutArea()
	{
		m_show_cursor = false;
		m_state = GAME_STATE.FADE_OUT_AREA;
		m_fade_alpha = 0f;
	}

	public virtual void FadeInArea()
	{
		try
		{
			HandleEvent("Game.FadeInArea");
			m_state = GAME_STATE.FADE_IN_AREA;
			m_fade_alpha = 2f;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
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
			if (m_hud != null && !m_input_blocked)
			{
				m_hud.FadeOut();
			}
		}
	}

	public void ClearTrigger()
	{
		m_over_trigger = null;
		m_active_trigger = null;
		if (!m_input_blocked)
		{
			m_input_enabled = true;
		}
		m_state = GAME_STATE.SCENE;
		if (m_hud != null)
		{
			if (m_world != null && m_world.GetCurrentView() != null)
			{
				m_hud.m_state = m_world.GetCurrentView().m_hud_state;
			}
			if (!m_input_blocked)
			{
				m_hud.FadeIn();
			}
		}
		if (m_tutorial_state == Tutorial.STATE.CHANGING_VIEW)
		{
			m_tutorial_state = Tutorial.STATE.USE;
			int tutorial_state = (int)m_tutorial_state;
			m_game_data.SetState("TutorialState", tutorial_state.ToString());
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

	public virtual void onCloseInventory()
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
			if (!m_input_blocked)
			{
				m_hud.FadeIn();
			}
		}
		if (!m_input_blocked)
		{
			m_show_cursor = true;
		}
	}

	public virtual void PlayMusic(int music)
	{
	}

	public virtual void PlayMusic(Song music)
	{
		try
		{
			Sound.PlayMusic(this, music);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void FadeOutMusic()
	{
		try
		{
			Sound.FadeOutMusic(this);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void FadeInMusic()
	{
		try
		{
			Sound.FadeInMusic();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void StopMusic()
	{
		try
		{
			Sound.StopMusic();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void PauseMusic()
	{
		try
		{
			Sound.PauseMusic();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public void ResumeMusic()
	{
		try
		{
			Sound.ResumeMusic();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public virtual void UseEventHandled()
	{
		m_use_event_handled = true;
	}

	public virtual void HandleEvent(string s_event)
	{
		try
		{
			if (m_world != null)
			{
				m_world.HandleEvent(s_event);
			}
			if (s_event.IndexOf(".Use.") == -1 || m_use_event_handled)
			{
				return;
			}
			View currentView = m_world.GetCurrentView();
			if (currentView != null)
			{
				string id = "";
				switch (GetRandom(0, 2))
				{
				case 0:
					id = "Cannot use this here ...";
					break;
				case 1:
					id = "No ...";
					break;
				case 2:
					id = "That doesn't work ...";
					break;
				}
				m_hud.ShowText(m_language.GetString(id), currentView.m_use_text_fade, currentView.m_no_text_fade);
				currentView.ResetCursorTriggers();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void Update(GameTime gameTime)
	{
		try
		{
			base.Update(gameTime);
			if (!base.IsActive || Guide.IsVisible)
			{
				if (!m_in_pause)
				{
					m_in_pause = true;
					HandleEvent("Game.Pause");
					if (m_inventory != null)
					{
						m_inventory.onGameMenu();
					}
				}
				GamePad.SetVibration(PLAYER_INDEX, 0f, 0f);
				m_a_pressed = true;
				if (m_state == GAME_STATE.SHOW_INTRO && m_intro != null)
				{
					m_intro.Update(gameTime.ElapsedGameTime);
				}
				return;
			}
			if (m_in_pause)
			{
				HandleEvent("Game.Resume");
				if (m_inventory != null)
				{
					m_inventory.onGameMenuClosed();
				}
				m_in_pause = false;
			}
			if (m_freeze)
			{
				return;
			}
			if (m_show_not_signed_in)
			{
				m_show_not_signed_in = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Not signed in", "Failed to load/save data. A signed in profile is required for this operation.", new string[1] { "Ok" }, 0, MessageBoxIcon.Warning, onMessageFinished, object.Equals(0, 0));
				return;
			}
			if (m_show_SD_not_connected)
			{
				m_show_SD_not_connected = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Device not connected", "Failed to load/save data. The selected device is not connected.", new string[1] { "Ok" }, 0, MessageBoxIcon.Warning, onMessageFinished, object.Equals(0, 0));
				return;
			}
			if (m_show_storage_failed)
			{
				m_show_storage_failed = false;
				Guide.BeginShowMessageBox(PLAYER_INDEX, "Failed", "Failed to load/save data. Check that the selected storage device is connected and that a valid profile is signed in. If the problem remains, restart the game and try again.", new string[1] { "Ok" }, 0, MessageBoxIcon.Warning, onMessageFinished, object.Equals(0, 0));
				return;
			}
			if (m_state == GAME_STATE.CHECK_TRIALMODE)
			{
				if (!Guide.IsTrialMode)
				{
					SelectStorageDevice();
				}
				m_start_menu = CreateStartMenu();
				m_state = GAME_STATE.START_MENU;
				return;
			}
			KeyboardState state = Keyboard.GetState();
			if (m_state == GAME_STATE.SELECT_CONTROLLER)
			{
				for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
				{
					if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed)
					{
						PLAYER_INDEX = playerIndex;
						m_state = GAME_STATE.CHECK_TRIALMODE;
					}
				}
				return;
			}
			Sound.Update(this, gameTime);
			if (m_game_menu != null && m_game_menu_enabled && (GamePad.GetState(PLAYER_INDEX).Buttons.Start == ButtonState.Pressed || state.IsKeyDown(Keys.Escape)))
			{
				GamePad.SetVibration(PLAYER_INDEX, 0f, 0f);
				m_show_game_menu = true;
				HandleEvent("Game.Pause");
				if (m_inventory != null)
				{
					m_inventory.onGameMenu();
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
				m_CL = new SGSContentLoader(base.Services);
				m_loading = CreateLoading();
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
				m_CL = new SGSContentLoader(base.Services);
				m_loading = CreateLoading();
				m_loading.Start(gameTime);
				m_shader = base.Content.Load<Effect>("Shader/Shader");
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
						m_fade_alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
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
							if (m_game_data.GetState("TutorialState") != "" && (m_tutorial_state == Tutorial.STATE.MOVE_CURSOR || m_tutorial_state == Tutorial.STATE.DECREASE_SPEED || m_tutorial_state == Tutorial.STATE.INCREASE_SPEED || m_tutorial_state == Tutorial.STATE.CHANGE_VIEW))
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
						m_fade_alpha += (float)gameTime.ElapsedGameTime.TotalSeconds;
						if (m_world != null)
						{
							m_world.Update(gameTime.ElapsedGameTime);
						}
						if (m_fade_alpha >= 1f)
						{
							if (m_next_area != "")
							{
								m_cursor.m_state = Cursor.CURSOR_STATE.IDLE;
								m_state = GAME_STATE.LOADING_AREA;
								m_loading = CreateLoading();
								m_loading.m_loading_area = false;
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
					if (m_hud != null)
					{
						m_hud.Update(gameTime.ElapsedGameTime);
					}
					if (!m_input_enabled)
					{
						m_world.Update(gameTime.ElapsedGameTime);
						return;
					}
					if (GamePad.GetState(PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
					{
						if (!m_a_pressed)
						{
							m_a_pressed = true;
							m_state = GAME_STATE.TUTORIAL;
							m_hud.m_state = HUD.HUD_STATE.NONE;
							m_tutorial_state = Tutorial.STATE.MOVE_CURSOR;
							m_show_cursor = true;
							int tutorial_state = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state.ToString());
							m_game_data.SetState("AskTutorial", "0");
						}
					}
					else
					{
						m_a_pressed = false;
					}
					if (GamePad.GetState(PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
					{
						if (!m_b_pressed)
						{
							m_b_pressed = true;
							m_state = GAME_STATE.SCENE;
							m_tutorial_state = Tutorial.STATE.NONE;
							int tutorial_state2 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state2.ToString());
							m_game_data.SetState("AskTutorial", "0");
							m_show_cursor = true;
							m_hud.FadeIn();
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
				if ((m_state != GAME_STATE.TUTORIAL || m_tutorial_state == Tutorial.STATE.INVENTORY) && m_inventory != null)
				{
					if (m_inventory_enabled)
					{
						m_use_event_handled = false;
						m_inventory.Update(gameTime.ElapsedGameTime);
					}
					else
					{
						m_inventory.m_state = Core.Inventory.Inventory.INVENTORY_STATE.DISABLED;
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
					case Tutorial.STATE.MOVE_CURSOR:
						if (GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.X != 0f || GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.Y != 0f)
						{
							m_tutorial_state = Tutorial.STATE.DECREASE_SPEED;
							int tutorial_state7 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state7.ToString());
						}
						break;
					case Tutorial.STATE.DECREASE_SPEED:
						if ((GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.X != 0f || GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.Y != 0f) && GamePad.GetState(PLAYER_INDEX).Triggers.Left != 0f)
						{
							m_tutorial_state = Tutorial.STATE.INCREASE_SPEED;
							int tutorial_state6 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state6.ToString());
						}
						break;
					case Tutorial.STATE.INCREASE_SPEED:
						if ((GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.X != 0f || GamePad.GetState(PLAYER_INDEX).ThumbSticks.Left.Y != 0f) && GamePad.GetState(PLAYER_INDEX).Triggers.Right != 0f)
						{
							m_tutorial_state = Tutorial.STATE.CHANGE_VIEW;
							int tutorial_state5 = (int)m_tutorial_state;
							m_game_data.SetState("TutorialState", tutorial_state5.ToString());
							m_hud.m_navigator.FadeIn();
						}
						break;
					case Tutorial.STATE.CHANGE_VIEW:
						if (!m_input_enabled)
						{
							break;
						}
						if (GamePad.GetState(PLAYER_INDEX).DPad.Down == ButtonState.Pressed || state.IsKeyDown(Keys.K))
						{
							if (!m_d_down_pressed)
							{
								m_d_down_pressed = true;
								m_hud.m_state = HUD.HUD_STATE.NAVIGATOR;
								m_tutorial_state = Tutorial.STATE.CHANGING_VIEW;
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
					break;
				case GAME_STATE.SHOW_TEXT:
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
					if (GamePad.GetState(PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
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
					if (GamePad.GetState(PLAYER_INDEX).DPad.Down == ButtonState.Pressed || state.IsKeyDown(Keys.K))
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
					if (GamePad.GetState(PLAYER_INDEX).DPad.Up == ButtonState.Pressed || state.IsKeyDown(Keys.I))
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
					if (GamePad.GetState(PLAYER_INDEX).DPad.Left == ButtonState.Pressed || state.IsKeyDown(Keys.J))
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
					if (GamePad.GetState(PLAYER_INDEX).DPad.Right == ButtonState.Pressed || state.IsKeyDown(Keys.L))
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
					if (GamePad.GetState(PLAYER_INDEX).Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.A))
					{
						if (!m_a_pressed)
						{
							m_a_pressed = true;
							if (m_over_trigger != null)
							{
								if (m_tutorial_state == Tutorial.STATE.USE)
								{
									m_tutorial_state = Tutorial.STATE.WAIT_FOR_PICKUP;
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
								HandleEvent(m_world.GetCurrentView().m_name + ".onProceed");
							}
						}
					}
					else
					{
						m_a_pressed = false;
					}
					if (GamePad.GetState(PLAYER_INDEX).Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.B))
					{
						if (!m_b_pressed)
						{
							m_b_pressed = true;
							if (m_world != null && m_world.GetCurrentView() != null)
							{
								m_world.GetCurrentView().onBack();
								HandleEvent(m_world.GetCurrentView().m_name + ".onBack");
							}
						}
					}
					else
					{
						m_b_pressed = false;
					}
					break;
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

	protected virtual void DrawSelectController(TimeSpan elapsed)
	{
	}

	protected override void Draw(GameTime gameTime)
	{
		try
		{
			if (base.GraphicsDevice.IsDisposed || base.GraphicsDevice.GraphicsDeviceStatus != GraphicsDeviceStatus.Normal)
			{
				return;
			}
			base.GraphicsDevice.Clear(Color.Black);
			base.GraphicsDevice.SetRenderTarget(m_RT);
			base.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
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
				float num = 1f;
				if (m_fade_alpha <= 1f)
				{
					num = m_fade_alpha;
				}
				if (m_fade_texture != null)
				{
					m_SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
					m_SB.Draw(m_fade_texture, VIEW_RECT, Color.White * num);
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
				if ((m_state == GAME_STATE.ASK_TUTORIAL && !m_input_enabled) || m_state == GAME_STATE.FADE_IN_AREA)
				{
					return;
				}
				m_hud.Draw(m_SB);
			}
			if (m_state != GAME_STATE.ASK_TUTORIAL)
			{
				if (m_show_cursor && m_cursor != null)
				{
					m_cursor.Draw(m_SB);
				}
				base.Draw(gameTime);
			}
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

	protected override void EndDraw()
	{
		try
		{
			if (m_overlay != null)
			{
				m_overlay.Draw(m_SB);
			}
			base.EndDraw();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}
