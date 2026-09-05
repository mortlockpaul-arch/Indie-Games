using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class sunbathing_vampires : minigame
{
	public class points_anim
	{
		private Texture2D img;

		private Vector2 pos;

		private float zoom;

		private uint anim_counter;

		private uint max_anim_counter;

		public points_anim(Texture2D image, int pos_x, int pos_y, float zoom_val, uint anim_steps)
		{
			img = image;
			anim_counter = 0u;
			max_anim_counter = anim_steps;
			zoom = zoom_val;
			pos = new Vector2(pos_x, pos_y);
		}

		public bool update()
		{
			return ++anim_counter >= max_anim_counter;
		}

		public void draw(SpriteBatch spr_batch, Vector2 offset)
		{
			spr_batch.Draw(img, new Rectangle((int)(pos.X + offset.X), (int)(pos.Y + offset.Y - (float)img.Height * zoom * (float)anim_counter / (float)max_anim_counter), (int)((float)img.Width * zoom), (int)((float)img.Height * zoom)), new Rectangle(0, 0, img.Width, img.Height), Color.White * (1f - (float)anim_counter / (float)max_anim_counter));
		}
	}

	private abstract class shootable
	{
		public abstract bool dead { get; }

		public abstract float distance { get; }

		public abstract Rectangle collision_rect { get; }

		public abstract void hit();

		public abstract bool update();

		public abstract void draw(SpriteBatch spr_batch, int offset_x, int offset_y);
	}

	private class bat : shootable
	{
		private enum STATE
		{
			FLYING,
			DYING,
			ATTACKING
		}

		private const int attack_propability_per_second = 10;

		private STATE current_state;

		private float dist;

		private Vector2 pos;

		private Vector2 mov;

		private bool is_dead;

		private int stage_width;

		private int stage_height;

		private sprite flying;

		private sprite dying;

		private SoundEffect attack_sfx;

		private SoundEffect die_sfx;

		public override bool dead => is_dead;

		public override float distance => dist;

		public override Rectangle collision_rect => new Rectangle((int)(pos.X - (float)flying.width * flying.zoom / 4f), (int)(pos.Y - (float)flying.height * flying.zoom / 4f), (int)((float)flying.width * flying.zoom / 2f), (int)((float)flying.height * flying.zoom / 2f));

		public bat(Texture2D fly, Texture2D die, SoundEffect atk_sfx, SoundEffect dying_sfx, int x, int y, float dist_val, int s_width = 640, int s_height = 480)
		{
			attack_sfx = atk_sfx;
			die_sfx = dying_sfx;
			flying = new sprite(fly, 4u, 1u, 8u);
			dying = new sprite(die, 6u, 1u, 6u);
			flying.zoom = dist_val;
			dying.zoom = dist_val;
			dist = dist_val;
			pos = new Vector2(x, y);
			mov = new Vector2(_2d_house_of_terror.game_state.random_gen.Next() % 60 / 10 - 3, _2d_house_of_terror.game_state.random_gen.Next() % 20 / 10);
			stage_width = s_width;
			stage_height = s_height;
		}

		public override void hit()
		{
			if (!is_dead)
			{
				is_dead = true;
				current_state = STATE.DYING;
				if (_2d_house_of_terror.game_state.random_gen.Next() % 3 < 1)
				{
					die_sfx.Play();
				}
			}
		}

		public override bool update()
		{
			if (current_state == STATE.DYING)
			{
				dying.animate();
				return dying.done;
			}
			pos += mov;
			if ((pos.X > (float)stage_width && mov.X >= 0f) || (pos.X < (float)(-flying.width) * flying.zoom && mov.X <= 0f) || (pos.Y > (float)stage_height && mov.Y >= 0f) || (pos.Y < (float)(-flying.height) * flying.zoom && mov.Y <= 0f))
			{
				return true;
			}
			flying.animate_cyclic();
			if (current_state == STATE.ATTACKING)
			{
				dist += 0.02f;
				flying.zoom = dist;
				dying.zoom = dist;
				if (dist > 2f)
				{
					return true;
				}
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 20)
				{
					current_state = STATE.FLYING;
				}
			}
			else if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 10)
			{
				current_state = STATE.ATTACKING;
				if (_2d_house_of_terror.game_state.random_gen.Next() % 3 < 2)
				{
					attack_sfx.Play();
				}
			}
			return false;
		}

		public override void draw(SpriteBatch spr_batch, int offset_x, int offset_y)
		{
			if (current_state == STATE.DYING)
			{
				dying.draw(spr_batch, (int)pos.X + offset_x, (int)pos.Y + offset_y);
			}
			else
			{
				flying.draw(spr_batch, (int)pos.X + offset_x, (int)pos.Y + offset_y);
			}
		}
	}

	private class planks : shootable
	{
		private sprite spr;

		private Texture2D light;

		private int light_length;

		private int pos_x;

		private int pos_y;

		private float dist;

		private Rectangle light_rect;

		private Color[] light_pixels;

		private bool is_dead;

		public override bool dead => is_dead;

		public override float distance => dist;

		public override Rectangle collision_rect => new Rectangle((int)((float)pos_x - (float)spr.width * spr.zoom / 4f), (int)((float)pos_y - (float)spr.height * spr.zoom / 4f), (int)((float)spr.width * spr.zoom / 2f), (int)((float)spr.height * spr.zoom / 2f));

		public planks(ContentManager content_mgr, int position_x, int position_y, float zoom, float dist_val = 0.3f, int light_type = 0)
		{
			spr = new sprite(content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/planks"), 5u, 3u, 5u);
			spr.state = 0u;
			spr.zoom = zoom;
			int num = 0;
			int num2 = 0;
			switch (light_type)
			{
			case 0:
				light = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/backlight_left");
				num2 = -30;
				break;
			case 1:
				light = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/frontlight_left");
				num = -30;
				break;
			case 2:
				light = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/backlight_right");
				num = -light.Width + 60;
				num2 = -20;
				break;
			default:
				light = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/frontlight_right");
				num = -light.Width + 105;
				num2 = -10;
				break;
			}
			pos_x = position_x;
			pos_y = position_y;
			dist = dist_val;
			light_rect = new Rectangle(pos_x + num, pos_y + num2, light.Width, light.Height);
			light_pixels = new Color[light.Width * light.Height];
			light.GetData(light_pixels);
		}

		public bool light_hits(shootable target)
		{
			if (target.distance > dist)
			{
				return false;
			}
			Rectangle value = target.collision_rect;
			light_rect.Height = light_length;
			if (value.Intersects(light_rect))
			{
				Rectangle rectangle = Rectangle.Intersect(value, light_rect);
				int num = rectangle.X - light_rect.X;
				int num2 = rectangle.Y - light_rect.Y;
				for (int i = 0; i < rectangle.Width; i++)
				{
					for (int j = 0; j < rectangle.Height; j++)
					{
						if (light_pixels[i + num + (j + num2) * light.Width] != Color.Transparent)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public override void hit()
		{
			if (spr.state < 2)
			{
				spr.state++;
			}
			else
			{
				is_dead = true;
			}
		}

		public override bool update()
		{
			if (!is_dead)
			{
				spr.animate();
			}
			else if (light_length < light.Height)
			{
				light_length += 4;
				light_length = ((light_length > light.Height) ? light.Height : light_length);
			}
			return is_dead;
		}

		public override void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			if (!dead)
			{
				spr.draw(spr_batch, dest_x + pos_x, dest_y + pos_y);
			}
			else
			{
				spr_batch.Draw(light, new Rectangle(dest_x + light_rect.X, dest_y + light_rect.Y, light.Width, light_length), new Rectangle(0, 0, light.Width, light_length), Color.White);
			}
		}
	}

	private class arrow
	{
		private Texture2D arrow_tex;

		private int pos_x;

		private int pos_y;

		private float dist = 1f;

		public float distance => dist;

		public Rectangle collision_rect => new Rectangle((int)((float)pos_x - (float)arrow_tex.Width * dist / 2f), pos_y, (int)((float)arrow_tex.Width * dist), (int)((float)arrow_tex.Height * dist));

		public arrow(Texture2D tex, int position_x, int position_y)
		{
			arrow_tex = tex;
			pos_x = position_x;
			pos_y = position_y;
			dist = 1f;
		}

		public bool update()
		{
			dist -= 0.04f;
			return (double)dist < 0.02;
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y)
		{
			spr_batch.Draw(arrow_tex, new Rectangle((int)((float)(offset_x + pos_x) - (float)arrow_tex.Width * dist / 2f), offset_y + pos_y, (int)((float)arrow_tex.Width * dist), (int)((float)arrow_tex.Height * dist)), new Rectangle(0, 0, arrow_tex.Width, arrow_tex.Height), Color.White);
		}
	}

	private class stage
	{
		private struct spawn_point
		{
			public int propability_per_second;

			public int pos_x;

			public int pos_y;

			public float dist;
		}

		private const int ai_max_shoot_counter = 16;

		private const int plank_kill_propability = 100;

		private RasterizerState raster_state;

		private int stage_w;

		private int stage_h;

		private SoundEffect arrow_sfx;

		private SoundEffect bat_attack_sfx;

		private SoundEffect bat_die_sfx;

		private Texture2D bg;

		private Texture2D cursor;

		private Texture2D arrow_tex;

		private Texture2D bat_flying;

		private Texture2D bat_dying;

		private Texture2D[] points_txts;

		private Vector2 scroll_pos;

		private Vector2 cursor_pos;

		private uint player_id;

		private bool ai_controlled;

		private int ai_move_speed = 2;

		private int ai_shoot_counter;

		private List<shootable> targets;

		private List<arrow> arrows;

		private List<points_anim> points_anims;

		private spawn_point[] spawn_points;

		private planks[] planked_holes;

		private int points;

		private bool hurt;

		private Texture2D blood_pixel;

		public int score => points;

		public stage(ContentManager content_mgr, int field_w, int field_h, uint pl_id)
		{
			raster_state = new RasterizerState
			{
				ScissorTestEnable = true
			};
			scroll_pos = new Vector2(0f, 0f);
			cursor_pos = new Vector2(field_w / 2, field_h / 2);
			targets = new List<shootable>();
			arrows = new List<arrow>();
			points_anims = new List<points_anim>();
			stage_w = field_w;
			stage_h = field_h;
			arrow_sfx = content_mgr.Load<SoundEffect>("minigame/sunbathing_vampires/sfx/arrow");
			bat_attack_sfx = content_mgr.Load<SoundEffect>("minigame/sunbathing_vampires/sfx/bat_attack");
			bat_die_sfx = content_mgr.Load<SoundEffect>("minigame/sunbathing_vampires/sfx/bat_die");
			bg = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/bg");
			cursor = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/crosshair_cursor");
			arrow_tex = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/arrow");
			bat_flying = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/bat/fly");
			bat_dying = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/bat/die");
			points_txts = new Texture2D[4]
			{
				content_mgr.Load<Texture2D>("minigame/gfx/5"),
				content_mgr.Load<Texture2D>("minigame/gfx/10"),
				content_mgr.Load<Texture2D>("minigame/gfx/15"),
				content_mgr.Load<Texture2D>("minigame/gfx/20")
			};
			player_id = pl_id;
			ai_controlled = game_mgr.player_ids[pl_id] < 0;
			blood_pixel = new Texture2D(bg.GraphicsDevice, 1, 1);
			blood_pixel.SetData(new Color[1] { Color.Red });
			planked_holes = new planks[4]
			{
				new planks(content_mgr, 180, 20, 0.9f),
				new planks(content_mgr, 20, 100, 0.6f, 0.4f, 1),
				new planks(content_mgr, 1160, 20, 1f, 0.3f, 2),
				new planks(content_mgr, 1245, 95, 0.5f, 0.4f, 3)
			};
			for (int i = 0; i < planked_holes.Length; i++)
			{
				targets.Add(planked_holes[i]);
			}
			spawn_points = new spawn_point[5];
			spawn_points[0].pos_x = -50;
			spawn_points[0].pos_y = -50;
			spawn_points[0].dist = 0.3f;
			spawn_points[0].propability_per_second = 100;
			spawn_points[1].pos_x = bg.Width;
			spawn_points[1].pos_y = -50;
			spawn_points[1].dist = 0.3f;
			spawn_points[1].propability_per_second = 100;
			spawn_points[2].pos_x = bg.Width / 4;
			spawn_points[2].pos_y = -50;
			spawn_points[2].dist = 0.3f;
			spawn_points[2].propability_per_second = 100;
			spawn_points[3].pos_x = bg.Width * 3 / 4;
			spawn_points[3].pos_y = -50;
			spawn_points[3].dist = 0.3f;
			spawn_points[3].propability_per_second = 100;
			spawn_points[4].pos_x = bg.Width / 2;
			spawn_points[4].pos_y = -50;
			spawn_points[4].dist = 0.3f;
			spawn_points[4].propability_per_second = 160;
		}

		~stage()
		{
			blood_pixel.Dispose();
		}

		private void add_points(int pnts, int pos_x, int pos_y, float dist)
		{
			points += pnts;
			if (pnts >= 5 && pnts <= 25)
			{
				points_anims.Add(new points_anim(points_txts[pnts / 5 - 1], pos_x, pos_y, dist, 60u));
			}
		}

		private void scroll()
		{
			if (cursor_pos.X < (float)(stage_w / 4))
			{
				scroll_pos.X -= cursor_pos.X - (float)(stage_w / 4);
				cursor_pos.X = stage_w / 4;
				cursor_pos.X = ((!(scroll_pos.X > 0f)) ? cursor_pos.X : ((cursor_pos.X - scroll_pos.X < 0f) ? 0f : (cursor_pos.X - scroll_pos.X)));
				scroll_pos.X = ((scroll_pos.X > 0f) ? 0f : scroll_pos.X);
			}
			else if (cursor_pos.X > (float)(stage_w * 3 / 4))
			{
				scroll_pos.X -= cursor_pos.X - (float)(stage_w * 3 / 4);
				cursor_pos.X = stage_w * 3 / 4;
				cursor_pos.X = ((!(scroll_pos.X < (float)(stage_w - bg.Width))) ? cursor_pos.X : (((float)(stage_w - bg.Width) - scroll_pos.X > (float)(stage_w / 4)) ? ((float)stage_w) : (cursor_pos.X + (float)(stage_w - bg.Width) - scroll_pos.X)));
				scroll_pos.X = ((scroll_pos.X < (float)(stage_w - bg.Width)) ? ((float)(stage_w - bg.Width)) : scroll_pos.X);
			}
			if (cursor_pos.Y < (float)(stage_h / 4))
			{
				scroll_pos.Y -= cursor_pos.Y - (float)(stage_h / 4);
				cursor_pos.Y = stage_h / 4;
				cursor_pos.Y = ((!(scroll_pos.Y > 0f)) ? cursor_pos.Y : ((cursor_pos.Y - scroll_pos.Y < 0f) ? 0f : (cursor_pos.Y - scroll_pos.Y)));
				scroll_pos.Y = ((scroll_pos.Y > 0f) ? 0f : scroll_pos.Y);
			}
			else if (cursor_pos.Y > (float)(stage_h * 3 / 4))
			{
				scroll_pos.Y -= cursor_pos.Y - (float)(stage_h * 3 / 4);
				cursor_pos.Y = stage_h * 3 / 4;
				cursor_pos.Y = ((!(scroll_pos.Y < (float)(stage_h - bg.Height))) ? cursor_pos.Y : (((float)(stage_h - bg.Height) - scroll_pos.Y > (float)(stage_h / 4)) ? ((float)stage_h) : (cursor_pos.Y + (float)(stage_h - bg.Height) - scroll_pos.Y)));
				scroll_pos.Y = ((scroll_pos.Y < (float)(stage_h - bg.Height)) ? ((float)(stage_h - bg.Height)) : scroll_pos.Y);
			}
		}

		private void move(Vector2 mov)
		{
			cursor_pos += mov;
			scroll();
		}

		private void shoot()
		{
			arrow_sfx.Play();
			arrows.Add(new arrow(arrow_tex, (int)(0f - scroll_pos.X + cursor_pos.X), (int)(0f - scroll_pos.Y + cursor_pos.Y)));
			points--;
		}

		private bool collide(arrow arr, shootable tar)
		{
			if (!tar.dead && arr.distance < tar.distance)
			{
				return arr.collision_rect.Intersects(tar.collision_rect);
			}
			return false;
		}

		private bool collide(planks plank, shootable tar)
		{
			if (!tar.dead && plank.distance > tar.distance)
			{
				return plank.collision_rect.Intersects(tar.collision_rect);
			}
			return false;
		}

		private void spawn_bats()
		{
			for (int i = 0; i < spawn_points.Length; i++)
			{
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < spawn_points[i].propability_per_second)
				{
					targets.Add(new bat(bat_flying, bat_dying, bat_attack_sfx, bat_die_sfx, spawn_points[i].pos_x, spawn_points[i].pos_y, spawn_points[i].dist, bg.Width, bg.Height));
				}
			}
		}

		private void update_points_anims()
		{
			for (int num = points_anims.Count - 1; num >= 0; num--)
			{
				if (points_anims[num].update())
				{
					points_anims.RemoveAt(num);
				}
			}
		}

		private void update_targets()
		{
			for (int num = targets.Count - 1; num >= 0; num--)
			{
				if (targets[num].update())
				{
					if (targets[num].distance > 2f && targets[num].collision_rect.Intersects(new Rectangle((int)(0f - scroll_pos.X), (int)(0f - scroll_pos.Y), stage_w, stage_h)))
					{
						points -= 10;
						hurt = true;
						game_mgr.moods[player_id] = game_mgr.MOOD.SAD;
					}
					targets.RemoveAt(num);
				}
			}
		}

		private void update_arrows()
		{
			for (int num = arrows.Count - 1; num >= 0; num--)
			{
				if (arrows[num].update())
				{
					arrows.RemoveAt(num);
				}
				else
				{
					for (int i = 0; i < targets.Count; i++)
					{
						if (collide(arrows[num], targets[i]))
						{
							targets[i].hit();
							arrows.RemoveAt(num);
							int pos_x = targets[i].collision_rect.Left + targets[i].collision_rect.Width / 2;
							int pos_y = targets[i].collision_rect.Top + targets[i].collision_rect.Height / 2;
							if (targets[i].distance < 0.4f)
							{
								add_points(20, pos_x, pos_y, targets[i].distance * 2f);
							}
							else if (targets[i].distance < 0.5f)
							{
								add_points(15, pos_x, pos_y, targets[i].distance * 2f);
							}
							else if (targets[i].distance < 0.7f)
							{
								add_points(10, pos_x, pos_y, targets[i].distance * 2f);
							}
							else
							{
								add_points(5, pos_x, pos_y, targets[i].distance * 2f);
							}
							game_mgr.moods[player_id] = ((targets[i].distance < 0.4f) ? game_mgr.MOOD.HAPPY : game_mgr.MOOD.NEUTRAL);
							break;
						}
					}
				}
			}
		}

		private void update_planks()
		{
			for (int i = 0; i < planked_holes.Length; i++)
			{
				if (!planked_holes[i].dead)
				{
					continue;
				}
				planked_holes[i].update();
				for (int j = 0; j < targets.Count; j++)
				{
					if (!targets[j].dead && planked_holes[i].light_hits(targets[j]) && _2d_house_of_terror.game_state.random_gen.Next() % 6000 < 100)
					{
						targets[j].hit();
						int pos_x = targets[j].collision_rect.Left + targets[j].collision_rect.Width / 2;
						int pos_y = targets[j].collision_rect.Top + targets[j].collision_rect.Height / 2;
						add_points(5, pos_x, pos_y, targets[j].distance * 2f);
					}
				}
			}
		}

		public void update()
		{
			hurt = false;
			update_points_anims();
			update_targets();
			update_arrows();
			update_planks();
			spawn_bats();
			if (!ai_controlled)
			{
				if (controllers.clicked(game_mgr.player_ids[player_id], CONTROLLER_BUTTONS.B))
				{
					shoot();
				}
				move(new Vector2(4f * controllers.lthumb[game_mgr.player_ids[player_id]].X, -4f * controllers.lthumb[game_mgr.player_ids[player_id]].Y));
				return;
			}
			if (++ai_shoot_counter == 16)
			{
				shoot();
				points++;
			}
			ai_shoot_counter %= 16;
			move(new Vector2(ai_move_speed, 0f));
			if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 20 || (cursor_pos.X < (float)(stage_w / 4) && ai_move_speed < 0) || (cursor_pos.X > (float)(3 * stage_w / 4) && ai_move_speed > 0))
			{
				ai_move_speed *= -1;
			}
		}

		private void draw_points_anims(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (points_anim points_anim in points_anims)
			{
				points_anim.draw(spr_batch, new Vector2((float)dest_x + scroll_pos.X, (float)dest_y + scroll_pos.Y));
			}
		}

		private void draw_targets(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (shootable target in targets)
			{
				target.draw(spr_batch, (int)((float)dest_x + scroll_pos.X), (int)((float)dest_y + scroll_pos.Y));
			}
		}

		private void draw_arrows(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (arrow arrow in arrows)
			{
				arrow.draw(spr_batch, (int)((float)dest_x + scroll_pos.X), (int)((float)dest_y + scroll_pos.Y));
			}
		}

		private void draw_planks(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			planks[] array = planked_holes;
			foreach (planks planks2 in array)
			{
				if (planks2.dead)
				{
					planks2.draw(spr_batch, (int)((float)dest_x + scroll_pos.X), (int)((float)dest_y + scroll_pos.Y));
				}
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			spr_batch.End();
			Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
			spr_batch.GraphicsDevice.ScissorRectangle = new Rectangle(dest_x, dest_y, stage_w, stage_h);
			spr_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, raster_state);
			spr_batch.Draw(bg, scroll_pos + new Vector2(dest_x, dest_y), Color.White);
			draw_points_anims(spr_batch, dest_x, dest_y);
			draw_targets(spr_batch, dest_x, dest_y);
			draw_planks(spr_batch, dest_x, dest_y);
			draw_arrows(spr_batch, dest_x, dest_y);
			spr_batch.Draw(cursor, new Rectangle((int)((float)dest_x + cursor_pos.X - (float)(cursor.Width / 8)), (int)((float)dest_y + cursor_pos.Y - (float)(cursor.Height / 2)), cursor.Width / 4, cursor.Height), new Rectangle((int)(player_id * cursor.Width / 4), 0, cursor.Width / 4, cursor.Height), Color.White);
			if (hurt)
			{
				spr_batch.Draw(blood_pixel, new Rectangle(0, 0, stage_w, stage_h), new Rectangle(0, 0, 1, 1), Color.White * 0.5f);
			}
			spr_batch.End();
			spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
			spr_batch.Begin();
		}
	}

	private enum STATE
	{
		INTRO,
		GAME,
		OUTRO
	}

	private const int max_intro_counter = 720;

	private const int max_outro_txt_counter = 60;

	private const int max_outro_counter = 120;

	private STATE game_state;

	private stage[] stages;

	private clock timer;

	private Texture2D border_v;

	private Texture2D border_h;

	private Song bgm;

	private int intro_counter;

	private int winner_id = -1;

	private int outro_counter;

	private int outro_points_counter;

	public sunbathing_vampires(IServiceProvider serv, GraphicsDevice dev, bool beginners_mode = false)
		: base(serv, dev, beginners_mode)
	{
		controls_spr = new sprite(content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/controls"), 1u, 1u, 1u);
		title_img = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/title");
		preview_img = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/preview");
		bgm = content_mgr.Load<Song>("bgm/sunbathing_vampires");
		border_v = content_mgr.Load<Texture2D>("minigame/gfx/border_vertical");
		border_h = content_mgr.Load<Texture2D>("minigame/gfx/border_horizontal");
		timer = new clock(content_mgr.Load<Texture2D>("minigame/gfx/clock"), default_font, 45, 76, new Color(75, 64, 21), 180);
		stages = new stage[4]
		{
			new stage(content_mgr, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, 0u),
			new stage(content_mgr, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, 1u),
			new stage(content_mgr, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, 2u),
			new stage(content_mgr, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, 3u)
		};
		game_state = STATE.INTRO;
		explanation_gfx = new Texture2D[3]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/sunbathing_vampires/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/sunbathing_vampires/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/sunbathing_vampires/3")
		};
		explanation_txt = new string[3] { "Vampire bats try to bite you.\nAvoid this sad fate\nby shooting them to dust\n", "Points are given depending\non the bats distance.\nThe closer the bat,\nthe fewer points are obtained.", "If a bat gets too close and\nmanages to bite you 5 points\nare deduced from your score.\nWasting your arrows without\nhitting anything will also\n    be penalized." };
	}

	~sunbathing_vampires()
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
		for (int i = 0; i < 4; i++)
		{
			stages[i].update();
		}
		timer.update();
		return timer.seconds < 1;
	}

	private bool update_intro()
	{
		if (intro_counter == 0)
		{
			content_mgr.Load<SoundEffect>("sfx/sunbathing_intro").Play();
		}
		else if (intro_counter == 540)
		{
			start_sfx.Play();
		}
		intro_counter++;
		return intro_counter > 720;
	}

	private bool update_outro()
	{
		if (outro_counter == 0)
		{
			timeup_sfx.Play();
		}
		if (outro_counter == 62)
		{
			fade.random(60u, Color.Black);
		}
		if (outro_counter <= 60)
		{
			outro_counter++;
		}
		else
		{
			if (winner_id < 0 || outro_points_counter >= minigame.points[winner_id])
			{
				return ++outro_counter > 120;
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
			int num = -9999999;
			winner_id = 0;
			for (int i = 0; i < minigame.points.Length; i++)
			{
				minigame.points[i] = stages[i].score;
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
		if (intro_counter <= 240)
		{
			Texture2D texture2D = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/bg");
			float num = 1f - (float)intro_counter / 240f;
			spr_batch.Draw(texture2D, new Vector2((float)(spr_batch.GraphicsDevice.Viewport.Width - texture2D.Width) * num, spr_batch.GraphicsDevice.Viewport.Height - texture2D.Height), Color.White);
		}
		else if (intro_counter <= 360)
		{
			Texture2D texture = content_mgr.Load<Texture2D>("minigame/sunbathing_vampires/bg");
			float num2 = 1f - (float)(intro_counter - 240) / 120f / 2f;
			for (int i = 0; i < 4; i++)
			{
				spr_batch.Draw(texture, new Rectangle((int)(num2 * (float)(i % 2) * (float)spr_batch.GraphicsDevice.Viewport.Width), (int)(num2 * (float)(i / 2) * (float)spr_batch.GraphicsDevice.Viewport.Height), spr_batch.GraphicsDevice.Viewport.Width, spr_batch.GraphicsDevice.Viewport.Height), new Rectangle(0, 0, spr_batch.GraphicsDevice.Viewport.Width, spr_batch.GraphicsDevice.Viewport.Height), Color.White);
			}
			num2--;
			num2 *= -2f;
			num2 = 1f - num2;
			spr_batch.Draw(border_h, new Vector2((float)(-border_h.Width) * num2, spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
			spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, (float)(-border_v.Height) * num2), Color.White);
			timer.zoom = 5f - 4f * (1f - num2);
			timer.draw(spr_batch, (int)((float)(spr_batch.GraphicsDevice.Viewport.Width / 2) + num2 * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), (int)((float)(spr_batch.GraphicsDevice.Viewport.Height / 2) - num2 * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f));
		}
		else
		{
			float num3 = 4f - 3f * ((float)(intro_counter - 360) / 180f);
			num3 = ((num3 < 1f) ? 1f : num3);
			draw_gameplay();
			for (int j = 0; j < 4; j++)
			{
				spr_batch.Draw(start_txt, new Rectangle((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num3), (int)((float)start_txt.Height * num3)), new Rectangle(0, 0, start_txt.Width, start_txt.Height), Color.White, 0f, new Vector2(start_txt.Width / 2, start_txt.Height / 2), SpriteEffects.None, 0f);
			}
		}
	}

	private void draw_outro()
	{
		draw_gameplay();
		if (outro_counter <= 60)
		{
			float num = 4f - 3f * ((float)outro_counter / 60f);
			for (int i = 0; i < 4; i++)
			{
				spr_batch.Draw(timeup_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num), (int)((float)start_txt.Height * num)), new Rectangle(0, 0, timeup_txt.Width, timeup_txt.Height), Color.White, 0f, new Vector2(timeup_txt.Width / 2, timeup_txt.Height / 2), SpriteEffects.None, 0f);
			}
		}
		else if (winner_id >= 0 && outro_points_counter <= minigame.points[winner_id])
		{
			for (int j = 0; j < 4; j++)
			{
				string text = Convert.ToString((outro_points_counter < minigame.points[j]) ? outro_points_counter : minigame.points[j]);
				Vector2 vector = default_font_large.MeasureString(text);
				spr_batch.DrawString(default_font_large, text, new Vector2((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4), new Color(0, 255, 0), 0f, new Vector2(vector.X / 2f, vector.Y / 2f), 1f, SpriteEffects.None, 0f);
			}
		}
		else
		{
			float num2 = 4f - 3f * ((float)(outro_counter - 60) / 60f);
			spr_batch.Draw(winner_txt, new Rectangle((winner_id % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (winner_id / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)winner_txt.Width * num2), (int)((float)winner_txt.Height * num2)), new Rectangle(0, 0, winner_txt.Width, winner_txt.Height), Color.White, 0f, new Vector2(winner_txt.Width / 2, winner_txt.Height / 2), SpriteEffects.None, 0f);
		}
	}

	private void draw_gameplay()
	{
		for (int i = 0; i < 4; i++)
		{
			stages[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
		}
		draw_faces(spr_batch);
		spr_batch.Draw(border_h, new Vector2(0f, spr_batch.GraphicsDevice.Viewport.Height / 2 - border_h.Height / 2), Color.White);
		spr_batch.Draw(border_v, new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2 - border_v.Width / 2, 0f), Color.White);
		timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
	}

	public override void draw_game()
	{
		switch (game_state)
		{
		case STATE.INTRO:
			draw_intro();
			if (intro_counter == 0)
			{
				fade.random(90u, Color.Black, fadein: true);
			}
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
