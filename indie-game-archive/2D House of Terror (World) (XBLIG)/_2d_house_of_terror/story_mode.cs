using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class story_mode : game_state
{
	private class credit_roll
	{
		private string txt;

		private SpriteFont font;

		private Vector2 credits_size;

		private int frame_counter;

		private int number_of_frames;

		private int delay;

		private Song bgm_sng;

		public credit_roll(string text, int duration, SpriteFont fnt, Song bgm = null, int delay_secs = 0)
		{
			txt = text;
			number_of_frames = ((duration <= 0) ? 1 : (duration * 60));
			delay = delay_secs * 60;
			font = fnt;
			credits_size = fnt.MeasureString(txt);
			bgm_sng = bgm;
		}

		~credit_roll()
		{
			free();
		}

		public void free()
		{
			MediaPlayer.Stop();
			MediaPlayer.IsRepeating = true;
		}

		public bool update()
		{
			if (bgm_sng != null && frame_counter == delay)
			{
				MediaPlayer.Play(bgm_sng);
				MediaPlayer.IsRepeating = false;
			}
			if (++frame_counter <= number_of_frames)
			{
				return controllers.clicked(CONTROLLER_BUTTONS.BACK);
			}
			return true;
		}

		public void draw(SpriteBatch spr_batch, int x = 0, int y = 0)
		{
			spr_batch.GraphicsDevice.Clear(Color.Black);
			spr_batch.Begin();
			float num = (float)(frame_counter - delay) / (float)number_of_frames;
			num = ((num > 1f) ? 1f : num);
			spr_batch.DrawString(font, txt, new Vector2((float)(x + 320) - credits_size.X / 2f, (float)(y + spr_batch.GraphicsDevice.Viewport.Height) - num * ((float)spr_batch.GraphicsDevice.Viewport.Height + credits_size.Y)), Color.White);
			spr_batch.End();
		}
	}

	private enum STATE
	{
		STAGE0,
		STAGE1,
		STAGE2,
		STAGE3,
		STAGE4,
		END
	}

	private STATE current_state;

	private minigame current_game;

	private video_player vid_player;

	private Texture2D loading_screen;

	private credit_roll credits;

	private bool show_credits;

	private string credits_txt;

	public story_mode(GraphicsDevice dev, IServiceProvider serv, bool beg_mode = false)
		: base(dev, serv, beg_mode)
	{
		current_game = null;
		vid_player = new video_player("fmv/story0", GAME_STATE.STORY_MODE, GAME_STATE.QUIT, dev, serv);
		using Stream stream = TitleContainer.OpenStream("credits.txt");
		using StreamReader streamReader = new StreamReader(stream);
		credits_txt = streamReader.ReadToEnd();
	}

	~story_mode()
	{
		free();
	}

	public override void free()
	{
		base.free();
	}

	public override GAME_STATE update()
	{
		base.update();
		if (show_credits)
		{
			if (credits.update())
			{
				credits.free();
				return GAME_STATE.HIGHSCORE;
			}
			return GAME_STATE.STORY_MODE;
		}
		if (vid_player != null && vid_player.update() == GAME_STATE.QUIT)
		{
			vid_player = null;
			switch (current_state)
			{
			case STATE.STAGE0:
				current_game = new crypt_of_the_dumb_undead(services, gfx_dev, beginners_mode);
				break;
			case STATE.STAGE1:
				current_game = new spiritual_ascension(services, gfx_dev, beginners_mode);
				break;
			case STATE.STAGE2:
				current_game = new goblin_kitchen(services, gfx_dev, beginners_mode);
				break;
			case STATE.STAGE3:
				current_game = new musical_madness(services, gfx_dev, beginners_mode);
				break;
			case STATE.STAGE4:
				current_game = new sunbathing_vampires(services, gfx_dev, beginners_mode);
				break;
			case STATE.END:
				content_mgr.Load<SoundEffect>("sfx/gamemaster_credits").Play();
				credits = new credit_roll(credits_txt, 120, content_mgr.Load<SpriteFont>("default_font"), content_mgr.Load<Song>("bgm/credits"), 10);
				show_credits = true;
				break;
			}
		}
		else if (current_game != null)
		{
			if (current_game.update())
			{
				current_state++;
				current_game.free();
				current_game = null;
				string filename = "fmv/story" + Convert.ToString((int)current_state);
				vid_player = new video_player(filename, GAME_STATE.STORY_MODE, GAME_STATE.QUIT, gfx_dev, services);
			}
			else if (current_game.quit_instructed)
			{
				current_game.free();
				current_game = null;
				return GAME_STATE.MAIN_MENU;
			}
		}
		return GAME_STATE.STORY_MODE;
	}

	public override void draw()
	{
		base.draw();
		if (current_game != null)
		{
			current_game.draw();
		}
		if (vid_player != null)
		{
			vid_player.draw();
		}
		if (show_credits && credits != null)
		{
			credits.draw(spr_batch);
		}
		spr_batch.Begin();
		spr_batch.End();
	}
}
