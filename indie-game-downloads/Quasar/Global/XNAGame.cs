using System;
using Microsoft.Xna.Framework;
using XnaToFna;

namespace Quasar.Global;

public abstract class XNAGame : XnaToFnaGame
{
	protected GraphicsDeviceManager graphics;

	protected Engine engine;

	public Engine Engine => engine;

	protected virtual void Init(GraphicsDeviceManager graphics)
	{
		this.graphics = graphics;
		base.IsFixedTimeStep = false;
		base.InactiveSleepTime = new TimeSpan(0, 0, 0, 0, 10);
		base.Exiting += Exit;
		engine = new Engine();
		ContentTracker contentTracker = new ContentTracker(base.Services);
		((IEngine)engine).SetContent(contentTracker);
		base.Content = contentTracker;
		contentTracker.RootDirectory = "Content";
		base.Activated += XNAGame_Activated;
		base.Deactivated += XNAGame_Deactivated;
	}

	private void XNAGame_Deactivated(object sender, EventArgs e)
	{
		if (engine != null)
		{
			((IEngine)engine).SetActive(false);
		}
	}

	private void XNAGame_Activated(object sender, EventArgs e)
	{
		if (engine != null)
		{
			((IEngine)engine).SetActive(true);
		}
	}

	protected override void Initialize()
	{
		((IEngine)engine).Initialize(base.GraphicsDevice);
		base.Initialize();
	}

	protected override void LoadContent()
	{
		((IEngine)engine).InitGame();
		base.LoadContent();
	}

	public void Run(Game game)
	{
		game.SetXNAGame(this);
		((IEngine)engine).SetGame(game);
		Run();
	}

	private void Exit(object sender, EventArgs e)
	{
		Dispose();
	}

	protected override void Update(GameTime gameTime)
	{
		XnaToFnaHelper.PreUpdate(gameTime);
		((IEngine)engine).Update();
		if (Engine.Game.Finished)
		{
			Exit();
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		((IEngine)engine).Draw();
		base.Draw(gameTime);
	}

	protected new void Dispose()
	{
		if (engine != null)
		{
			engine.Dispose();
		}
		engine = null;
		base.Dispose();
	}
}
