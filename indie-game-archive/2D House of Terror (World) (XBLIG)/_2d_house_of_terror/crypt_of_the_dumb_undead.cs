using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class crypt_of_the_dumb_undead : minigame
{
	private class points_anim
	{
		private Texture2D img;

		private uint anim_counter;

		private uint max_anim_counter;

		public bool done => anim_counter >= max_anim_counter;

		public points_anim(Texture2D image, uint anim_steps)
		{
			img = image;
			anim_counter = 0u;
			max_anim_counter = anim_steps;
		}

		public void update()
		{
			anim_counter++;
		}

		public void draw(SpriteBatch spr_batch, Vector2 offset)
		{
			spr_batch.Draw(img, new Vector2(offset.X, offset.Y - (float)anim_counter), Color.White * (1f - (float)anim_counter / (float)max_anim_counter));
		}
	}

	private class player
	{
		private enum STATE
		{
			ATTACK,
			HURT,
			IDLE,
			SHOVEL
		}

		private enum SIDE
		{
			LEFT,
			RIGHT
		}

		private const uint max_dirtcount = 60u;

		private STATE current_state;

		private uint idle_counter;

		private uint pl_id;

		private bool ai_controlled;

		private SoundEffect shovel_sfx;

		private SoundEffect hit_sfx;

		private sprite[] sprites;

		private Texture2D[][] dirtpile;

		private uint[] dirtcount;

		private bool burial_complete;

		private Texture2D points5;

		private Texture2D points10;

		private Texture2D points15;

		private Texture2D points20;

		private List<points_anim>[] point_anims;

		private uint points;

		public uint score => points;

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

		public bool completed_burial => burial_complete;

		public bool stunned => current_state == STATE.HURT;

		public player(string folder, ContentManager content_mgr, uint player_id)
		{
			shovel_sfx = content_mgr.Load<SoundEffect>("minigame/cotdu/sfx/shovel");
			hit_sfx = content_mgr.Load<SoundEffect>("minigame/cotdu/sfx/hit");
			sprites = new sprite[4]
			{
				new sprite(content_mgr.Load<Texture2D>(folder + "/attack"), 4u, 2u, 7u),
				new sprite(content_mgr.Load<Texture2D>(folder + "/hurt"), 1u, 1u, 1u),
				new sprite(content_mgr.Load<Texture2D>(folder + "/idle"), 3u, 1u, 3u),
				new sprite(content_mgr.Load<Texture2D>(folder + "/shovel"), 5u, 2u, 5u)
			};
			dirtpile = new Texture2D[2][]
			{
				new Texture2D[3]
				{
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/left0"),
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/left1"),
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/left2")
				},
				new Texture2D[3]
				{
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/right0"),
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/right1"),
					content_mgr.Load<Texture2D>("minigame/cotdu/dirt/right2")
				}
			};
			uint[] array = new uint[2];
			dirtcount = array;
			current_state = STATE.IDLE;
			idle_counter = 0u;
			pl_id = player_id;
			ai_controlled = game_mgr.player_ids[pl_id] < 0;
			points5 = content_mgr.Load<Texture2D>("minigame/gfx/5");
			points10 = content_mgr.Load<Texture2D>("minigame/gfx/10");
			points15 = content_mgr.Load<Texture2D>("minigame/gfx/15");
			points20 = content_mgr.Load<Texture2D>("minigame/gfx/20");
			point_anims = new List<points_anim>[2]
			{
				new List<points_anim>(),
				new List<points_anim>()
			};
			points = 0u;
		}

		public bool vulnerable(int side_id)
		{
			if (side_id < 0 || side_id > 1)
			{
				return false;
			}
			return dirtcount[side_id] < 30;
		}

		public void shock()
		{
			if (current_state != STATE.HURT && current_state != STATE.ATTACK)
			{
				idle_counter = 0u;
				current_state = STATE.HURT;
			}
		}

		public bool has_hit(int side_id)
		{
			if (side_id < 0 || side_id > 1)
			{
				return false;
			}
			if (current_state == STATE.ATTACK && sprites[(int)current_state].done)
			{
				return sprites[(int)current_state].state == side_id;
			}
			return false;
		}

		public void add_points(int side_id, uint p_num)
		{
			if (side_id >= 0 && side_id <= 1)
			{
				points += p_num;
				switch (p_num)
				{
				case 5u:
					point_anims[side_id].Add(new points_anim(points5, 60u));
					break;
				case 10u:
					point_anims[side_id].Add(new points_anim(points10, 60u));
					break;
				case 15u:
					point_anims[side_id].Add(new points_anim(points15, 60u));
					break;
				case 20u:
					point_anims[side_id].Add(new points_anim(points20, 60u));
					break;
				}
			}
		}

		public void update()
		{
			switch (current_state)
			{
			case STATE.ATTACK:
				if (sprites[(int)current_state].done)
				{
					current_state = STATE.IDLE;
				}
				break;
			case STATE.HURT:
				if (idle_counter++ == 120)
				{
					current_state = STATE.IDLE;
					idle_counter = 0u;
				}
				break;
			default:
				if (controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LEFT_SHOULDER) || controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.RIGHT_SHOULDER))
				{
					hit_sfx.Play();
					current_state = STATE.ATTACK;
					sprites[(int)current_state].state = ((!controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.LEFT_SHOULDER)) ? 1u : 0u);
				}
				if ((ai_controlled && game_state.random_gen.Next() % 11 < 2) || controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.X) || controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.B))
				{
					idle_counter = 0u;
					current_state = STATE.SHOVEL;
					if (ai_controlled)
					{
						sprites[(int)current_state].state = ((game_state.random_gen.Next() % 30 == 0) ? ((sprites[(int)current_state].state + 1) % 2) : sprites[(int)current_state].state);
					}
					else
					{
						sprites[(int)current_state].state = ((!controllers.clicked(game_mgr.player_ids[pl_id], CONTROLLER_BUTTONS.X)) ? 1u : 0u);
					}
					burial_complete = false;
					dirtcount[sprites[(int)current_state].state]++;
					if (dirtcount[sprites[(int)current_state].state] == 15)
					{
						add_points((int)sprites[(int)current_state].state, 5u);
					}
					else if (dirtcount[sprites[(int)current_state].state] == 30)
					{
						add_points((int)sprites[(int)current_state].state, 10u);
					}
					else if (dirtcount[sprites[(int)current_state].state] == 45)
					{
						add_points((int)sprites[(int)current_state].state, 15u);
					}
					else if (dirtcount[sprites[(int)current_state].state] == 60)
					{
						add_points((int)sprites[(int)current_state].state, 20u);
						burial_complete = true;
					}
					dirtcount[sprites[(int)current_state].state] %= 60u;
				}
				else
				{
					idle_counter++;
					if (idle_counter > 15 && current_state == STATE.SHOVEL)
					{
						current_state = STATE.IDLE;
					}
				}
				break;
			}
			if (current_state == STATE.SHOVEL && sprites[(int)current_state].done)
			{
				shovel_sfx.Play();
			}
			sprites[(int)current_state].animate();
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < point_anims[i].Count; j++)
				{
					point_anims[i][j].update();
					if (point_anims[i][j].done)
					{
						point_anims[i].RemoveAt(j);
						j--;
					}
				}
			}
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			sprites[(int)current_state].draw(spr_batch, dest_x, dest_y);
			if (dirtcount[0] >= 15)
			{
				Texture2D texture2D = dirtpile[0][dirtcount[0] / 15 - 1];
				spr_batch.Draw(texture2D, new Vector2(dest_x - spr_batch.GraphicsDevice.Viewport.Width / 8 - texture2D.Width / 2, dest_y - texture2D.Height + 90), Color.White);
			}
			if (dirtcount[1] >= 15)
			{
				Texture2D texture2D2 = dirtpile[1][dirtcount[1] / 15 - 1];
				spr_batch.Draw(texture2D2, new Vector2(dest_x + spr_batch.GraphicsDevice.Viewport.Width / 8 - texture2D2.Width / 2, dest_y - texture2D2.Height + 90), Color.White);
			}
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < point_anims[i].Count; j++)
				{
					point_anims[i][j].draw(spr_batch, new Vector2(dest_x + ((i != 0) ? 1 : (-1)) * spr_batch.GraphicsDevice.Viewport.Width / 8, dest_y));
				}
			}
		}
	}

	private class zombie
	{
		private enum STATE
		{
			NORMAL,
			ATTACK
		}

		private sprite spr;

		private SoundEffect hit_sfx;

		private SoundEffect sfx;

		public bool attacking
		{
			get
			{
				return spr.state == 1;
			}
			set
			{
				if (!value && spr.state != 0)
				{
					hit_sfx.Play();
				}
				spr.state = (value ? 1u : 0u);
			}
		}

		public zombie(Texture2D img, ContentManager content_mgr)
		{
			spr = new sprite(img, 4u, 2u, 5u);
			sfx = content_mgr.Load<SoundEffect>("minigame/cotdu/sfx/zombie");
			hit_sfx = content_mgr.Load<SoundEffect>("minigame/cotdu/sfx/" + ((game_state.random_gen.Next() % 2 == 0) ? "zombie_hit" : "zombie_hit2"));
		}

		public void hit()
		{
			if (spr.state != 0)
			{
				hit_sfx.Play();
			}
			spr.state = 0u;
		}

		public void update()
		{
			if (game_state.random_gen.Next() % 12000 < 2)
			{
				sfx.Play();
			}
			spr.animate();
		}

		public void draw(SpriteBatch spr_batch, int dest_x, int dest_y)
		{
			spr.draw(spr_batch, dest_x, dest_y);
		}
	}

	private enum STATE
	{
		INTRO,
		GAME,
		OUTRO
	}

	private const uint intro_step_2 = 120u;

	private const uint intro_step_3 = 180u;

	private const uint intro_end = 300u;

	private const uint intro_txt_end = 270u;

	private const uint outro_timeup_step = 60u;

	private const uint outro_step_2 = 100u;

	private const uint outro_winner_txt = 160u;

	private const uint outro_end = 200u;

	private STATE crypt_state;

	private Texture2D bg;

	private clock timer;

	private player[] players;

	private zombie[][] zombies;

	private uint intro_counter;

	private bool[] winner = new bool[4];

	private uint outro_counter;

	private uint[] outro_score_counter = new uint[4];

	private bool outro_score_count_done;

	private Song bgm;

	public crypt_of_the_dumb_undead(IServiceProvider serv, GraphicsDevice dev, bool beginners_mode = false)
		: base(serv, dev, beginners_mode)
	{
		controls_spr = new sprite(content_mgr.Load<Texture2D>("minigame/cotdu/controls"), 1u, 1u, 1u);
		title_img = content_mgr.Load<Texture2D>("minigame/cotdu/title");
		preview_img = content_mgr.Load<Texture2D>("minigame/cotdu/preview");
		bg = content_mgr.Load<Texture2D>("minigame/cotdu/bg");
		timer = new clock(content_mgr.Load<Texture2D>("minigame/gfx/clock"), content_mgr.Load<SpriteFont>("default_font"), 45, 76, new Color(75, 64, 21), 99);
		players = new player[4]
		{
			new player("minigame/cotdu/sprites/jimmy", content_mgr, 0u),
			new player("minigame/cotdu/sprites/sam", content_mgr, 1u),
			new player("minigame/cotdu/sprites/erik", content_mgr, 2u),
			new player("minigame/cotdu/sprites/billy", content_mgr, 3u)
		};
		for (uint num = 0u; num < 4; num++)
		{
			players[game_mgr.char_ids[num]].player_id = num;
		}
		Texture2D img = content_mgr.Load<Texture2D>("minigame/cotdu/sprites/zombie/left");
		Texture2D img2 = content_mgr.Load<Texture2D>("minigame/cotdu/sprites/zombie/right");
		zombies = new zombie[4][]
		{
			new zombie[2]
			{
				new zombie(img, content_mgr),
				new zombie(img2, content_mgr)
			},
			new zombie[2]
			{
				new zombie(img, content_mgr),
				new zombie(img2, content_mgr)
			},
			new zombie[2]
			{
				new zombie(img, content_mgr),
				new zombie(img2, content_mgr)
			},
			new zombie[2]
			{
				new zombie(img, content_mgr),
				new zombie(img2, content_mgr)
			}
		};
		crypt_state = STATE.INTRO;
		bgm = content_mgr.Load<Song>("bgm/crypt");
		explanation_gfx = new Texture2D[9]
		{
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/1"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/2"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/3"),
			content_mgr.Load<Texture2D>("minigame/gfx/explanation/crypt/3")
		};
		explanation_txt = new string[9] { "You are surrounded by zombies and\nhave shovels to defend yourself.\nUse those to deal with the attackers\ndepending on the state they are in.\n  Its simple.", "There are two states.\nIn their first state your fiends try\nto get out of their resting places\nand only the arms are visible.\n If you wait too long\n (or simply run out of luck),\nthe zombie will manage to get out.", "While the zombies are still\nin their graves you have to try\nand bury them by shovelling dirt\non top of them.", "This is accomplished by rapidly\npressing the X-button for your left\n or the B-button for your right side.", "There are three different scales\nof earthpiles. Your pile will\nget higher as you shovel\nand grow from state 1 to 3.", "You will be awarded\n5 Points for the lowest\n10 for the middle one and\n20 for the highest.", "You have to shovel until the\nlast one dissappears, after\nwhich a new zombie will spawn.\nThis cicle repeats until\nyou run out of time.", "If the zombies manage to get\nout and grab you, just hit\nthem with your shovel by\npressing the right or left\nshoulder button.", "But beware, the shock of being\ngrabbed might stun you\nfor a short moment of time." };
	}

	~crypt_of_the_dumb_undead()
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
		timer.update();
		for (int i = 0; i < 4; i++)
		{
			players[i].update();
			for (int j = 0; j < 2; j++)
			{
				if (!players[game_mgr.char_ids[i]].stunned || !zombies[i][j].attacking)
				{
					zombies[i][j].update();
				}
				if (players[game_mgr.char_ids[i]].completed_burial)
				{
					zombies[i][j].attacking = false;
					if (game_state.random_gen.Next() % 10 < 2)
					{
						random_success_sfx().Play();
					}
				}
				if (!zombies[i][j].attacking && players[game_mgr.char_ids[i]].vulnerable(j) && game_state.random_gen.Next() % 600 == 0)
				{
					zombies[i][j].attacking = true;
					players[game_mgr.char_ids[i]].shock();
					if (game_state.random_gen.Next() % 10 < 2)
					{
						random_fail_sfx().Play();
					}
				}
				else if (zombies[i][j].attacking)
				{
					if (players[game_mgr.char_ids[i]].has_hit(j))
					{
						zombies[i][j].hit();
						players[game_mgr.char_ids[i]].add_points(j, 10u);
					}
					else if (game_state.random_gen.Next() % 60 == 0)
					{
						players[game_mgr.char_ids[i]].shock();
					}
				}
			}
		}
		if (timer.seconds == 0)
		{
			for (uint num = 0u; num < 4; num++)
			{
				minigame.points[num] = (int)players[game_mgr.char_ids[num]].score;
			}
		}
		return timer.seconds == 0;
	}

	private bool update_intro()
	{
		intro_counter++;
		if (intro_counter == 180)
		{
			start_sfx.Play();
		}
		return intro_counter > 300;
	}

	private bool update_outro()
	{
		if (outro_counter == 0)
		{
			timeup_sfx.Play();
		}
		else if (outro_counter > 100)
		{
			if (!outro_score_count_done)
			{
				outro_counter--;
				outro_score_count_done = true;
				bool[] array = new bool[4];
				bool[] array2 = array;
				for (int i = 0; i < 4; i++)
				{
					array2[i] = false;
					if (outro_score_counter[i] < minigame.points[i])
					{
						array2[i] = true;
						outro_score_counter[i]++;
						outro_score_count_done = false;
					}
				}
				if (!outro_score_count_done)
				{
					winner = array2;
				}
			}
			else if (outro_counter == 101)
			{
				random_success_sfx().Play();
			}
		}
		if (outro_counter <= 200)
		{
			outro_counter++;
			if (outro_counter > 200)
			{
				fade.random(90u, Color.Black);
			}
		}
		if (outro_counter > 200)
		{
			return fade.almost_done;
		}
		return false;
	}

	public override bool update_game()
	{
		switch (crypt_state)
		{
		case STATE.INTRO:
			if (update_intro())
			{
				MediaPlayer.Play(bgm);
				crypt_state = STATE.GAME;
			}
			break;
		case STATE.GAME:
			if (update_gameplay())
			{
				crypt_state = STATE.OUTRO;
			}
			break;
		case STATE.OUTRO:
			return update_outro();
		}
		return false;
	}

	private void draw_intro()
	{
		spr_batch.GraphicsDevice.Clear(Color.Black);
		if (intro_counter <= 120)
		{
			float num = 1f - (float)intro_counter / 120f;
			float rotation = (float)intro_counter * 0.1f;
			spr_batch.Draw(bg, new Rectangle(spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num), (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num)), new Rectangle(0, 0, bg.Width / 2, bg.Height / 2), Color.White, rotation, new Vector2(bg.Width / 4, bg.Height / 4), SpriteEffects.None, 0f);
			return;
		}
		if (intro_counter <= 180)
		{
			float num2 = (float)(intro_counter - 120) / 60f;
			float rotation2 = (float)(Math.PI * 2.0 * (double)intro_counter / 18.0);
			spr_batch.Draw(bg, new Rectangle(spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2, (int)((float)spr_batch.GraphicsDevice.Viewport.Width * num2), (int)((float)spr_batch.GraphicsDevice.Viewport.Height * num2)), new Rectangle(0, 0, bg.Width, bg.Height), Color.White, rotation2, new Vector2(bg.Width / 2, bg.Height / 2), SpriteEffects.None, 0f);
			timer.zoom = 5f - 4f * num2;
			timer.draw(spr_batch, (int)((float)(spr_batch.GraphicsDevice.Viewport.Width / 2) + (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Width / 2f), (int)((float)(spr_batch.GraphicsDevice.Viewport.Height / 2) - (1f - num2) * (float)spr_batch.GraphicsDevice.Viewport.Height / 2f));
			return;
		}
		float num3 = 4f - 3f * (float)(intro_counter - 180) / 90f;
		num3 = ((num3 < 1f) ? 1f : num3);
		draw_gameplay();
		for (int i = 0; i < 4; i++)
		{
			spr_batch.Draw(start_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num3), (int)((float)start_txt.Height * num3)), new Rectangle(0, 0, start_txt.Width, start_txt.Height), Color.White, 0f, new Vector2(start_txt.Width / 2, start_txt.Height / 2), SpriteEffects.None, 0f);
		}
		timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
	}

	private void draw_outro()
	{
		draw_gameplay();
		if (outro_counter <= 100)
		{
			float num = 4f - 3f * (float)outro_counter / 60f;
			num = ((num < 1f) ? 1f : num);
			for (int i = 0; i < 4; i++)
			{
				spr_batch.Draw(timeup_txt, new Rectangle((i % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (i / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)start_txt.Width * num), (int)((float)start_txt.Height * num)), new Rectangle(0, 0, timeup_txt.Width, timeup_txt.Height), Color.White, 0f, new Vector2(timeup_txt.Width / 2, timeup_txt.Height / 2), SpriteEffects.None, 0f);
			}
		}
		else if (!outro_score_count_done)
		{
			for (int j = 0; j < 4; j++)
			{
				string text = Convert.ToString(outro_score_counter[j]);
				Vector2 vector = default_font_large.MeasureString(text);
				spr_batch.DrawString(default_font_large, text, new Vector2((j % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (j / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4), new Color(0, 255, 0), 0f, new Vector2(vector.X / 2f, vector.Y / 2f), 1f, SpriteEffects.None, 0f);
			}
		}
		else
		{
			float num2 = 4f - 3f * (float)(outro_counter - 100) / 60f;
			num2 = ((num2 < 1f) ? 1f : num2);
			for (int k = 0; k < 4; k++)
			{
				if (winner[k])
				{
					spr_batch.Draw(winner_txt, new Rectangle((k % 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Width / 4, (k / 2 * 2 + 1) * spr_batch.GraphicsDevice.Viewport.Height / 4, (int)((float)winner_txt.Width * num2), (int)((float)winner_txt.Height * num2)), new Rectangle(0, 0, winner_txt.Width, winner_txt.Height), Color.White, 0f, new Vector2(winner_txt.Width / 2, winner_txt.Height / 2), SpriteEffects.None, 0f);
				}
			}
		}
		timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
	}

	private void draw_gameplay()
	{
		spr_batch.Draw(bg, new Vector2(0f, 0f), Color.White);
		Vector2 vector = new Vector2(spr_batch.GraphicsDevice.Viewport.Width / 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
		for (int i = 0; i < 4; i++)
		{
			Vector2 vector2 = new Vector2((float)(i % 2) * vector.X, (float)(i / 2) * vector.Y);
			zombies[i][0].draw(spr_batch, (int)(vector2.X + vector.X / 4f), (int)(vector2.Y + vector.Y * 2f / 3f));
			zombies[i][1].draw(spr_batch, (int)(vector2.X + vector.X * 3f / 4f), (int)(vector2.Y + vector.Y * 2f / 3f));
			players[game_mgr.char_ids[i]].draw(spr_batch, (int)(vector2.X + vector.X / 2f), (int)(vector2.Y + vector.Y * 4f / 7f));
		}
		timer.draw(spr_batch, spr_batch.GraphicsDevice.Viewport.Width / 2 + 2, spr_batch.GraphicsDevice.Viewport.Height / 2);
	}

	public override void draw_game()
	{
		switch (crypt_state)
		{
		case STATE.INTRO:
			if (intro_counter == 0)
			{
				fade.cross(90u, 40u, Color.Black, fadein: true);
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
