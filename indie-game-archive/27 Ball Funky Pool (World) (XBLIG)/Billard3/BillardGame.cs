using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Billard3;

public class BillardGame : MaximinusGame
{
	public const ID GameID = ID.FunkyPool;

	private MultiMonitorGraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private bool started;

	public static readonly bool FPS = false;

	public static bool DebugRecord = false;

	public static bool DisableMusic = false;

	public static Texture2D splash;

	public static Threading.ManagedThread threadLoadPre;

	public BillardGame()
		: base(ID.FunkyPool)
	{
		base.Components.Add(new GamerServicesComponent(this));
		base.IsFixedTimeStep = true;
		graphics = new MultiMonitorGraphicsDeviceManager(this, 1);
		if (CameraBillard.BoxShot)
		{
			graphics.PreferredBackBufferWidth = 1000;
			graphics.PreferredBackBufferHeight = 1199;
			graphics.PreferMultiSampling = true;
		}
		else
		{
			Utils.InitializeGraphics.InitializeDevice(graphics, 1920, "8 Ball Champion", 1.7777778f, antiAliasing: false, fullscreen: false);
		}
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		Statics.draw2D = MaximinusGame.Draw2D;
		splash = base.Content.Load<Texture2D>("tex/maximinus" + MaximinusGame.TexSizeName);
		threadLoadPre = null;
		threadLoadPre = new Threading.ManagedThread();
		threadLoadPre.AddTask(new Threading.ThreadTask(LoadContentCB));
	}

	private void LoadContentCB()
	{
		Statics.LoadContent(this, base.Content);
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		if (Statics.ContentLoadedTime == -1.0)
		{
			Statics.ContentLoadedTime = gameTime.TotalGameTime.TotalSeconds;
			threadLoadPre.KillImmediately();
		}
		if (Statics.ContentLoadedTime != -2.0)
		{
			if (!started)
			{
				Statics.callbacks.FirstFrame(gameTime);
				started = true;
			}
			base.Update(gameTime);
			Updates.Update(gameTime);
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		Draws.Draw(gameTime);
	}
}
