using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury.Renderers;

namespace JamSouls;

internal class LogoScreen : GameState
{
	public struct entry(TextID id, Vector2 position, Color col)
	{
		public Color color = col;

		public Vector2 pos = position;

		public TextID textid = id;

		public void SetColor(Color col)
		{
			color = col;
		}
	}

	private const float MOVE_TIMER = 150f;

	private const float FADE_LATENCY = 0.8f;

	private const byte FADE_SPEED = 2;

	private Texture2D m_LogoPic;

	private Texture2D m_LogoSkull;

	private Texture2D m_Selector;

	private AnimatedSprite m_FireAnim;

	private float m_wait;

	private bool m_bFadeIn = true;

	private Color m_TextColor;

	private float m_fadeTimer;

	private AudioClip m_StartSound;

	private AudioClip m_SelectSound;

	private int m_CurrentEntry;

	private float m_ValidTime;

	private bool m_DataLoad;

	private bool m_StartMenuChoice;

	private int StarterIndex = -1;

	private float m_PadTimer;

	private bool m_bStartPushed;

	private PlayerIndex m_LastPlayerIndex;

	private Vector2 m_LogoPos;

	private Vector2 m_LogoSkullPos;

	private Vector2 m_PressStartPos;

	private Vector2 m_SelectorPos;

	private List<entry> MenuEntry = new List<entry>();

