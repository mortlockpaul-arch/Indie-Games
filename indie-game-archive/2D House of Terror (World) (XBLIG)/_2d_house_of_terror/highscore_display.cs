using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class highscore_display : game_state
{
	private class entry
	{
		private const int cursor_start_x = 183;

		private const int cursor_start_y = 289;

		private const int max_name_length = 14;

		private Texture2D bg;

		private Texture2D cursor;

		private Texture2D box;

		private Texture2D[] frames;

		private Texture2D[] faces;

		private SpriteFont font;

		private highscore list;

		private int winner_id;

		private bool is_done;

		private int cursor_char;

		private string current_name;

		public bool done => is_done;

		public entry(ContentManager con_mgr, highscore lst)
		{
			list = lst;
			winner_id = 0;
			for (int i = 0; i < 4; i++)
			{
				if (game_mgr.ranking[i] == 0)
				{
					winner_id = i;
					break;
				}
			}
			if (!list.is_highscore(game_mgr.points[winner_id]) || game_mgr.player_ids[winner_id] < 0)
			{
				is_done = true;
				return;
			}
			bg = con_mgr.Load<Texture2D>("menu/name_entry/bg");
			cursor = con_mgr.Load<Texture2D>("menu/name_entry/cursor");
			box = con_mgr.Load<Texture2D>("menu/name_entry/name_box");
			frames = new Texture2D[4]
			{
				con_mgr.Load<Texture2D>("menu/name_entry/frame1"),
				con_mgr.Load<Texture2D>("menu/name_entry/frame2"),
				con_mgr.Load<Texture2D>("menu/name_entry/frame3"),
				con_mgr.Load<Texture2D>("menu/name_entry/frame4")
			};
			faces = new Texture2D[4]
			{
				con_mgr.Load<Texture2D>("menu/name_entry/jimmy"),
				con_mgr.Load<Texture2D>("menu/name_entry/sam"),
				con_mgr.Load<Texture2D>("menu/name_entry/erik"),
				con_mgr.Load<Texture2D>("menu/name_entry/billy")
			};
			font = con_mgr.Load<SpriteFont>("default_font");
			current_name = ((game_mgr.char_ids[winner_id] == 0) ? "Jimmy" : ((game_mgr.char_ids[winner_id] == 1) ? "Sam" : ((game_mgr.char_ids[winner_id] == 2) ? "Erik" : "Billy")));
		}

		public void update()
		{
			if (controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.DPAD_LEFT) || controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.LTHUMB_LEFT))
			{
				cursor_char--;
			}
			if (controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.DPAD_RIGHT) || controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.LTHUMB_RIGHT))
			{
				cursor_char++;
			}
			if (controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.LTHUMB_UP))
			{
				cursor_char -= 9;
			}
			if (controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.LTHUMB_DOWN))
			{
				cursor_char += 9;
			}
			cursor_char = ((cursor_char < 0) ? 27 : ((cursor_char <= 27) ? cursor_char : 0));
			if (controllers.clicked(game_mgr.player_ids[winner_id], CONTROLLER_BUTTONS.A))
			{
				if (cursor_char < 26 && current_name.Length < 14)
				{
					current_name += (char)(((current_name.Length == 0) ? 65 : 97) + (ushort)cursor_char);
				}
				else if (cursor_char == 26 && current_name.Length > 0)
				{
					current_name = current_name.Remove(current_name.Length - 1);
				}
				else if (cursor_char == 27 && current_name.Length > 0)
				{
					list.insert_new(game_mgr.points[winner_id], current_name, (short)game_mgr.char_ids[winner_id]);
					is_done = true;
				}
			}
		}

		public void draw(SpriteBatch spr_batch)
		{
			spr_batch.Begin();
			spr_batch.Draw(bg, new Vector2(0f, 0f), Color.White);
			spr_batch.Draw(box, new Vector2(250f, 110f), Color.White);
			spr_batch.Draw(frames[winner_id], new Vector2(250f, 50f), Color.White);
			int num = ((game_mgr.char_ids[winner_id] > 0) ? game_mgr.char_ids[winner_id] : 0);
			spr_batch.Draw(faces[num], new Vector2(275 - faces[num].Width / 2, 75 - faces[num].Height / 2), Color.White);
			Vector2 vector = font.MeasureString(current_name);
			spr_batch.DrawString(font, current_name, new Vector2(342f - vector.X / 2f, 126f - vector.Y / 2f), Color.Green);
			if (cursor_char < 27)
			{
				spr_batch.Draw(cursor, new Vector2(183 - cursor.Width / 2 + cursor_char % 9 * 35, 289 - cursor.Height / 2 + cursor_char / 9 * 50), Color.White);
			}
			else
			{
				spr_batch.Draw(cursor, new Vector2(183 - cursor.Width / 2 + 315, 289 - cursor.Height / 2 + 100), Color.White);
			}
			spr_batch.End();
		}
	}

	private Texture2D bg;

	private Texture2D overlay;

	private Texture2D[] faces;

	private SpriteFont font;

	private entry name_entry;

	private highscore highscore_list;

	private bool done;

	public highscore_display(GraphicsDevice dev, IServiceProvider serv, highscore score)
		: base(dev, serv)
	{
		bg = content_mgr.Load<Texture2D>("minigame/gfx/results_bg");
		overlay = content_mgr.Load<Texture2D>("menu/highscore/box");
		faces = new Texture2D[4]
		{
			content_mgr.Load<Texture2D>("menu/name_entry/jimmy"),
			content_mgr.Load<Texture2D>("menu/name_entry/sam"),
			content_mgr.Load<Texture2D>("menu/name_entry/erik"),
			content_mgr.Load<Texture2D>("menu/name_entry/billy")
		};
		font = content_mgr.Load<SpriteFont>("default_font");
		highscore_list = score;
		name_entry = new entry(content_mgr, highscore_list);
	}

	~highscore_display()
	{
		free();
	}

	public override void free()
	{
		if (highscore_list != null)
		{
			highscore_list.save();
		}
		base.free();
	}

	public override GAME_STATE update()
	{
		base.update();
		if (!name_entry.done)
		{
			name_entry.update();
			return GAME_STATE.HIGHSCORE;
		}
		if (!done && controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			done = true;
			fade.random(90u, Color.Black);
		}
		if (!done || !fade.done)
		{
			return GAME_STATE.HIGHSCORE;
		}
		return GAME_STATE.MAIN_MENU;
	}

	private void draw_list()
	{
		spr_batch.Begin();
		spr_batch.Draw(bg, new Vector2(0f, 0f), Color.White);
		spr_batch.Draw(overlay, new Vector2(320 - overlay.Width / 2, 240 - overlay.Height / 2), Color.White);
		highscore.list list = highscore_list.get_list();
		Vector2 position = new Vector2(120f, 105f);
		for (int i = 0; i < list.names.Length; i++)
		{
			spr_batch.DrawString(font, list.names[i], position, Color.White);
			spr_batch.Draw(faces[list.face_id[i]], new Vector2(position.X + 180f - (float)(faces[list.face_id[i]].Width / 2), position.Y - 10f), Color.White);
			string text = list.scores[i].ToString();
			Vector2 vector = font.MeasureString(text);
			spr_batch.DrawString(font, text, new Vector2(position.X + 274f - vector.X / 2f, position.Y), Color.White);
			position.Y += 94f;
		}
		fade.draw(spr_batch);
		spr_batch.End();
	}

	public override void draw()
	{
		if (name_entry.done)
		{
			draw_list();
		}
		else
		{
			name_entry.draw(spr_batch);
		}
	}
}
