using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace JamSouls;

internal class OptionScreen : GameScreen
{
	private const float INPUT_LATENCY = 200f;

	private ContentManager content;

	private List<OptionEntry> m_OptionEntry = new List<OptionEntry>();

	private Vector2 PowerUpTextOffset = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop + 180);

	private Vector2 SfxVolPos = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop);

	private Vector2 BmgVolPos = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop + 50);

	private Vector2 SfxValuePos = new Vector2(GameContext.TileSafeLeft + 200, GameContext.TileSafeTop);

	private Vector2 BmgValuePos = new Vector2(GameContext.TileSafeLeft + 200, GameContext.TileSafeTop + 50);

	private Vector2 SoulPos = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop + 100);

	private Vector2 ExitPos = new Vector2(GameContext.TileSafeLeft + 80, GameContext.TileSafeTop + 510);

	private Vector2 BtPos = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeTop + 470);

	private bool bLoadStart;

	private int m_SelectedId;

	private float m_InputTime;

	private bool m_bSaveRequest;

	private AnimatedSprite m_btB;

	public OptionScreen()
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(1.5);
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		Sprite[] array = new Sprite[10];
		array[0] = LoadSprite("PowerUpOption_skull", GameState.GameAtlas.GAME);
		array[2] = LoadSprite("PowerUpOption_soldier", GameState.GameAtlas.GAME);
		array[8] = LoadSprite("PowerUpOption_fireprout", GameState.GameAtlas.GAME);
		array[3] = LoadSprite("PowerUpOption_bomb", GameState.GameAtlas.GAME);
		array[5] = LoadSprite("PowerUpOption_seed", GameState.GameAtlas.GAME);
		array[4] = LoadSprite("PowerUpOption_cloud", GameState.GameAtlas.GAME);
		array[1] = LoadSprite("PowerUpOption_fly", GameState.GameAtlas.GAME);
		array[6] = LoadSprite("PowerUpOption_heart", GameState.GameAtlas.GAME);
		array[7] = LoadSprite("PowerUpOption_wood", GameState.GameAtlas.GAME);
		array[9] = LoadSprite("PowerUpOption_sugar1", GameState.GameAtlas.GAME);
		m_OptionEntry.Add(new OptionEntry(null, "SFX : " + TextManager.GetText(TextID.VOLUME), SfxVolPos, SaveHandler.GetSaveData().SfxVolume));
		for (int i = 0; i < 10; i++)
		{
			m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(i.ToString());
		}
		m_OptionEntry.Add(new OptionEntry(null, "Music : " + TextManager.GetText(TextID.VOLUME), BmgVolPos, SaveHandler.GetSaveData().BmgVolume));
		for (int j = 0; j < 10; j++)
		{
			m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(j.ToString());
		}
		m_OptionEntry.Add(new OptionEntry(null, "Soul : ", SoulPos, SaveHandler.GetSaveData().bUseSouls));
		m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(TextManager.GetText(TextID.ON));
		m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(TextManager.GetText(TextID.OFF));
		for (int k = 0; k < array.Length; k++)
		{
			Vector2 powerUpTextOffset = PowerUpTextOffset;
			if (k >= 5)
			{
				powerUpTextOffset.Y += (k - 5) * 60;
				powerUpTextOffset.X += 400f;
			}
			else
			{
				powerUpTextOffset.Y += k * 60;
			}
			m_OptionEntry.Add(new OptionEntry(array[k], TextManager.GetText((TextID)(71 + k)) + " : ", powerUpTextOffset, SaveHandler.GetSaveData().BonusFrequency[k]));
			m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(TextManager.GetText(TextID.ON));
			m_OptionEntry[m_OptionEntry.Count - 1].m_Entry.Add(TextManager.GetText(TextID.OFF));
		}
		m_btB = LoadAnimatedSpriteFromXml("Hud/bt/bulle.xml", GameState.GameAtlas.GAME, GameContext.PAD_BUTTON_HUD[1]);
		m_btB.SetPosition(BtPos);
		base.ScreenManager.Game.ResetElapsedTime();
	}

	public override void UnloadContent()
	{
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
		if (m_bSaveRequest && !bLoadStart)
		{
			if (SaveHandler.IsSaveRequestDone())
			{
				bLoadStart = true;
				LoadingScreen.Load(base.ScreenManager, false, base.ControllingPlayer.Value, new LogoScreen());
			}
		}
		else if (m_InputTime <= 0f && !base.IsExiting)
		{
			ManageInput();
		}
		else
		{
			m_InputTime -= gameTime.ElapsedGameTime.Milliseconds;
		}
		m_btB.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
	}

	public void ManageInput()
	{
		if (InputManager.GetKeyState(base.ControllingPlayer.Value, 0) == ButtonState.Pressed)
		{
			m_OptionEntry[m_SelectedId].color = Color.Gray;
			m_SelectedId--;
			if (m_SelectedId < 0)
			{
				m_SelectedId = m_OptionEntry.Count - 1;
			}
			m_InputTime = 200f;
		}
		else if (InputManager.GetKeyState(base.ControllingPlayer.Value, 2) == ButtonState.Pressed)
		{
			m_OptionEntry[m_SelectedId].color = Color.Gray;
			m_SelectedId++;
			if (m_SelectedId > m_OptionEntry.Count - 1)
			{
				m_SelectedId = 0;
			}
			m_InputTime = 200f;
		}
		else if (InputManager.GetKeyState(base.ControllingPlayer.Value, 4) == ButtonState.Pressed)
		{
			if (m_OptionEntry[m_SelectedId].m_Entry.Count <= 2)
			{
				if (m_OptionEntry[m_SelectedId].m_SelectedOption == 0)
				{
					m_OptionEntry[m_SelectedId].m_SelectedOption = 1;
				}
				else
				{
					m_OptionEntry[m_SelectedId].m_SelectedOption = 0;
				}
			}
			else if (m_OptionEntry[m_SelectedId].m_SelectedOption < m_OptionEntry[m_SelectedId].m_Entry.Count - 1)
			{
				m_OptionEntry[m_SelectedId].m_SelectedOption++;
			}
			else
			{
				m_OptionEntry[m_SelectedId].m_SelectedOption = 0;
			}
			m_InputTime = 200f;
		}
		else if (InputManager.GetKeyState(base.ControllingPlayer.Value, 5) == ButtonState.Pressed)
		{
			base.ScreenManager.AddScreen(new DataScreen(bLoad: false), base.ControllingPlayer.Value);
			m_bSaveRequest = true;
			SaveHandler.m_data.SfxVolume = m_OptionEntry[0].m_SelectedOption;
			SaveHandler.m_data.BmgVolume = m_OptionEntry[1].m_SelectedOption;
			SaveHandler.m_data.bUseSouls = m_OptionEntry[2].m_SelectedOption;
			for (int i = 3; i < m_OptionEntry.Count; i++)
			{
				SaveHandler.m_data.BonusFrequency[i - 3] = m_OptionEntry[i].m_SelectedOption;
			}
			MediaPlayer.Volume = (float)SaveHandler.m_data.BmgVolume / 10f;
			AudioManager.SetSfxVolume(SaveHandler.m_data.SfxVolume);
		}
		m_OptionEntry[m_SelectedId].color = Color.White;
	}

	public override void Draw(GameTime gameTime)
	{
		SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
		spriteBatch.Begin();
		for (int i = 0; i < m_OptionEntry.Count; i++)
		{
			m_OptionEntry[i].Draw(base.ScreenManager);
		}
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref ExitPos, TextManager.GetText(TextID.EXIT_AND_SAVE), ScreenManager.TextOrigin.top_Left, Color.White);
		m_btB.DrawFixed(SpriteEffects.None, Color.White, 1f);
		spriteBatch.End();
		if (base.TransitionPosition > 0f)
		{
			base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
		}
	}

	public AnimatedSprite LoadAnimatedSpriteFromXml(string XmlPath, GameState.GameAtlas AtlasID, string TextureName)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.IgnoreWhitespace = true;
		xmlReaderSettings.IgnoreComments = true;
		XmlReader xmlReader = XmlReader.Create(content.RootDirectory + "/" + XmlPath, xmlReaderSettings);
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.AttributeCount > 0 && xmlReader.Name == "Anim")
			{
				int frameCount = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				int width = int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				int height = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				float speed = float.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture);
				Sprite sprite = GameState.m_GameAtlas[(int)AtlasID].FindInAtlas(TextureName);
				return new AnimatedSprite(base.ScreenManager.SpriteBatch, GameState.m_GameAtlas[(int)AtlasID].GetTexture(), frameCount, width, height, speed, 0, sprite.rect.X, sprite.rect.Y);
			}
		}
		return null;
	}
}
