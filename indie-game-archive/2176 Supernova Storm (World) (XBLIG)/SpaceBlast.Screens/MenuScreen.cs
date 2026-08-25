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
		FontSmallMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuSmallFont");
		FontMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuFont");
		FontBigMenuItem = MainGame.ContentMan.Load<SpriteFont>("Fonts/MenuHeading");
		m_TexRedButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonRed");
		m_TexGreenButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonGreen");
		m_TexYellowButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonYellow");
		m_TexBlueButton = MainGame.ContentMan.Load<Texture2D>("Textures/Menu_ButtonBlue");
		Spritebatch = new SpriteBatch(MainGame.Instance.GraphicsDevice);
		base.LoadContent();
	}

	private void CalcMenuPositions()
	{
		float num = MainGame.Instance.GraphicsDevice.Viewport.Width;
		float num2 = MainGame.Instance.GraphicsDevice.Viewport.Height;
		Rectangle screenRect = GetScreenRect();
		m_PosRedButton = new Vector2(screenRect.X + 8, (float)screenRect.Bottom - 45f);
		m_PosYellowButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 0.25f + 8f, (float)screenRect.Bottom - 45f);
		m_PosBlueButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 0.75f - 40f, (float)screenRect.Bottom - 45f);
		m_PosGreenButton = new Vector2((float)screenRect.X + (float)screenRect.Width * 1f - 40f, (float)screenRect.Bottom - 45f);
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
		Color white = Color.White;
		white.A = (byte)(alpha * 255f);
		Color gray = Color.Gray;
		gray.A = (byte)(alpha * 255f);
		Spritebatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
		Rectangle screenRect = GetScreenRect();
		Vector2 center = new Vector2
		{
			X = screenRect.Center.X,
			Y = (float)screenRect.Top + 20f
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
		Vector2 origin = font.MeasureString(text);
		origin.Y = 0f;
		switch (align)
		{
		case TextAlign.textLeft:
			origin.X = 0f;
			break;
		case TextAlign.textCentered:
			origin.X /= 2f;
			break;
		}
		Spritebatch.DrawString(font, text, center, color, 0f, origin, 1f, SpriteEffects.None, 1f);
	}

	public override void Update()
	{
		if (Guide.IsVisible)
		{
			return;
		}
		GamePadState player1Input = InputManager.GetPlayer1Input();
		KeyboardState state = Keyboard.GetState();
		MenuAction menuAction = MenuAction.None;
		if (player1Input.ThumbSticks.Left.Y < -0.5f || player1Input.DPad.Down == ButtonState.Pressed || state.IsKeyDown(Keys.Down))
		{
			menuAction = MenuAction.Down;
		}
		else if (player1Input.ThumbSticks.Left.Y > 0.5f || player1Input.DPad.Up == ButtonState.Pressed || state.IsKeyDown(Keys.Up))
		{
			menuAction = MenuAction.Up;
		}
		else if (player1Input.Buttons.A == ButtonState.Pressed || player1Input.Buttons.Start == ButtonState.Pressed || state.IsKeyDown(Keys.Space))
		{
			menuAction = MenuAction.Green;
		}
		else if (player1Input.Buttons.B == ButtonState.Pressed || state.IsKeyDown(Keys.Back))
		{
			menuAction = MenuAction.Red;
		}
		else if (player1Input.Buttons.X == ButtonState.Pressed || state.IsKeyDown(Keys.X))
		{
			menuAction = MenuAction.Blue;
		}
		else if (player1Input.Buttons.Y == ButtonState.Pressed || state.IsKeyDown(Keys.Y))
		{
			menuAction = MenuAction.Yellow;
		}
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
