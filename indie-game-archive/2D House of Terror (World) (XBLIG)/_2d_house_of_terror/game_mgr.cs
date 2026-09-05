using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace _2d_house_of_terror;

public class game_mgr : Game
{
	public enum MOOD
	{
		NEUTRAL,
		HAPPY,
		SAD
	}

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private Texture2D info_win;

	private SpriteFont font;

	private game_state current_state;

	private GAME_STATE current_state_id;

	public static int[] player_ids;

	public static int[] char_ids;

	public static int[] points;

	public static int[] ranking;

	public static MOOD[] moods;

	private bool redraw;

	private bool storage_warning;

	public static bool storage_selected;

	public static bool use_storage;

	public static StorageDevice storage_dev;

	private IAsyncResult storage_dialog_result;

	private highscore story_score;

	private highscore[] random_score;

	private bool wait_storage;

	public game_mgr()
	{
		graphics = new GraphicsDeviceManager(this);
		graphics.PreferredBackBufferWidth = 640;
		graphics.PreferredBackBufferHeight = 480;
		base.Content.RootDirectory = "Content";
		base.Components.Add(new GamerServicesComponent(this));
		story_score = null;
		highscore[] array = new highscore[4];
		random_score = array;
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		storage_selected = false;
		use_storage = true;
		storage_warning = false;
		storage_dialog_result = null;
		info_win = base.Content.Load<Texture2D>("menu/information_box");
		font = base.Content.Load<SpriteFont>("default_font");
		current_state = new video_player("fmv/intro", GAME_STATE.INTRO_VIDEO, GAME_STATE.MAIN_MENU, base.GraphicsDevice, base.Services);
		current_state_id = GAME_STATE.INTRO_VIDEO;
	}

	protected override void UnloadContent()
	{
		spriteBatch.Dispose();
		base.Content.Unload();
	}

	protected override bool BeginDraw()
	{
		if (redraw)
		{
			return base.BeginDraw();
		}
		base.BeginDraw();
		return false;
	}

	public bool manage_storage()
	{
		if (!use_storage)
		{
			return true;
		}
		if (storage_warning)
		{
			controllers.update();
			for (int i = 0; i < 4; i++)
			{
				if (controllers.clicked(i, CONTROLLER_BUTTONS.A))
				{
					use_storage = false;
					storage_warning = false;
					return true;
				}
				if (controllers.clicked(i, CONTROLLER_BUTTONS.B))
				{
					storage_warning = false;
					storage_selected = false;
					storage_dev = null;
					storage_dialog_result = null;
					return false;
				}
			}
			return false;
		}
		if (storage_selected)
		{
			if (storage_dev != null && storage_dev.IsConnected)
			{
				return true;
			}
			storage_dev = null;
			storage_selected = false;
			return false;
		}
		if (storage_dialog_result == null && !storage_selected && use_storage)
		{
			storage_dialog_result = StorageDevice.BeginShowSelector(null, null);
			return false;
		}
		if (storage_dialog_result != null && storage_dialog_result.IsCompleted)
		{
			storage_dev = StorageDevice.EndShowSelector(storage_dialog_result);
			if (storage_dev != null && storage_dev.IsConnected)
			{
				storage_dialog_result = null;
				storage_selected = true;
				return true;
			}
			storage_warning = true;
			return false;
		}
		return false;
	}

	protected override void Update(GameTime gameTime)
	{
		redraw = true;
		base.Update(gameTime);
		if (Guide.IsVisible)
		{
			return;
		}
		if (wait_storage)
		{
			if (!manage_storage())
			{
				return;
			}
			highscore highscore2 = null;
			if (current_state_id == GAME_STATE.STORY_MODE)
			{
				if (story_score == null)
				{
					story_score = new highscore("story.sav");
				}
				highscore2 = story_score;
			}
			else
			{
				int highscore_list_id = ((random_mode)current_state).highscore_list_id;
				if (random_score[highscore_list_id] == null)
				{
					random_score[highscore_list_id] = new highscore("random" + highscore_list_id + ".sav");
				}
				highscore2 = random_score[highscore_list_id];
			}
			current_state_id = GAME_STATE.HIGHSCORE;
			current_state.free();
			current_state = new highscore_display(base.GraphicsDevice, base.Services, highscore2);
			wait_storage = false;
		}
		GAME_STATE gAME_STATE = current_state.update();
		if (gAME_STATE == current_state_id)
		{
			return;
		}
		bool beg_mode = false;
		if (current_state_id == GAME_STATE.MAIN_MENU)
		{
			beg_mode = current_state.easy_mode;
		}
		if (gAME_STATE == GAME_STATE.HIGHSCORE)
		{
			wait_storage = true;
			return;
		}
		current_state_id = gAME_STATE;
		current_state.free();
		current_state = null;
		switch (current_state_id)
		{
		case GAME_STATE.MAIN_MENU:
			current_state = new main_menu(base.GraphicsDevice, base.Services);
			break;
		case GAME_STATE.RANDOM_MODE:
			current_state = new random_mode(base.GraphicsDevice, base.Services, beg_mode);
			break;
		case GAME_STATE.STORY_MODE:
			current_state = new story_mode(base.GraphicsDevice, base.Services, beg_mode);
			break;
		case GAME_STATE.QUIT:
			Exit();
			break;
		case GAME_STATE.HIGHSCORE:
			break;
		}
	}

	public void draw_storage_warning()
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		spriteBatch.Draw(info_win, new Vector2(320 - info_win.Width / 2, 240 - info_win.Height / 2), Color.White);
		Vector2 vector = font.MeasureString("You did not select a\nvalid storage device,\nyour highscores will\nnot be saved.\nPress A to continue or\nB to select a device.");
		spriteBatch.DrawString(font, "You did not select a\nvalid storage device,\nyour highscores will\nnot be saved.\nPress A to continue or\nB to select a device", new Vector2(320f - vector.X / 2f, 240f - vector.Y / 2f), Color.White);
		spriteBatch.End();
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		current_state.draw();
		if (storage_warning)
		{
			draw_storage_warning();
		}
	}
}
