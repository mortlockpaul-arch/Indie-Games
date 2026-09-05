using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class random_mode : game_state
{
	private minigame current_game;

	private Texture2D bg;

	private SpriteFont font;

	private bool confirmed_quantity;

	private int played_games;

	private int number_of_games = 5;

	private int last_random = -1;

	private award_ceremony award_cerem;

	public int highscore_list_id => number_of_games / 5 - 1;

	public random_mode(GraphicsDevice dev, IServiceProvider serv, bool beg_mode = false)
		: base(dev, serv, beg_mode)
	{
		font = content_mgr.Load<SpriteFont>("default_font");
		bg = content_mgr.Load<Texture2D>("menu/random/bg");
		current_game = random_game();
		fade.to_color(60u, Color.Black, fadein: true);
	}

	~random_mode()
	{
		free();
	}

	public override void free()
	{
		base.free();
	}

	private minigame random_game()
	{
		int num = -1;
		while (num < 0 || num == last_random)
		{
			num = game_state.random_gen.Next() % 5;
		}
		last_random = num;
		return num switch
		{
			0 => new sunbathing_vampires(services, gfx_dev, beginners_mode), 
			1 => new spiritual_ascension(services, gfx_dev, beginners_mode), 
			2 => new goblin_kitchen(services, gfx_dev, beginners_mode), 
			3 => new musical_madness(services, gfx_dev, beginners_mode), 
			_ => new crypt_of_the_dumb_undead(services, gfx_dev, beginners_mode), 
		};
	}

	private void update_quantity_selection()
	{
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_UP) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_UP))
		{
			number_of_games += 5;
		}
		if (controllers.clicked(CONTROLLER_BUTTONS.DPAD_DOWN) || controllers.clicked(CONTROLLER_BUTTONS.LTHUMB_DOWN))
		{
			number_of_games -= 5;
		}
		number_of_games = ((number_of_games <= 0) ? 20 : ((number_of_games > 20) ? 5 : number_of_games));
		if (controllers.clicked(CONTROLLER_BUTTONS.START) || controllers.clicked(CONTROLLER_BUTTONS.A))
		{
			confirmed_quantity = true;
		}
	}

	private GAME_STATE update_games()
	{
		if (award_cerem != null)
		{
			if (!award_cerem.update())
			{
				return GAME_STATE.RANDOM_MODE;
			}
			return GAME_STATE.HIGHSCORE;
		}
		if (current_game.update())
		{
			current_game.free();
			current_game = null;
			if (++played_games == number_of_games)
			{
				award_cerem = new award_ceremony(content_mgr, spr_batch);
			}
			else
			{
				current_game = random_game();
			}
		}
		else if (current_game.quit_instructed)
		{
			current_game.free();
			current_game = null;
			return GAME_STATE.MAIN_MENU;
		}
		return GAME_STATE.RANDOM_MODE;
	}

	public override GAME_STATE update()
	{
		base.update();
		if (!confirmed_quantity)
		{
			update_quantity_selection();
			return GAME_STATE.RANDOM_MODE;
		}
		return update_games();
	}

	private void draw_quantity_selection()
	{
		spr_batch.Draw(bg, new Vector2(0f, 0f), Color.White);
		string text = "How many games would\nyou like to play?\n" + Convert.ToString(number_of_games) + " Games";
		Vector2 vector = font.MeasureString(text);
		spr_batch.DrawString(font, text, new Vector2(gfx_dev.Viewport.Width / 2, (float)(gfx_dev.Viewport.Height / 2) - vector.Y), new Color(255, 255, 255), 0f, new Vector2(vector.X / 2f, vector.Y / 2f), 1f, SpriteEffects.None, 0f);
	}

	public override void draw()
	{
		base.draw();
		if (confirmed_quantity)
		{
			if (current_game != null)
			{
				current_game.draw();
			}
			if (award_cerem != null)
			{
				award_cerem.draw();
			}
		}
		else
		{
			gfx_dev.Clear(Color.Black);
			spr_batch.Begin();
			draw_quantity_selection();
			fade.draw(spr_batch);
			spr_batch.End();
		}
	}
}
