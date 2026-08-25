using System;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JamSouls;

internal class PauseMenuScreen : MenuScreen
{
	private const float BUTTON_OFFSET_Y = 70f;

	private const float TEXT_OFFSET_X = 100f;

	private const float TEXT_OFFSET_Y = 30f;

	private const int BACKGROUND_HEIGHT = 560;

	private const int BACKGROUND_Y = 80;

	private const int BORDER_SIZE = 10;

	private GameState gamestate;

	private Vector2 Apos = new Vector2(590f, 470f);

	private Vector2 Atextpos = new Vector2(650f, 500f);

	private Vector2 Bpos = new Vector2(700f, 550f);

	private Vector2 Btextpos = new Vector2(760f, 580f);

	private Vector2 SplatPos = new Vector2(350f, 100f);

	private Vector2 TitlePos = new Vector2(480f, 210f);

	private Vector2 StartButtonPos = new Vector2(350f, 80f);

	private MessageBoxScreen confirmQuitMessageBox;

	private bool bDrawHud = true;

	private bool m_bApushed;

	private bool m_DrawTuto;

	private TimeSpan m_SongElapsed;

	private TextID m_BtextId;

	private TextID m_XtextId;

	public PauseMenuScreen(GameState thegamestate)
		: base("")
	{
		base.IsPopup = true;
		gamestate = thegamestate;
		StartX = 480;
		StartY = 290;
		MenuEntry menuEntry = new MenuEntry(TextManager.GetText(TextID.RESUME_GAME));
		MenuEntry menuEntry2 = new MenuEntry("Music : " + SaveHandler.m_data.BmgVolume);
		MenuEntry menuEntry3 = new MenuEntry("Sfx : " + SaveHandler.m_data.SfxVolume);
		MenuEntry menuEntry4 = new MenuEntry(TextManager.GetText(TextID.TUTO));
		MenuEntry menuEntry5 = new MenuEntry(TextManager.GetText(TextID.QUIT_GAME));
		menuEntry.Selected += base.OnCancel;
		menuEntry5.Selected += QuitGameMenuEntrySelected;
		menuEntry4.Selected += OnDisplayTuto;
		menuEntry2.Selected += OnChangeMusicVolume;
		menuEntry3.Selected += OnChangeSfxVolume;
		base.MenuEntries.Add(menuEntry);
		base.MenuEntries.Add(menuEntry2);
		base.MenuEntries.Add(menuEntry3);
		base.MenuEntries.Add(menuEntry4);
		base.MenuEntries.Add(menuEntry5);
		m_SongElapsed = MediaPlayer.PlayPosition;
		m_BtextId = TextID.GRAB_POWERUP;
		m_XtextId = TextID.USE_POWERUP;
		if (GameContext.GameMode == GAME_MODE.JAM_BALL)
		{
			m_BtextId = TextID.SHOOT_LOW;
			m_XtextId = TextID.SHOOT_HIGH;
		}
		MediaPlayer.Pause();
	}

	private void OnChangeSfxVolume(object sender, PlayerIndexEventArgs e)
	{
		if (!m_bApushed)
		{
			SaveHandler.m_data.SfxVolume++;
			if (SaveHandler.GetSaveData().SfxVolume > 9)
			{
				SaveHandler.m_data.SfxVolume = 0;
			}
			base.MenuEntries[2].Text = "Sfx : " + SaveHandler.m_data.SfxVolume;
			m_bApushed = true;
			AudioManager.SetSfxVolume(SaveHandler.m_data.SfxVolume);
		}
	}

	private void OnChangeMusicVolume(object sender, PlayerIndexEventArgs e)
	{
		if (!m_bApushed)
		{
			SaveHandler.m_data.BmgVolume++;
			if (SaveHandler.m_data.BmgVolume > 9)
			{
				SaveHandler.m_data.BmgVolume = 0;
			}
			base.MenuEntries[1].Text = "Music : " + SaveHandler.m_data.BmgVolume;
			m_bApushed = true;
			MediaPlayer.Volume = (float)SaveHandler.m_data.BmgVolume / 10f;
		}
	}

	protected override void OnCancel(PlayerIndex playerIndex)
	{
		gamestate.ResumePause();
		MediaPlayer.Resume();
		base.OnCancel(playerIndex);
	}

