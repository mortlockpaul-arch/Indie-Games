using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

internal class CreditScreen : GameState
{
	private const int TITLE_OFFSET = 50;

	private const float SCROLL_SPEED = 0.035f;

	private const int LIMIT = 1300;

	private Sprite[] m_TableTexture = new Sprite[10];

	private Texture2D m_Logo;

	private Sprite m_Head;

	private Vector2 LEFT_POS = new Vector2(GameContext.TileSafeLeft, 200f);

	private Vector2 RIGHT_POS = new Vector2(GameContext.TileSafeRight - 124, 200f);

	private Vector2 OFFSET = new Vector2(0f, 250f);

	private Vector2 TextStart = new Vector2(0f, 200f);

	public CreditScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.0);
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		for (int i = 0; i < 10; i++)
		{
			m_TableTexture[i] = LoadSprite("MM_Tableau_" + PlayerConfig.CHARACTER_NAME[i], GameAtlas.GAME);
		}
		m_Logo = content.Load<Texture2D>("Common/ChromaticDreamSmall");
		m_Head = LoadSprite("MVS_Up", GameAtlas.GAME);
		InitHud(initPlayerButton: true);
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (LEFT_POS.Y > -1300f)
		{
			LEFT_POS.Y -= 0.035f * (float)gameTime.ElapsedGameTime.Milliseconds;
			RIGHT_POS.Y -= 0.035f * (float)gameTime.ElapsedGameTime.Milliseconds;
		}
		else
		{
			LEFT_POS.Y = -1300f;
		}
		UpdateHud(gameTime);
	}

	public override void HandleInput()
	{
		if (base.IsActive && InputManager.GetKeyState(base.ControllingPlayer.Value, 5) == ButtonState.Pressed)
		{
			LoadingScreen.Load(base.ScreenManager, false, base.ControllingPlayer, new LogoScreen());
		}
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		base.ScreenManager.GraphicsDevice.Clear(Color.Black);
		for (int i = 0; i < 10; i++)
		{
			if (i <= 4)
			{
				m_TableTexture[i].Draw(LEFT_POS + OFFSET * i, Color.White);
			}
			else
			{
				m_TableTexture[i].Draw(RIGHT_POS + OFFSET * (i - 5), Color.White);
			}
		}
		TextStart = new Vector2(640f, LEFT_POS.Y + 150f);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomVeryBig, ref TextStart, TextManager.GetText(TextID.CODE), ScreenManager.TextOrigin.center_center, Color.White);
		TextStart.Y += 50f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Cheminade Pierrick", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 100f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomVeryBig, ref TextStart, TextManager.GetText(TextID.ARTIST), ScreenManager.TextOrigin.center_center, Color.White);
		TextStart.Y += 50f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Bax Rodolphe", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 100f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomVeryBig, ref TextStart, TextManager.GetText(TextID.MUSIC_COMPOSITOR), ScreenManager.TextOrigin.center_center, Color.White);
		TextStart.Y += 50f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Gallard Vivien", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Alvaro Iglesias", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 100f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomVeryBig, ref TextStart, TextManager.GetText(TextID.LOCALISATION), ScreenManager.TextOrigin.center_center, Color.White);
		TextStart.Y += 50f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Yiling Tu", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Gonzalo Peces Nicolás", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Valentina Colombo", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Ayami Yokayama", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "M.Guery:", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 100f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomVeryBig, ref TextStart, TextManager.GetText(TextID.THANKS), ScreenManager.TextOrigin.center_center, Color.White);
		TextStart.Y += 50f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Junying Yang", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Bax David", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Domitin Yann", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Mouret Laurie", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Girin Jonathan", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Lebre Remi", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Chagnoleau Alban", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Cunzi Matthieu", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Lallée Stéphane", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 40f;
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref TextStart, "Sarah Li-Waï-Yeung", ScreenManager.TextOrigin.center_center, Color.Gray);
		TextStart.Y += 120f;
		TextStart.X = 640 - m_Logo.Width / 2;
		base.ScreenManager.SpriteBatch.Draw(m_Logo, TextStart, Color.White);
		m_Head.Draw(new Vector2(0f, -20f), Color.White);
		Vector2 position = new Vector2(GameContext.TileSafeLeft + 60, GameContext.TileSafeTop);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref position, TextManager.GetText(TextID.BACK), ScreenManager.TextOrigin.top_Left, Color.Gray);
		position.X -= 60f;
		position.Y -= 30f;
		m_btSprite[1].Draw(ref position, SpriteEffects.None, Color.White, 1f);
		base.ScreenManager.SpriteBatch.End();
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}
}
