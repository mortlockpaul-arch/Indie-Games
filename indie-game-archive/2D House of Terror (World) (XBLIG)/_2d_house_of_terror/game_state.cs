using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_house_of_terror;

public class game_state
{
	protected GraphicsDevice gfx_dev;

	protected SpriteBatch spr_batch;

	protected ContentManager content_mgr;

	protected IServiceProvider services;

	protected fade_effect fade;

	public static Random random_gen;

	protected bool beginners_mode;

	public virtual bool easy_mode => false;

	protected game_state(GraphicsDevice dev, IServiceProvider serv, bool beg_mode = false)
	{
		gfx_dev = dev;
		services = serv;
		beginners_mode = beg_mode;
		load();
	}

	~game_state()
	{
		free();
	}

	protected virtual void load()
	{
		free();
		random_gen = new Random();
		spr_batch = new SpriteBatch(gfx_dev);
		content_mgr = new ContentManager(services);
		content_mgr.RootDirectory = "Content";
		fade = new fade_effect(gfx_dev);
	}

	public virtual void free()
	{
		if (spr_batch != null)
		{
			spr_batch.Dispose();
		}
		if (content_mgr != null)
		{
			content_mgr.Unload();
		}
	}

	public virtual GAME_STATE update()
	{
		controllers.update();
		fade.update();
		return GAME_STATE.QUIT;
	}

	public virtual void draw()
	{
	}
}
public enum GAME_STATE
{
	INTRO_VIDEO,
	MAIN_MENU,
	STORY_MODE,
	RANDOM_MODE,
	HIGHSCORE,
	QUIT
}
