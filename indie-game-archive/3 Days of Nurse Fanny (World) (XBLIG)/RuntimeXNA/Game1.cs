using BinaryRead;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA;

public class Game1 : Game
{
	public GraphicsDeviceManager graphics;

	public SpriteBatchEffect spriteBatch;

	public bool bPreviousActive;

	public bool bInitialActivation;

	private CRunApp application;

	public GamerServicesComponent gamerServices;

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		bPreviousActive = false;
		bInitialActivation = true;
	}

	protected override void Initialize()
	{
		GamerServicesDispatcher.WindowHandle = base.Window.Handle;
		GamerServicesDispatcher.Initialize(base.Services);
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatchEffect(base.Content, base.GraphicsDevice);
		base.IsMouseVisible = true;
		Data data = base.Content.Load<Data>("Application");
		CFile f = new CFile(data.data);
		application = new CRunApp(this, f);
		if (application.load())
		{
			application.startApplication();
		}
		LoadUpfront.LoadContentUpfront(base.Content, base.GraphicsDevice);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (!LoadUpfront.Active)
		{
			return;
		}
		if (bPreviousActive != base.IsActive)
		{
			bPreviousActive = base.IsActive;
			if (!bInitialActivation)
			{
				if (application.run != null)
				{
					if (base.IsActive)
					{
						application.run.resume();
					}
					else
					{
						application.run.pause();
					}
				}
			}
			else
			{
				bInitialActivation = false;
			}
		}
		double totalMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;
		if (!application.playApplication(bOnlyRestartApp: false, totalMilliseconds))
		{
			Exit();
		}
		if (GamerServicesDispatcher.IsInitialized)
		{
			GamerServicesDispatcher.Update();
		}
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		if (LoadUpfront.Active)
		{
			_ = gameTime.TotalGameTime.TotalMilliseconds;
			application.draw();
			base.Draw(gameTime);
		}
		else
		{
			LoadUpfront.DrawLoadingScreen();
		}
	}
}
