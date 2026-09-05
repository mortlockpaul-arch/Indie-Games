using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class spiritual_ascension : minigame
{
	private class ghost
	{
		private enum STATE
		{
			SLEEP,
			APPEAR,
			IDLE,
			ATTACK,
			DISAPPEAR
		}

		private const int appear_propability = 2;

		private const int disappear_propability = 10;

		private const int attack_propability = 20;

		private STATE current_state;

		private sprite[] sprites;

		private bool orientation_right = true;

		private SoundEffect attack_sfx;

		private Vector2 position;

		public ghost(ContentManager content_mgr, int initial_x, int initial_y, bool orient_right = true)
		{
			orientation_right = orient_right;
			attack_sfx = content_mgr.Load<SoundEffect>("minigame/spirit_asc/sfx/ghost_attack");
			sprites = new sprite[2]
			{
				new sprite(content_mgr.Load<Texture2D>("minigame/spirit_asc/sprites/ghost/appear"), 5u, 1u, 6u),
				new sprite(content_mgr.Load<Texture2D>("minigame/spirit_asc/sprites/ghost/attack"), 7u, 1u, 7u)
			};
			position = new Vector2(initial_x, initial_y);
			current_state = STATE.SLEEP;
		}

		public bool hits(Rectangle target_rect)
		{
			if (current_state != STATE.ATTACK)
			{
				return false;
			}
			return target_rect.Intersects(new Rectangle((int)(orientation_right ? position.X : (position.X - (float)sprites[1].width)), (int)(position.Y - (float)(sprites[1].height / 2)), sprites[1].width, sprites[1].height));
		}

		public void update()
		{
			switch (current_state)
			{
			case STATE.SLEEP:
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 2)
				{
					current_state = STATE.APPEAR;
				}
				break;
			case STATE.APPEAR:
				sprites[0].animate();
				if (sprites[0].done)
				{
					current_state = STATE.IDLE;
				}
				break;
			case STATE.DISAPPEAR:
				sprites[0].animate_cyclic();
				if (sprites[0].cycle_finished)
				{
					current_state = STATE.SLEEP;
				}
				break;
			case STATE.ATTACK:
				sprites[1].animate_cyclic();
				if (sprites[1].cycle_finished)
				{
					current_state = STATE.IDLE;
				}
				break;
			case STATE.IDLE:
				sprites[0].animate_cyclic(3u, 4u);
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 20)
				{
					if (_2d_house_of_terror.game_state.random_gen.Next() % 10 == 1)
					{
						attack_sfx.Play();
					}
					current_state = STATE.ATTACK;
				}
				else if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 10)
				{
					current_state = STATE.DISAPPEAR;
				}
				break;
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y, float zoom)
		{
			sprite sprite2 = sprites[(current_state == STATE.ATTACK) ? 1u : 0u];
			float zoom2 = sprite2.zoom;
			sprite2.zoom = zoom;
			sprite2.draw(spr_batch, (int)((float)offset_x + position.X * zoom + (float)(orientation_right ? (sprite2.width / 2) : (-sprite2.width / 2))), (int)((float)offset_y + position.Y * zoom), orientation_right);
			sprite2.zoom = zoom2;
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y)
		{
			sprite sprite2 = sprites[(current_state == STATE.ATTACK) ? 1u : 0u];
			sprite2.draw(spr_batch, (int)((float)offset_x + position.X + (float)(orientation_right ? (sprite2.width / 2) : (-sprite2.width / 2))), (int)((float)offset_y + position.Y), orientation_right);
		}
	}

	private class player
	{
		private enum STATE
		{
			IDLE,
			FALLING,
			CLIMBING
		}

		private const float fall_acceleration = 0.4f;

		private const float max_fall = 90f;

		private const float max_speed = 20f;

		private const float climb_speed = 0.6f;

		private const float initial_jump_speed = 4f;

		private const float jump_speed = 8f;

		private const uint max_idle_counter = 20u;

		private STATE current_state;

		private Vector2 pos;

		private sprite[] sprites;

		private SoundEffect hit_sfx;

		private SoundEffect goal_sfx;

		private bool end_of_ladder;

		private float fall_count;

		private float fall_speed;

		private float jump_count;

		private uint idle_counter;

		private bool last_click_left;

		private uint pl_id;

		private bool ai_controlled;

		private uint ladder_id;

		public Vector2 position
		{
			get
			{
				return new Vector2(pos.X, pos.Y);
			}
			set
			{
				pos = new Vector2(value.X, value.Y);
			}
		}

		public uint current_ladder
		{
			get
			{
				return ladder_id;
			}
			set
			{
				float num = pos.X / (float)(ladder_id + 1);
				ladder_id = value;
				pos.X = num * (float)(ladder_id + 1);
			}
		}

		public bool ladder_end
		{
			set
			{
				end_of_ladder = value;
			}
		}

		public Rectangle collision_rect
		{
			get
			{
				sprite sprite2 = sprites[(current_state == STATE.FALLING) ? 1u : 0u];
				return new Rectangle((int)(pos.X - (float)(sprite2.width / 2)), (int)(pos.Y - (float)(sprite2.height / 2)), sprite2.width, sprite2.height);
			}
		}

		public uint player_id
		{
			get
			{
				return pl_id;
			}
			set
			{
				pl_id = value % 4;
				ai_controlled = game_mgr.player_ids[pl_id] < 0;
			}
		}

		public player(ContentManager content_mgr, string gfx_folder, uint player_id)
		{
			pl_id = player_id;
			ai_controlled = game_mgr.player_ids[pl_id] < 0;
			current_state = STATE.IDLE;
			current_ladder = 0u;
			int num = _2d_house_of_terror.game_state.random_gen.Next() % 100;
			hit_sfx = content_mgr.Load<SoundEffect>("minigame/sfx/oh");
			goal_sfx = content_mgr.Load<SoundEffect>("minigame/sfx/" + ((num < 10) ? "great" : ((num < 20) ? "i_did_it" : ((num < 30) ? "juhu" : ((num < 40) ? "juhu2" : ((num < 50) ? "yeah" : ((num < 60) ? "yeah2" : ((num < 70) ? "yeahaha" : ((num < 80) ? "yippie" : ((num < 90) ? "wuhu" : "wuhu2"))))))))));
			sprites = new sprite[2]
			{
				new sprite(content_mgr.Load<Texture2D>(gfx_folder + "/climb"), 3u, 1u, 6u),
				new sprite(content_mgr.Load<Texture2D>(gfx_folder + "/fall"), 1u, 1u, 2u)
			};
			pos = new Vector2(0f, 0f);
		}

		public void shock()
		{
			if (current_state != STATE.FALLING)
			{
				hit_sfx.Play();
				current_state = STATE.FALLING;
				fall_speed = 0f;
			}
			fall_count = 90f;
		}

		private void jump_left()
		{
			jump_count = pos.X / (float)(ladder_id + 1);
			ladder_id--;
			current_state = STATE.FALLING;
			fall_speed = -4f;
		}

		private void jump_right()
		{
			jump_count = (0f - pos.X) / (float)(ladder_id + 1);
			ladder_id++;
			current_state = STATE.FALLING;
			fall_speed = -4f;
		}

		public void update()
		{
			switch (current_state)
			{
			case STATE.IDLE:
				if (ai_controlled)
				{
					current_state = STATE.CLIMBING;
					if (end_of_ladder)
					{
						if (ladder_id == 0 || (ladder_id != 2 && _2d_house_of_terror.game_state.random_gen.Next() % 2 == 1))
						{
							jump_right();
						}
						else
						{
							jump_left();
						}
					}
				}
				else if ((!last_click_left && controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LEFT_SHOULDER)) || (last_click_left && controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.RIGHT_SHOULDER)))
				{
					idle_counter = 0u;
					last_click_left = !last_click_left;
					current_state = STATE.CLIMBING;
				}
				else if (ladder_id != 0 && (controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.DPAD_LEFT) || controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LTHUMB_LEFT)))
				{
					jump_left();
				}
				else if (ladder_id < 2 && (controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.DPAD_RIGHT) || controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LTHUMB_RIGHT)))
				{
					jump_right();
				}
				break;
			case STATE.CLIMBING:
				if (end_of_ladder)
				{
					current_state = STATE.IDLE;
					break;
				}
				if (!ai_controlled)
				{
					if ((last_click_left && controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.RIGHT_SHOULDER)) || (!last_click_left && controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LEFT_SHOULDER)))
					{
						idle_counter = 0u;
						last_click_left = !last_click_left;
					}
					else if (idle_counter++ >= 20)
					{
						current_state = STATE.IDLE;
					}
				}
				sprites[0].animate_cyclic();
				pos.Y -= 0.6f;
				if (collision_rect.Top <= 0)
				{
					goal_sfx.Play();
				}
				break;
			case STATE.FALLING:
				fall_speed += 0.4f;
				fall_speed = ((fall_speed > 90f) ? 20f : fall_speed);
				pos.Y += fall_speed;
				fall_count -= fall_speed;
				if (Math.Abs(jump_count) > 8f)
				{
					pos.X += ((jump_count < 0f) ? 8f : (-8f));
					jump_count += ((jump_count < 0f) ? 8f : (-8f));
					break;
				}
				if (jump_count != 0f)
				{
					pos.X -= jump_count;
					jump_count = 0f;
				}
				if (fall_count <= 0f && !end_of_ladder)
				{
					current_state = STATE.IDLE;
				}
				break;
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y)
		{
			sprites[(current_state == STATE.FALLING) ? 1u : 0u].draw(spr_batch, (int)((float)offset_x + pos.X), (int)((float)offset_y + pos.Y));
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y, float zoom)
		{
			float zoom2 = sprites[(current_state == STATE.FALLING) ? 1u : 0u].zoom;
			sprites[(current_state == STATE.FALLING) ? 1u : 0u].zoom = zoom;
			sprites[(current_state == STATE.FALLING) ? 1u : 0u].draw(spr_batch, (int)((float)offset_x + pos.X * zoom), (int)((float)offset_y + pos.Y * zoom));
			sprites[(current_state == STATE.FALLING) ? 1u : 0u].zoom = zoom2;
		}
	}

	private class track
	{
		private const uint number_of_ladder_types = 4u;

		private RasterizerState raster_state;

		private int field_w;

		private int field_h;

		private Texture2D bg;

		private Texture2D ladder_sheet;

		private Texture2D goal_flag;

		private Vector2 scroll_pos;

		private int[][] ladders;

		private ghost[] ghosts;

		private player player;

		public int[][] ladder_track
		{
			get
			{
				return ladders;
			}
			set
			{
				ladders = value;
			}
		}

		public bool completed => player.collision_rect.Top <= 0;

		public int length => ladders[0].Length * ladder_sheet.Height - ladder_sheet.Height / 2 - field_h / 2;

		public track(ContentManager content_mgr, int field_width, int field_height, player pl)
		{
			raster_state = new RasterizerState
			{
				ScissorTestEnable = true
			};
			field_w = field_width;
			field_h = field_height;
			bg = content_mgr.Load<Texture2D>("minigame/spirit_asc/gfx/bg");
			ladder_sheet = content_mgr.Load<Texture2D>("minigame/spirit_asc/gfx/ladder");
			goal_flag = content_mgr.Load<Texture2D>("minigame/spirit_asc/gfx/flag");
			scroll_pos = new Vector2(0f, 0f);
			ladders = new int[3][]
			{
				new int[15]
				{
					-1, 1, 1, -1, 1, 2, 2, 2, 2, 0,
					0, 0, 0, 0, 0
				},
				new int[15]
				{
					-1, 0, 1, 1, -1, 2, 2, 2, 2, 0,
					0, 0, 0, 0, 0
				},
				new int[15]
				{
					0, 0, 1, 1, 1, -2, 2, 2, 2, 0,
					0, 0, 0, 0, 0
				}
			};
			randomize_track();
			ghosts = new ghost[30];
			for (int i = 0; i < 30; i++)
			{
				ghosts[i] = new ghost(content_mgr, 30 + i % 2 * (field_w - 60), i * 80, i % 2 < 1);
			}
			player = pl;
			pl.position = new Vector2(field_w / 4, ladders[0].Length * ladder_sheet.Height - ladder_sheet.Height / 2);
			update_scrolling();
		}

		~track()
		{
		}

		private void randomize_track()
		{
			for (int num = ladders[0].Length - 2; num > 0; num--)
			{
				int num2 = 0;
				for (int i = 0; i < ladders.Length; i++)
				{
					ladders[i][num] = (int)((long)_2d_house_of_terror.game_state.random_gen.Next() % 4L);
					if (_2d_house_of_terror.game_state.random_gen.Next() % 10 < 8 && num2 < 2)
					{
						ladders[i][num] = -1;
						num2++;
					}
				}
			}
			ladders[1][1] = (int)((long)_2d_house_of_terror.game_state.random_gen.Next() % 4L);
			for (int num3 = ladders[0].Length - 2; num3 > 0; num3--)
			{
				if (ladders[1][num3] < 0 && ladders[1][num3 + 1] < 0)
				{
					ladders[1][num3] = (int)((long)_2d_house_of_terror.game_state.random_gen.Next() % 4L);
				}
			}
		}

		private void update_scrolling()
		{
			scroll_pos.Y = 0f - player.position.Y + (float)(field_h / 2);
		}

		private void update_ghosts()
		{
			for (int i = 0; i < ghosts.Length; i++)
			{
				ghosts[i].update();
				if (ghosts[i].hits(player.collision_rect))
				{
					player.shock();
				}
			}
		}

		public void update()
		{
			if (!completed)
			{
				update_ghosts();
				player.update();
				int num = (int)(player.position.Y / (float)ladder_sheet.Height);
				player.ladder_end = num >= ladders[player.current_ladder].Length || player.position.Y < 0f || ladders[player.current_ladder][num] < 0;
				update_scrolling();
			}
		}

		private void draw_bg(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom, Vector2 scroll_val)
		{
			Vector2 vector = new Vector2(scroll_val.X % (float)field_w, scroll_val.Y % (float)field_h);
			vector *= zoom;
			spr_batch.Draw(bg, new Rectangle((int)((float)dest_x + vector.X), (int)((float)dest_y + vector.Y), (int)((float)bg.Width * zoom), (int)((float)bg.Height * zoom)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
			spr_batch.Draw(bg, new Rectangle((int)((float)dest_x + vector.X), (int)((float)dest_y + ((vector.Y > 0f) ? (vector.Y - (float)field_h * zoom) : (vector.Y + (float)field_h * zoom))), (int)((float)bg.Width * zoom), (int)((float)bg.Height * zoom)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
			spr_batch.Draw(bg, new Rectangle((int)((float)dest_x + ((vector.X > 0f) ? (vector.X - (float)field_w * zoom) : (vector.X + (float)field_w * zoom))), (int)((float)dest_y + vector.Y), (int)((float)bg.Width * zoom), (int)((float)bg.Height * zoom)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
			spr_batch.Draw(bg, new Rectangle((int)((float)dest_x + ((vector.X > 0f) ? (vector.X - (float)field_w * zoom) : (vector.X + (float)field_w * zoom))), (int)((float)dest_y + ((vector.Y > 0f) ? (vector.Y - (float)field_h * zoom) : (vector.Y + (float)field_h * zoom))), (int)((float)bg.Width * zoom), (int)((float)bg.Height * zoom)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
		}

		private void draw_bg(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			Vector2 vector = new Vector2(scroll_pos.X % (float)field_w, scroll_pos.Y % (float)field_h);
			spr_batch.Draw(bg, new Vector2((float)dest_x + vector.X, (float)dest_y + vector.Y), Color.White);
			spr_batch.Draw(bg, new Vector2((float)dest_x + vector.X, (float)dest_y + ((vector.Y > 0f) ? (vector.Y - (float)field_h) : (vector.Y + (float)field_h))), Color.White);
			spr_batch.Draw(bg, new Vector2((float)dest_x + ((vector.X > 0f) ? (vector.X - (float)field_w) : (vector.X + (float)field_w)), (float)dest_y + vector.Y), Color.White);
			spr_batch.Draw(bg, new Vector2((float)dest_x + ((vector.X > 0f) ? (vector.X - (float)field_w) : (vector.X + (float)field_w)), (float)dest_y + ((vector.Y > 0f) ? (vector.Y - (float)field_h) : (vector.Y + (float)field_h))), Color.White);
		}

		private void draw_ladders(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom, Vector2 scroll_val)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < ladders[i].Length; j++)
				{
					if (ladders[i][j] >= 0)
					{
						spr_batch.Draw(ladder_sheet, new Rectangle((int)((float)dest_x + scroll_val.X * zoom + (float)((1 + i) * field_w) * zoom / 4f - (float)ladder_sheet.Width * zoom / 8f), (int)((float)dest_y + scroll_val.Y * zoom + (float)(j * ladder_sheet.Height) * zoom), (int)((float)ladder_sheet.Width * zoom / 4f), (int)((float)ladder_sheet.Height * zoom)), new Rectangle(ladders[i][j] * (int)((long)ladder_sheet.Width / 4L), 0, (int)((long)ladder_sheet.Width / 4L), ladder_sheet.Height), Color.White);
					}
				}
			}
		}

		private void draw_ladders(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < ladders[i].Length; j++)
				{
					if (ladders[i][j] >= 0)
					{
						spr_batch.Draw(ladder_sheet, new Rectangle((int)((float)dest_x + scroll_pos.X + (float)((1 + i) * field_w / 4) - (float)((long)ladder_sheet.Width / 8L)), (int)((float)dest_y + scroll_pos.Y + (float)(j * ladder_sheet.Height)), (int)((long)ladder_sheet.Width / 4L), ladder_sheet.Height), new Rectangle(ladders[i][j] * (int)((long)ladder_sheet.Width / 4L), 0, (int)((long)ladder_sheet.Width / 4L), ladder_sheet.Height), Color.White);
					}
				}
			}
		}

		private void draw_ghosts(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom, Vector2 scroll_val)
		{
			for (int i = 0; i < ghosts.Length; i++)
			{
				ghosts[i].draw(spr_batch, (int)((float)dest_x + scroll_val.X * zoom), (int)((float)dest_y + scroll_val.Y * zoom), zoom);
			}
		}

		private void draw_ghosts(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			for (int i = 0; i < ghosts.Length; i++)
			{
				ghosts[i].draw(spr_batch, (int)((float)dest_x + scroll_pos.X), (int)((float)dest_y + scroll_pos.Y));
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			spr_batch.End();
			Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
			spr_batch.GraphicsDevice.ScissorRectangle = new Rectangle(dest_x, dest_y, field_w, field_h);
			spr_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, raster_state);
			draw_bg(spr_batch, dest_x, dest_y);
			draw_ladders(spr_batch, dest_x, dest_y);
			spr_batch.Draw(goal_flag, new Vector2((float)dest_x + scroll_pos.X + (float)(3 * field_w / 4) - (float)goal_flag.Width + (float)((long)ladder_sheet.Width / 8L), (float)dest_y + scroll_pos.Y), Color.White);
			player.draw(spr_batch, (int)((float)dest_x + scroll_pos.X), (int)((float)dest_y + scroll_pos.Y));
			draw_ghosts(spr_batch, dest_x, dest_y);
			spr_batch.End();
			spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
			spr_batch.Begin();
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom, Vector2 scroll_val)
		{
			if (dest_x <= spr_batch.GraphicsDevice.Viewport.Width && dest_y <= spr_batch.GraphicsDevice.Viewport.Height)
			{
				spr_batch.End();
				Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
				Rectangle scissorRectangle2 = new Rectangle(dest_x, dest_y, (int)((float)field_w * zoom), (int)((float)field_h * zoom));
				if (scissorRectangle2.X < 0)
				{
					scissorRectangle2.Width += scissorRectangle2.X;
					scissorRectangle2.X = 0;
				}
				if (scissorRectangle2.Y < 0)
				{
					scissorRectangle2.Height += scissorRectangle2.Y;
					scissorRectangle2.Y = 0;
				}
				scissorRectangle2.Width = ((scissorRectangle2.X + scissorRectangle2.Width > spr_batch.GraphicsDevice.Viewport.Width) ? (spr_batch.GraphicsDevice.Viewport.Width - scissorRectangle2.X) : scissorRectangle2.Width);
				scissorRectangle2.Height = ((scissorRectangle2.Y + scissorRectangle2.Height > spr_batch.GraphicsDevice.Viewport.Height) ? (spr_batch.GraphicsDevice.Viewport.Height - scissorRectangle2.Y) : scissorRectangle2.Height);
				spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spr_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, raster_state);
				draw_bg(spr_batch, dest_x, dest_y, zoom, scroll_val);
				draw_ladders(spr_batch, dest_x, dest_y, zoom, scroll_val);
				spr_batch.Draw(goal_flag, new Rectangle((int)((float)dest_x + scroll_val.X * zoom + (float)(3 * field_w) * zoom / 4f - (float)goal_flag.Width * zoom + (float)ladder_sheet.Width * zoom / 8f), (int)((float)dest_y + scroll_val.Y * zoom), (int)((float)goal_flag.Width * zoom), (int)((float)goal_flag.Height * zoom)), new Rectangle(0, 0, goal_flag.Width, goal_flag.Height), Color.White);
				player.draw(spr_batch, (int)((float)dest_x + scroll_val.X * zoom), (int)((float)dest_y + scroll_val.Y * zoom), zoom);
				draw_ghosts(spr_batch, dest_x, dest_y, zoom, scroll_val);
				spr_batch.End();
				spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
				spr_batch.Begin();
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom)
		{
			draw(spr_batch, dest_x, dest_y, zoom, scroll_pos);
		}
	}

	private enum STATE
	{
		INTRO,
		GAME,
		OUTRO
	}

	private const int max_time = 150;

	private const int max_intro_scroll_counter = 400;

	private const int max_intro_zoom_counter = 520;

	private const int max_intro_announce_counter = 580;

	private const int max_intro_counter = 700;

	private const int max_outro_text_counter = 60;

	private const int max_outro_zoom_counter = 120;

	private const int max_outro_counter = 180;

	private STATE game_state;

	private track[] tracks;

	private int[] completion_times;

	private player[] players;

	private clock timer;

	private Texture2D border_v;

	private Texture2D border_h;

	private Texture2D fog;

	private float fog_pos;

	private Song bgm;

	private int intro_counter;

	private int outro_counter;

	private int outro_points_counter;

	private int winner_id = -1;

	public spiritual_ascension(IServiceProvider serv, GraphicsDevice dev, bool beginners_mode = false)
		: base(serv, dev, beginners_mode)
	{
		controls_spr = new sprite(content_mgr.Load<Texture2D>("minigame/spirit_asc/controls"), 1u, 1u, 1u);
		title_img = content_mgr.Load<Texture2D>("minigame/spirit_asc/title");
		preview_img = content_mgr.Load<Texture2D>("minigame/spirit_asc/preview");
		bgm = content_mgr.Load<Song>("bgm/spirit_asc");
		border_v = content_mgr.Load<Texture2D>("minigame/gfx/border_vertical");
		border_h = content_mgr.Load<Texture2D>("minigame/gfx/border_horizontal");
		fog = content_mgr.Load<Texture2D>("menu/main/fog");
		fog_pos = dev.Viewport.Width;
		timer = new clock(content_mgr.Load<Texture2D>("minigame/gfx/clock"), default_font, 45, 76, new Color(75, 64, 21), 0, count_down: false);
		players = new player[4]
		{
			new player(content_mgr, "minigame/spirit_asc/sprites/jimmy", 0u),
			new player(content_mgr, "minigame/spirit_asc/sprites/sam", 1u),
			new player(content_mgr, "minigame/spirit_asc/sprites/erik", 2u),
			new player(content_mgr, "minigame/spirit_asc/sprites/billy", 3u)
		};
		for (int i = 0; i < 4; i++)
		{
			players[game_mgr.char_ids[i]].player_id = (uint)i;
		}
		tracks = new track[4]
		{
			new track(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, players[game_mgr.char_ids[0]]),
			new track(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, players[game_mgr.char_ids[1]]),
			new track(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, players[game_mgr.char_ids[2]]),
			new track(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, players[game_mgr.char_ids[3]])
		};
		completion_times = new int[4] { -1, -1, -1, -1 };
		for (int j = 1; j < 4; j++)
		{
			tracks[j].ladder_track = tracks[0].ladder_track;
		}
		game_state = STATE.INTRO;
		explanation_gfx = new Texture2D[5]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/spiritual_ascension/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/spiritual_ascension/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/spiritual_ascension/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/spiritual_ascension/3"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/spiritual_ascension/3")
		};
		explanation_txt = new string[5] { "  You and your friends try to\n    leave the cellar to look\n    for more clues, so you\n    try climbing the only way up:\n  dangerous, partially broken ladders.", "You have to reach the top\nin less than 150 seconds,\nthe faster the better.\nYour score is the remaining\ntime multiplied by 10.", "However you are not alone.\nGhosts try to scare you\nand prevent you from\nreaching the top.\n    If they touch you, you fall.", "First the ghosts disguise\nthemselfs as small balls and\nyou have no need to fear them.\nYet, once you can clearly\nsee their shape\nthey start to be dangerous.", "They wont hurt you without\npunshing, however in this state\nthey may punsh any time.\nSometimes you can and should\nescape the ghosts by jumping\nto one of the other ladders." };
	}

	~spiritual_ascension()
	{
		free();
	}

	public override void free()
	{
		MediaPlayer.Stop();
		base.free();
	}

	private void update_fog()
	{
		fog_pos -= 0.5f;
		fog_pos %= fog.Width;
	}

	private bool update_gameplay()
	{
		timer.update();
		update_fog();
		bool flag = true;
		for (int i = 0; i < tracks.Length; i++)
		{
			tracks[i].update();
			if (!tracks[i].completed)
			{
				flag = false;
			}
			else if (completion_times[i] < 0)
			{
				completion_times[i] = timer.seconds;
			}
		}
		if (!flag)
		{
			return timer.seconds >= 150;
		}
		return true;
	}

	private bool update_intro()
	{
		intro_counter++;
		if (intro_counter == 520)
		{
			start_sfx.Play();
		}
		return intro_counter > 700;
	}

	private bool update_outro()
	{
		update_fog();
		if (outro_counter > 60 && winner_id >= 0 && outro_points_counter < minigame.points[winner_id])
		{
			outro_points_counter++;
		}
		else
		{
			if (outro_counter == 61)
			{
				random_success_sfx().Play();
			}
			else if (outro_counter == 120)
			{
				fade.random(60u, Color.Black);
			}
			outro_counter++;
		}
		if (outro_counter <= 180)
		{
			if (outro_counter > 60)
			{
				return winner_id < 0;
			}
			return false;
		}
		return true;
	}

	public override bool update_game()
	{
		switch (game_state)
		{
		case STATE.INTRO:
			if (update_intro())
			{
				MediaPlayer.IsRepeating = true;
				MediaPlayer.Play(bgm);
				game_state = STATE.GAME;
			}
			break;
		case STATE.GAME:
		{
			if (!update_gameplay())
			{
				break;
			}
			game_state = STATE.OUTRO;
			int num = 0;
			for (int i = 0; i < minigame.points.Length; i++)
			{
				minigame.points[i] = ((completion_times[i] > 0) ? ((150 - completion_times[i]) * 10) : 0);
				if (minigame.points[i] > num)
				{
					winner_id = i;
					num = minigame.points[i];
				}
			}
			if (timer.seconds < 150)
			{
				finish_sfx.Play();
			}
			else
			{
				timeup_sfx.Play();
			}
			break;
		}
		case STATE.OUTRO:
			return update_outro();
		}
		return false;
	}

	private void draw_fog()
	{
		spr_batch.Draw(fog, new Vector2(fog_pos, 0f), Color.White);
		spr_batch.Draw(fog, new Vector2(fog_pos + (float)fog.Width, 0f), Color.White);
	}

	private void draw_intro()
	{
		if (intro_counter <= 400)
		{
			tracks[0].draw(spr_batch, 0, 0, 2f, new Vector2(0f, (float)(-tracks[0].length * 2 / 3) - (float)(tracks[0].length / 3) * ((float)intro_counter / 400f)));
			return;
		}
		if (intro_counter <= 520)
		{
			float num = 2f - (float)(intro_counter - 400) / 120f;
			tracks[0].draw(spr_batch, 0, 0, num, new Vector2(0f, -tracks[0].length));
			tracks[1].draw(spr_batch, (int)(num * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), 0, num, new Vector2(0f, -tracks[0].length));
			tracks[2].draw(spr_batch, 0, (int)(num * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f), 2f - (float)(intro_counter - 400) / 120f, new Vector2(0f, -tracks[0].length));
			tracks[3].draw(spr_batch, (int)(num * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), (int)(num * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f), 2f - (float)(intro_counter - 400) / 120f, new Vector2(0f, -tracks[0].length));
			return;
		}
		float num2 = (float)(intro_counter - 520) / 60f;
		num2 = ((num2 > 1f) ? 1f : num2);
		float num3 = 4f - 3f * num2;
		for (int i = 0; i < 4; i++)
		{
			tracks[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
			spr_batch.Draw(start_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num3), (int)((float)start_txt.Height * num3)), new Rectangle(0, 0, start_txt.Width, start_txt.Height), Color.White, 0f, new Vector2(start_txt.Width / 2, start_txt.Height / 2), SpriteEffects.None, 0f);
		}
		spr_batch.Draw(border_h, new Vector2((float)(-border_h.Width) * (1f - num2), spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
		spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, (float)(-border_v.Height) * (1f - num2)), Color.White);
		timer.zoom = 5f - 4f * num2;
		timer.draw(spr_batch, (int)((float)(spr_batch.GraphicsDevice.Viewport.Width / 2) + (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), (int)((float)(spr_batch.GraphicsDevice.Viewport.Height / 2) - (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f));
	}

	private void draw_outro()
	{
		if (outro_counter <= 60)
		{
			float num = 2f - (float)outro_counter / 60f;
			for (int i = 0; i < 4; i++)
			{
				tracks[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
				if (timer.seconds >= 150)
				{
					spr_batch.Draw(timeup_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num), (int)((float)start_txt.Height * num)), new Rectangle(0, 0, timeup_txt.Width, timeup_txt.Height), Color.White, 0f, new Vector2(timeup_txt.Width / 2, timeup_txt.Height / 2), SpriteEffects.None, 0f);
				}
			}
			draw_fog();
			spr_batch.Draw(border_h, new Vector2(0f, spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
			spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, 0f), Color.White);
			timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
		}
		else if (outro_counter > 60 && winner_id >= 0 && outro_points_counter < minigame.points[winner_id])
		{
			for (int j = 0; j < 4; j++)
			{
				tracks[j].draw(spr_batch, j % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, j / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
				string text = Convert.ToString((outro_points_counter < minigame.points[j]) ? outro_points_counter : minigame.points[j]);
				Vector2 vector = default_font_large.MeasureString(text);
				spr_batch.DrawString(default_font_large, text, new Vector2((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4), new Color(0, 255, 0), 0f, new Vector2(vector.X / 2f, vector.Y / 2f), 1f, SpriteEffects.None, 0f);
			}
			draw_fog();
			spr_batch.Draw(border_h, new Vector2(0f, spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
			spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, 0f), Color.White);
			timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
		}
		else
		{
			float num2 = 1f + (float)(outro_counter - 60) / 60f;
			num2 = ((num2 < 2f) ? num2 : 2f);
			int num3 = (int)((float)(winner_id % 2) * (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Width);
			int num4 = (int)((float)(winner_id / 2) * (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Height);
			tracks[0].draw(spr_batch, num3, num4, num2);
			tracks[1].draw(spr_batch, num3 + (int)(num2 * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), num4, num2);
			tracks[2].draw(spr_batch, num3, num4 + (int)(num2 * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f), num2);
			tracks[3].draw(spr_batch, num3 + (int)(num2 * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), num4 + (int)(num2 * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f), num2);
			spr_batch.Draw(winner_txt, new Rectangle(num3 + (int)((float)((winner_id % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width) * num2 / 4f), num4 + (int)((float)((winner_id / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height) * num2 / 4f), (int)((float)winner_txt.Width * (3f - num2)), (int)((float)winner_txt.Height * (3f - num2))), new Rectangle(0, 0, winner_txt.Width, winner_txt.Height), Color.White, 0f, new Vector2(winner_txt.Width / 2, winner_txt.Height / 2), SpriteEffects.None, 0f);
		}
	}

	private void draw_gameplay()
	{
		for (int i = 0; i < 4; i++)
		{
			tracks[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
		}
		draw_fog();
		spr_batch.Draw(border_h, new Vector2(0f, spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
		spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, 0f), Color.White);
		timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
	}

	public override void draw_game()
	{
		switch (game_state)
		{
		case STATE.INTRO:
			if (intro_counter == 0)
			{
				fade.random(90u, Color.Black, fadein: true);
			}
			draw_intro();
			break;
		case STATE.GAME:
			draw_gameplay();
			break;
		case STATE.OUTRO:
			draw_outro();
			break;
		}
	}
}