	public LogoScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.2);
		SignedInGamer.SignedIn += base.SignedInGamer_SignedIn;
		SignedInGamer.SignedOut += base.SignedInGamer_SignedOut;
		m_Renderer = new MercurySpriteBatchRenderer();
		m_TextColor = Color.White;
		m_TextColor.A = 0;
		Random random = new Random();
		GameContext.CurrentMusic = random.Next(GameContext.BACKGROUND_MUSIC.Length);
		MenuEntry.Add(new entry(TextID.ARENA_MODE, Vector2.Zero, Color.White));
		MenuEntry.Add(new entry(TextID.OPTION, Vector2.Zero, Color.Gray));
		MenuEntry.Add(new entry(TextID.CREDIT, Vector2.Zero, Color.Gray));
		m_SelectorPos = new Vector2(640f, 360f);
		SignedInGamer.SignedOut += OnSignOut;
		SaveHandler.ResetState();
	}

	public void OnSignOut(object sender, SignedOutEventArgs e)
	{
		SignedInGamer_SignedOut(sender, e);
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			InputManager.GamerIndex[i] = -1;
		}
		LoadingScreen.Load(base.ScreenManager, false, PlayerIndex.One, new BumperScreen());
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		m_LogoPic = content.Load<Texture2D>("Common/SplashScreen/JS_SplashScreen_Jamsouls");
		m_LogoSkull = content.Load<Texture2D>("Common/SplashScreen/JS_SplashScreen_Head");
		m_FireAnim = LoadAnimatedSpriteFromXml("Common/SplashScreen/JS_SplashAnim.xml", "Common/SplashScreen/JS_SplashScreen_Halo");
		m_Selector = content.Load<Texture2D>("Common/SplashScreen/SS_Select");
		m_SelectorPos.X -= m_Selector.Width / 2;
		m_StartSound = new AudioClip("Menu_PlayerStart");
		m_SelectSound = new AudioClip("Menu_Valid");
		m_LogoPos = new Vector2(640 - m_LogoPic.Width / 2, -75f);
		m_LogoSkullPos = new Vector2(m_LogoPos.X + 380f, m_LogoPos.Y + 70f);
		m_FireAnim.m_FixedPos = new Vector2(m_LogoPos.X + 380f, m_LogoPos.Y + 70f);
		m_PressStartPos = new Vector2(640f, 560f);
		for (int i = 0; i < MenuEntry.Count; i++)
		{
			entry value = MenuEntry[i];
			value.pos = m_PressStartPos;
			value.pos.Y += 50 * i;
			MenuEntry[i] = value;
		}
		base.ScreenManager.Game.ResetElapsedTime();
		if (Gamer.SignedInGamers.Count <= 0 && !Guide.IsVisible)
		{
			Guide.ShowSignIn(4, onlineOnly: false);
		}
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (m_wait > 0f)
		{
			m_wait -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_wait <= 0f)
			{
				InitGameAsset();
			}
			return;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (m_PadTimer > 0f)
		{
			m_PadTimer -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (base.IsActive && m_ValidTime > 0f)
		{
			UpdateValid(gameTime.ElapsedGameTime.Milliseconds);
		}
		if (m_fadeTimer > 0.8f)
		{
			if (m_bFadeIn)
			{
				m_TextColor.A += 2;
			}
			else
			{
				m_TextColor.A -= 2;
			}
			if (m_TextColor.A >= 200)
			{
				m_TextColor.A = 200;
				m_bFadeIn = false;
			}
			if (m_TextColor.A <= 25)
			{
				m_TextColor.A = 25;
				m_bFadeIn = true;
			}
			m_fadeTimer = 0f;
		}
		if (!m_StartMenuChoice && m_DataLoad)
		{
			m_StartMenuChoice = true;
			m_StartSound.Play();
		}
		m_FireAnim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		m_fadeTimer += gameTime.ElapsedGameTime.Milliseconds;
	}

	public override void HandleInput()
	{
		for (int i = 0; i < 4; i++)
		{
			PlayerIndex playerIndex = (PlayerIndex)i;
			if (!GamePad.GetState(playerIndex).IsConnected || !base.IsActive || !(m_ValidTime <= 0f))
			{
				continue;
			}
			bool flag = IsSignedIn(playerIndex);
			if (GamePad.GetState(playerIndex).Buttons.Start == ButtonState.Pressed && !flag && !Guide.IsVisible)
			{
				Guide.ShowSignIn(4, onlineOnly: false);
			}
			if (flag && InputManager.GamerIndex[i] != -1)
			{
				if ((InputManager.GetKeyState(playerIndex, 4) == ButtonState.Pressed && m_StartMenuChoice) || (InputManager.GetKeyState(playerIndex, 8) == ButtonState.Pressed && (StarterIndex == i || StarterIndex == -1)))
				{
					if (StarterIndex == -1)
					{
						StarterIndex = i;
					}
					if (!m_bStartPushed)
					{
						m_bStartPushed = true;
						if (!m_StartMenuChoice)
						{
							if (!SaveHandler.IsLoadRequestDone())
							{
								base.ScreenManager.AddScreen(new DataScreen(bLoad: true), playerIndex);
								m_wait = 100f;
							}
							m_DataLoad = true;
						}
						else
						{
							m_LastPlayerIndex = (PlayerIndex)InputManager.GamerIndex[StarterIndex];
							m_StartSound.Play();
							m_ValidTime = 800f;
							m_PadTimer = 4000f;
						}
					}
				}
				if (StarterIndex == i)
				{
					m_bStartPushed = InputManager.GetKeyState(playerIndex, 8) == ButtonState.Pressed;
				}
				if (m_StartMenuChoice && m_PadTimer <= 0f)
				{
					if (InputManager.GetKeyState(playerIndex, 2) == ButtonState.Pressed)
					{
						entry value = MenuEntry[m_CurrentEntry];
						value.color = Color.Gray;
						MenuEntry[m_CurrentEntry] = value;
						m_SelectSound.Play();
						m_CurrentEntry++;
						if (m_CurrentEntry >= MenuEntry.Count)
						{
							m_CurrentEntry = 0;
						}
						value = MenuEntry[m_CurrentEntry];
						value.color = Color.White;
						MenuEntry[m_CurrentEntry] = value;
						m_PadTimer = 150f;
					}
					else if (InputManager.GetKeyState(playerIndex, 0) == ButtonState.Pressed)
					{
						entry value2 = MenuEntry[m_CurrentEntry];
						value2.color = Color.Gray;
						MenuEntry[m_CurrentEntry] = value2;
						m_SelectSound.Play();
						m_CurrentEntry--;
						if (m_CurrentEntry < 0)
						{
							m_CurrentEntry = MenuEntry.Count - 1;
						}
						value2 = MenuEntry[m_CurrentEntry];
						value2.color = Color.White;
						MenuEntry[m_CurrentEntry] = value2;
						m_PadTimer = 150f;
					}
				}
			}
			base.HandleInput();
		}
	}

	public void UpdateValid(float elapsed)
	{
		m_ValidTime -= elapsed;
		if (m_ValidTime <= 0f)
		{
			switch (m_CurrentEntry)
			{
			case 0:
				LoadingScreen.Load(base.ScreenManager, true, m_LastPlayerIndex, new MultiPlayerMenuScreen(m_LastPlayerIndex));
				break;
			case 1:
				LoadingScreen.Load(base.ScreenManager, false, m_LastPlayerIndex, new OptionScreen());
				break;
			case 2:
				LoadingScreen.Load(base.ScreenManager, false, m_LastPlayerIndex, new CreditScreen());
				break;
			case 3:
			case 4:
				break;
			}
		}
	}

	public bool IsSignedIn(PlayerIndex idx)
	{
		for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
		{
			if (Gamer.SignedInGamers[i].PlayerIndex == idx)
			{
				return true;
			}
		}
		return false;
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
		m_FireAnim.DrawFixed(SpriteEffects.None, Color.White, 0f);
		spriteBatch.Draw(m_LogoSkull, m_LogoSkullPos, Color.White);
		spriteBatch.Draw(m_LogoPic, m_LogoPos, Color.White);
		Color color = new Color(0, 0, 0, 0);
		if (!m_StartMenuChoice)
		{
			base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref m_PressStartPos, TextManager.GetText(TextID.PRESS_START), ScreenManager.TextOrigin.center_center, m_TextColor);
		}
		else
		{
			color = new Color(255, 255, 255, 255);
			for (int i = 0; i < MenuEntry.Count; i++)
			{
				entry entry2 = MenuEntry[i];
				base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref entry2.pos, TextManager.GetText(entry2.textid), ScreenManager.TextOrigin.center_center, MenuEntry[i].color);
			}
		}
		m_SelectorPos.Y = MenuEntry[m_CurrentEntry].pos.Y;
		spriteBatch.Draw(m_Selector, m_SelectorPos, color);
		spriteBatch.End();
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}
}
