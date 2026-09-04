using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceBlast.Screens;

internal abstract class MenuScreen : GameScreen
{
	protected enum TextAlign
	{
		textLeft,
		textCentered,
		textRight
	}

	private enum MenuAction
	{
		None,
		Down,
		Up,
		Left,
		Right,
		Red,
		Green,
		Yellow,
		Blue
	}

	protected const float constMenuTextSpacingSmall = 25f;

	protected const float constMenuTextSpacing = 32f;

	protected const float constMenuTextSpacingBig = 40f;

	protected List<MenuItem> MenuItems = new List<MenuItem>();

	protected SpriteBatch Spritebatch;

	protected SpriteFont FontBigMenuItem;

	protected SpriteFont FontMenuItem;

	protected SpriteFont FontSmallMenuItem;

	protected int SelectedItemIndex;

	protected string RedButtonText;

	protected string GreenButtonText;

	protected string YellowButtonText;

	protected string BlueButtonText;

	private Texture2D m_TexRedButton;

	private Texture2D m_TexGreenButton;

	private Texture2D m_TexYellowButton;

	private Texture2D m_TexBlueButton;

	private Vector2 m_PosRedButton;

	private Vector2 m_PosGreenButton;

	private Vector2 m_PosYellowButton;

	private Vector2 m_PosBlueButton;

	private MenuAction m_LastMenuAction;

	private double m_LastMenuActionTime;

	protected int SelectedItemID => MenuItems[SelectedItemIndex].ItemID;

	public MenuScreen(ScreenManager manager)
		: base(manager)
	{
	}

