using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary;

public class GameObject : IDisposable
{
	public delegate void UpdateEventHandler(object sender, GameTime gameTime);

	public delegate void DrawEventHandler(object sender, GameTime gameTime, SpriteBatch batch);

	public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

	public ContentManager Content => Game.Content;

	public Game Game { get; private set; }

	public bool Initialized { get; private set; }

	public bool Disposed { get; private set; }

	public bool Visible { get; set; }

	public bool Enabled { get; set; }

	public object Tag { get; set; }

	public event UpdateEventHandler update;

	public event DrawEventHandler draw;

	public GameObject(Game game)
	{
		Game = game;
	}

	public virtual void Initialize()
	{
		Initialized = true;
	}

	public virtual void Dispose()
	{
		Disposed = true;
	}

	public void Update(GameTime gameTime)
	{
		if (update != null && !Disposed && Enabled)
		{
			update(this, gameTime);
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch batch)
	{
		if (draw != null && !Disposed && Visible)
		{
			draw(this, gameTime, batch);
		}
	}
}
