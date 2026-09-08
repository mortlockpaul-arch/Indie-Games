using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class ScreenManager : DrawableGameComponent
{
	public enum TextOrigin
	{
		top_Left,
		top_center,
		top_right,
		center_center,
		bottom_center,
		bottom_left,
		bottom_right
	}

	public enum FadeFx
	{
		NONE,
		FADE_TO_BLACK,
		FADE_TO_WHITE,
		FADE_FROM_BLACK,
		FADE_FROM_WHITE
	}

	public bool m_bGamePadDisconnected;

	private List<GameScreen> screens = new List<GameScreen>();

	private List<GameScreen> screensToUpdate = new List<GameScreen>();

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	public SpriteFont FontBig;

	public SpriteFont Smallfont;

	public SpriteFont BubbleFont;

	public SpriteFont BubbleFontBig;

	public SpriteFont BubbleFontVeryBig;

	public SpriteFont GoBoom;

	public SpriteFont GoBoomMiddle;

	public SpriteFont GoBoomSmall;

	public SpriteFont GoBoomBig;

	public SpriteFont GoBoomVeryBig;

	private Effect m_GammaShader;

	public Texture2D blankTexture;

	private float FadeSpeed;

	private float FadeTime;

	private FadeFx FadeType;

	private bool isInitialized;

	private bool traceEnabled;

	public static Matrix m_TransformMatrix;

	public Rectangle m_ScreenSource;

	public Vector2 m_ViewportPosition;

	public RenderTarget2D ViewPort;

	public SpriteBatch SpriteBatch => spriteBatch;

	public SpriteFont Font => font;

	public bool TraceEnabled
	{
		get
		{
			return traceEnabled;
		}
		set
		{
			traceEnabled = value;
		}
	}

	public ScreenManager(Game game)
		: base(game)
	{
		m_TransformMatrix = Matrix.CreateScale(1f);
		m_ScreenSource = new Rectangle(0, 0, 1280, 720);
	}

	public override void Initialize()
	{
		base.Initialize();
		isInitialized = true;
	}

	public Texture2D CreateRectangle(int width, int height, Color colori)
	{
		Texture2D texture2D = new Texture2D(base.GraphicsDevice, width, height, mipMap: false, SurfaceFormat.Color);
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = colori;
		}
		texture2D.SetData(array);
		return texture2D;
	}

	protected override void LoadContent()
	{
		ContentManager content = base.Game.Content;
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		font = content.Load<SpriteFont>("Font/Genericfont");
		FontBig = content.Load<SpriteFont>("Font/GenericfontBig");
		BubbleFont = content.Load<SpriteFont>("Font/Bubblel");
		BubbleFontBig = content.Load<SpriteFont>("Font/Bubble2");
		BubbleFontVeryBig = content.Load<SpriteFont>("Font/Bubble3");
		GoBoomBig = content.Load<SpriteFont>("Font/GoBoomBig");
		GoBoomSmall = content.Load<SpriteFont>("Font/GoBoomSmall");
		GoBoomMiddle = content.Load<SpriteFont>("Font/GoBoomMiddle");
		GoBoomVeryBig = content.Load<SpriteFont>("Font/GoBoomVeryBig");
		GoBoom = content.Load<SpriteFont>("Font/GoBoom");
		blankTexture = content.Load<Texture2D>("blank");
		m_GammaShader = content.Load<Effect>("Fx/PostProcess/GammaCorrect");
		m_GammaShader.Parameters["Correction"].SetValue(0.7f);
		foreach (GameScreen screen in screens)
		{
			screen.LoadContent();
		}
	}

	protected override void UnloadContent()
	{
		foreach (GameScreen screen in screens)
		{
			screen.UnloadContent();
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (!base.Game.IsActive)
		{
			return;
		}
		screensToUpdate.Clear();
		foreach (GameScreen screen in screens)
		{
			screensToUpdate.Add(screen);
		}
		bool flag = !base.Game.IsActive;
		bool coveredByOtherScreen = false;
		while (screensToUpdate.Count > 0)
		{
			GameScreen gameScreen = screensToUpdate[screensToUpdate.Count - 1];
			screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
			gameScreen.Update(gameTime, flag, coveredByOtherScreen);
			if (gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active)
			{
				if (!flag)
				{
					gameScreen.HandleInput();
					flag = true;
				}
				if (!gameScreen.IsPopup)
				{
					coveredByOtherScreen = true;
				}
			}
		}
		if (traceEnabled)
		{
			TraceScreens();
		}
		ManageInput();
	}

	private void ManageInput()
	{
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			int num = InputManager.GamerIndex[i];
			if (num == -1)
			{
				continue;
			}
			if (!m_bGamePadDisconnected && !GamePad.GetState((PlayerIndex)num).IsConnected)
			{
				foreach (GameScreen screen in screens)
				{
					if ((object)screen.GetType() == typeof(GameState))
					{
						GameState gameState = (GameState)screen;
						gameState.m_bIsPaused = true;
					}
				}
			}
			else if (m_bGamePadDisconnected && GamePad.GetState((PlayerIndex)num).IsConnected)
			{
				foreach (GameScreen screen2 in screens)
				{
					if ((object)screen2.GetType() == typeof(GameState))
					{
						GameState gameState2 = (GameState)screen2;
						gameState2.m_bIsPaused = false;
					}
				}
			}
			InputManager.Update(num);
		}
	}

	private void TraceScreens()
	{
		List<string> list = new List<string>();
		foreach (GameScreen screen in screens)
		{
			list.Add(screen.GetType().Name);
		}
	}

	public void DrawText(SpriteFont font, ref Vector2 position, string text, TextOrigin textorigin, Color textcolor)
	{
		Vector2 vector = font.MeasureString(text);
		Vector2 zero = Vector2.Zero;
		switch (textorigin)
		{
		case TextOrigin.top_center:
			zero.X = vector.X / 2f;
			break;
		case TextOrigin.top_right:
			zero.X = vector.X;
			break;
		case TextOrigin.center_center:
			zero.X = vector.X / 2f;
			zero.Y = vector.Y / 2f;
			break;
		case TextOrigin.bottom_center:
			zero.X = vector.X / 2f;
			zero.Y = vector.Y;
			break;
		case TextOrigin.bottom_left:
			zero.Y = vector.Y;
			break;
		case TextOrigin.bottom_right:
			zero.Y = vector.Y;
			zero.X = vector.X;
			break;
		}
		SpriteBatch.DrawString(font, text, position, textcolor, 0f, zero, 1f, SpriteEffects.None, 1f);
	}

	public void DrawText(SpriteFont font, ref Vector2 position, string text, TextOrigin textorigin, Color textcolor, float rot)
	{
		Vector2 vector = font.MeasureString(text);
		Vector2 zero = Vector2.Zero;
		if (textorigin != TextOrigin.top_Left && textorigin == TextOrigin.center_center)
		{
			zero.X = vector.X / 2f;
			zero.Y = vector.Y / 2f;
		}
		SpriteBatch.DrawString(font, text, position, textcolor, rot, zero, 1f, SpriteEffects.None, 1f);
	}

	public void DrawTextOutline(SpriteFont font, string text, Color backColor, Color frontColor, float thickness, Vector2 position, TextOrigin textorigin)
	{
		new Vector2(font.MeasureString(text).X / 2f, font.MeasureString(text).Y / 2f);
		Vector2 position2 = position + new Vector2(thickness, thickness);
		DrawText(font, ref position2, text, textorigin, backColor);
		Vector2 position3 = position + new Vector2(0f - thickness, 0f - thickness);
		DrawText(font, ref position3, text, textorigin, backColor);
		Vector2 position4 = position + new Vector2(0f - thickness, thickness);
		DrawText(font, ref position4, text, textorigin, backColor);
		Vector2 position5 = position + new Vector2(thickness, 0f - thickness);
		DrawText(font, ref position5, text, textorigin, backColor);
		DrawText(font, ref position, text, textorigin, frontColor);
	}

	public void DrawTextOutline(SpriteFont font, string text, Color backColor, Color frontColor, float thickness, float scale, float rotation, Vector2 position)
	{
		Vector2 origin = new Vector2(font.MeasureString(text).X / 2f, font.MeasureString(text).Y / 2f);
		SpriteBatch.DrawString(font, text, position + new Vector2(thickness * scale, thickness * scale), backColor, rotation, origin, scale, SpriteEffects.None, 1f);
		SpriteBatch.DrawString(font, text, position + new Vector2((0f - thickness) * scale, (0f - thickness) * scale), backColor, rotation, origin, scale, SpriteEffects.None, 1f);
		SpriteBatch.DrawString(font, text, position + new Vector2((0f - thickness) * scale, thickness * scale), backColor, rotation, origin, scale, SpriteEffects.None, 1f);
		SpriteBatch.DrawString(font, text, position + new Vector2(thickness * scale, (0f - thickness) * scale), backColor, rotation, origin, scale, SpriteEffects.None, 1f);
		SpriteBatch.DrawString(font, text, position, frontColor, rotation, origin, scale, SpriteEffects.None, 1f);
	}

	public void StartFx(FadeFx fx, float time)
	{
		FadeSpeed = 1f / time;
		FadeType = fx;
		FadeTime = 0f;
	}

	public override void Draw(GameTime gameTime)
	{
		if (ViewPort != null && base.GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal)
		{
			base.GraphicsDevice.SetRenderTarget(ViewPort);
			base.GraphicsDevice.Clear(Color.Black);
			foreach (GameScreen screen in screens)
			{
				if (screen.ScreenState != ScreenState.Hidden)
				{
					screen.Draw(gameTime);
				}
			}
			base.GraphicsDevice.SetRenderTarget(null);
			base.GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, m_GammaShader);
			SpriteBatch.Draw(ViewPort, m_ViewportPosition, m_ScreenSource, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			SpriteBatch.End();
			if (FadeType == FadeFx.NONE)
			{
				return;
			}
			FadeTime += (float)gameTime.ElapsedGameTime.Milliseconds / 1000f * FadeSpeed;
			if (FadeTime >= 1f)
			{
				FadeTime = 1f;
				if (FadeType == FadeFx.FADE_FROM_BLACK || FadeType == FadeFx.FADE_FROM_WHITE)
				{
					FadeType = FadeFx.NONE;
				}
			}
			float num = ((FadeType != FadeFx.FADE_TO_BLACK && FadeType != FadeFx.FADE_TO_WHITE) ? MathHelper.Lerp(255f, 0f, FadeTime) : MathHelper.Lerp(0f, 255f, FadeTime));
			switch (FadeType)
			{
			case FadeFx.FADE_TO_BLACK:
			case FadeFx.FADE_TO_WHITE:
				FadeBackBufferToWhite((int)num);
				break;
			case FadeFx.FADE_FROM_BLACK:
			case FadeFx.FADE_FROM_WHITE:
				FadeBackBufferToBlack((int)num);
				break;
			}
		}
		else
		{
			PresentationParameters presentationParameters = base.GraphicsDevice.PresentationParameters;
			ViewPort = new RenderTarget2D(base.GraphicsDevice, 1280, 720, mipMap: false, SurfaceFormat.Color, presentationParameters.DepthStencilFormat, presentationParameters.MultiSampleCount, presentationParameters.RenderTargetUsage);
			m_ViewportPosition = new Vector2(0f, 0f);
			base.GraphicsDevice.Clear(Color.Black);
		}
	}

	public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
	{
		screen.ControllingPlayer = controllingPlayer;
		screen.ScreenManager = this;
		screen.IsExiting = false;
		if (isInitialized)
		{
			screen.LoadContent();
		}
		screens.Add(screen);
	}

	public void AddScreenWithoutLoad(GameScreen screen, PlayerIndex? controllingPlayer)
	{
		screen.IsExiting = false;
		screen.ControllingPlayer = controllingPlayer;
		screens.Add(screen);
	}

	public void RemoveScreen(GameScreen screen)
	{
		if (isInitialized)
		{
			screen.UnloadContent();
		}
		screens.Remove(screen);
		screensToUpdate.Remove(screen);
	}

	public GameScreen[] GetScreens()
	{
		return screens.ToArray();
	}

	public void FadeBackBufferToBlack(int alpha)
	{
		Viewport viewport = base.GraphicsDevice.Viewport;
		spriteBatch.Begin();
		spriteBatch.Draw(blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, (byte)alpha));
		spriteBatch.End();
	}

	public void FadeBackBufferToWhite(int alpha)
	{
		Viewport viewport = base.GraphicsDevice.Viewport;
		spriteBatch.Begin();
		spriteBatch.Draw(blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(255, 255, 255, (byte)alpha));
		spriteBatch.End();
	}
}
