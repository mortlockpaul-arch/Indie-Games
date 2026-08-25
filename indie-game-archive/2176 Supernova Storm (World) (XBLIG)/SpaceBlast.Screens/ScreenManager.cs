using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast.Screens;

internal class ScreenManager : DrawableGameComponent
{
	private enum TransitionStep
	{
		FadingOut,
		ChangingSize,
		FadingIn
	}

	private const float constTransitionSpeed = 0.05f;

	private ScreenType m_CurrentScreenType;

	private GameScreen m_CurrentScreen;

	private GameScreen m_TransitionFrom;

	private bool m_InTransition;

	private TransitionStep m_TransitionStep;

	private float m_TransitionProgress;

	private Vector2 m_TransitionFromTopLeft;

	private Vector2 m_TransitionToTopLeft;

	private Vector2 m_TransitionFromBottomRight;

	private Vector2 m_TransitionToBottomRight;

	private SpriteBatch m_SpriteBatch;

	private Texture2D m_TexBackground;

	private Effect m_FXColourChanger;

	private Rectangle m_CurrentScreenRect;

	private Dictionary<ScreenType, GameScreen> m_ScreensMap = new Dictionary<ScreenType, GameScreen>();

	public ScreenType CurrentScreenType => m_CurrentScreenType;

	public ScreenManager(Game game)
		: base(game)
	{
		m_CurrentScreenType = ScreenType.None;
		m_ScreensMap.Add(ScreenType.PressStart, new PressStartScreen(this));
		m_ScreensMap.Add(ScreenType.Player2ControllerScreen, new Player2ControllerScreen(this));
		m_ScreensMap.Add(ScreenType.MainMenu, new MainMenuScreen(this));
		m_ScreensMap.Add(ScreenType.TrialMainMenu, new TrialMainMenuScreen(this));
		m_ScreensMap.Add(ScreenType.PauseMenu, new PauseMenuScreen(this));
		m_ScreensMap.Add(ScreenType.GameResults, new GameResultsScreen(this));
		m_ScreensMap.Add(ScreenType.Purchase, new PurchaseScreen(this));
		m_ScreensMap.Add(ScreenType.ThankYou, new ThankYouScreen(this));
		m_ScreensMap.Add(ScreenType.Promotion, new PromotionScreen(this));
		m_ScreensMap.Add(ScreenType.Instructions, new ControllerInstructionsScreen(this));
		m_ScreensMap.Add(ScreenType.LayoutInstructions, new LayoutInstructionsScreen(this));
		m_ScreensMap.Add(ScreenType.Credits, new CreditsScreen(this));
		m_ScreensMap.Add(ScreenType.MultiplayerGameSearch, new MultiplayerGameSearchScreen(this));
		m_ScreensMap.Add(ScreenType.SinglePlayerMenu, new SinglePlayerMenuScreen(this));
		m_ScreensMap.Add(ScreenType.SinglePlayerDeathmatchOptions, new SinglePlayerDeathMatchOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.SinglePlayerTeamDeathmatchOptions, new SinglePlayerTeamDeathMatchOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.SinglePlayerCustomOptions, new SinglePlayerCustomOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.TrialRestriction, new TrialRestrictionScreen(this));
		m_ScreensMap.Add(ScreenType.SplitScreenMenu, new SplitScreenMenuScreen(this));
		m_ScreensMap.Add(ScreenType.SplitScreenCoOpOptions, new SplitScreenCoOpOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.SplitScreenAllVsAllOptions, new SplitScreenAllVsAllOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.SplitScreenCustomOptions, new SplitScreenCustomOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.SystemLinkMenuScreen, new SystemLinkMenuScreen(this));
		m_ScreensMap.Add(ScreenType.SystemLinkCustomGameScreen, new SystemLinkCustomOptionsScreen(this));
		m_ScreensMap.Add(ScreenType.MultiplayerMenuScreen, new MultiplayerMenuScreen(this));
		m_ScreensMap.Add(ScreenType.MultiplayerCreateCustomGameScreen, new MultiplayerCreateCustomGameScreen(this));
		m_ScreensMap.Add(ScreenType.MultiplayerAvailableGamesScreen, new MultiplayerAvailableGamesScreen(this));
		m_ScreensMap.Add(ScreenType.ControllerDisconnected, new ControllerDisconnectedScreen(this));
		m_ScreensMap.Add(ScreenType.PrivateGameScreen, new PrivateGameScreen(this));
	}

