using System;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheMare1.Inventory;
using TheMare1.World;

namespace TheMare1;

public class TheMare1 : Core.Game
{
	private Texture2D m_title;

	private TextureAnimation m_start_screen_bg;

	public Song m_current_music;

	public Song m_menu_music;

	public Song m_ambient_music;

	public Song m_end_music;

	private bool m_game_finished;

	public bool m_exit_game;

	protected bool m_goto_store;

	public TheMare1()
	{
		Core.Game.STORAGE_LOCATION = "DecayMare";
		Core.Game.STORAGE_SETTINGS_FILE = "Settings1.sav";
		Core.Game.STORAGE_SAVE_FILE = "TheMare1.sav";
	}

	public override void Clear()
	{
		try
		{
			m_title = null;
			if (m_start_screen_bg != null)
			{
				m_start_screen_bg.Clear();
				m_start_screen_bg = null;
			}
			if (m_menu_music != null)
			{
				m_menu_music.Dispose();
				m_menu_music = null;
			}
			if (m_ambient_music != null)
			{
				m_ambient_music.Dispose();
				m_ambient_music = null;
			}
			if (m_end_music != null)
			{
				m_end_music.Dispose();
				m_end_music = null;
			}
			base.Clear();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void LoadContent()
	{
		try
		{
			base.LoadContent();
			m_title = m_CL.LoadTexture("StartScreen/decay_new_logo");
			m_start_screen_bg = new TextureAnimation(this, m_CL.m_CM, "StartScreen/Animation/", 1u, reverse: false);
			m_start_screen_bg.UseCombinedFrames(640, 360, 3, 1280);
			m_start_screen_bg.m_random_mode = true;
			m_start_screen_bg.SetFPS(7.0);
			m_start_screen_bg.Play();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override Core.StartMenu CreateStartMenu()
	{
		try
		{
			if (m_menu_music == null)
			{
				m_menu_music = base.Content.Load<Song>("Music/menu");
			}
			if (m_ambient_music == null)
			{
				m_ambient_music = base.Content.Load<Song>("Music/ambient");
			}
			if (m_end_music == null)
			{
				m_end_music = base.Content.Load<Song>("Music/End");
			}
			return new StartMenu(this);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
		return null;
	}

	protected override Core.GameMenu CreateGameMenu()
	{
		return new GameMenu(this);
	}

	protected override Core.Loading CreateLoading()
	{
		return new Loading(this);
	}

	protected override void LoadIntro()
	{
		try
		{
			base.LoadIntro();
			m_intro = new Intro(this, m_CL);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void LoadInterface()
	{
		try
		{
			base.LoadInterface();
			m_inventory = new global::TheMare1.Inventory.Inventory(this);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void LoadWorld()
	{
		try
		{
			if (m_world != null)
			{
				m_world.Clear();
				m_world = null;
			}
			if (m_ambient_music == null)
			{
				m_ambient_music = base.Content.Load<Song>("Music/ambient");
			}
			m_door_open1 = m_CL.LoadSound("Sound/doors_sound1");
			m_door_open2 = m_CL.LoadSound("Sound/doors_sound2");
			m_door_open3 = m_CL.LoadSound("Sound/doors_sound3");
			m_door_open4 = m_CL.LoadSound("Sound/doors_sound4");
			m_world = new global::TheMare1.World.World(this, "XMLContent/World/World");
			if (m_game_data.m_area != "")
			{
				m_world.ChangeArea(m_game_data.m_area, m_game_data.m_view, fade_in: true);
			}
			else
			{
				m_world.ChangeArea("DreamOwnRoom", "View1", fade_in: true);
			}
			base.LoadWorld();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void FadeInArea()
	{
		try
		{
			base.FadeInArea();
			if (m_game_data != null)
			{
				switch (m_game_data.GetState("Music"))
				{
				case "1":
					PlayMusic(m_ambient_music);
					break;
				case "2":
					PlayMusic(m_menu_music);
					break;
				case "3":
					PlayMusic(m_end_music);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	public override void PlayMusic(int music)
	{
		switch (music)
		{
		case 1:
			PlayMusic(m_ambient_music);
			break;
		case 2:
			PlayMusic(m_menu_music);
			break;
		case 3:
			PlayMusic(m_end_music);
			break;
		}
		base.PlayMusic(music);
	}

	public override void PlayMusic(Song music)
	{
		try
		{
			if (music == m_ambient_music)
			{
				Sound.MUSIC_VOL_DEC_MULTI = 0.1f;
			}
			else if (music == m_menu_music)
			{
				Sound.MUSIC_VOL_DEC_MULTI = 0.3f;
			}
			else
			{
				Sound.MUSIC_VOL_DEC_MULTI = 0.3f;
			}
			base.PlayMusic(music);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		if (m_game_finished)
		{
			base.GraphicsDevice.Clear(Color.Black);
			m_SB.GraphicsDevice.SetRenderTarget(m_RT);
			m_SB.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0f, 0);
			m_SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			string text = m_language.GetString("Saving, do not turn off your console.");
			Vector2 vector = m_font.MeasureString(text);
			Vector2 zero = Vector2.Zero;
			zero.X = ((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f;
			zero.Y = (float)Core.Game.TS_AREA.Bottom - vector.Y;
			m_SB.DrawString(m_font, text, new Vector2(zero.X + 1f, zero.Y + 2f), Color.Black);
			m_SB.DrawString(m_font, text, zero, Color.White);
			m_SB.End();
			m_overlay.Draw(m_SB);
			base.GraphicsDevice.Present();
			SaveGameData();
			SaveSettings();
			m_game_finished = false;
			m_exit_game = true;
		}
		else
		{
			base.Draw(gameTime);
		}
	}

	protected override void DrawSelectController(TimeSpan elapsed)
	{
		try
		{
			m_start_screen_bg.Update(elapsed);
			m_start_screen_bg.Draw(m_SB);
			m_SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			m_SB.Draw(m_title, new Vector2((Core.Game.VIEW_RECT.Width - m_title.Width) / 2, 40f), null, Color.White);
			string text = m_language.GetString("PRESS START");
			Vector2 vector = m_font.MeasureString(text);
			m_SB.DrawString(m_font, text, new Vector2(((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f, (float)Core.Game.VIEW_RECT.Bottom - vector.Y - 180f), Color.White);
			text = m_language.GetString("Copyright (c) 2013 Shining Gate Software");
			vector = m_font2.MeasureString(text);
			m_SB.DrawString(m_font2, text, new Vector2(((float)Core.Game.VIEW_RECT.Width - vector.X) / 2f, (float)Core.Game.TS_AREA.Bottom - vector.Y), Color.White);
			m_SB.End();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected override void Update(GameTime gameTime)
	{
		if (m_game_finished)
		{
			return;
		}
		if (m_exit_game)
		{
			onExitGame();
			m_exit_game = false;
			return;
		}
		if (m_goto_store && !Guide.IsVisible)
		{
			m_goto_store = false;
			try
			{
				Guide.ShowMarketplace(Core.Game.PLAYER_INDEX);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Guide.BeginShowMessageBox(Core.Game.PLAYER_INDEX, m_language.GetString("Message"), ex.Message, new string[1] { m_language.GetString("Ok") }, 0, MessageBoxIcon.None, onTrialMessage2Finished, object.Equals(0, 0));
			}
		}
		base.Update(gameTime);
	}

	public override void HandleEvent(string s_event)
	{
		base.HandleEvent(s_event);
		string text;
		if ((text = s_event) != null && text == "Game.Finished")
		{
			m_game_finished = true;
			if (m_game_data != null && m_game_data.GetState("Coins") == "5" && m_game_settings != null)
			{
				m_game_settings.m_extras_unlocked = true;
			}
		}
	}

	public override void onCloseInventory()
	{
		base.onCloseInventory();
	}

	public override void ChangeArea(string area, string view, bool door_sound)
	{
		try
		{
			if (Guide.IsTrialMode && area == "DreamRoom3")
			{
				Guide.BeginShowMessageBox(Core.Game.PLAYER_INDEX, m_language.GetString("Message"), m_language.GetString("This area is only available in the full version."), new string[2]
				{
					m_language.GetString("Unlock full game"),
					m_language.GetString("Continue")
				}, 1, MessageBoxIcon.None, onTrialMessageFinished, object.Equals(0, 0));
			}
			else
			{
				base.ChangeArea(area, view, door_sound);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}

	protected void onTrialMessageFinished(IAsyncResult res)
	{
		try
		{
			int? num = Guide.EndShowMessageBox(res);
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0 && num2.HasValue)
			{
				m_goto_store = true;
			}
		}
		catch
		{
		}
	}

	protected void onTrialMessage2Finished(IAsyncResult res)
	{
		try
		{
			Guide.EndShowMessageBox(res);
		}
		catch
		{
		}
	}
}
