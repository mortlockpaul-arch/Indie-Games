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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		m_SpriteBatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
		m_FXColourChanger = MainGame.ContentMan.Load<Effect>("Effects/ColourChanger");
		EffectParameter obj = m_FXColourChanger.Parameters["OriginalColour"];
		Color val = new Color(byte.MaxValue, (byte)254, (byte)254);
		obj.SetValue(((Color)(ref val)).ToVector4());
		EffectParameter obj2 = m_FXColourChanger.Parameters["ReplacementColour"];
		Color val2 = new Color((byte)200, (byte)200, byte.MaxValue, (byte)192);
		obj2.SetValue(((Color)(ref val2)).ToVector4());
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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		GameScreen gameScreen = m_CurrentScreen;
		Rectangle val = m_CurrentScreenRect;
		if (m_InTransition)
		{
			switch (m_TransitionStep)
			{
			case TransitionStep.FadingOut:
				gameScreen = m_TransitionFrom;
				num = 1f - m_TransitionProgress;
				val = m_TransitionFrom.GetScreenRect();
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
				Vector2 val2 = Vector2.Lerp(m_TransitionFromTopLeft, m_TransitionToTopLeft, m_TransitionProgress);
				Vector2 val3 = Vector2.Lerp(m_TransitionFromBottomRight, m_TransitionToBottomRight, m_TransitionProgress);
				val.X = (int)val2.X;
				val.Y = (int)val2.Y;
				val.Width = (int)(val3.X - val2.X);
				val.Height = (int)(val3.Y - val2.Y);
				num = 0f;
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
						((DrawableGameComponent)this).Visible = false;
					}
				}
				break;
			}
			case TransitionStep.FadingIn:
				num = m_TransitionProgress;
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
			m_SpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			m_FXColourChanger.Begin();
			m_FXColourChanger.CurrentTechnique.Passes[0].Begin();
			m_SpriteBatch.Draw(m_TexBackground, val, Color.White);
			m_FXColourChanger.CurrentTechnique.Passes[0].End();
			m_FXColourChanger.End();
			m_SpriteBatch.End();
		}
		num = MathHelper.Clamp(num, 0f, 1f);
		gameScreen?.Draw(num);
	}

	public void ShowScreen(ScreenType type)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
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
			m_TransitionFromTopLeft = new Vector2((float)screenRect.X, (float)screenRect.Y);
			m_TransitionToTopLeft = new Vector2((float)screenRect2.X, (float)screenRect2.Y);
			m_TransitionFromBottomRight = new Vector2((float)((Rectangle)(ref screenRect)).Right, (float)((Rectangle)(ref screenRect)).Bottom);
			m_TransitionToBottomRight = new Vector2((float)((Rectangle)(ref screenRect2)).Right, (float)((Rectangle)(ref screenRect2)).Bottom);
		}
		else
		{
			m_TransitionFrom = null;
			m_InTransition = true;
			m_TransitionStep = TransitionStep.ChangingSize;
			m_TransitionProgress = 0f;
			Rectangle screenRect3 = gameScreen.GetScreenRect();
			m_TransitionFromTopLeft = new Vector2(50f, 50f);
			m_TransitionToTopLeft = new Vector2((float)screenRect3.X, (float)screenRect3.Y);
			m_TransitionFromBottomRight = new Vector2(50f, 50f);
			m_TransitionToBottomRight = new Vector2((float)((Rectangle)(ref screenRect3)).Right, (float)((Rectangle)(ref screenRect3)).Bottom);
			gameScreen.OnShowScreen();
		}
		m_CurrentScreen = gameScreen;
		m_CurrentScreenType = type;
		m_CurrentScreenRect = m_CurrentScreen.GetScreenRect();
		((DrawableGameComponent)this).Visible = true;
	}

	public void HideScreen()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (m_CurrentScreen != null)
		{
			m_TransitionFrom = m_CurrentScreen;
			m_InTransition = true;
			m_TransitionStep = TransitionStep.FadingOut;
			m_TransitionProgress = 0f;
			Rectangle screenRect = m_TransitionFrom.GetScreenRect();
			m_TransitionFromTopLeft = new Vector2((float)screenRect.X, (float)screenRect.Y);
			Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
			float num = ((Viewport)(ref viewport)).Width - 50;
			Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
			m_TransitionToTopLeft = new Vector2(num, (float)(((Viewport)(ref viewport2)).Height - 50));
			m_TransitionFromBottomRight = new Vector2((float)((Rectangle)(ref screenRect)).Right, (float)((Rectangle)(ref screenRect)).Bottom);
			m_TransitionToBottomRight = m_TransitionToTopLeft;
			m_CurrentScreenType = ScreenType.None;
			m_CurrentScreen = null;
		}
	}

	public void HandleScreenResize()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
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