	private void OnDisplayTuto(object sender, PlayerIndexEventArgs e)
	{
		m_DrawTuto = true;
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (base.ScreenState != ScreenState.TransitionOff)
		{
			AnimatedSprite[] btSpriteSoft = gamestate.m_btSpriteSoft;
			foreach (AnimatedSprite animatedSprite in btSpriteSoft)
			{
				animatedSprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			}
		}
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 4) == ButtonState.Released)
		{
			m_bApushed = false;
		}
		if (!m_DrawTuto)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			return;
		}
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 5) == ButtonState.Pressed)
		{
			m_DrawTuto = false;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	private void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
	{
		string text = TextManager.GetText(TextID.MESSAGE_QUIT_CONFIRM);
		bDrawHud = false;
		confirmQuitMessageBox = new MessageBoxScreen(text, includeUsageText: false, gamestate.m_btSpriteSoft[1], gamestate.m_btSpriteSoft[0]);
		confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
		confirmQuitMessageBox.Cancelled += CancelQuitMessageBoxCancel;
		base.ScreenManager.AddScreen(confirmQuitMessageBox, base.ControllingPlayer);
	}

	private void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
	{
		LoadingScreen.Load(base.ScreenManager, true, null, new MultiPlayerMenuScreen(e.PlayerIndex));
	}

	private void CancelQuitMessageBoxCancel(object sender, PlayerIndexEventArgs e)
	{
		bDrawHud = true;
	}

	public override void Draw(GameTime gameTime)
	{
		base.ScreenManager.FadeBackBufferToBlack(base.TransitionAlpha * 2 / 3);
		Color white = Color.White;
		white.A = base.TransitionAlpha;
		if (!bDrawHud)
		{
			return;
		}
		if (!m_DrawTuto)
		{
			base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			gamestate.m_PauseTexture.Draw(SplatPos, white, SpriteEffects.None, 1f);
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.PAUSED), TitlePos, Color.White);
			gamestate.m_btSpriteSoft[0].Draw(ref Apos, SpriteEffects.None, white, 1f);
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.VALID), Atextpos, Color.White);
			base.ScreenManager.SpriteBatch.End();
			base.Draw(gameTime);
			return;
		}
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
		Vector2 Position = StartButtonPos;
		Vector2 startButtonPos = StartButtonPos;
		startButtonPos.X += 100f;
		startButtonPos.Y += 30f;
		base.ScreenManager.SpriteBatch.Draw(gamestate.m_BackGroundTex, new Rectangle(0, 70, 1280, 580), Color.White);
		base.ScreenManager.SpriteBatch.Draw(gamestate.m_BackGroundTex, new Rectangle(0, 80, 1280, 560), Color.Black);
		gamestate.m_btTexture[0].Draw(Position, white, SpriteEffects.None, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.MOVE), startButtonPos, Color.White);
		Position.Y += 70f;
		startButtonPos.Y += 70f;
		gamestate.m_btSpriteSoft[0].Draw(ref Position, SpriteEffects.None, white, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.JUMP), startButtonPos, Color.White);
		Position.Y += 70f;
		startButtonPos.Y += 70f;
		gamestate.m_btSpriteSoft[3].Draw(ref Position, SpriteEffects.None, white, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(m_XtextId), startButtonPos, Color.White);
		Position.Y += 70f;
		startButtonPos.Y += 70f;
		gamestate.m_btSpriteSoft[1].Draw(ref Position, SpriteEffects.None, white, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(m_BtextId), startButtonPos, Color.White);
		Position.Y += 70f;
		startButtonPos.Y += 70f;
		if (GameContext.GameMode == GAME_MODE.JAM_BALL)
		{
			gamestate.m_btSpriteSoft[2].Draw(ref Position, SpriteEffects.None, white, 1f);
			base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.SHOOT_UP), startButtonPos, Color.White);
			Position.Y += 70f;
			startButtonPos.Y += 70f;
		}
		gamestate.m_btTexture[1].Draw(Position, white, SpriteEffects.None, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.RUN), startButtonPos, Color.White);
		Position.Y += 70f;
		startButtonPos.Y += 70f;
		gamestate.m_btTexture[2].Draw(Position, white, SpriteEffects.None, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.DRAW_NAME), startButtonPos, Color.White);
		gamestate.m_btSpriteSoft[1].Draw(ref Bpos, SpriteEffects.None, white, 1f);
		base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomBig, TextManager.GetText(TextID.RESUME_GAME), Btextpos, Color.White);
		base.ScreenManager.SpriteBatch.End();
	}
}
