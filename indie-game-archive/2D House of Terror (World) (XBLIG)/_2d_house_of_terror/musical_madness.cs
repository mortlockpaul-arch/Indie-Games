using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class musical_madness : minigame
{
	private class rat
	{
		private enum STATE
		{
			WALK,
			ATTACK,
			DIE
		}

		private const float max_radian = (float)Math.PI / 4f;

		private const int attack_threshold = 180;

		private const float gravity = 0.05f;

		private const float opacity_change = 0.05f;

		private STATE current_state;

		private Vector2 pos;

		private Vector2 mov;

		private sprite walk_spr;

		private sprite attack_spr;

		private float radian;

		private float radian_change = 0.05f;

		public Rectangle collision_rect => new Rectangle((int)pos.X - (int)((float)walk_spr.width * walk_spr.zoom / 4f), (int)pos.Y - (int)((float)walk_spr.height * walk_spr.zoom / 4f), (int)((float)walk_spr.width * walk_spr.zoom / 2f), (int)((float)walk_spr.height * walk_spr.zoom / 2f));

		public float distance => walk_spr.zoom;

		public bool alive => current_state != STATE.DIE;

		public rat(Texture2D walk, Texture2D attack, int x, int y, float dist)
		{
			pos = new Vector2(x, y);
			mov = default(Vector2);
			mov.X = ((dist < 1f) ? (-0.5f) : (-0.7f));
			mov.Y = ((dist < 1f) ? 0.05f : (-0.07f));
			walk_spr = new sprite(walk, 4u, 1u, 4u);
			attack_spr = new sprite(attack, 1u, 1u, 1u);
			walk_spr.zoom = dist;
			attack_spr.zoom = dist;
		}

		public void hit()
		{
			if (current_state != STATE.DIE)
			{
				radian_change = ((radian_change < 0f) ? (-2f * radian_change) : (2f * radian_change));
				current_state = STATE.DIE;
			}
		}

		public bool update()
		{
			switch (current_state)
			{
			case STATE.WALK:
				pos += mov;
				walk_spr.animate();
				if (pos.X < 180f)
				{
					mov.X *= 4f;
					if (walk_spr.zoom >= 1f)
					{
						mov.Y *= 32f;
					}
					else
					{
						mov.Y *= -32f;
					}
					current_state = STATE.ATTACK;
				}
				break;
			case STATE.ATTACK:
				if (attack_spr.zoom < 1f)
				{
					attack_spr.zoom += 0.05f;
				}
				radian += radian_change;
				if (radian > (float)Math.PI / 4f)
				{
					radian_change *= -0.5f;
				}
				radian = ((radian < -(float)Math.PI / 4f) ? (-(float)Math.PI / 4f) : radian);
				attack_spr.radian = radian;
				mov.Y += 0.05f;
				pos += mov;
				break;
			case STATE.DIE:
				radian += radian_change;
				attack_spr.radian = radian;
				attack_spr.opacity -= 0.05f;
				return attack_spr.opacity <= 0.05f;
			}
			return pos.X + (float)attack_spr.width < 0f;
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			if (current_state == STATE.WALK)
			{
				walk_spr.draw(spr_batch, dest_x + (int)pos.X, dest_y + (int)pos.Y);
			}
			else
			{
				attack_spr.draw(spr_batch, dest_x + (int)pos.X, dest_y + (int)pos.Y);
			}
		}
	}

	private class note
	{
		private enum STATE
		{
			FLYING,
			DYING
		}

		private const float max_zoom = 1.5f;

		private const float min_zoom = 0.5f;

		private const int max_change_counter = 4;

		private STATE current_state;

		private Vector2 pos;

		private Vector2 mov;

		private float zoom;

		private sprite fly;

		private sprite explode;

		private float zoom_change = -0.02f;

		private int change_counter;

		private bool fly_distant;

		private float real_zoom = 1f;

		public Rectangle collision_rect => new Rectangle((int)pos.X - (int)((float)fly.width * fly.zoom / 4f), (int)pos.Y - (int)((float)fly.height * fly.zoom / 4f), (int)((float)fly.width * fly.zoom / 2f), (int)((float)fly.height * fly.zoom / 2f));

		public bool alive => current_state != STATE.DYING;

		public float distance => real_zoom;

		public note(Texture2D fly_img, Texture2D exp_img, int dest_x, int dest_y, float mx, float my, bool fly_dist = false)
		{
			fly_distant = fly_dist;
			zoom = 0.1f;
			pos = new Vector2(dest_x, dest_y);
			mov = new Vector2(mx, my);
			fly = new sprite(fly_img, 4u, 1u, 8u);
			explode = new sprite(exp_img, 7u, 1u, 28u);
			fly.zoom = zoom;
			explode.zoom = zoom;
		}

		public void hit()
		{
			current_state = STATE.DYING;
			explode.frame = 0u;
		}

		public bool update()
		{
			if (current_state == STATE.DYING)
			{
				zoom += Math.Abs(zoom_change);
			}
			else
			{
				zoom -= zoom_change;
				if ((zoom_change < 0f && zoom > 1.5f) || (zoom < 0.5f && zoom_change > 0f))
				{
					zoom_change *= -1f;
					change_counter++;
				}
			}
			if (fly_distant)
			{
				mov.Y -= 0.005f;
				real_zoom -= 0.005f;
			}
			real_zoom = ((real_zoom < 0f) ? 0f : real_zoom);
			fly.zoom = real_zoom * zoom;
			explode.zoom = real_zoom * zoom;
			pos += mov;
			fly.animate_cyclic();
			explode.animate();
			if ((change_counter <= 4 || !(zoom < 0.5f)) && (current_state != STATE.DYING || !explode.done))
			{
				return real_zoom == 0f;
			}
			return true;
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			if (current_state == STATE.FLYING)
			{
				fly.draw(spr_batch, (int)((float)dest_x + pos.X), (int)((float)dest_y + pos.Y));
			}
			else
			{
				explode.draw(spr_batch, (int)((float)dest_x + pos.X), (int)((float)dest_y + pos.Y));
			}
		}
	}

	private class player
	{
		private const int note_spawn_x = 50;

		private const int note_spawn_y = 110;

		private const float note_mx = 1f;

		private const float yellow_my = 0.8f;

		private const float green_my = 0.4f;

		private const int pos_x = 80;

		private const int max_running_counter = 60;

		private const int max_idle_counter = 60;

		private Texture2D green_fly;

		private Texture2D green_exp;

		private Texture2D yellow_fly;

		private Texture2D yellow_exp;

		private sprite running_spr;

		private sprite hit_spr;

		private sprite stars_of_confusion;

		private int pos_y = 155;

		private Vector2[] checkpoints;

		private int last_checkpoint;

		private int running_counter;

		private int idle_counter;

		private bool is_running = true;

		private bool was_hit;

		private int player_id;

		private bool ai_controlled;

		private List<note> notes;

		public bool running => is_running;

		public bool stunned => was_hit;

		public Rectangle collision_rect => new Rectangle(80 - (int)((float)running_spr.width * running_spr.zoom / 4f), pos_y - (int)((float)running_spr.height * running_spr.zoom / 4f), (int)((float)running_spr.width * running_spr.zoom / 2f), (int)((float)running_spr.height * running_spr.zoom / 2f));

		public player(ContentManager content_mgr, int pl_id = 0)
		{
			player_id = pl_id;
			ai_controlled = game_mgr.player_ids[pl_id] < 0;
			string text = ((game_mgr.char_ids[pl_id] == 0) ? "jimmy" : ((game_mgr.char_ids[pl_id] == 1) ? "sam" : ((game_mgr.char_ids[pl_id] == 2) ? "erik" : "billy")));
			if (text == "erik")
			{
				pos_y -= 8;
			}
			running_spr = new sprite(content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/" + text + "/walk"), 8u, 1u, 16u);
			hit_spr = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/" + text + "/hit"), 3u, 1u, 6u);
			stars_of_confusion = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/stars_of_confusion"), 3u, 1u, 9u);
			checkpoints = new Vector2[4]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, -1f),
				new Vector2(-1f, 0f)
			};
			green_fly = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/note_green");
			green_exp = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/note_green_explode");
			yellow_fly = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/note_yellow");
			yellow_exp = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/note_yellow_explode");
			notes = new List<note>();
		}

		public void hit()
		{
			if (!was_hit)
			{
				was_hit = true;
				hit_spr.frame = 0u;
			}
		}

		private int hit_checkpoint()
		{
			Vector2 vector = controllers.lthumb[game_mgr.player_ids[player_id]];
			for (int i = 0; i < checkpoints.Length; i++)
			{
				Vector2 vector2 = checkpoints[i] - vector;
				if (Math.Abs(vector2.X) < 0.35f && Math.Abs(vector2.Y) < 0.35f)
				{
					return i;
				}
			}
			return -1;
		}

		private bool update_rotating()
		{
			int num = (last_checkpoint + 1) % checkpoints.Length;
			int num2 = (ai_controlled ? num : hit_checkpoint());
			if (num2 == num)
			{
				last_checkpoint = num2;
				if (num2 == 0)
				{
					idle_counter = 0;
				}
			}
			else
			{
				if (num2 >= 0 && num2 != last_checkpoint)
				{
					last_checkpoint = 0;
				}
				idle_counter++;
			}
			idle_counter = ((idle_counter > 60) ? 60 : idle_counter);
			return idle_counter < 60;
		}

		private void update_notes()
		{
			for (int num = notes.Count - 1; num >= 0; num--)
			{
				if (notes[num].update())
				{
					notes.RemoveAt(num);
				}
			}
		}

		private void spawn_note()
		{
			if (ai_controlled || !controllers.pressed(game_mgr.player_ids[player_id], CONTROLLER_BUTTONS.Y) || !controllers.pressed(game_mgr.player_ids[player_id], CONTROLLER_BUTTONS.A))
			{
				if (controllers.pressed(game_mgr.player_ids[player_id], CONTROLLER_BUTTONS.Y) || (ai_controlled && _2d_house_of_terror.game_state.random_gen.Next() % 2 == 0))
				{
					notes.Add(new note(yellow_fly, yellow_exp, 50, 110, 1f, 0.8f, fly_dist: true));
				}
				else if (controllers.pressed(game_mgr.player_ids[player_id], CONTROLLER_BUTTONS.A) || (ai_controlled && _2d_house_of_terror.game_state.random_gen.Next() % 2 == 0))
				{
					notes.Add(new note(green_fly, green_exp, 50, 110, 1f, 0.4f));
				}
			}
		}

		public int handle_rat(rat ra)
		{
			foreach (note note in notes)
			{
				if (((ra.distance < 1f && note.distance < 1f) || (ra.distance >= 1f && note.distance >= 1f)) && ra.collision_rect.Intersects(note.collision_rect) && ra.alive && note.alive && ra.collision_rect.X < 300)
				{
					note.hit();
					ra.hit();
					return (note.distance < 1f) ? 15 : 10;
				}
			}
			if (ra.alive && ra.collision_rect.Intersects(collision_rect) && !was_hit)
			{
				hit();
				return -10;
			}
			return 0;
		}

		public void update()
		{
			update_notes();
			if (was_hit)
			{
				stars_of_confusion.animate();
				hit_spr.animate_cyclic();
				if ((double)hit_spr.radian > 4.71238898038469 || hit_spr.radian == 0f)
				{
					hit_spr.radian -= 0.1f;
				}
				if (hit_spr.cycle_finished)
				{
					hit_spr.radian = 0f;
					was_hit = false;
				}
				return;
			}
			is_running = update_rotating();
			if (is_running)
			{
				if (++running_counter == 60)
				{
					running_counter = 0;
					spawn_note();
				}
				running_spr.animate();
			}
		}

		public void draw_notes(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (note note in notes)
			{
				note.draw(spr_batch, dest_x, dest_y);
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			if (was_hit)
			{
				stars_of_confusion.draw(spr_batch, dest_x + 80, dest_y + pos_y);
				hit_spr.draw(spr_batch, dest_x + 80 - (int)(Math.Cos(hit_spr.radian) * (double)hit_spr.width / 2.0), dest_y + pos_y - (int)(Math.Sin(hit_spr.radian) * (double)hit_spr.height / 2.0), h_flipped: true);
			}
			else
			{
				running_spr.draw(spr_batch, dest_x + 80, dest_y + pos_y);
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom)
		{
			float zoom2 = running_spr.zoom;
			running_spr.zoom = zoom;
			running_spr.draw(spr_batch, (int)((float)dest_x + zoom * 80f), (int)((float)dest_y + zoom * (float)pos_y));
			running_spr.zoom = zoom2;
		}
	}

	private class lab
	{
		private const int rat_spawn_propability = 40;

		private const int rat_right_x = 400;

		private const int rat_right_y = 200;

		private const int rat_left_x = 400;

		private const int rat_left_y = 150;

		private const float rat_dist = 0.3f;

		private const int vinyl_pos_x = 70;

		private const int vinyl_pos_y = 195;

		private const float lamp_max_radian_change = -0.05f;

		private RasterizerState raster_state;

		private int lab_w;

		private int lab_h;

		private Texture2D bg;

		private Texture2D fg;

		private Texture2D rat_walk;

		private Texture2D rat_attack;

		private player pl;

		private sprite vinyl;

		private List<rat> rats;

		private Texture2D[] pnttex_good;

		private Texture2D[] pnttex_bad;

		private List<sunbathing_vampires.points_anim> point_anims;

		private int points;

		private float lamp_radian;

		private float radian_change;

		private float lamp_radian_slowdown = -0.001f;

		private SoundEffect rat_sfx;

		public int score => points;

		public lab(ContentManager content_mgr, int lab_width, int lab_height, int player_id)
		{
			raster_state = new RasterizerState
			{
				ScissorTestEnable = true
			};
			lab_w = lab_width;
			lab_h = lab_height;
			bg = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/bg");
			fg = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/fg");
			rat_walk = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/rat/walk");
			rat_attack = content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/rat/attack");
			pl = new player(content_mgr, player_id);
			vinyl = new sprite(content_mgr.Load<Texture2D>("minigame/musical_madness/gfx/vinyl"), 4u, 1u, 8u);
			rats = new List<rat>();
			point_anims = new List<sunbathing_vampires.points_anim>();
			pnttex_good = new Texture2D[4]
			{
				content_mgr.Load<Texture2D>("minigame/gfx/5"),
				content_mgr.Load<Texture2D>("minigame/gfx/10"),
				content_mgr.Load<Texture2D>("minigame/gfx/15"),
				content_mgr.Load<Texture2D>("minigame/gfx/20")
			};
			pnttex_bad = new Texture2D[2]
			{
				content_mgr.Load<Texture2D>("minigame/gfx/-5"),
				content_mgr.Load<Texture2D>("minigame/gfx/-10")
			};
			rat_sfx = content_mgr.Load<SoundEffect>("minigame/sunbathing_vampires/sfx/bat_attack");
		}

		private void add_points(int pnts, int dest_x, int dest_y)
		{
			points += pnts;
			if (pnts < 0 && pnts < -4 && pnts > -11)
			{
				rat_sfx.Play();
				point_anims.Add(new sunbathing_vampires.points_anim(pnttex_bad[-pnts / 5 - 1], dest_x, dest_y, 1f, 60u));
			}
			else if (pnts > 4 && pnts < 21)
			{
				point_anims.Add(new sunbathing_vampires.points_anim(pnttex_good[pnts / 5 - 1], dest_x, dest_y, 1f, 60u));
			}
		}

		private void spawn_rats()
		{
			if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 40)
			{
				rats.Add(new rat(rat_walk, rat_attack, 400, 200, 1f));
			}
			if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 40)
			{
				rats.Add(new rat(rat_walk, rat_attack, 400, 150, 0.3f));
			}
		}

		private void update_points()
		{
			for (int num = point_anims.Count - 1; num >= 0; num--)
			{
				if (point_anims[num].update())
				{
					point_anims.RemoveAt(num);
				}
			}
		}

		private void update_rats()
		{
			spawn_rats();
			for (int num = rats.Count - 1; num >= 0; num--)
			{
				int num2 = pl.handle_rat(rats[num]);
				if (num2 != 0)
				{
					Rectangle collision_rect = rats[num].collision_rect;
					add_points(num2, collision_rect.Center.X, collision_rect.Center.Y);
				}
				if (rats[num].update())
				{
					rats.RemoveAt(num);
				}
			}
		}

		private void update_lamp()
		{
			if (pl.stunned && (double)Math.Abs(lamp_radian) < 0.005 && (double)Math.Abs(radian_change) < 0.005)
			{
				radian_change = -0.05f;
			}
			radian_change += ((radian_change == 0f && lamp_radian == 0f) ? 0f : ((radian_change * lamp_radian_slowdown > 0f) ? lamp_radian_slowdown : (2f * lamp_radian_slowdown)));
			lamp_radian += radian_change;
			lamp_radian_slowdown *= (((!(lamp_radian < 0f) || !(lamp_radian_slowdown < 0f)) && (!(lamp_radian > 0f) || !(lamp_radian_slowdown > 0f))) ? 1 : (-1));
		}

		public void update()
		{
			if (pl.running)
			{
				vinyl.animate();
			}
			pl.update();
			update_rats();
			update_points();
			update_lamp();
		}

		private void draw_point_anims(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (sunbathing_vampires.points_anim point_anim in point_anims)
			{
				point_anim.draw(spr_batch, new Vector2(dest_x, dest_y));
			}
		}

		private void draw_rats(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (rat rat in rats)
			{
				rat.draw(spr_batch, dest_x, dest_y);
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			spr_batch.End();
			Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
			spr_batch.GraphicsDevice.ScissorRectangle = new Rectangle(dest_x, dest_y, lab_w, lab_h);
			spr_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, raster_state);
			spr_batch.Draw(bg, new Vector2(dest_x, dest_y), Color.White);
			vinyl.draw(spr_batch, dest_x + 70, dest_y + 195);
			pl.draw(spr_batch, dest_x, dest_y);
			draw_rats(spr_batch, dest_x, dest_y);
			pl.draw_notes(spr_batch, dest_x, dest_y);
			draw_point_anims(spr_batch, dest_x, dest_y);
			spr_batch.Draw(fg, new Rectangle(dest_x + fg.Width / 2, dest_y - 10, fg.Width, fg.Height), new Rectangle(0, 0, fg.Width, fg.Height), Color.White, lamp_radian, new Vector2(fg.Width / 2, 0f), SpriteEffects.None, 0f);
			spr_batch.End();
			spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
			spr_batch.Begin();
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y, float zoom)
		{
			if (dest_x <= spr_batch.GraphicsDevice.Viewport.Width && dest_y <= spr_batch.GraphicsDevice.Viewport.Height)
			{
				spr_batch.End();
				Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
				Rectangle scissorRectangle2 = new Rectangle(dest_x, dest_y, (int)((float)lab_w * zoom), (int)((float)lab_h * zoom));
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
				spr_batch.Draw(bg, new Rectangle(dest_x, dest_y, (int)((float)bg.Width * zoom), (int)((float)bg.Height * zoom)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White);
				float zoom2 = vinyl.zoom;
				vinyl.draw(spr_batch, (int)((float)dest_x + 70f * zoom), (int)((float)dest_y + 195f * zoom));
				vinyl.zoom = zoom2;
				pl.draw(spr_batch, dest_x, dest_y, zoom);
				spr_batch.Draw(fg, new Rectangle((int)((float)dest_x + zoom * (float)fg.Width / 2f), (int)((float)dest_y - zoom * 10f), (int)(zoom * (float)fg.Width), (int)(zoom * (float)fg.Height)), new Rectangle(0, 0, fg.Width, fg.Height), Color.White, lamp_radian, new Vector2(fg.Width / 2, 0f), SpriteEffects.None, 0f);
				spr_batch.End();
				spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
				spr_batch.Begin();
			}
		}
	}

	private enum STATE
	{
		INTRO,
		GAME,
		OUTRO
	}

	private const int intro_zoom_counter = 120;

	private const int intro_start_counter = 240;

	private const int intro_wait_counter = 300;

	private const int outro_timeup_counter = 60;

	private const int outro_zoom_counter = 120;

	private const int outro_wait_counter = 180;

	private STATE game_state;

	private lab[] labs;

	private clock timer;

	private Texture2D border_v;

	private Texture2D border_h;

	private Song bgm;

	private int winner_id = -1;

	private int intro_counter;

	private int outro_counter;

	private int outro_points_counter;

	public musical_madness(IServiceProvider serv, GraphicsDevice dev, bool beginners_mode = false)
		: base(serv, dev, beginners_mode)
	{
		controls_spr = new sprite(content_mgr.Load<Texture2D>("minigame/musical_madness/controls"), 1u, 1u, 1u);
		title_img = content_mgr.Load<Texture2D>("minigame/musical_madness/title");
		preview_img = content_mgr.Load<Texture2D>("minigame/musical_madness/preview");
		bgm = content_mgr.Load<Song>("bgm/musical_madness");
		border_v = content_mgr.Load<Texture2D>("minigame/gfx/border_vertical");
		border_h = content_mgr.Load<Texture2D>("minigame/gfx/border_horizontal");
		timer = new clock(content_mgr.Load<Texture2D>("minigame/gfx/clock"), default_font, 45, 76, new Color(75, 64, 21), 99);
		labs = new lab[4]
		{
			new lab(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 0),
			new lab(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 1),
			new lab(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 2),
			new lab(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 3)
		};
		game_state = STATE.INTRO;
		explanation_gfx = new Texture2D[2]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/musical_madness/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/musical_madness/2")
		};
		explanation_txt = new string[2] { "Your shrunken self is standing on\na vinyl with rats attacking from\n2 sides. Those rats are vulnerable\nto music, so you have to run to\n      keep it playing.", "Whilst running you should press\neighter Y or A. If Y is held,\na yellow note will spawn and\nattack the rats coming from the\nbackground, if A is held a green\n    one will attack the others." };
	}

	~musical_madness()
	{
		free();
	}

	public override void free()
	{
		MediaPlayer.Stop();
		base.free();
	}

	private bool update_gameplay()
	{
		for (int i = 0; i < labs.Length; i++)
		{
			labs[i].update();
		}
		timer.update();
		return timer.seconds < 1;
	}

	private bool update_intro()
	{
		if (intro_counter == 120)
		{
			start_sfx.Play();
		}
		return ++intro_counter >= 300;
	}

	private bool update_outro()
	{
		if (outro_counter == 0)
		{
			timeup_sfx.Play();
		}
		if (outro_counter <= 60)
		{
			outro_counter++;
		}
		else
		{
			if (outro_points_counter >= minigame.points[winner_id])
			{
				if (outro_counter == 61)
				{
					random_success_sfx().Play();
				}
				if (outro_counter == 120)
				{
					fade.random(60u, Color.Black);
				}
				return ++outro_counter >= 180;
			}
			outro_points_counter++;
		}
		return false;
	}

	public override bool update_game()
	{
		switch (game_state)
		{
		case STATE.INTRO:
			if (update_intro())
			{
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
			int num = -9999999;
			winner_id = 0;
			for (int i = 0; i < minigame.points.Length; i++)
			{
				minigame.points[i] = labs[i].score;
				if (minigame.points[i] > num)
				{
					winner_id = i;
					num = minigame.points[i];
				}
			}
			break;
		}
		case STATE.OUTRO:
			return update_outro();
		}
		return false;
	}

	private void draw_intro()
	{
		if (intro_counter <= 120)
		{
			float num = 2f - (float)intro_counter / 120f;
			for (int i = 0; i < labs.Length; i++)
			{
				labs[i].draw(spr_batch, i % 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num / 2f), i / 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num / 2f), num);
			}
			return;
		}
		float num2 = (float)(intro_counter - 120) / 120f;
		num2 = ((num2 > 1f) ? 1f : num2);
		float num3 = 4f - 3f * num2;
		for (int j = 0; j < labs.Length; j++)
		{
			labs[j].draw(spr_batch, j % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, j / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
			spr_batch.Draw(start_txt, new Rectangle((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num3), (int)((float)start_txt.Height * num3)), new Rectangle(0, 0, start_txt.Width, start_txt.Height), Color.White, 0f, new Vector2(start_txt.Width / 2, start_txt.Height / 2), SpriteEffects.None, 0f);
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
			draw_gameplay();
			for (int i = 0; i < 4; i++)
			{
				spr_batch.Draw(timeup_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num), (int)((float)start_txt.Height * num)), new Rectangle(0, 0, timeup_txt.Width, timeup_txt.Height), Color.White, 0f, new Vector2(timeup_txt.Width / 2, timeup_txt.Height / 2), SpriteEffects.None, 0f);
			}
		}
		else if (outro_points_counter < minigame.points[winner_id])
		{
			draw_gameplay();
			for (int j = 0; j < 4; j++)
			{
				string text = Convert.ToString((outro_points_counter < minigame.points[j]) ? outro_points_counter : minigame.points[j]);
				Vector2 vector = default_font_large.MeasureString(text);
				spr_batch.DrawString(default_font_large, text, new Vector2((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4), new Color(0, 255, 0), 0f, new Vector2(vector.X / 2f, vector.Y / 2f), 1f, SpriteEffects.None, 0f);
			}
		}
		else if (outro_counter < 180)
		{
			float num2 = 1f + (float)(outro_counter - 60) / 60f;
			num2 = ((num2 < 2f) ? num2 : 2f);
			int num3 = (int)((float)(winner_id % 2) * (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Width);
			int num4 = (int)((float)(winner_id / 2) * (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Height);
			for (int k = 0; k < labs.Length; k++)
			{
				labs[k].draw(spr_batch, num3 + k % 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num2 / 2f), num4 + k / 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num2 / 2f), num2);
			}
			spr_batch.Draw(winner_txt, new Rectangle(num3 + (int)((float)((winner_id % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width) * num2 / 4f), num4 + (int)((float)((winner_id / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height) * num2 / 4f), (int)((float)winner_txt.Width * (3f - num2)), (int)((float)winner_txt.Height * (3f - num2))), new Rectangle(0, 0, winner_txt.Width, winner_txt.Height), Color.White, 0f, new Vector2(winner_txt.Width / 2, winner_txt.Height / 2), SpriteEffects.None, 0f);
		}
	}

	private void draw_gameplay()
	{
		for (int i = 0; i < labs.Length; i++)
		{
			labs[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
		}
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
