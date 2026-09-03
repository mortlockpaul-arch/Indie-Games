using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Loading
{
	protected Game m_game;

	private GameTime m_start_time;

	private Thread m_thread;

	private EventWaitHandle m_wait_handle;

	public bool m_loading_area = true;

	public Loading(Game game)
	{
		m_game = game;
		m_thread = new Thread(LoadingThread);
		m_wait_handle = new ManualResetEvent(initialState: false);
	}

	public virtual void Clear()
	{
		m_game = null;
	}

	private void LoadingThread()
	{
		long lastTime = Stopwatch.GetTimestamp();
		while (!m_wait_handle.WaitOne(33) && m_game != null)
		{
			GameTime gameTime = GetGameTime(ref lastTime);
			Update(gameTime.ElapsedGameTime);
			Draw(m_game.GraphicsDevice, m_game.m_SB);
		}
	}

	private GameTime GetGameTime(ref long lastTime)
	{
		long timestamp = Stopwatch.GetTimestamp();
		long num = timestamp - lastTime;
		lastTime = timestamp;
		TimeSpan elapsedGameTime = TimeSpan.FromTicks(num * 10000000 / Stopwatch.Frequency);
		return new GameTime(m_start_time.TotalGameTime, elapsedGameTime, isRunningSlowly: true);
	}

	public virtual void Start(GameTime start_time)
	{
		Draw(m_game.GraphicsDevice, m_game.m_SB);
		m_start_time = start_time;
		m_thread.Start();
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
	}

	public virtual void Draw(GraphicsDevice device, SpriteBatch SB)
	{
	}
}
