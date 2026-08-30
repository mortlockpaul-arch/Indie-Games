using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Kobingo.Xna.Library.Common;
using Kobingo.Xna.Library.Game;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace Kobingo.Xna.Games.Painter;

public class PainterGame : Game
{
	public const string GAME_NAME = "Let's Draw A Picture Together!";

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private PainterPlayScreen m_PainterPlayScreen;

	private Version m_Version;

	private float m_Progress;

	public TickTimer StartTimer { get; set; }

	public bool StartVisible { get; set; }

	public static bool FirstRun { get; set; }

	public PainterGame()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		((Game)this)._002Ector();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		graphics.IsFullScreen = false;
		graphics.PreparingDeviceSettings += delegate(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = (RenderTargetUsage)1;
		};
		((Game)this).Content.RootDirectory = "Content";
		m_Version = Assembly.GetExecutingAssembly().GetName().Version;
		StartTimer = new TickTimer(TimeSpan.FromSeconds(0.5));
		StartTimer.Tick += delegate
		{
			StartVisible = !StartVisible;
		};
	}

	protected override void Initialize()
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		GameManager.Initialize((Game)(object)this, "Let's Draw A Picture Together!");
		m_PainterPlayScreen = new PainterPlayScreen(GameManager.ScreenManager);
		GameManager.TitleScreen.MainMenu = new PainterMainMenu(GameManager.ScreenManager);
		GameManager.TitleScreen.MainMenu.PlayScreen = m_PainterPlayScreen;
		MenuScreen.DrawingMenu += delegate(object sender, EventArgs e)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			MenuScreen menuScreen = sender as MenuScreen;
			SpriteBatch val = GameManager.ScreenManager.SpriteBatch;
			val.Begin();
			val.DrawAligned(Graphics.MenuBack, GameManager.ScreenManager.ScreenCenter, Align.Center, Color.White);
			val.DrawAlignedString(Fonts.HeaderFont, menuScreen.Title, GameManager.ScreenManager.ScreenCenter - new Vector2(0f, 230f), Align.Center, Color.Black);
			val.End();
		};
		GameManager.TitleScreen.MainMenu.Showing += delegate
		{
			NetworkSession.InviteAccepted += OnInviteAccepted;
		};
		GameManager.TitleScreen.MainMenu.Closing += delegate
		{
			NetworkSession.InviteAccepted -= OnInviteAccepted;
		};
		GameManager.TitleScreen.Drawing += delegate
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			GameManager.ScreenManager.SpriteBatch.Begin();
			GameManager.ScreenManager.SpriteBatch.DrawAligned(Graphics.Background, GameManager.ScreenManager.ScreenCenter, Align.Center, Color.White);
			GameManager.ScreenManager.SpriteBatch.DrawAligned(Graphics.Title, GameManager.ScreenManager.ScreenCenter - new Vector2(0f, 22f), Align.Center, Color.White);
			if (StartVisible)
			{
				GameManager.ScreenManager.SpriteBatch.DrawAligned(Graphics.ButtonA, GameManager.ScreenManager.ScreenCenter + new Vector2(0f, 110f), Align.Center, Color.White);
				GameManager.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.DefaultFont, "Start!", GameManager.ScreenManager.ScreenCenter + new Vector2(0f, 150f), Align.Center, Color.Black);
			}
			GameManager.ScreenManager.SpriteBatch.DrawAlignedString(Fonts.DefaultFont, $"Created By Jens Andersson 2009 Version {m_Version.Major}.{m_Version.Minor}.{m_Version.Build}", GameManager.ScreenManager.ScreenCenter + new Vector2(0f, 230f), Align.Center, Color.White);
			GameManager.ScreenManager.SpriteBatch.End();
		};
		GameManager.TitleScreen.LoadingScreen.LoadPlayerStorage = true;
		GameManager.TitleScreen.LoadingScreen.Loading += delegate(object sender, LoadingEventArgs e)
		{
			if (e.Container != null)
			{
				string path = Path.Combine(e.Container.Path, "firstrun.bin");
				if (!File.Exists(path))
				{
					FirstRun = true;
					using (new FileStream(path, FileMode.Create))
					{
					}
				}
			}
			Thread.Sleep(2000);
		};
		GameManager.TitleScreen.LoadingScreen.Drawing += delegate
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			GameManager.ScreenManager.SpriteBatch.Begin();
			GameManager.ScreenManager.SpriteBatch.DrawAligned(Graphics.Progress, GameManager.ScreenManager.ScreenCenter - new Vector2(0f, 45f), m_Progress, 1f, Align.Center, Color.Black);
			GameManager.ScreenManager.SpriteBatch.End();
		};
		MenuScreen.DisabledEntryColor = Color.Purple;
		MenuScreen.SelectedDisabledEntryColor = Color.Violet;
		((Game)this).Initialize();
	}

	private void OnInviteAccepted(object sender, InviteAcceptedEventArgs e)
	{
		GameManager.ActiveGamer = e.Gamer;
		m_PainterPlayScreen.Show(PainterSessionType.Invited, null);
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		Fonts.Load(((Game)this).GraphicsDevice, ((Game)this).Content);
		Graphics.Load(((Game)this).GraphicsDevice, ((Game)this).Content);
		GameManager.Font = Fonts.DefaultFont;
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				current.Presence.PresenceMode = (GamerPresenceMode)38;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		StartTimer.Update(gameTime);
		m_Progress += 0.03f;
		((Game)this).Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((Game)this).GraphicsDevice.Clear(Color.SteelBlue);
		GameManager.ScreenManager.SpriteBatch.Begin();
		GameManager.ScreenManager.SpriteBatch.DrawAligned(Graphics.Background, GameManager.ScreenManager.ScreenCenter, Align.Center, Color.White);
		GameManager.ScreenManager.SpriteBatch.End();
		((Game)this).Draw(gameTime);
	}
}
