using Microsoft.Xna.Framework;

namespace Deep_waters;

public class GameStateManagementGame : Game
{
	private GraphicsDeviceManager graphics;

	private ScreenManager screenManager;

	private static readonly string[] preloadAssets = new string[1] { "Sprites/gradient" };

	public GameStateManagementGame()
	{
		base.Content.RootDirectory = "Content";
		graphics = new GraphicsDeviceManager(this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		screenManager = new ScreenManager(this, graphics);
		base.Components.Add(screenManager);
		screenManager.AddScreen(new BackgroundScreen("Logo"), null);
		screenManager.AddScreen(new Splashscreen(), null);
	}

	protected override void LoadContent()
	{
		string[] array = preloadAssets;
		foreach (string assetName in array)
		{
			base.Content.Load<object>(assetName);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		graphics.GraphicsDevice.Clear(Color.Black);
		base.Draw(gameTime);
	}
}
