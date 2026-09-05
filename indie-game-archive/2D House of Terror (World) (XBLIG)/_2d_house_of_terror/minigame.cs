using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public abstract class minigame
{
	private class pause_menu
	{
		public enum ITEM
		{
			NOTHING = -1,
			RESUME,
			EXPLANATION,
			QUIT
		}

		private const int spacing = 15;

		private Texture2D bg;

		private Texture2D[][] items;

		private selection_arrow arrow;

		private int selected_item;

		private SoundEffect select_sfx;

		public pause_menu(ContentManager con_mgr)
		{
			bg = con_mgr.Load<Texture2D>("menu/pause/box");
			arrow = new selection_arrow(con_mgr.Load<Texture2D>("menu/main/arrow_right"), 0.2, 0.0, 8.0, 0.0);
			items = new Texture2D[3][]
			{
				new Texture2D[2]
				{
					con_mgr.Load<Texture2D>("menu/pause/resume_txt"),
					con_mgr.Load<Texture2D>("menu/pause/resume_txt_inactive")
				},
				new Texture2D[2]
				{
					con_mgr.Load<Texture2D>("menu/pause/explain_txt"),
					con_mgr.Load<Texture2D>("menu/pause/explain_txt_inactive")
				},
				new Texture2D[2]
				{
					con_mgr.Load<Texture2D>("menu/pause/quit_txt"),
					con_mgr.Load<Texture2D>("menu/pause/quit_txt_inactive")
				}
			};
			select_sfx = con_mgr.Load<SoundEffect>("sfx/select");
		}

		public ITEM update()
		{
			int num = selected_item;
			if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_UP))
			{
				selected_item--;
			}
			if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_DOWN))
			{
				selected_item++;
			}
			selected_item = ((selected_item < 0) ? (items.Length - 1) : (selected_item % items.Length));
			if (num != selected_item)
			{
				select_sfx.Play();
			}
			arrow.update();
			if (!controllers.clicked(CONTROLLER_BUTTONS.A) && !controllers.clicked(CONTROLLER_BUTTONS.START))
			{
				return ITEM.NOTHING;
			}
			return (ITEM)selected_item;
		}

		public void draw(SpriteBatch spr_batch, int x = 320, int y = 200)
		{
			spr_batch.Draw(bg, new Vector2(x - bg.Width / 2, y - bg.Height / 2), Color.White);
			y -= 30;
			for (int i = 0; i < items.Length; i++)
			{
				if (i == selected_item)
				{
					arrow.draw(spr_batch, x - (items[i][(i != selected_item) ? 1 : 0].Width / 2 + 30), y);
				}
				spr_batch.Draw(items[i][(i != selected_item) ? 1 : 0], new Vector2(x - items[i][(i != selected_item) ? 1 : 0].Width / 2, y), Color.White);
				y += items[i][(i != selected_item) ? 1 : 0].Height + 15;
			}
		}
	}

	private enum STATE
	{
		CONTROL_DISPLAY,
		GAME,
		RESULT_DISPLAY,
		FADE_OUT
	}

	private STATE minigame_state;

	protected ContentManager content_mgr;

	protected SpriteBatch spr_batch;

	protected fade_effect fade;

	private Texture2D[] ranking_imgs;

	private Texture2D[] char_imgs;

	private Texture2D controls_bg;

	private Texture2D results_bg;

	private Texture2D results_overall;

	private Texture2D results_minigame;

	private Vector2 controls_bg_pos;

	private Vector2 results_pos;

	private Texture2D face_frame;

	private sprite faceset;

	private bool controls_confirmed;

	private bool results_transition_to_minigame;

	private bool results_transition_to_overall;

	protected sprite controls_spr;

	protected Texture2D title_img;

	protected Texture2D preview_img;

	protected Texture2D start_txt;

	protected Texture2D timeup_txt;

	protected Texture2D winner_txt;

	protected SpriteFont default_font;

	protected SpriteFont default_font_large;

	protected SoundEffect start_sfx;

	protected SoundEffect finish_sfx;

	protected SoundEffect timeup_sfx;

	private Texture2D explanation_bg;

	protected Texture2D[] explanation_gfx;

	protected string[] explanation_txt;

	private int explanation_page;

	private bool explanation_visible;

	protected static int[] points;

	protected static int[] ranking;

	private bool paused;

	private pause_menu pause_men;

	private bool quit_instruction;

	public bool quit_instructed => quit_instruction;

	public minigame(IServiceProvider serv, GraphicsDevice gfx_dev, bool beginners_mode = false)
	{
		content_mgr = new ContentManager(serv);
		content_mgr.RootDirectory = "Content";
		spr_batch = new SpriteBatch(gfx_dev);
		fade = new fade_effect(gfx_dev);
		controls_bg = content_mgr.Load<Texture2D>("minigame/gfx/controls_bg");
		controls_bg_pos = new Vector2(gfx_dev.Viewport.Width - controls_bg.Width, 0f);
		results_bg = content_mgr.Load<Texture2D>("minigame/gfx/results_bg");
		results_overall = content_mgr.Load<Texture2D>("minigame/gfx/results_overall");
		results_minigame = content_mgr.Load<Texture2D>("minigame/gfx/results_minigame");
		results_pos = new Vector2(0f, 0f);
		ranking_imgs = new Texture2D[4]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/ranking_1"),
			content_mgr.Load<Texture2D>("minigame/gfx/ranking_2"),
			content_mgr.Load<Texture2D>("minigame/gfx/ranking_3"),
			content_mgr.Load<Texture2D>("minigame/gfx/ranking_4")
		};
		char_imgs = new Texture2D[4]
		{
			content_mgr.Load<Texture2D>("menu/main/character/jimmy"),
			content_mgr.Load<Texture2D>("menu/main/character/sam"),
			content_mgr.Load<Texture2D>("menu/main/character/erik"),
			content_mgr.Load<Texture2D>("menu/main/character/billy")
		};
		start_txt = content_mgr.Load<Texture2D>("minigame/gfx/start_txt");
		timeup_txt = content_mgr.Load<Texture2D>("minigame/gfx/timeup_txt");
		winner_txt = content_mgr.Load<Texture2D>("minigame/gfx/winner_txt");
		start_sfx = content_mgr.Load<SoundEffect>("minigame/sfx/start");
		finish_sfx = content_mgr.Load<SoundEffect>("minigame/sfx/finish");
		timeup_sfx = content_mgr.Load<SoundEffect>("minigame/sfx/timeup");
		face_frame = content_mgr.Load<Texture2D>("minigame/gfx/face_frame");
		faceset = new sprite(content_mgr.Load<Texture2D>("minigame/gfx/faceset"), 3u, 4u, 1u);
		minigame_state = STATE.CONTROL_DISPLAY;
		controls_confirmed = false;
		int[] array = new int[4];
		points = array;
		ranking = new int[4] { 0, 1, 2, 3 };
		fade.to_color(280u, Color.Black, fadein: true);
		default_font = content_mgr.Load<SpriteFont>("default_font");
		default_font_large = content_mgr.Load<SpriteFont>("default_font_large");
		pause_men = new pause_menu(content_mgr);
		explanation_bg = content_mgr.Load<Texture2D>("minigame/gfx/explanation/bg");
		explanation_visible = beginners_mode;
	}

	~minigame()
	{
		free();
	}

	public virtual void free()
	{
		content_mgr.Unload();
	}

	protected SoundEffect random_success_sfx()
	{
		int num = game_state.random_gen.Next() % 100;
		return content_mgr.Load<SoundEffect>("minigame/sfx/" + ((num < 10) ? "great" : ((num < 20) ? "i_did_it" : ((num < 30) ? "juhu" : ((num < 40) ? "juhu2" : ((num < 50) ? "yeah" : ((num < 60) ? "yeah2" : ((num < 70) ? "yeahaha" : ((num < 80) ? "yippie" : ((num < 90) ? "wuhu" : "wuhu2"))))))))));
	}

	protected SoundEffect random_fail_sfx()
	{
		int num = game_state.random_gen.Next() % 100;
		return content_mgr.Load<SoundEffect>("minigame/sfx/" + ((num < 50) ? "no" : "oh"));
	}

	public abstract bool update_game();

	public virtual bool update_controls_display()
	{
		if (controls_bg_pos.X < -955f)
		{
			controls_bg_pos.X += 1.5f;
		}
		else if (controls_confirmed && controls_bg_pos.X < -150f)
		{
			if (fade.done && controls_bg_pos.X >= -270f)
			{
				fade.to_color(120u, Color.Black);
			}
			controls_bg_pos.X += 4f;
			if (controls_bg_pos.X > -150f)
			{
				fade.done = true;
			}
		}
		else if (!controls_confirmed && controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			controls_confirmed = true;
		}
		controls_spr.animate();
		return controls_bg_pos.X > -150f;
	}

	public virtual bool update_result_display()
	{
		if (results_pos.X == 0f && controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			results_transition_to_overall = true;
		}
		if (results_pos.X == (float)(-spr_batch.GraphicsDevice.Viewport.Width) && controllers.clicked(CONTROLLER_BUTTONS.B))
		{
			results_transition_to_minigame = true;
		}
		if (results_pos.X == (float)(-spr_batch.GraphicsDevice.Viewport.Width) && controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			fade.cross(90u, 40u, Color.Black);
			return true;
		}
		if (results_transition_to_overall)
		{
			results_pos.X -= 6f;
			if (results_pos.X < (float)(-spr_batch.GraphicsDevice.Viewport.Width))
			{
				results_pos.X = -spr_batch.GraphicsDevice.Viewport.Width;
				results_transition_to_overall = false;
			}
		}
		if (results_transition_to_minigame)
		{
			results_pos.X += 6f;
			if (results_pos.X > 0f)
			{
				results_pos.X = 0f;
				results_transition_to_minigame = false;
			}
		}
		return false;
	}

	private void update_results()
	{
		for (int i = 0; i < points.Length; i++)
		{
			game_mgr.points[i] += points[i];
		}
		bool flag = false;
		int[] array = new int[4] { 0, 1, 2, 3 };
		do
		{
			flag = false;
			for (int j = 0; j < points.Length - 1; j++)
			{
				if (points[array[j]] < points[array[j + 1]])
				{
					int num = array[j];
					array[j] = array[j + 1];
					array[j + 1] = num;
					flag = true;
				}
			}
		}
		while (flag);
		for (int k = 0; k < 4; k++)
		{
			ranking[array[k]] = k;
		}
		array = new int[4] { 0, 1, 2, 3 };
		do
		{
			flag = false;
			for (int l = 0; l < game_mgr.points.Length - 1; l++)
			{
				if (game_mgr.points[array[l]] < game_mgr.points[array[l + 1]])
				{
					int num2 = array[l];
					array[l] = array[l + 1];
					array[l + 1] = num2;
					flag = true;
				}
			}
		}
		while (flag);
		for (int m = 0; m < 4; m++)
		{
			game_mgr.ranking[array[m]] = m;
		}
	}

	private void update_explanation()
	{
		if (controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			explanation_page++;
		}
		if (controllers.clicked(CONTROLLER_BUTTONS.B))
		{
			explanation_page--;
		}
		explanation_page = ((explanation_page < 0) ? (explanation_txt.Length - 1) : (explanation_page % explanation_txt.Length));
		if (controllers.clicked(CONTROLLER_BUTTONS.START))
		{
			explanation_page = 0;
			explanation_visible = false;
		}
	}

	public bool update()
	{
		if (explanation_visible)
		{
			update_explanation();
			return false;
		}
		if (!paused && controllers.clicked(CONTROLLER_BUTTONS.START) && minigame_state == STATE.GAME)
		{
			paused = !paused;
			return false;
		}
		if (paused)
		{
			switch (pause_men.update())
			{
			case pause_menu.ITEM.RESUME:
				paused = false;
				break;
			case pause_menu.ITEM.EXPLANATION:
				explanation_visible = true;
				return false;
			case pause_menu.ITEM.QUIT:
				quit_instruction = true;
				break;
			case pause_menu.ITEM.NOTHING:
				return false;
			}
		}
		switch (minigame_state)
		{
		case STATE.CONTROL_DISPLAY:
			if (update_controls_display())
			{
				minigame_state = STATE.GAME;
			}
			break;
		case STATE.GAME:
			if (update_game())
			{
				minigame_state = STATE.RESULT_DISPLAY;
				update_results();
			}
			break;
		case STATE.RESULT_DISPLAY:
			if (update_result_display())
			{
				minigame_state = STATE.FADE_OUT;
			}
			break;
		case STATE.FADE_OUT:
			fade.update();
			return fade.done;
		}
		fade.update();
		return false;
	}

	protected void draw_faces(SpriteBatch spr_batch)
	{
		Vector2 position = new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - face_frame.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2 - face_frame.Height / 2);
		spr_batch.Draw(face_frame, position, Color.White);
		for (int i = 0; i < game_mgr.player_ids.Length; i++)
		{
			faceset.state = (uint)game_mgr.char_ids[i];
			faceset.frame = (uint)game_mgr.moods[i];
			faceset.draw(spr_batch, (int)(position.X + (float)((i % 2 * 2 + 1) * face_frame.Width / 4)), (int)(position.Y + (float)((i / 2 * 2 + 1) * face_frame.Height / 4)));
		}
	}

	public abstract void draw_game();

	public virtual void draw_controls_display()
	{
		if (controls_bg_pos.X < -320f)
		{
			spr_batch.Draw(controls_bg, controls_bg_pos, Color.White);
		}
		else
		{
			float num = controls_bg_pos.X / -320f;
			spr_batch.Draw(controls_bg, new Rectangle(0, 0, 1280, 480), new Rectangle((int)(0f - controls_bg_pos.X), 0, (int)(1280f * num), (int)(480f * num)), Color.White);
		}
		controls_spr.draw(spr_batch, (int)controls_bg_pos.X + controls_bg.Width - 795, (int)controls_bg_pos.Y + 240);
		spr_batch.Draw(title_img, new Vector2(controls_bg_pos.X + (float)controls_bg.Width - (float)(title_img.Width / 2) - 460f, controls_bg_pos.Y + 75f), Color.White);
		spr_batch.Draw(preview_img, new Vector2(controls_bg_pos.X + (float)controls_bg.Width - 600f, controls_bg_pos.Y + 240f), Color.White);
	}

	public virtual void draw_result_display()
	{
		spr_batch.Draw(results_bg, new Vector2(0f, 0f), Color.White);
		spr_batch.Draw(results_minigame, results_pos, Color.White);
		spr_batch.Draw(results_overall, new Vector2(results_pos.X + (float)spr_batch.GraphicsDevice.Viewport.Width, results_pos.Y), Color.White);
		Vector2 vector = new Vector2(150f, 95f);
		Vector2 vector2 = new Vector2(spr_batch.GraphicsDevice.Viewport.Width, 0f) + vector;
		vector += results_pos;
		vector2 += results_pos;
		Vector2 vector3 = new Vector2(0f, 90f);
		int num = 0;
		while (num < ranking.Length)
		{
			Texture2D texture2D = char_imgs[game_mgr.char_ids[num]];
			spr_batch.Draw(ranking_imgs[ranking[num]], vector, Color.White);
			spr_batch.Draw(texture2D, vector + new Vector2(150 - texture2D.Width / 2, 40 - texture2D.Height), Color.White);
			spr_batch.Draw(ranking_imgs[game_mgr.ranking[num]], vector2, Color.White);
			spr_batch.Draw(char_imgs[game_mgr.char_ids[num]], vector2 + new Vector2(150 - texture2D.Width / 2, 40 - texture2D.Height), Color.White);
			string text = Convert.ToString(points[num]);
			Vector2 vector4 = default_font.MeasureString(text);
			spr_batch.DrawString(default_font, text, vector + new Vector2(265f, 17f), new Color(0, 255, 0), 0f, new Vector2(vector4.X / 2f, vector4.Y / 2f), 1.2f, SpriteEffects.None, 0f);
			text = Convert.ToString(game_mgr.points[num]);
			vector4 = default_font.MeasureString(text);
			spr_batch.DrawString(default_font, text, vector2 + new Vector2(265f, 17f), new Color(0, 255, 0), 0f, new Vector2(vector4.X / 2f, vector4.Y / 2f), 1.2f, SpriteEffects.None, 0f);
			num++;
			vector += vector3;
			vector2 += vector3;
		}
	}

	public virtual void draw_explanation()
	{
		spr_batch.Draw(explanation_bg, new Vector2(0f, 0f), Color.White);
		float num = 320f;
		float num2 = 50f;
		spr_batch.Draw(explanation_gfx[explanation_page], new Rectangle((int)num, (int)num2, explanation_gfx[explanation_page].Width, explanation_gfx[explanation_page].Height), new Rectangle(0, 0, explanation_gfx[explanation_page].Width, explanation_gfx[explanation_page].Height), Color.White, 0f, new Vector2(explanation_gfx[explanation_page].Width / 2, 0f), SpriteEffects.None, 0f);
		Vector2 vector = default_font.MeasureString(explanation_txt[explanation_page]);
		vector *= 0.85714287f;
		spr_batch.DrawString(default_font, explanation_txt[explanation_page], new Vector2(num - 50f, num2 + (float)explanation_gfx[explanation_page].Height), Color.White, 0f, new Vector2(vector.X / 2f, 0f), 0.85714287f, SpriteEffects.None, 0f);
	}

	public void draw()
	{
		spr_batch.Begin();
		switch (minigame_state)
		{
		case STATE.CONTROL_DISPLAY:
			draw_controls_display();
			break;
		case STATE.GAME:
			draw_game();
			break;
		default:
			draw_result_display();
			break;
		}
		fade.draw(spr_batch);
		if (paused)
		{
			pause_men.draw(spr_batch);
		}
		if (explanation_visible)
		{
			draw_explanation();
		}
		spr_batch.End();
	}
}
