using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eyehook.Framework;

public abstract class EyehookGame : Game
{
	private string name;

	public readonly GraphicsDeviceManager graphics;

	public string Name => name;

	public EyehookGame(string name, bool fixedTimeStep, bool vSync)
	{
		this.name = name;
		base.Content.RootDirectory = "Content";
		graphics = new GraphicsDeviceManager(this);
		graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.SynchronizeWithVerticalRetrace = vSync;
		base.IsFixedTimeStep = fixedTimeStep;
	}

	protected override void Initialize()
	{
		MC.Initialize(this);
		base.Initialize();
	}

	protected override void LoadContent()
	{
		MC.LoadContent();
		base.LoadContent();
		DisplayStartScreen();
	}

	protected override void UnloadContent()
	{
		MC.UnloadContent();
		base.UnloadContent();
	}

	public abstract void ResetGame();

	public abstract void DisplayStartScreen();

	protected override void Update(GameTime gameTime)
	{
		try
		{
			MC.Update(gameTime);
		}
		catch (ResetException ex)
		{
			MC.Reset();
			ResetGame();
			MC.ScreenManager.addScreen(new GuideMessage("Error", ex.Message, new string[1] { "Ok" }, new GuideMessageDelegate[1] { reset }, reset));
		}
		base.Update(gameTime);
	}

	private void reset()
	{
		DisplayStartScreen();
	}

	protected override void Draw(GameTime gameTime)
	{
		MC.Draw(gameTime);
		base.Draw(gameTime);
	}
}
