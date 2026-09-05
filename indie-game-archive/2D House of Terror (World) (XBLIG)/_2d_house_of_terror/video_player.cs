using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace _2d_house_of_terror;

public class video_player : game_state
{
	private VideoPlayer pl;

	private Texture2D vid_tex;

	private Video vid;

	private GAME_STATE current_state;

	private GAME_STATE next_state;

	public video_player(string filename, GAME_STATE state_current, GAME_STATE state_next, GraphicsDevice dev, IServiceProvider serv)
		: base(dev, serv)
	{
		vid = content_mgr.Load<Video>(filename);
		pl = new VideoPlayer();
		pl.Play(vid);
		current_state = state_current;
		next_state = state_next;
	}

	~video_player()
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
		if (pl.State == MediaState.Stopped)
		{
			return next_state;
		}
		return current_state;
	}

	public override void draw()
	{
		vid_tex = null;
		spr_batch.GraphicsDevice.Clear(Color.Black);
		if (pl.State == MediaState.Playing)
		{
			vid_tex = pl.GetTexture();
		}
		if (vid_tex != null)
		{
			spr_batch.Begin();
			spr_batch.Draw(vid_tex, new Rectangle(0, 0, gfx_dev.Viewport.Width, gfx_dev.Viewport.Height), Color.White);
			spr_batch.End();
		}
	}
}
