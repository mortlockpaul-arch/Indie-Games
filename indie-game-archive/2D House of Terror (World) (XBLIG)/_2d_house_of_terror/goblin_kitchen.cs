using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class goblin_kitchen : minigame
{
	private class dish
	{
		private enum STATE
		{
			FLYING,
			DYING
		}

		private const float rotation_speed = 0.2f;

		private const int max_dying_counter = 60;

		private STATE current_state;

		private Texture2D img;

		private Vector2 pos;

		private Vector2 velocity;

		private float rotation_radian;

		private int dying_counter;

		public Rectangle collision_rect => new Rectangle((int)(pos.X - (float)(img.Width / 4)), (int)(pos.Y - (float)(img.Height / 4)), img.Width / 2, img.Height / 2);

		public bool dead => current_state == STATE.DYING;

		public dish(Texture2D image, int x, int y, float sx, float sy)
		{
			img = image;
			pos = new Vector2(x + ((sx > 0f) ? image.Width : (-image.Width)), y);
			velocity = new Vector2(sx, sy);
		}

		public void hit()
		{
			current_state = STATE.DYING;
		}

		public bool update()
		{
			if (current_state == STATE.FLYING)
			{
				pos += velocity;
				rotation_radian += ((velocity.X > 0f) ? 0.2f : (-0.2f));
				rotation_radian %= (float)Math.PI;
				return false;
			}
			return ++dying_counter > 60;
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			Color color = Color.White * ((current_state == STATE.DYING) ? (1f - (float)dying_counter / 60f) : 1f);
			spr_batch.Draw(img, new Rectangle((int)((float)dest_x + pos.X), (int)((float)dest_y + pos.Y), img.Width, img.Height), new Rectangle(0, 0, img.Width, img.Height), color, rotation_radian, new Vector2(img.Width / 2, img.Height / 2), SpriteEffects.None, 0f);
		}
	}

	private abstract class duelist
	{
		protected const float gravity = 0.5f;

		protected const float max_jump_speed = -10f;

		protected float jump_pos;

		protected float jump_speed;

		protected bool jumping;

		protected Vector2 throw_speed_vec;

		public abstract Rectangle collision_rect { get; }

		public abstract bool thrown { get; }

		public Vector2 throw_speed => throw_speed_vec;

		public bool airborne => jumping;

		protected void update_jump()
		{
			jump_pos += jump_speed;
			jump_speed += 0.5f;
			if (jump_pos >= 0f)
			{
				jump_pos = 0f;
				jump_speed = -10f;
				jumping = false;
			}
		}

		protected void jump()
		{
			if (!jumping)
			{
				jump_pos = 0f;
				jump_speed = -10f;
				jumping = true;
			}
		}

		public bool collides(dish di)
		{
			if (!di.dead)
			{
				return collision_rect.Intersects(di.collision_rect);
			}
			return false;
		}
	}

	private class goblin : duelist
	{
		private enum STATE
		{
			IDLE,
			THROW,
			HIT,
			LAUGHING
		}

		private const int max_laughter = 4;

		private const int throw_propability = 120;

		private const int jump_propability = 120;

		private STATE current_state;

		private Vector2 pos;

		private sprite idle_throw_hit;

		private sprite laughing;

		private SoundEffect[] laugh_sfx;

		private SoundEffect[] hit_sfx;

		private int laugh_counter;

		public override Rectangle collision_rect => new Rectangle((int)(pos.X - 10f), (int)(pos.Y + jump_pos - 25f), 20, 50);

		public override bool thrown
		{
			get
			{
				if (current_state == STATE.THROW)
				{
					return idle_throw_hit.done;
				}
				return false;
			}
		}

		public goblin(ContentManager content_mgr, int x, int y)
		{
			pos = new Vector2(x, y);
			idle_throw_hit = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/goblin/idle_throw_hit"), 4u, 3u, 8u);
			laughing = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/goblin/laughing"), 2u, 1u, 4u);
			laugh_sfx = new SoundEffect[3]
			{
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_laugh0"),
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_laugh1"),
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_laugh2")
			};
			hit_sfx = new SoundEffect[3]
			{
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_hurt0"),
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_hurt1"),
				content_mgr.Load<SoundEffect>("minigame/goblin_kitchen/sfx/goblin_hurt2")
			};
			current_state = STATE.IDLE;
		}

		public void laugh()
		{
			if (!jumping && current_state != STATE.LAUGHING)
			{
				laugh_counter = 0;
				laughing.frame = 0u;
				current_state = STATE.LAUGHING;
				laugh_sfx[_2d_house_of_terror.game_state.random_gen.Next() % laugh_sfx.Length].Play();
			}
		}

		public void hit()
		{
			current_state = STATE.HIT;
			idle_throw_hit.state = 2u;
			idle_throw_hit.frame = 0u;
			hit_sfx[_2d_house_of_terror.game_state.random_gen.Next() % hit_sfx.Length].Play();
			if (jump_speed < 0f)
			{
				jump_speed = 0f;
			}
		}

		private void throw_dish()
		{
			current_state = STATE.THROW;
			idle_throw_hit.frame = 0u;
			idle_throw_hit.state = 1u;
			throw_speed_vec = new Vector2(2 + _2d_house_of_terror.game_state.random_gen.Next() % 30 / 5, 0f);
		}

		public void update()
		{
			if (current_state == STATE.LAUGHING)
			{
				laughing.animate();
				if (laughing.done && ++laugh_counter == 4)
				{
					current_state = STATE.IDLE;
				}
			}
			else
			{
				idle_throw_hit.animate_cyclic();
			}
			if (current_state == STATE.HIT)
			{
				if (idle_throw_hit.cycle_finished)
				{
					idle_throw_hit.state = 0u;
					current_state = STATE.IDLE;
				}
			}
			else
			{
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 120)
				{
					throw_dish();
				}
				if (current_state == STATE.THROW && idle_throw_hit.cycle_finished)
				{
					current_state = STATE.IDLE;
					idle_throw_hit.state = 0u;
				}
			}
			if (jumping)
			{
				update_jump();
			}
			else if (current_state != STATE.LAUGHING && current_state != STATE.HIT && _2d_house_of_terror.game_state.random_gen.Next() % 6000 < 120)
			{
				jumping = true;
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x = 0, int offset_y = 0)
		{
			if (current_state == STATE.LAUGHING)
			{
				laughing.draw(spr_batch, offset_x + (int)pos.X, offset_y + (int)pos.Y + (int)jump_pos);
			}
			else
			{
				idle_throw_hit.draw(spr_batch, offset_x + (int)pos.X, offset_y + (int)pos.Y + (int)jump_pos);
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y, float zoom)
		{
			laughing.zoom = zoom;
			idle_throw_hit.zoom = zoom;
			if (current_state == STATE.LAUGHING)
			{
				laughing.draw(spr_batch, offset_x + (int)(pos.X * zoom), offset_y + (int)(pos.Y * zoom) + (int)(jump_pos * zoom));
			}
			else
			{
				idle_throw_hit.draw(spr_batch, offset_x + (int)(pos.X * zoom), offset_y + (int)(pos.Y * zoom) + (int)(jump_pos * zoom));
			}
			laughing.zoom = 1f;
			idle_throw_hit.zoom = 1f;
		}
	}

	private class player : duelist
	{
		private enum STATE
		{
			IDLE,
			THROW,
			HIT
		}

		private STATE current_state;

		private Vector2 pos;

		private Vector2 throw_vec;

		private sprite idle_throw;

		private sprite hit_spr;

		private sprite stars_of_confusion;

		private SoundEffect[] hit_sfx;

		private int pl_id;

		private bool ai_controlled;

		private float last_con_x;

		public override Rectangle collision_rect => new Rectangle((int)(pos.X - 20f), (int)(pos.Y + jump_pos - 35f), 40, 70);

		public override bool thrown
		{
			get
			{
				if (current_state == STATE.THROW)
				{
					return idle_throw.done;
				}
				return false;
			}
		}

		public player(ContentManager content_mgr, int x, int y, int player_id = 0)
		{
			pl_id = player_id;
			ai_controlled = game_mgr.player_ids[pl_id] < 0;
			pos = new Vector2(x, y);
			string text = ((game_mgr.char_ids[pl_id] == 0) ? "jimmy" : ((game_mgr.char_ids[pl_id] == 1) ? "sam" : ((game_mgr.char_ids[pl_id] == 2) ? "erik" : "billy")));
			idle_throw = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/" + text + "/idle_throw"), 4u, 2u, 6u);
			hit_spr = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/" + text + "/hit"), 3u, 1u, 8u);
			stars_of_confusion = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/stars_of_confusion"), 3u, 1u, 9u);
			current_state = STATE.IDLE;
			hit_sfx = new SoundEffect[2]
			{
				content_mgr.Load<SoundEffect>("minigame/sfx/oh"),
				content_mgr.Load<SoundEffect>("minigame/sfx/no")
			};
			throw_speed_vec = new Vector2(0f, 0f);
			throw_vec = new Vector2(0f, 0f);
		}

		private bool throw_input()
		{
			if (ai_controlled)
			{
				if (_2d_house_of_terror.game_state.random_gen.Next() % 6000 < 160)
				{
					throw_speed_vec = new Vector2(-(1 + _2d_house_of_terror.game_state.random_gen.Next() % 30 / 5), 0f);
					return true;
				}
				return false;
			}
			float x = controllers.lthumb[game_mgr.player_ids[pl_id]].X;
			float num = last_con_x - x;
			if (num >= 0f)
			{
				throw_vec.X -= 3f * num;
				last_con_x = x;
				if ((double)x < 0.05 && (double)x > -0.05 && throw_vec.X < -1f)
				{
					throw_speed_vec.X = throw_vec.X;
					throw_vec.X = 0f;
					return true;
				}
				return false;
			}
			last_con_x = x;
			throw_vec.X = 0f;
			return false;
		}

		public void hit()
		{
			if (current_state != STATE.HIT)
			{
				hit_sfx[_2d_house_of_terror.game_state.random_gen.Next() % hit_sfx.Length].Play();
			}
			current_state = STATE.HIT;
			idle_throw.state = 0u;
			hit_spr.frame = 0u;
			if (jump_speed < 0f)
			{
				jump_speed = 0f;
			}
		}

		private void throw_dish()
		{
			current_state = STATE.THROW;
			idle_throw.state = 1u;
			idle_throw.frame = 0u;
		}

		public void update()
		{
			if (current_state == STATE.HIT)
			{
				stars_of_confusion.animate();
				hit_spr.animate_cyclic();
				if (hit_spr.cycle_finished)
				{
					current_state = STATE.IDLE;
				}
			}
			else if (current_state == STATE.THROW)
			{
				idle_throw.animate_cyclic();
			}
			else
			{
				idle_throw.animate();
			}
			if (jumping)
			{
				update_jump();
			}
			if (current_state != STATE.HIT && controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.B))
			{
				jump();
			}
			if (throw_input() && current_state != STATE.HIT && current_state != STATE.THROW)
			{
				throw_dish();
			}
			else if (current_state == STATE.THROW && idle_throw.cycle_finished)
			{
				current_state = STATE.IDLE;
				idle_throw.state = 0u;
				throw_speed_vec.X = 0f;
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x = 0, int offset_y = 0)
		{
			if (current_state == STATE.HIT)
			{
				hit_spr.draw(spr_batch, offset_x + (int)pos.X, offset_y + (int)pos.Y + (int)jump_pos);
				stars_of_confusion.draw(spr_batch, offset_x + (int)pos.X, offset_y + collision_rect.Y);
			}
			else
			{
				idle_throw.draw(spr_batch, offset_x + (int)pos.X, offset_y + (int)pos.Y + (int)jump_pos);
			}
		}

		public void draw(SpriteBatch spr_batch, int offset_x, int offset_y, float zoom)
		{
			hit_spr.zoom = zoom;
			stars_of_confusion.zoom = zoom;
			idle_throw.zoom = zoom;
			if (current_state == STATE.HIT)
			{
				hit_spr.draw(spr_batch, offset_x + (int)(pos.X * zoom), offset_y + (int)(pos.Y * zoom) + (int)(jump_pos * zoom));
				stars_of_confusion.draw(spr_batch, offset_x + (int)(pos.X * zoom), offset_y + (int)((float)collision_rect.Y * zoom));
			}
			else
			{
				idle_throw.draw(spr_batch, offset_x + (int)(pos.X * zoom), offset_y + (int)(pos.Y * zoom) + (int)(jump_pos * zoom));
			}
			hit_spr.zoom = 1f;
			stars_of_confusion.zoom = 1f;
			idle_throw.zoom = 1f;
		}
	}

	private class kitchen
	{
		private RasterizerState raster_state;

		private int kitchen_w;

		private int kitchen_h;

		private Texture2D bg;

		private Texture2D[] dish_imgs;

		private sprite fg_spr;

		private goblin gob;

		private player pl;

		private List<dish> dishes;

		private List<sunbathing_vampires.points_anim> points_anims;

		private int points;

		private Texture2D[] pnttex_good;

		private Texture2D[] pnttex_bad;

		public int score => points;

		public kitchen(ContentManager content_mgr, int kitchen_width, int kitchen_height, int player_id = 0)
		{
			raster_state = new RasterizerState
			{
				ScissorTestEnable = true
			};
			kitchen_w = kitchen_width;
			kitchen_h = kitchen_height;
			bg = content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/bg");
			fg_spr = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/fg"), 4u, 1u, 5u);
			pl = new player(content_mgr, 260, 180, player_id);
			gob = new goblin(content_mgr, 50, 180);
			points_anims = new List<sunbathing_vampires.points_anim>();
			dishes = new List<dish>();
			dish_imgs = new Texture2D[2]
			{
				content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/dish"),
				content_mgr.Load<Texture2D>("minigame/goblin_kitchen/gfx/plate")
			};
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
		}

		private void add_points(int pnts, int dest_x, int dest_y)
		{
			points += pnts;
			if (pnts < 0 && pnts < -4 && pnts > -11)
			{
				points_anims.Add(new sunbathing_vampires.points_anim(pnttex_bad[-pnts / 5 - 1], dest_x, dest_y, 1f, 60u));
			}
			else if (pnts > 4 && pnts < 21)
			{
				points_anims.Add(new sunbathing_vampires.points_anim(pnttex_good[pnts / 5 - 1], dest_x, dest_y, 1f, 60u));
			}
		}

		private void update_dishes()
		{
			for (int num = dishes.Count - 1; num >= 0; num--)
			{
				if (dishes[num].update() || dishes[num].collision_rect.X > kitchen_w || dishes[num].collision_rect.X + dishes[num].collision_rect.Width < 0)
				{
					dishes.RemoveAt(num);
				}
				else if (gob.collides(dishes[num]))
				{
					gob.hit();
					dishes[num].hit();
					add_points(gob.airborne ? 20 : 15, gob.collision_rect.X, gob.collision_rect.Y);
				}
				else if (pl.collides(dishes[num]))
				{
					pl.hit();
					dishes[num].hit();
					gob.laugh();
					add_points(pl.airborne ? (-5) : (-10), pl.collision_rect.X, pl.collision_rect.Y);
				}
			}
			if (gob.thrown)
			{
				dishes.Add(new dish(dish_imgs[_2d_house_of_terror.game_state.random_gen.Next() % dish_imgs.Length], gob.collision_rect.X + gob.collision_rect.Width, gob.collision_rect.Y, gob.throw_speed.X, gob.throw_speed.Y));
			}
			if (pl.thrown)
			{
				dishes.Add(new dish(dish_imgs[_2d_house_of_terror.game_state.random_gen.Next() % dish_imgs.Length], pl.collision_rect.X, pl.collision_rect.Y + pl.collision_rect.Height / 2, pl.throw_speed.X, pl.throw_speed.Y));
			}
		}

		private void update_points()
		{
			for (int num = points_anims.Count - 1; num >= 0; num--)
			{
				if (points_anims[num].update())
				{
					points_anims.RemoveAt(num);
				}
			}
		}

		public void update()
		{
			update_dishes();
			gob.update();
			pl.update();
			update_points();
			fg_spr.animate_cyclic();
		}

		private void draw_dishes(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (dish dish in dishes)
			{
				dish.draw(spr_batch, dest_x, dest_y);
			}
		}

		private void draw_point_anims(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			foreach (sunbathing_vampires.points_anim points_anim in points_anims)
			{
				points_anim.draw(spr_batch, new Vector2(dest_x, dest_y));
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			spr_batch.End();
			Rectangle scissorRectangle = spr_batch.GraphicsDevice.ScissorRectangle;
			spr_batch.GraphicsDevice.ScissorRectangle = new Rectangle(dest_x, dest_y, kitchen_w, kitchen_h);
			spr_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, raster_state);
			spr_batch.Draw(bg, new Vector2(dest_x, dest_y), Color.White);
			gob.draw(spr_batch, dest_x, dest_y);
			pl.draw(spr_batch, dest_x, dest_y);
			draw_dishes(spr_batch, dest_x, dest_y);
			draw_point_anims(spr_batch, dest_x, dest_y);
			fg_spr.draw(spr_batch, dest_x + fg_spr.width / 2, dest_y + kitchen_h - fg_spr.height / 2);
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
				Rectangle scissorRectangle2 = new Rectangle(dest_x, dest_y, (int)((float)kitchen_w * zoom), (int)((float)kitchen_h * zoom));
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
				gob.draw(spr_batch, dest_x, dest_y, zoom);
				pl.draw(spr_batch, dest_x, dest_y, zoom);
				spr_batch.End();
				spr_batch.GraphicsDevice.ScissorRectangle = scissorRectangle;
				spr_batch.Begin();
			}
		}

		public void update_intro()
		{
			gob.laugh();
			gob.update();
		}
	}

	private enum STATE
	{
		INTRO,
		GAME,
		OUTRO
	}

	private const int intro_laugh_counter = 120;

	private const int intro_zoom_counter = 240;

	private const int intro_start_counter = 360;

	private const int intro_wait_counter = 420;

	private const int outro_timeup_counter = 60;

	private const int outro_zoom_counter = 120;

	private const int outro_wait_counter = 180;

	private STATE game_state;

	private kitchen[] kitchens;

	private int winner_id = -1;

	private clock timer;

	private Texture2D border_v;

	private Texture2D border_h;

	private Song bgm;

	private int intro_counter;

	private int outro_counter;

	private int outro_points_counter;

	public goblin_kitchen(IServiceProvider serv, GraphicsDevice dev, bool beginners_mode = false)
		: base(serv, dev, beginners_mode)
	{
		controls_spr = new sprite(content_mgr.Load<Texture2D>("minigame/goblin_kitchen/controls"), 4u, 1u, 4u);
		title_img = content_mgr.Load<Texture2D>("minigame/goblin_kitchen/title");
		preview_img = content_mgr.Load<Texture2D>("minigame/goblin_kitchen/preview");
		bgm = content_mgr.Load<Song>("bgm/goblin_kitchen");
		border_v = content_mgr.Load<Texture2D>("minigame/gfx/border_vertical");
		border_h = content_mgr.Load<Texture2D>("minigame/gfx/border_horizontal");
		timer = new clock(content_mgr.Load<Texture2D>("minigame/gfx/clock"), default_font, 45, 76, new Color(75, 64, 21), 99);
		kitchens = new kitchen[4]
		{
			new kitchen(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2),
			new kitchen(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 1),
			new kitchen(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 2),
			new kitchen(content_mgr, dev.Viewport.Width / 2, dev.Viewport.Height / 2, 3)
		};
		game_state = STATE.INTRO;
		explanation_gfx = new Texture2D[2]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/goblin_kitchen/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/goblin_kitchen/1")
		};
		explanation_txt = new string[2] { "You arrived in the kitchen where goblins roam.  \nThey will try to hurt you by\nthrowing whatever they can find.\nAll you have to do is the very same thing.", "Avoid the projectiles by jumping\nand throw your own dishes by\nflicking the left analog stick." };
	}

	~goblin_kitchen()
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
		for (int i = 0; i < kitchens.Length; i++)
		{
			kitchens[i].update();
		}
		timer.update();
		return timer.seconds < 1;
	}

	private bool update_intro()
	{
		if (intro_counter < 120)
		{
			for (int i = 0; i < 4; i++)
			{
				kitchens[i].update_intro();
			}
		}
		else if (intro_counter == 240)
		{
			start_sfx.Play();
		}
		return ++intro_counter >= 420;
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
				minigame.points[i] = kitchens[i].score;
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
			kitchens[0].draw(spr_batch, 0, 0, 2f);
			return;
		}
		if (intro_counter <= 240)
		{
			float num = 2f - (float)(intro_counter - 120) / 120f;
			for (int i = 0; i < kitchens.Length; i++)
			{
				kitchens[i].draw(spr_batch, i % 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num / 2f), i / 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num / 2f), num);
			}
			return;
		}
		float num2 = (float)(intro_counter - 240) / 120f;
		num2 = ((num2 > 1f) ? 1f : num2);
		float num3 = 4f - 3f * num2;
		for (int j = 0; j < kitchens.Length; j++)
		{
			kitchens[j].draw(spr_batch, j % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, j / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
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
			for (int k = 0; k < kitchens.Length; k++)
			{
				kitchens[k].draw(spr_batch, num3 + k % 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num2 / 2f), num4 + k / 2 * (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num2 / 2f), num2);
			}
			spr_batch.Draw(winner_txt, new Rectangle(num3 + (int)((float)((winner_id % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width) * num2 / 4f), num4 + (int)((float)((winner_id / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height) * num2 / 4f), (int)((float)winner_txt.Width * (3f - num2)), (int)((float)winner_txt.Height * (3f - num2))), new Rectangle(0, 0, winner_txt.Width, winner_txt.Height), Color.White, 0f, new Vector2(winner_txt.Width / 2, winner_txt.Height / 2), SpriteEffects.None, 0f);
		}
	}

	private void draw_gameplay()
	{
		for (int i = 0; i < kitchens.Length; i++)
		{
			kitchens[i].draw(spr_batch, i % 2 * spr_batch.GraphicsDevice.Viewport.Width / 2, i / 2 * spr_batch.GraphicsDevice.Viewport.Height / 2);
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
