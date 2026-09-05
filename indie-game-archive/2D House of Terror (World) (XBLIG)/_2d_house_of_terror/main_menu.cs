using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class main_menu : game_state
{
	private class easy_mode_selection
	{
		public enum ITEMS
		{
			NOTHING = -1,
			ON,
			OFF
		}

		private Texture2D header;

		private Texture2D box;

		private Texture2D ghost;

		private Texture2D[][] items;

		private SpriteFont font;

		private string txt;

		private int selection;

		private selection_arrow arrow;

		private Vector2 ghost_sway;

		private Vector2 ghost_max_sway;

		private Vector2 ghost_sway_speed;

		public easy_mode_selection(ContentManager con_mgr)
		{
			ghost_sway = new Vector2(0f, 0f);
			ghost_max_sway = new Vector2(10f, 10f);
			ghost_sway_speed = new Vector2(0.1f, 0.1f);
			header = con_mgr.Load<Texture2D>("menu/main/easy_mode/header");
			box = con_mgr.Load<Texture2D>("menu/main/easy_mode/box");
			ghost = con_mgr.Load<Texture2D>("menu/main/easy_mode/ghost");
			font = con_mgr.Load<SpriteFont>("12");
			items = new Texture2D[2][]
			{
				new Texture2D[2]
				{
					con_mgr.Load<Texture2D>("menu/main/easy_mode/on"),
					con_mgr.Load<Texture2D>("menu/main/easy_mode/on_inactive")
				},
				new Texture2D[2]
				{
					con_mgr.Load<Texture2D>("menu/main/easy_mode/off"),
					con_mgr.Load<Texture2D>("menu/main/easy_mode/off_inactive")
				}
			};
			arrow = new selection_arrow(con_mgr.Load<Texture2D>("menu/main/arrow_right"), 0.2, 0.0, 8.0, 0.0);
			selection = 0;
			txt = "If switched on the explanation\nfor each mini-game will be \nextended, otherwise only the\ncontrol information will\nbe shown.\nYou can view the extended\nexplanation via the pause menu\nregardless of this choice.";
		}

		public void draw(SpriteBatch spr_batch)
		{
			spr_batch.Draw(header, new Vector2(60f, 50f), Color.White);
			spr_batch.Draw(box, new Vector2(60f, 100f), Color.White);
			spr_batch.Draw(ghost, new Vector2(300f + ghost_sway.X, 35f + ghost_sway.Y), Color.White);
			for (int i = 0; i < items.Length; i++)
			{
				spr_batch.Draw(items[i][(i != selection) ? 1 : 0], new Vector2(185 - items[i][(i != selection) ? 1 : 0].Width / 2, 140 + i * 40), Color.White);
				if (i == selection)
				{
					arrow.draw(spr_batch, 185 - items[i][(i != selection) ? 1 : 0].Width / 2 - 30, 140 + i * 40);
				}
			}
			spr_batch.DrawString(font, txt, new Vector2(60f, 250f), Color.White);
		}

		private void update_ghost()
		{
			ghost_sway += ghost_sway_speed;
			if ((ghost_sway.X > ghost_max_sway.X && ghost_sway_speed.X > 0f) || (ghost_sway.X < 0f && ghost_sway_speed.X < 0f))
			{
				ghost_sway_speed.X *= -1f;
			}
			if ((ghost_sway.Y > ghost_max_sway.Y && ghost_sway_speed.Y > 0f) || (ghost_sway.Y < 0f && ghost_sway_speed.Y < 0f))
			{
				ghost_sway_speed.Y *= -1f;
			}
		}

		public ITEMS update()
		{
			if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_UP))
			{
				selection--;
			}
			if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_DOWN))
			{
				selection++;
			}
			selection = ((selection < 0) ? (items.Length - 1) : (selection % items.Length));
			arrow.update();
			update_ghost();
			if (!controllers.clicked(CONTROLLER_BUTTONS.A) && !controllers.clicked(CONTROLLER_BUTTONS.START))
			{
				return ITEMS.NOTHING;
			}
			return (ITEMS)selection;
		}
	}

	private enum STATE
	{
		MODE_SELECT,
		PLAYER_SELECT,
		TRANSITION_PL_TO_CHAR,
		TRANSITION_CHAR_TO_PL,
		CHARACTER_SELECT,
		FADE_OUT,
		EASY_SELECT
	}

	private STATE menu_state;

	private Song bgm;

	private SoundEffect select_sfx;

	private SoundEffect confirm_sfx;

	private SoundEffect quit_sfx;

	private Texture2D press_start_txt;

	private Texture2D tick_symbol;

	private Texture2D bg;

	private Texture2D fog;

	private Vector2 bg_pos;

	private Vector2 fog_pos;

	private Texture2D[] game_mode_txts;

	private Texture2D[] player_txts;

	private Texture2D[] player_txts_grayed;

	private Texture2D[][] char_sprites;

	private selection_arrow arrow_left;

	private selection_arrow arrow_right;

	private GAME_MODE selected_mode;

	private int[] player_ids;

	private int[] char_ids;

	private bool[] confirmed;

	private int number_of_players;

	private int number_of_possible_players;

	private easy_mode_selection easy_select;

	private bool easy_mode_selected;

	public override bool easy_mode => easy_mode_selected;

	public main_menu(GraphicsDevice dev, IServiceProvider serv)
		: base(dev, serv)
	{
		press_start_txt = content_mgr.Load<Texture2D>("menu/main/player/press_start");
		tick_symbol = content_mgr.Load<Texture2D>("menu/main/player/check_mark");
		bg = content_mgr.Load<Texture2D>("menu/main/bg");
		fog = content_mgr.Load<Texture2D>("menu/main/fog");
		bg_pos = new Vector2(0f, 0f);
		fog_pos = new Vector2(640f, 0f);
		game_mode_txts = new Texture2D[3]
		{
			content_mgr.Load<Texture2D>("menu/main/mode/story"),
			content_mgr.Load<Texture2D>("menu/main/mode/random"),
			content_mgr.Load<Texture2D>("menu/main/mode/quit")
		};
		player_txts = new Texture2D[4]
		{
			content_mgr.Load<Texture2D>("menu/main/player/1_player"),
			content_mgr.Load<Texture2D>("menu/main/player/2_player"),
			content_mgr.Load<Texture2D>("menu/main/player/3_player"),
			content_mgr.Load<Texture2D>("menu/main/player/4_player")
		};
		player_txts_grayed = new Texture2D[3]
		{
			content_mgr.Load<Texture2D>("menu/main/player/2_player_grayed"),
			content_mgr.Load<Texture2D>("menu/main/player/3_player_grayed"),
			content_mgr.Load<Texture2D>("menu/main/player/4_player_grayed")
		};
		char_sprites = new Texture2D[2][]
		{
			new Texture2D[4]
			{
				content_mgr.Load<Texture2D>("menu/main/character/jimmy_unavailable"),
				content_mgr.Load<Texture2D>("menu/main/character/sam_unavailable"),
				content_mgr.Load<Texture2D>("menu/main/character/erik_unavailable"),
				content_mgr.Load<Texture2D>("menu/main/character/billy_unavailable")
			},
			new Texture2D[4]
			{
				content_mgr.Load<Texture2D>("menu/main/character/jimmy"),
				content_mgr.Load<Texture2D>("menu/main/character/sam"),
				content_mgr.Load<Texture2D>("menu/main/character/erik"),
				content_mgr.Load<Texture2D>("menu/main/character/billy")
			}
		};
		arrow_left = new selection_arrow(content_mgr.Load<Texture2D>("menu/main/arrow_left"), 0.2, 0.0, 8.0, 0.0);
		arrow_right = new selection_arrow(content_mgr.Load<Texture2D>("menu/main/arrow_right"), 0.2, 0.0, 8.0, 0.0);
		menu_state = STATE.MODE_SELECT;
		selected_mode = GAME_MODE.STORY;
		number_of_players = 1;
		player_ids = new int[4] { -1, -1, -1, -1 };
		char_ids = new int[4] { 0, 1, 2, 3 };
		bool[] array = new bool[4];
		confirmed = array;
		select_sfx = content_mgr.Load<SoundEffect>("sfx/select");
		confirm_sfx = content_mgr.Load<SoundEffect>("sfx/confirm");
		quit_sfx = content_mgr.Load<SoundEffect>("sfx/quit");
		bgm = content_mgr.Load<Song>("bgm/main_menu");
		MediaPlayer.IsRepeating = true;
		MediaPlayer.Play(bgm);
		easy_select = new easy_mode_selection(content_mgr);
	}

	~main_menu()
	{
		free();
	}

	public override void free()
	{
		base.free();
		MediaPlayer.Stop();
	}

	private GAME_STATE update_mode_select()
	{
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_DOWN))
		{
			select_sfx.Play();
			selected_mode++;
		}
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_UP))
		{
			select_sfx.Play();
			selected_mode--;
		}
		selected_mode = ((selected_mode < GAME_MODE.STORY) ? GAME_MODE.QUIT : ((GAME_MODE)((int)selected_mode % 3)));
		if (controllers.clicked(CONTROLLER_BUTTONS.A) || controllers.clicked(CONTROLLER_BUTTONS.START))
		{
			GAME_MODE gAME_MODE = selected_mode;
			if (gAME_MODE == GAME_MODE.QUIT)
			{
				quit_sfx.Play();
				return GAME_STATE.QUIT;
			}
			confirm_sfx.Play();
			menu_state = STATE.PLAYER_SELECT;
		}
		return GAME_STATE.MAIN_MENU;
	}

	private void update_player_select()
	{
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_DOWN))
		{
			select_sfx.Play();
			number_of_players++;
		}
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_UP))
		{
			select_sfx.Play();
			number_of_players--;
		}
		number_of_players = ((number_of_players < 1) ? number_of_possible_players : ((number_of_players > number_of_possible_players) ? (number_of_players % (number_of_possible_players + 1) + 1) : number_of_players));
		if (controllers.clicked(CONTROLLER_BUTTONS.A) || controllers.clicked(CONTROLLER_BUTTONS.START))
		{
			confirm_sfx.Play();
			menu_state = STATE.TRANSITION_PL_TO_CHAR;
		}
		if (controllers.clicked(CONTROLLER_BUTTONS.B))
		{
			quit_sfx.Play();
			menu_state = STATE.MODE_SELECT;
		}
	}

	private void update_character_select()
	{
		bool[] array = new bool[4] { true, true, true, true };
		for (int i = 0; i < 4; i++)
		{
			if (confirmed[i])
			{
				array[char_ids[i]] = false;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (!controllers.clicked(j, CONTROLLER_BUTTONS.START))
			{
				continue;
			}
			bool flag = true;
			for (int k = 0; k < 4; k++)
			{
				if (player_ids[k] == j)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				continue;
			}
			for (int l = 0; l < number_of_players; l++)
			{
				if (player_ids[l] == -1)
				{
					confirm_sfx.Play();
					player_ids[l] = j;
					break;
				}
			}
		}
		for (int m = 0; m < number_of_players; m++)
		{
			if (player_ids[m] < 0)
			{
				continue;
			}
			if (controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.A) && array[char_ids[m]])
			{
				confirm_sfx.Play();
				confirmed[m] = true;
			}
			if (!confirmed[m])
			{
				if (controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.DPAD_RIGHT) || controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.LTHUMB_RIGHT))
				{
					select_sfx.Play();
					char_ids[m]++;
				}
				if (controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.DPAD_LEFT) || controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.LTHUMB_LEFT))
				{
					select_sfx.Play();
					char_ids[m]--;
				}
				char_ids[m] = ((char_ids[m] < 0) ? 3 : (char_ids[m] % 4));
			}
			else if (controllers.clicked(player_ids[m], CONTROLLER_BUTTONS.B))
			{
				quit_sfx.Play();
				controllers.reset();
				confirmed[m] = false;
			}
		}
		bool flag2 = true;
		bool flag3 = true;
		for (int n = 0; n < number_of_players; n++)
		{
			if (confirmed[n])
			{
				flag3 = false;
			}
			else
			{
				flag2 = false;
			}
		}
		if (flag3 && controllers.clicked(CONTROLLER_BUTTONS.B))
		{
			quit_sfx.Play();
			menu_state = STATE.TRANSITION_CHAR_TO_PL;
			player_ids = new int[4] { -1, -1, -1, -1 };
		}
		if (!flag2 || !controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			return;
		}
		menu_state = STATE.FADE_OUT;
		fade.stripe(90u, 40u, Color.Black);
		for (int num = 0; num < number_of_players; num++)
		{
			array[char_ids[num]] = false;
		}
		for (int num2 = number_of_players; num2 < 4; num2++)
		{
			for (int num3 = 0; num3 < 4; num3++)
			{
				if (array[num3])
				{
					char_ids[num2] = num3;
					array[num3] = false;
					break;
				}
			}
		}
	}

	public override GAME_STATE update()
	{
		base.update();
		number_of_possible_players = 0;
		for (int i = 0; i < 4; i++)
		{
			if (controllers.connected[i])
			{
				number_of_possible_players++;
			}
		}
		if (number_of_possible_players == 0)
		{
			number_of_possible_players = 1;
		}
		arrow_left.update();
		arrow_right.update();
		fog_pos.X -= 0.5f;
		fog_pos.X = ((fog_pos.X < (float)(-fog.Width)) ? 1280f : fog_pos.X);
		switch (menu_state)
		{
		case STATE.MODE_SELECT:
			return update_mode_select();
		case STATE.PLAYER_SELECT:
			update_player_select();
			break;
		case STATE.TRANSITION_PL_TO_CHAR:
			bg_pos.X -= 4f;
			if (bg_pos.X <= -640f)
			{
				bg_pos.X = -640f;
				menu_state = STATE.CHARACTER_SELECT;
			}
			break;
		case STATE.TRANSITION_CHAR_TO_PL:
			bg_pos.X += 4f;
			if (bg_pos.X >= 0f)
			{
				bg_pos.X = 0f;
				menu_state = STATE.PLAYER_SELECT;
			}
			break;
		case STATE.CHARACTER_SELECT:
			update_character_select();
			break;
		case STATE.FADE_OUT:
			if (fade.done)
			{
				game_mgr.player_ids = player_ids;
				game_mgr.char_ids = char_ids;
				int[] points = new int[4];
				game_mgr.points = points;
				game_mgr.ranking = new int[4] { 0, 1, 2, 3 };
				game_mgr.MOOD[] moods = new game_mgr.MOOD[4];
				game_mgr.moods = moods;
				menu_state = STATE.EASY_SELECT;
			}
			break;
		case STATE.EASY_SELECT:
		{
			easy_mode_selection.ITEMS iTEMS = easy_select.update();
			if (iTEMS != easy_mode_selection.ITEMS.NOTHING)
			{
				easy_mode_selected = iTEMS == easy_mode_selection.ITEMS.ON;
				if (selected_mode != GAME_MODE.STORY)
				{
					return GAME_STATE.RANDOM_MODE;
				}
				return GAME_STATE.STORY_MODE;
			}
			break;
		}
		}
		return GAME_STATE.MAIN_MENU;
	}

	private void draw_mode_selection()
	{
		Vector2 vector = new Vector2(gfx_dev.Viewport.Width / 2 - 10, gfx_dev.Viewport.Height / 2 + 20);
		vector += bg_pos;
		for (int i = 0; i < 3; i++)
		{
			if (i == (int)selected_mode)
			{
				arrow_right.draw(spr_batch, vector.X - (float)(game_mode_txts[i].Width / 2) - 30f, vector.Y);
			}
			spr_batch.Draw(game_mode_txts[i], new Vector2(vector.X - (float)(game_mode_txts[i].Width / 2), vector.Y), Color.White);
			vector.Y += game_mode_txts[i].Height + 20;
		}
	}

	private void draw_player_selection()
	{
		Vector2 vector = new Vector2(gfx_dev.Viewport.Width / 2 - 10, gfx_dev.Viewport.Height / 2 + 20);
		vector += bg_pos;
		for (int i = 0; i < 4; i++)
		{
			if (i == number_of_players - 1)
			{
				arrow_right.draw(spr_batch, vector.X - (float)(player_txts[i].Width / 2) - 30f, vector.Y);
			}
			spr_batch.Draw((number_of_possible_players > i) ? player_txts[i] : player_txts_grayed[(i > 0) ? (i - 1) : i], new Vector2(vector.X - (float)(player_txts[i].Width / 2), vector.Y), Color.White);
			vector.Y += player_txts[i].Height + 20;
		}
	}

	private void draw_character_selection()
	{
		Vector2[] array = new Vector2[4]
		{
			new Vector2(870f, 110f),
			new Vector2(1047f, 210f),
			new Vector2(892f, 290f),
			new Vector2(1082f, 365f)
		};
		bool[] array2 = new bool[4] { true, true, true, true };
		for (int i = 0; i < 4; i++)
		{
			if (confirmed[i])
			{
				array2[char_ids[i]] = false;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			array[j] += bg_pos;
			if (player_ids[j] >= 0)
			{
				Texture2D texture2D = char_sprites[(confirmed[j] || array2[char_ids[j]]) ? 1 : 0][char_ids[j]];
				spr_batch.Draw(texture2D, new Vector2(array[j].X - (float)(texture2D.Width / 2), array[j].Y + 35f - (float)texture2D.Height), Color.White);
				if (confirmed[j])
				{
					spr_batch.Draw(tick_symbol, array[j], Color.White);
					continue;
				}
				arrow_left.draw(spr_batch, array[j].X - (float)(arrow_left.width() + 52), array[j].Y - (float)(arrow_left.height() / 2));
				arrow_right.draw(spr_batch, array[j].X + 52f, array[j].Y - (float)(arrow_right.height() / 2), swap_direction: true);
			}
			else if (j < number_of_players)
			{
				spr_batch.Draw(press_start_txt, new Vector2(array[j].X - (float)(press_start_txt.Width / 2), array[j].Y - (float)(press_start_txt.Height / 2)), Color.White);
			}
			else
			{
				spr_batch.Draw(char_sprites[0][0], new Vector2(array[j].X - (float)(char_sprites[0][0].Width / 2), array[j].Y + 35f - (float)char_sprites[0][0].Height), Color.White);
			}
		}
	}

	public override void draw()
	{
		base.draw();
		spr_batch.GraphicsDevice.Clear(Color.Black);
		spr_batch.Begin();
		if (menu_state == STATE.EASY_SELECT)
		{
			easy_select.draw(spr_batch);
		}
		else
		{
			spr_batch.Draw(bg, bg_pos, Color.White);
			if (menu_state == STATE.MODE_SELECT)
			{
				draw_mode_selection();
			}
			else
			{
				draw_player_selection();
			}
			draw_character_selection();
			spr_batch.Draw(fog, fog_pos + bg_pos, Color.White);
			fade.draw(spr_batch);
		}
		spr_batch.End();
	}
}