	protected override void LoadContent()
	{
		m_SpriteBatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		m_FXColourChanger = MainGame.ContentMan.Load<Effect>("Effects/ColourChanger");
		m_FXColourChanger.Parameters["OriginalColour"].SetValue(new Color(byte.MaxValue, 254, 254).ToVector4());
		m_FXColourChanger.Parameters["ReplacementColour"].SetValue(new Color(200, 200, byte.MaxValue, 192).ToVector4());
		m_TexBackground = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_Background");
		foreach (KeyValuePair<ScreenType, GameScreen> item in m_ScreensMap)
		{
			item.Value.LoadContent();
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (!m_InTransition && m_CurrentScreen != null)
		{
			m_CurrentScreen.Update();
		}
	}

	public override void Draw(GameTime gameTime)
	{
		float value = 1f;
		GameScreen gameScreen = m_CurrentScreen;
		Rectangle destinationRectangle = m_CurrentScreenRect;
		if (m_InTransition)
		{
			switch (m_TransitionStep)
			{
			case TransitionStep.FadingOut:
				gameScreen = m_TransitionFrom;
				value = 1f - m_TransitionProgress;
				destinationRectangle = m_TransitionFrom.GetScreenRect();
				m_TransitionProgress += 0.05f;
				if (m_TransitionProgress >= 1f)
				{
					m_TransitionStep = TransitionStep.ChangingSize;
					m_TransitionProgress = 0f;
					if (m_TransitionFromTopLeft == m_TransitionToTopLeft && m_TransitionFromBottomRight == m_TransitionToBottomRight)
					{
						m_TransitionProgress = 1f;
					}
				}
				break;
			case TransitionStep.ChangingSize:
			{
				if (m_TransitionFrom != null)
				{
					m_TransitionFrom.OnHideScreen();
					m_TransitionFrom = null;
					if (m_CurrentScreen != null)
					{
						m_CurrentScreen.OnShowScreen();
					}
				}
				Vector2 vector = Vector2.Lerp(m_TransitionFromTopLeft, m_TransitionToTopLeft, m_TransitionProgress);
				Vector2 vector2 = Vector2.Lerp(m_TransitionFromBottomRight, m_TransitionToBottomRight, m_TransitionProgress);
				destinationRectangle.X = (int)vector.X;
				destinationRectangle.Y = (int)vector.Y;
				destinationRectangle.Width = (int)(vector2.X - vector.X);
				destinationRectangle.Height = (int)(vector2.Y - vector.Y);
				value = 0f;
				m_TransitionProgress += 0.05f;
				if (m_TransitionProgress >= 1f)
				{
					if (m_CurrentScreen != null)
					{
						m_TransitionStep = TransitionStep.FadingIn;
						m_TransitionProgress = 0f;
					}
					else
					{
						m_InTransition = false;
						base.Visible = false;
					}
				}
				break;
			}
			case TransitionStep.FadingIn:
				value = m_TransitionProgress;
				m_TransitionProgress += 0.05f;
				if (m_TransitionProgress >= 1f)
				{
					m_InTransition = false;
				}
				break;
			}
		}
		if (gameScreen != null && gameScreen.HasBackground)
		{
			m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
			m_FXColourChanger.Begin();
			m_FXColourChanger.CurrentTechnique.Passes[0].Begin();
			m_SpriteBatch.Draw(m_TexBackground, destinationRectangle, Color.White);
			m_FXColourChanger.CurrentTechnique.Passes[0].End();
			m_FXColourChanger.End();
			m_SpriteBatch.End();
		}
		value = MathHelper.Clamp(value, 0f, 1f);
		gameScreen?.Draw(value);
	}

	public void ShowScreen(ScreenType type)
	{
		if (type == ScreenType.MainMenu && Guide.IsTrialMode)
		{
			type = ScreenType.TrialMainMenu;
		}
		GameScreen gameScreen = m_ScreensMap[type];
		if (m_CurrentScreen != null)
		{
			m_TransitionFrom = m_CurrentScreen;
			m_InTransition = true;
			m_TransitionStep = TransitionStep.FadingOut;
			m_TransitionProgress = 0f;
			Rectangle screenRect = m_TransitionFrom.GetScreenRect();
			Rectangle screenRect2 = gameScreen.GetScreenRect();
			m_TransitionFromTopLeft = new Vector2(screenRect.X, screenRect.Y);
			m_TransitionToTopLeft = new Vector2(screenRect2.X, screenRect2.Y);
			m_TransitionFromBottomRight = new Vector2(screenRect.Right, screenRect.Bottom);
			m_TransitionToBottomRight = new Vector2(screenRect2.Right, screenRect2.Bottom);
		}
		else
		{
			m_TransitionFrom = null;
			m_InTransition = true;
			m_TransitionStep = TransitionStep.ChangingSize;
			m_TransitionProgress = 0f;
			Rectangle screenRect3 = gameScreen.GetScreenRect();
			m_TransitionFromTopLeft = new Vector2(50f, 50f);
			m_TransitionToTopLeft = new Vector2(screenRect3.X, screenRect3.Y);
			m_TransitionFromBottomRight = new Vector2(50f, 50f);
			m_TransitionToBottomRight = new Vector2(screenRect3.Right, screenRect3.Bottom);
			gameScreen.OnShowScreen();
		}
		m_CurrentScreen = gameScreen;
		m_CurrentScreenType = type;
		m_CurrentScreenRect = m_CurrentScreen.GetScreenRect();
		base.Visible = true;
	}

	public void HideScreen()
	{
		if (m_CurrentScreen != null)
		{
			m_TransitionFrom = m_CurrentScreen;
			m_InTransition = true;
			m_TransitionStep = TransitionStep.FadingOut;
			m_TransitionProgress = 0f;
			Rectangle screenRect = m_TransitionFrom.GetScreenRect();
			m_TransitionFromTopLeft = new Vector2(screenRect.X, screenRect.Y);
			m_TransitionToTopLeft = new Vector2(MainGame.Instance.GraphicsDevice.Viewport.Width - 50, MainGame.Instance.GraphicsDevice.Viewport.Height - 50);
			m_TransitionFromBottomRight = new Vector2(screenRect.Right, screenRect.Bottom);
			m_TransitionToBottomRight = m_TransitionToTopLeft;
			m_CurrentScreenType = ScreenType.None;
			m_CurrentScreen = null;
		}
	}

	public void HandleScreenResize()
	{
		if (m_CurrentScreen != null)
		{
			m_CurrentScreenRect = m_CurrentScreen.GetScreenRect();
		}
		foreach (KeyValuePair<ScreenType, GameScreen> item in m_ScreensMap)
		{
			item.Value.OnScreenResize();
		}
	}

	public void PreventClickThrough()
	{
		if (m_CurrentScreen is MenuScreen)
		{
			((MenuScreen)m_CurrentScreen).PreventClickThrough();
		}
	}

	public GameScreen GetScreen(ScreenType type)
	{
		return m_ScreensMap[type];
	}
}