	public override void LoadContent()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		FontSmallMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuSmallFont");
		FontMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuFont");
		FontBigMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuHeading");
		m_TexRedButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonRed");
		m_TexGreenButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonGreen");
		m_TexYellowButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonYellow");
		m_TexBlueButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonBlue");
		Spritebatch = new SpriteBatch(((Game)MainGame.Instance).GraphicsDevice);
		base.LoadContent();
	}

	private void CalcMenuPositions()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = ((Game)MainGame.Instance).GraphicsDevice.Viewport;
		float num2 = ((Viewport)(ref viewport2)).Height;
		Rectangle screenRect = GetScreenRect();
		m_PosRedButton = new Vector2((float)(screenRect.X + 8), (float)((Rectangle)(ref screenRect)).Bottom - 45f);
		m_PosYellowButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 0.25f + 8f, (float)((Rectangle)(ref screenRect)).Bottom - 45f);
		m_PosBlueButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 0.75f - 40f, (float)((Rectangle)(ref screenRect)).Bottom - 45f);
		m_PosGreenButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 1f - 40f, (float)((Rectangle)(ref screenRect)).Bottom - 45f);
	}

	public override void OnShowScreen()
	{
		PreventClickThrough();
		if (SelectedItemIndex >= MenuItems.Count())
		{
			SelectedItemIndex = 0;
		}
		CalcMenuPositions();
		base.OnShowScreen();
	}

	public void PreventClickThrough()
	{
		m_LastMenuActionTime = TimeManager.RawTime + 2.0;
		m_LastMenuAction = MenuAction.Green;
	}

	public override void OnScreenResize()
	{
		CalcMenuPositions();
		base.OnScreenResize();
	}

	public override void Draw(float alpha)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		((Color)(ref white)).A = (byte)(alpha * 255f);
		Color gray = Color.Gray;
		((Color)(ref gray)).A = (byte)(alpha * 255f);
		Spritebatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = ((Rectangle)(ref screenRect)).Center.X,
			Y = (float)((Rectangle)(ref screenRect)).Top + 20f
		};
		int num = 0;
		foreach (MenuItem menuItem in MenuItems)
		{
			DrawText(FontMenuItem, center, menuItem.MenuText, (SelectedItemIndex == num) ? white : gray, TextAlign.textCentered);
			center.Y += 40f;
			num++;
		}
		if (RedButtonText != null)
		{
			Spritebatch.Draw(m_TexRedButton, m_PosRedButton, white);
			center = m_PosRedButton;
			center.X += 35f;
			center.Y -= 4f;
			DrawText(FontMenuItem, center, RedButtonText, white, TextAlign.textLeft);
		}
		if (GreenButtonText != null)
		{
			Spritebatch.Draw(m_TexGreenButton, m_PosGreenButton, white);
			center = m_PosGreenButton;
			center.X -= 3f;
			center.Y -= 4f;
			DrawText(FontMenuItem, center, GreenButtonText, white, TextAlign.textRight);
		}
		if (YellowButtonText != null)
		{
			Spritebatch.Draw(m_TexYellowButton, m_PosYellowButton, white);
			center = m_PosYellowButton;
			center.X += 35f;
			center.Y -= 4f;
			DrawText(FontMenuItem, center, YellowButtonText, white, TextAlign.textLeft);
		}
		if (BlueButtonText != null)
		{
			Spritebatch.Draw(m_TexBlueButton, m_PosBlueButton, white);
			center = m_PosBlueButton;
			center.X -= 3f;
			center.Y -= 4f;
			DrawText(FontMenuItem, center, BlueButtonText, white, TextAlign.textRight);
		}
		Spritebatch.End();
		base.Draw(alpha);
	}

	protected void DrawText(SpriteFont font, Vector2 center, string text, Color color, TextAlign align)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = font.MeasureString(text);
		val.Y = 0f;
		switch (align)
		{
		case TextAlign.textLeft:
			val.X = 0f;
			break;
		case TextAlign.textCentered:
			val.X /= 2f;
			break;
		}
		Spritebatch.DrawString(font, text, center, color, 0f, val, 1f, (SpriteEffects)0, 1f);
	}

	public override void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Invalid comparison between Unknown and I4
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Invalid comparison between Unknown and I4
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Invalid comparison between Unknown and I4
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Invalid comparison between Unknown and I4
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Invalid comparison between Unknown and I4
		if (Guide.IsVisible)
		{
			return;
		}
		GamePadState player1Input = InputManager.GetPlayer1Input();
		KeyboardState state = Keyboard.GetState();
		MenuAction menuAction = MenuAction.None;
		GamePadThumbSticks thumbSticks = ((GamePadState)(ref player1Input)).ThumbSticks;
		if (!(((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5f))
		{
			GamePadDPad dPad = ((GamePadState)(ref player1Input)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Down != 1 && !((KeyboardState)(ref state)).IsKeyDown((Keys)40))
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref player1Input)).ThumbSticks;
				if (!(((GamePadThumbSticks)(ref thumbSticks2)).Left.Y > 0.5f))
				{
					GamePadDPad dPad2 = ((GamePadState)(ref player1Input)).DPad;
					if ((int)((GamePadDPad)(ref dPad2)).Up != 1 && !((KeyboardState)(ref state)).IsKeyDown((Keys)38))
					{
						GamePadButtons buttons = ((GamePadState)(ref player1Input)).Buttons;
						if ((int)((GamePadButtons)(ref buttons)).A != 1)
						{
							GamePadButtons buttons2 = ((GamePadState)(ref player1Input)).Buttons;
							if ((int)((GamePadButtons)(ref buttons2)).Start != 1 && !((KeyboardState)(ref state)).IsKeyDown((Keys)32))
							{
								GamePadButtons buttons3 = ((GamePadState)(ref player1Input)).Buttons;
								if ((int)((GamePadButtons)(ref buttons3)).B == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)8))
								{
									menuAction = MenuAction.Red;
								}
								else
								{
									GamePadButtons buttons4 = ((GamePadState)(ref player1Input)).Buttons;
									if ((int)((GamePadButtons)(ref buttons4)).X == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)88))
									{
										menuAction = MenuAction.Blue;
									}
									else
									{
										GamePadButtons buttons5 = ((GamePadState)(ref player1Input)).Buttons;
										if ((int)((GamePadButtons)(ref buttons5)).Y == 1 || ((KeyboardState)(ref state)).IsKeyDown((Keys)89))
										{
											menuAction = MenuAction.Yellow;
										}
									}
								}
								goto IL_012f;
							}
						}
						menuAction = MenuAction.Green;
						goto IL_012f;
					}
				}
				menuAction = MenuAction.Up;
				goto IL_012f;
			}
		}
		menuAction = MenuAction.Down;
		goto IL_012f;
		IL_012f:
		if (menuAction != m_LastMenuAction || TimeManager.RawTime - m_LastMenuActionTime > 0.25)
		{
			switch (menuAction)
			{
			case MenuAction.Up:
				if (MenuItems.Count > 0)
				{
					SelectedItemIndex--;
					if (SelectedItemIndex < 0)
					{
						SelectedItemIndex = MenuItems.Count - 1;
					}
					MainGame.AudioMan.Play(Sound.Click);
				}
				break;
			case MenuAction.Down:
				if (MenuItems.Count > 0)
				{
					SelectedItemIndex++;
					if (SelectedItemIndex >= MenuItems.Count())
					{
						SelectedItemIndex = 0;
					}
					MainGame.AudioMan.Play(Sound.Click);
				}
				break;
			case MenuAction.Red:
				if (RedButtonText != null)
				{
					MainGame.AudioMan.Play(Sound.Click);
				}
				OnRedButtonPressed();
				break;
			case MenuAction.Green:
				if (GreenButtonText != null)
				{
					MainGame.AudioMan.Play(Sound.Click);
				}
				OnGreenButtonPressed();
				break;
			case MenuAction.Yellow:
				if (YellowButtonText != null)
				{
					MainGame.AudioMan.Play(Sound.Click);
				}
				OnYellowButtonPressed();
				break;
			case MenuAction.Blue:
				if (BlueButtonText != null)
				{
					MainGame.AudioMan.Play(Sound.Click);
				}
				OnBlueButtonPressed();
				break;
			}
			m_LastMenuAction = menuAction;
			m_LastMenuActionTime = TimeManager.RawTime;
		}
		base.Update();
	}

	protected virtual void OnRedButtonPressed()
	{
	}

	protected virtual void OnGreenButtonPressed()
	{
	}

	protected virtual void OnYellowButtonPressed()
	{
	}

	protected virtual void OnBlueButtonPressed()
	{
	}
}
