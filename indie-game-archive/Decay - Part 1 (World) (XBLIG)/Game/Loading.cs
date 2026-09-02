using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game;

public class Loading
{
	protected Texture2D m_bkg;

	protected Texture2D m_loading;

	protected Texture2D m_icon;

	protected Game m_game;

	private GameTime m_start_time;

	private Thread m_thread;

	private EventWaitHandle m_wait_handle;

	protected float m_icon_rotation;

	public bool m_draw_background = true;

	public Loading(Game game)
	{
		m_game = game;
		m_bkg = m_game.m_CL.LoadTexture("Loading/loading_bkg");
		m_loading = m_game.m_CL.LoadTexture("Loading/loading_with_effect");
		m_icon = m_game.m_CL.LoadTexture("Loading/loadingtriangle");
		m_thread = new Thread(LoadingThread);
		m_wait_handle = new ManualResetEvent(initialState: false);
	}

	public virtual void Clear()
	{
		m_game = null;
		m_bkg = null;
		m_loading = null;
		m_icon = null;
	}

	private void LoadingThread()
	{
		long lastTime = Stopwatch.GetTimestamp();
		while (!m_wait_handle.WaitOne(33, exitContext: false))
		{
			GameTime gameTime = GetGameTime(ref lastTime);
			Update(gameTime.ElapsedGameTime);
			Draw(((Game)m_game).GraphicsDevice, m_game.m_SB);
		}
	}

	private GameTime GetGameTime(ref long lastTime)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		long timestamp = Stopwatch.GetTimestamp();
		long num = timestamp - lastTime;
		lastTime = timestamp;
		TimeSpan timeSpan = TimeSpan.FromTicks(num * 10000000 / Stopwatch.Frequency);
		return new GameTime(m_start_time.TotalRealTime + timeSpan, timeSpan, m_start_time.TotalGameTime + timeSpan, timeSpan);
	}

	public virtual void Start(GameTime start_time)
	{
		m_start_time = start_time;
		m_thread.Start();
		Draw(((Game)m_game).GraphicsDevice, m_game.m_SB);
	}

	public virtual void Stop()
	{
		if (m_thread != null)
		{
			m_wait_handle.Set();
			m_thread.Join();
		}
	}

	public virtual void Update(TimeSpan elapsed)
	{
		m_icon_rotation += (float)elapsed.TotalMilliseconds * 0.001f * 3f;
	}

	public virtual void Draw(GraphicsDevice device, SpriteBatch SB)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (device == null || device.IsDisposed || SB == null)
		{
			return;
		}
		try
		{
			device.Clear(Color.Black);
			SB.Begin((SpriteBlendMode)1);
			Vector2 zero = Vector2.Zero;
			if (m_draw_background)
			{
				SB.Draw(m_bkg, Game.VIEW_RECT, Color.White);
			}
			zero.X = ((Rectangle)(ref Game.TS_AREA)).Right - m_loading.Width;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_icon.Height - m_loading.Height;
			SB.Draw(m_loading, zero, Color.White);
			zero.X = ((Rectangle)(ref Game.TS_AREA)).Right - 95;
			zero.Y = ((Rectangle)(ref Game.TS_AREA)).Bottom - m_icon.Height + 50;
			SB.Draw(m_icon, zero, (Rectangle?)null, Color.White, m_icon_rotation, new Vector2(45f, 50f), 1f, (SpriteEffects)0, 0f);
			SB.End();
			device.Present();
		}
		catch
		{
			device = null;
		}
	}
}
