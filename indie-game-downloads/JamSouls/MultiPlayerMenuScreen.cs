using System;
using System.Collections.Generic;
using JamSouls.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectMercury.Renderers;

namespace JamSouls;

internal class MultiPlayerMenuScreen : GameState
{
	public enum PlayerMenuState
	{
		CHARACTER_JOIN,
		CHARACTER_SELECT,
		CHARACTER_CONFIRM
	}

	private struct GamerLite(Texture2D pPic, string sName, PlayerMenuState pMs)
	{
		public string Name = sName;

		public PlayerMenuState Pms = pMs;

		public int SlotIdx = -1;

		public int dollIdx = -1;
	}

	private delegate void OnScrollOver();

	private enum LoadState
	{
		LOAD_INIT,
		LOADING_HUD,
		LOADING_LEVEL,
		LOADING_SOUND,
		INIT_BUTTON,
		INIT_BORDEL,
		LOADING_FINISHED
	}

	private const int ARROW_OFFSET = 370;

	private const int PLAYER_OFFSET = 350;

	private const int CHAR_NAME_MAX = 8;

	private const int OPTION_MENU_OFFSET = 55;

	private const int GAMESCREEN_CHARACTER = 0;

	private const int GAMESCREEN_MODE = 1;

	private const int GAMESCREEN_LEVEL = 2;

	private const int GAMESCREEN_OPTION = 3;

	private const int GAMESCREEN_COUNT = 4;

	private const float SCROLL_SPEED = 5f;

	private const float SCROLL_UPDATE_RATE = 0.6f;

	private const float BUBBLE_FADE_TIME = 300f;

	private List<Level.DummyPoint> Dummy = new List<Level.DummyPoint>();

	private List<ButtonIcon> m_ButtonArray = new List<ButtonIcon>();

	private bool m_bScroll;

	private Vector2 m_CurrentScroll;

	private Vector2 m_FinishScroll;

	private List<int> m_DollIdx = new List<int>();

	private List<int> m_ModeIdx = new List<int>();

	private GamerLite[] m_Gamer = new GamerLite[4];

	private int m_nLevelNum;

	private PlayerIndex m_FirstPlayerIndex;

	private List<BackgroundLayer> m_ButtonLayer = new List<BackgroundLayer>();

	private Vector2[] m_PlayerPos = new Vector2[4];

	private Vector2[] m_ArrowPos = new Vector2[4];

	private Color[] m_PlayerColor = new Color[4];

	private string[] m_PlayerCharName = new string[4];

	private Vector2[] m_AvatarPos = new Vector2[4];

	private Vector2 m_LevelNamePos;

	private Vector2 m_OptionStart;

	private int m_nRmapBtIdx;

	private int m_nLmapBtIdx;

	private List<Option> m_OptionList = new List<Option>();

	private int m_CurrentOption;

	private BackgroundLayer m_LevelIcon;

	private BackgroundLayer m_ReferenceBackground;

	private Sprite m_Cross;

	private Sprite m_Back;

	private Sprite m_BackgroundTuto;

	public Sprite m_LockIcon;

	public Sprite m_LockIconPerso;

	private AudioClip m_SelectSound;

	private AudioClip m_ValidSound;

	private AudioClip m_BackSound;

	private AudioClip m_StarTSound;

	private Sprite m_PlayerArrow;

	private Color m_HudTextColor = Color.White;

	private Color m_TitleColor = new Color(255, 168, 0);

	private BackgroundLayer m_Miroir;

	private Vector2 BTA_POS = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeBottom);

	private Vector2 BTB_POS = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeBottom);

	private Vector2 CROSS_POS = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeBottom);

	private Vector2 BACK_POS = new Vector2(GameContext.TileSafeLeft, GameContext.TileSafeBottom);

	private Vector2 BKG_POS = new Vector2(0f, 570f);

	private float m_ScrollTimer;

	private int m_CurrentScreen;

	private int m_FootBtId;

	private float m_pStartTimer;

	private Color m_pStartColor = Color.White;

	private float[] m_PlayerInputTimer = new float[InputManager.Controller.Length];

	private List<int> m_SelectedDoll = new List<int>();

	private OnScrollOver m_ScrollCbk;

	private LoadState loadState;

	private bool m_bScreenFading;

	public MultiPlayerMenuScreen(PlayerIndex FirstPlayerIndex)
	{
		base.TransitionOnTime = TimeSpan.FromSeconds(1.5);
		base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		m_ScrollTimer = 0f;
		m_ScrollCbk = null;
		for (int i = 0; i < 4; i++)
		{
			GameContext.Pinfo[i].CharacterIdx = -1;
			GameContext.Pinfo[i].Controller = PlayerController.NONE;
			GameContext.Pinfo[i].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
			GameContext.Pinfo[i].pIndex = (PlayerIndex)i;
			InputManager.SetLockPad(i, block: false);
			m_PlayerInputTimer[i] = 0f;
		}
		m_FirstPlayerIndex = FirstPlayerIndex;
		if (GameContext.IsTrialMode())
		{
			m_nLevelNum = 9;
		}
		base.Initialize();
	}

	public override void LoadContent()
	{
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		base.ScreenManager.Game.ResetElapsedTime();
		InitGameAsset();
		LoadBordel();
	}

	public void LoadBordel()
	{
		switch (loadState)
		{
		case LoadState.LOAD_INIT:
			loadState = LoadState.LOADING_HUD;
			m_LightMgr = new LightManager(base.ScreenManager, content);
			m_Renderer = new MercurySpriteBatchRenderer();
			m_Renderer.GraphicsDeviceService = JamSoulGame.graphics;
			m_Renderer.LoadContent(content);
			break;
		case LoadState.LOADING_HUD:
			InitHud(initPlayerButton: true);
			loadState = LoadState.LOADING_LEVEL;
			break;
		case LoadState.LOADING_LEVEL:
		{
			m_Level = new Level(this, "MultiplayerMenu", bGameLevel: false);
			Dummy.Add(m_Level.GetDummyByName("CharacterScreen"));
			Dummy.Add(m_Level.GetDummyByName("GameModeScreen"));
			Dummy.Add(m_Level.GetDummyByName("LevelScreen"));
			Dummy.Add(m_Level.GetDummyByName("GameOptionScreen"));
			for (int k = 0; k < Dummy.Count; k++)
			{
				Level.DummyPoint value = Dummy[k];
				if (k > 0)
				{
					value.Position.Y += 50f;
				}
				else
				{
					value.Position.X += 50f;
				}
				Dummy[k] = value;
			}
			m_OptionStart = m_Level.GetDummyByName("Option_Start").Position;
			m_LevelNamePos = m_Level.GetDummyByName("LvlName").Position;
			m_ReferenceBackground = (BackgroundLayer)GetEntitieByName("BKG");
			m_FinishScroll = Vector2.Zero;
			m_CurrentScroll = Vector2.Zero;
			m_CurrentScreen = 0;
			m_bScroll = false;
			m_LevelIcon = (BackgroundLayer)GetEntitieByName("Miroir_Level");
			m_Cross = LoadSprite("ICO_Pad", GameAtlas.GAME);
			m_Back = LoadSprite("bt_Back", GameAtlas.GAME);
			m_BackgroundTuto = LoadSprite("MM_Ombre", GameAtlas.GAME);
			m_LockIcon = LoadSprite("TacheDemo", GameAtlas.GAME);
			m_LockIconPerso = LoadSprite("TacheDemo2", GameAtlas.GAME);
			loadState = LoadState.LOADING_SOUND;
			break;
		}
		case LoadState.LOADING_SOUND:
			m_BackgroundMusic = content.Load<Song>("Sound/Bgm/Introduction");
			m_SelectSound = new AudioClip("Menu_Select");
			m_ValidSound = new AudioClip("Menu_Valid");
			m_BackSound = new AudioClip("Menu_Back");
			m_StarTSound = new AudioClip("Menu_PlayerStart");
			loadState = LoadState.INIT_BUTTON;
			break;
		case LoadState.INIT_BUTTON:
		{
			for (int l = 0; l < m_ButtonLayer.Count; l++)
			{
				LoadBtTexture(m_ButtonLayer[l], l);
			}
			loadState = LoadState.INIT_BORDEL;
			break;
		}
		case LoadState.INIT_BORDEL:
		{
			InitButton();
			m_nRmapBtIdx = FindButtonIdx("RB");
			m_nLmapBtIdx = FindButtonIdx("LB");
			m_PlayerArrow = LoadSprite("PlayerArrow", GameAtlas.GAME);
			for (int i = 0; i < InputManager.GamerIndex.Length; i++)
			{
				m_Gamer[i].SlotIdx = -1;
			}
			SignedInGamer.SignedIn += OnSignIn;
			SignedInGamer.SignedOut += OnSignOut;
			SwitchScreen(0);
			MediaPlayer.Play(m_BackgroundMusic);
			for (int j = 0; j < InputManager.GamerIndex.Length; j++)
			{
				ref Vector2 reference = ref m_AvatarPos[j];
				reference = m_Level.GetDummyByName("Holder" + (j + 1)).Position;
				m_Level.GetDummyByName("Holder" + (j + 1));
			}
			loadState = LoadState.LOADING_FINISHED;
			break;
		}
		}
		if (loadState != LoadState.LOADING_FINISHED)
		{
			LoadBordel();
		}
	}

	public void OnSignIn(object sender, SignedInEventArgs e)
	{
		int gamerIdxFromPlayerIndex = GetGamerIdxFromPlayerIndex(e.Gamer.PlayerIndex);
		m_Gamer[gamerIdxFromPlayerIndex].Name = e.Gamer.Gamertag;
		if (e.Gamer.Gamertag.Length > 8)
		{
			m_Gamer[gamerIdxFromPlayerIndex].Name = m_Gamer[gamerIdxFromPlayerIndex].Name.Substring(0, 8);
		}
		if (e.Gamer.PlayerIndex != m_FirstPlayerIndex)
		{
			m_Gamer[gamerIdxFromPlayerIndex].Pms = PlayerMenuState.CHARACTER_JOIN;
		}
		else
		{
			if (gamerIdxFromPlayerIndex != -1)
			{
				m_ButtonArray[m_DollIdx[gamerIdxFromPlayerIndex]].SetFocus(e.Gamer.PlayerIndex);
				ref Vector2 reference = ref m_ArrowPos[gamerIdxFromPlayerIndex];
				reference = new Vector2(m_ButtonArray[m_DollIdx[gamerIdxFromPlayerIndex]].GetMiddle().X - (float)(m_PlayerArrow.Width / 2), 370f);
				ref Vector2 reference2 = ref m_PlayerPos[gamerIdxFromPlayerIndex];
				reference2 = new Vector2(m_ButtonArray[m_DollIdx[gamerIdxFromPlayerIndex]].GetMiddle().X, 350f);
				ref Color reference3 = ref m_PlayerColor[gamerIdxFromPlayerIndex];
				reference3 = PlayerConfig.CHARACTER_COLOR[m_ButtonArray[m_DollIdx[gamerIdxFromPlayerIndex]].m_UserData];
				m_PlayerCharName[gamerIdxFromPlayerIndex] = TextManager.GetText((TextID)(48 + gamerIdxFromPlayerIndex));
				m_Gamer[gamerIdxFromPlayerIndex].dollIdx = m_DollIdx[gamerIdxFromPlayerIndex];
			}
			m_Gamer[gamerIdxFromPlayerIndex].Pms = PlayerMenuState.CHARACTER_SELECT;
		}
		m_Gamer[gamerIdxFromPlayerIndex].SlotIdx = -1;
	}

	public void OnSignOut(object sender, SignedOutEventArgs e)
	{
		SignedInGamer_SignedOut(sender, e);
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			InputManager.GamerIndex[i] = -1;
		}
		LoadingScreen.Load(base.ScreenManager, false, PlayerIndex.One, new LogoScreen());
	}

	private void LoadBtTexture(BackgroundLayer BgLayer, int id)
	{
		int num = int.Parse(BgLayer.Name[0].ToString());
		string text = BgLayer.Name.Split('-')[1];
		int id2 = LoadSprite(text, GameAtlas.MAIN_MENU).id;
		int id3 = LoadSprite(text + "_UP", GameAtlas.MAIN_MENU).id;
		int downTex = id3;
		if (num >= 1)
		{
			downTex = LoadSprite(text + "_DN", GameAtlas.MAIN_MENU).id;
		}
		m_ButtonArray.Add(new ButtonIcon(BgLayer, id2, id3, downTex, id, OnEvent, Getatlas(2)));
		m_ButtonArray[m_ButtonArray.Count - 1].m_RealName = text;
		if (num >= 2)
		{
			m_ButtonArray[m_ButtonArray.Count - 1].m_ColoredSelection = LoadSprite(text + "_white", GameAtlas.MAIN_MENU).id;
		}
		BgLayer.SetVisible(bVisible: false);
	}

	public override void AddLayer(string path, int x, int y, string name, SpriteEffects spe, float zOrder, Color color)
	{
		base.AddLayer(path, x, y, name, spe, zOrder, color);
		if (name.Contains("bt"))
		{
			m_ButtonLayer.Add((BackgroundLayer)m_Entities[m_Entities.Count - 1]);
		}
		else if (name.Contains("Miroir"))
		{
			m_Miroir = (BackgroundLayer)m_Entities[m_Entities.Count - 1];
		}
	}

	private void InitButton()
	{
		m_FootBtId = FindButtonIdx("ICO_Foot");
		m_DollIdx.Add(FindButtonIdx("Passion"));
		m_DollIdx.Add(FindButtonIdx("Maladie"));
		m_DollIdx.Add(FindButtonIdx("Vice"));
		m_DollIdx.Add(FindButtonIdx("Folie"));
		m_DollIdx.Add(FindButtonIdx("Misere"));
		m_DollIdx.Add(FindButtonIdx("Famine"));
		m_DollIdx.Add(FindButtonIdx("Mort"));
		m_DollIdx.Add(FindButtonIdx("Guerre"));
		m_DollIdx.Add(FindButtonIdx("Tromperie"));
		m_DollIdx.Add(FindButtonIdx("Esperance"));
		m_ModeIdx.Add(FindButtonIdx("ICO_deathmatch"));
		m_ModeIdx.Add(FindButtonIdx("ICO_flag"));
		m_ModeIdx.Add(FindButtonIdx("ICO_Foot"));
		for (int i = 0; i < m_DollIdx.Count; i++)
		{
			m_ButtonArray[m_DollIdx[i]].m_UserData = i;
			m_ButtonArray[m_DollIdx[i]].m_bSelectable = false;
			if (GameContext.IsTrialMode())
			{
				if (GameContext.LOCKED_CHAR[i])
				{
					m_ButtonArray[m_DollIdx[i]].m_bSelectable = true;
				}
				else
				{
					m_ButtonArray[m_DollIdx[i]].SetLocked(m_LockIconPerso);
				}
			}
			else
			{
				m_ButtonArray[m_DollIdx[i]].m_bSelectable = true;
			}
			m_SelectedDoll.Add(m_DollIdx[i]);
		}
		m_ButtonArray[m_DollIdx[0]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[1]]);
		m_ButtonArray[m_DollIdx[0]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[8]]);
		m_ButtonArray[m_DollIdx[1]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[2]]);
		m_ButtonArray[m_DollIdx[1]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[0]]);
		m_ButtonArray[m_DollIdx[2]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[3]]);
		m_ButtonArray[m_DollIdx[2]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[1]]);
		m_ButtonArray[m_DollIdx[3]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[4]]);
		m_ButtonArray[m_DollIdx[3]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[2]]);
		m_ButtonArray[m_DollIdx[4]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[5]]);
		m_ButtonArray[m_DollIdx[4]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[3]]);
		m_ButtonArray[m_DollIdx[5]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[6]]);
		m_ButtonArray[m_DollIdx[5]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[4]]);
		m_ButtonArray[m_DollIdx[6]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[7]]);
		m_ButtonArray[m_DollIdx[6]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[5]]);
		m_ButtonArray[m_DollIdx[7]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[8]]);
		m_ButtonArray[m_DollIdx[7]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[6]]);
		m_ButtonArray[m_DollIdx[8]].SetNeightBour(ButtonIcon.NEIGHTBOUR.RIGHT, m_ButtonArray[m_DollIdx[0]]);
		m_ButtonArray[m_DollIdx[8]].SetNeightBour(ButtonIcon.NEIGHTBOUR.LEFT, m_ButtonArray[m_DollIdx[7]]);
	}

	public override void UnloadContent()
	{
		MediaPlayer.Stop();
		m_LightMgr.Clear();
		content.Unload();
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (loadState != LoadState.LOADING_FINISHED)
		{
			return;
		}
		float elapsed = gameTime.ElapsedGameTime.Milliseconds;
		if (!m_bScreenFading)
		{
			if (m_bScroll)
			{
				UpdateScroll(elapsed);
			}
			else
			{
				bool flag = false;
				for (int i = 0; i < m_ButtonArray.Count; i++)
				{
					int playerControllerIdx = m_ButtonArray[i].m_PlayerControllerIdx;
					if (playerControllerIdx != -1 && m_PlayerInputTimer[playerControllerIdx] > InputManager.INPUT_LATENCY && m_ButtonArray[i].ManageInput(gameTime))
					{
						m_PlayerInputTimer[playerControllerIdx] = 0f;
					}
				}
				for (int j = 0; j < m_Gamer.Length; j++)
				{
					m_PlayerInputTimer[j] += gameTime.ElapsedGameTime.Milliseconds;
				}
				if (base.TransitionPosition == 0f)
				{
					ManageScreen();
				}
			}
			UpdateEntities(gameTime);
		}
		UpdateHud(gameTime);
		if (m_pStartTimer > 1f)
		{
			m_pStartColor.A = (byte)MathHelper.Lerp(50f, 255f, m_pStartTimer - 1f);
		}
		else
		{
			m_pStartColor.A = (byte)MathHelper.Lerp(255f, 50f, m_pStartTimer);
		}
		m_pStartTimer -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;
		if (m_pStartTimer <= 0f)
		{
			m_pStartTimer = 2f;
		}
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void HandleInput()
	{
		base.HandleInput();
	}

	public override void Draw(GameTime gameTime)
	{
		if (loadState == LoadState.LOADING_FINISHED)
		{
			m_LightMgr.BuildLightMap();
			base.ScreenManager.GraphicsDevice.Clear(Color.Black);
			base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
			DrawEntities();
			for (int i = 0; i < m_ButtonArray.Count; i++)
			{
				m_ButtonArray[i].Draw();
			}
			base.ScreenManager.SpriteBatch.End();
			m_LightMgr.DrawLightMap();
			PostDraw();
			if (base.TransitionPosition > 0f)
			{
				base.ScreenManager.FadeBackBufferToBlack(255 - base.TransitionAlpha);
			}
		}
	}

	public override void PostDraw()
	{
		base.PostDraw();
		base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
		GameState.m_GameAtlas[1].Draw(m_nLevelNum, m_LevelIcon.GetPosition(), m_LevelIcon.GetSpriteEffect(), 0.99f);
		if (GameContext.IsTrialMode() && GameContext.LOCKED_LEVEL[m_nLevelNum])
		{
			Vector2 position = m_LevelIcon.GetPosition();
			position.X += m_LevelIcon.Width / 2;
			position.Y += m_LevelIcon.Height / 2;
			position.X -= m_LockIcon.Width / 2;
			position.Y -= m_LockIcon.Height / 2;
			m_LockIcon.Draw(position, Color.White);
		}
		m_Miroir.Draw();
		if (!m_bScroll)
		{
			switch (m_CurrentScreen)
			{
			case 0:
			{
				for (int i = 0; i < 4; i++)
				{
					if (InputManager.GamerIndex[i] != -1 && m_Gamer[i].Pms != PlayerMenuState.CHARACTER_JOIN)
					{
						m_PlayerArrow.Draw(m_ArrowPos[i], m_HudTextColor, SpriteEffects.None, 1f);
						base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoomMiddle, m_Gamer[i].Name, Color.Black, m_HudTextColor, 2f, 1f, 0f, m_PlayerPos[i]);
					}
				}
				break;
			}
			case 3:
			{
				int num = 0;
				Vector2 optionStart = m_OptionStart;
				Color white = Color.White;
				optionStart.Y -= 55f;
				foreach (Option option in m_OptionList)
				{
					optionStart.Y += 55f;
					optionStart.X = m_OptionStart.X;
					white = ((num != m_CurrentOption) ? ((!option.bLocked) ? Color.White : Color.Gray) : Color.Green);
					base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, option.Title, optionStart, m_TitleColor);
					optionStart.X += 100f;
					base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoom, option.GetCaption(), optionStart, white);
					num++;
				}
				break;
			}
			}
		}
		for (int j = 0; j < InputManager.GamerIndex.Length; j++)
		{
			if (InputManager.GamerIndex[j] == -1)
			{
				continue;
			}
			string text = m_Gamer[j].Name;
			if (text.Length > 16)
			{
				text = m_Gamer[j].Name.Substring(0, 16);
			}
			base.ScreenManager.DrawTextOutline(base.ScreenManager.GoBoom, text, Color.Black, Color.White, 1f, 1f, 0f, new Vector2(m_AvatarPos[j].X, m_AvatarPos[j].Y + 20f));
			if (m_Gamer[j].Pms == PlayerMenuState.CHARACTER_JOIN)
			{
				if (m_CurrentScreen == 0)
				{
					Vector2 vector = base.ScreenManager.GoBoomMiddle.MeasureString(TextManager.GetText(TextID.PRESS_START_MENU));
					Vector2 position2 = new Vector2(m_AvatarPos[j].X - vector.X / 2f, m_AvatarPos[j].Y + vector.Y / 2f + 40f);
					base.ScreenManager.SpriteBatch.DrawString(base.ScreenManager.GoBoomMiddle, TextManager.GetText(TextID.PRESS_START_MENU), position2, m_pStartColor);
				}
			}
			else
			{
				Vector2 position3 = new Vector2(m_AvatarPos[j].X, m_AvatarPos[j].Y + 100f);
				base.ScreenManager.DrawText(base.ScreenManager.GoBoomSmall, ref position3, m_PlayerCharName[j], ScreenManager.TextOrigin.center_center, Color.White);
			}
		}
		m_BackgroundTuto.Draw(BKG_POS, Color.White, SpriteEffects.None, 1f);
		Vector2 Position = BTA_POS;
		Position.X -= m_btSpriteSoft[0].GetFrameWidth();
		Position.Y -= m_btSpriteSoft[0].GetFrameHeight();
		BTB_POS.X = BTA_POS.X + base.ScreenManager.GoBoomMiddle.MeasureString(TextManager.GetText(TextID.VALID)).X + (float)m_btSpriteSoft[1].GetFrameWidth() + 20f;
		Vector2 Position2 = BTB_POS;
		Position2.X -= m_btSpriteSoft[1].GetFrameWidth();
		Position2.Y -= m_btSpriteSoft[1].GetFrameHeight();
		CROSS_POS.X = BTB_POS.X + base.ScreenManager.GoBoomMiddle.MeasureString(TextManager.GetText(TextID.UNDO)).X + (float)m_btSpriteSoft[1].GetFrameWidth() + 40f;
		Vector2 cROSS_POS = CROSS_POS;
		cROSS_POS.X -= m_Cross.Width;
		cROSS_POS.Y -= (float)m_Cross.Height / 1.2f;
		BACK_POS.X = CROSS_POS.X + base.ScreenManager.GoBoomMiddle.MeasureString(TextManager.GetText(TextID.MOVE)).X + (float)m_btSpriteSoft[1].GetFrameWidth() + 20f;
		Vector2 bACK_POS = BACK_POS;
		bACK_POS.X -= m_Back.Width + 10;
		bACK_POS.Y -= (float)m_Back.Height * 1.5f;
		m_btSpriteSoft[0].Draw(ref Position, SpriteEffects.None, Color.White, 0.7f, 1f);
		m_btSpriteSoft[1].Draw(ref Position2, SpriteEffects.None, Color.White, 0.7f, 1f);
		m_Cross.Draw(cROSS_POS, Color.White, SpriteEffects.None, 1f);
		m_Back.Draw(bACK_POS, Color.White, SpriteEffects.None, 1f);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref BTA_POS, TextManager.GetText(TextID.VALID), ScreenManager.TextOrigin.bottom_left, Color.Green);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref BTB_POS, TextManager.GetText(TextID.UNDO), ScreenManager.TextOrigin.bottom_left, Color.Red);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref CROSS_POS, TextManager.GetText(TextID.MOVE), ScreenManager.TextOrigin.bottom_left, Color.White);
		base.ScreenManager.DrawText(base.ScreenManager.GoBoomMiddle, ref BACK_POS, TextManager.GetText(TextID.BACK), ScreenManager.TextOrigin.bottom_left, Color.White);
		base.ScreenManager.SpriteBatch.End();
	}

	public bool ManageScreen()
	{
		int num = 0;
		switch (m_CurrentScreen)
		{
		case 0:
		{
			bool flag2 = true;
			for (int n = 0; n < 4; n++)
			{
				if (InputManager.GamerIndex[n] == -1)
				{
					continue;
				}
				if (m_Gamer[n].Pms == PlayerMenuState.CHARACTER_JOIN && InputManager.GetKeyState((PlayerIndex)InputManager.GamerIndex[n], 8) == ButtonState.Pressed)
				{
					int gamerIdxFromPlayerIndex = GetGamerIdxFromPlayerIndex((PlayerIndex)InputManager.GamerIndex[n]);
					if (gamerIdxFromPlayerIndex != -1)
					{
						int num4 = 0;
						for (int num5 = 0; num5 < m_DollIdx.Count - 1; num5++)
						{
							if (m_ButtonArray[m_DollIdx[num5]].GetState() != ButtonIcon.STATE.PUSHED && m_ButtonArray[m_DollIdx[num5]].GetState() != ButtonIcon.STATE.OVER)
							{
								num4 = num5;
								m_ButtonArray[m_DollIdx[num4]].SetFocus((PlayerIndex)InputManager.GamerIndex[n]);
								ref Vector2 reference = ref m_ArrowPos[gamerIdxFromPlayerIndex];
								reference = new Vector2(m_ButtonArray[m_DollIdx[num4]].GetMiddle().X - (float)(m_PlayerArrow.Width / 2), 370f);
								ref Vector2 reference2 = ref m_PlayerPos[gamerIdxFromPlayerIndex];
								reference2 = new Vector2(m_ButtonArray[m_DollIdx[num4]].GetMiddle().X, 350f);
								ref Color reference3 = ref m_PlayerColor[gamerIdxFromPlayerIndex];
								reference3 = PlayerConfig.CHARACTER_COLOR[m_ButtonArray[m_DollIdx[num4]].m_UserData];
								m_PlayerCharName[gamerIdxFromPlayerIndex] = TextManager.GetText((TextID)(48 + num4));
								m_Gamer[gamerIdxFromPlayerIndex].dollIdx = num4;
								break;
							}
						}
					}
					m_StarTSound.Play();
					m_Gamer[n].Pms = PlayerMenuState.CHARACTER_SELECT;
				}
				if (InputManager.GetKeyState((PlayerIndex)InputManager.GamerIndex[n], 9) == ButtonState.Pressed)
				{
					m_bScreenFading = true;
					LoadingScreen.Load(base.ScreenManager, false, PlayerIndex.One, new LogoScreen());
				}
				if (m_Gamer[n].Pms == PlayerMenuState.CHARACTER_SELECT)
				{
					flag2 = false;
				}
			}
			if (!flag2)
			{
				break;
			}
			bool flag3 = true;
			for (int num6 = 0; num6 < m_DollIdx.Count; num6++)
			{
				if (m_ButtonArray[m_DollIdx[num6]].m_FlashTimer > 0f)
				{
					flag3 = false;
				}
			}
			if (!flag3)
			{
				break;
			}
			for (int num7 = 0; num7 < m_DollIdx.Count; num7++)
			{
				if (m_ButtonArray[m_DollIdx[num7]].IsFocused())
				{
					m_ButtonArray[m_DollIdx[num7]].Unfocus();
					m_ButtonArray[m_DollIdx[num7]].SetState(ButtonIcon.STATE.PUSHED);
				}
			}
			int num8 = 0;
			for (int num9 = 0; num9 < m_Gamer.Length; num9++)
			{
				if (GameContext.Pinfo[num9].Controller != PlayerController.NONE)
				{
					num8++;
				}
			}
			if (num8 <= 1)
			{
				m_ButtonArray[m_FootBtId].m_UsedColor = Color.Black;
			}
			for (int num10 = 0; num10 < m_DollIdx.Count; num10++)
			{
				m_ButtonArray[m_DollIdx[num10]].m_bDrawLock = false;
			}
			SwitchScreen(1);
			break;
		}
		case 1:
		{
			m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.PUSHED);
			int num12 = 0;
			for (int num13 = 0; num13 < m_Gamer.Length; num13++)
			{
				if (GameContext.Pinfo[num13].Controller != PlayerController.NONE)
				{
					num12++;
				}
			}
			for (int num14 = 0; num14 < InputManager.GamerIndex.Length; num14++)
			{
				PlayerIndex playerIndex = (PlayerIndex)InputManager.GamerIndex[num14];
				if (playerIndex == (PlayerIndex)(-1))
				{
					continue;
				}
				num = GetGamerIdxFromPlayerIndex((PlayerIndex)InputManager.GamerIndex[num14]);
				if (InputManager.GamerIndex[num14] == -1 || !(m_PlayerInputTimer[num14] > InputManager.INPUT_LATENCY) || m_Gamer[num].Pms == PlayerMenuState.CHARACTER_JOIN)
				{
					continue;
				}
				playerIndex = (PlayerIndex)InputManager.GamerIndex[num14];
				if (InputManager.GetKeyState(playerIndex, 0) == ButtonState.Pressed)
				{
					m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.NORMAL);
					if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
					{
						if (num12 <= 1)
						{
							GameContext.GameMode = GAME_MODE.CAPTURE_THE_JAM;
						}
						else
						{
							GameContext.GameMode = GAME_MODE.JAM_BALL;
						}
					}
					else if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
					{
						GameContext.GameMode = GAME_MODE.DEATHMATCH;
					}
					else if (GameContext.GameMode == GAME_MODE.JAM_BALL)
					{
						GameContext.GameMode = GAME_MODE.CAPTURE_THE_JAM;
					}
					m_PlayerInputTimer[num14] = 0f;
					m_SelectSound.Play();
				}
				if (InputManager.GetKeyState(playerIndex, 2) == ButtonState.Pressed)
				{
					m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.NORMAL);
					if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM)
					{
						if (num12 <= 1)
						{
							GameContext.GameMode = GAME_MODE.DEATHMATCH;
						}
						else
						{
							GameContext.GameMode = GAME_MODE.JAM_BALL;
						}
					}
					else if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
					{
						GameContext.GameMode = GAME_MODE.CAPTURE_THE_JAM;
					}
					else if (GameContext.GameMode == GAME_MODE.JAM_BALL)
					{
						GameContext.GameMode = GAME_MODE.DEATHMATCH;
					}
					m_PlayerInputTimer[num14] = 0f;
					m_SelectSound.Play();
				}
				if (InputManager.GetKeyState(playerIndex, 4) == ButtonState.Pressed)
				{
					m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.OVER);
					m_PlayerInputTimer[num14] = 0f;
					m_ValidSound.Play();
					CheckLevelAvailableInGameMode(1);
					m_FirstPlayerIndex = playerIndex;
					SwitchScreen(2);
				}
				if (InputManager.GetKeyState(playerIndex, 5) != ButtonState.Pressed)
				{
					continue;
				}
				m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.NORMAL);
				m_SelectedDoll.Clear();
				for (int num15 = 0; num15 < m_DollIdx.Count; num15++)
				{
					m_SelectedDoll.Add(m_DollIdx[num15]);
					m_ButtonArray[m_DollIdx[num15]].Unfocus();
				}
				for (int num16 = 0; num16 < GameContext.Pinfo.Length; num16++)
				{
					if (InputManager.GamerIndex[num16] != -1 && m_Gamer[num16].Pms != PlayerMenuState.CHARACTER_JOIN)
					{
						GameContext.Pinfo[num16].CharacterIdx = -1;
						m_Gamer[num16].Pms = PlayerMenuState.CHARACTER_SELECT;
						m_Gamer[num16].SlotIdx = -1;
						m_ButtonArray[m_DollIdx[num16]].SetFocus(GameContext.Pinfo[num16].pIndex);
						ref Vector2 reference4 = ref m_ArrowPos[num16];
						reference4 = new Vector2(m_ButtonArray[m_DollIdx[num16]].GetMiddle().X - (float)(m_PlayerArrow.Width / 2), 370f);
						ref Vector2 reference5 = ref m_PlayerPos[num16];
						reference5 = new Vector2(m_ButtonArray[m_DollIdx[num16]].GetMiddle().X, 350f);
						ref Color reference6 = ref m_PlayerColor[num16];
						reference6 = PlayerConfig.CHARACTER_COLOR[m_ButtonArray[m_DollIdx[num16]].m_UserData];
						m_PlayerCharName[num16] = TextManager.GetText((TextID)(48 + num16));
					}
				}
				m_BackSound.Play();
				m_PlayerInputTimer[num14] = 0f;
				SwitchScreen(0);
			}
			break;
		}
		case 2:
		{
			PlayerIndex playerIndex = PlayerIndex.One;
			if (m_bScroll)
			{
				break;
			}
			for (int num11 = 0; num11 < InputManager.GamerIndex.Length; num11++)
			{
				playerIndex = (PlayerIndex)InputManager.GamerIndex[num11];
				if (playerIndex != m_FirstPlayerIndex)
				{
					continue;
				}
				num = GetGamerIdxFromPlayerIndex(playerIndex);
				if (InputManager.GamerIndex[num11] != -1 && m_PlayerInputTimer[num11] > InputManager.INPUT_LATENCY && m_Gamer[num].Pms != PlayerMenuState.CHARACTER_JOIN)
				{
					playerIndex = (PlayerIndex)InputManager.GamerIndex[num11];
					if (InputManager.GetKeyState(playerIndex, 4) == ButtonState.Pressed && (!GameContext.LOCKED_LEVEL[m_nLevelNum] || !GameContext.IsTrialMode()))
					{
						m_ValidSound.Play();
						SwitchScreen(3);
						m_PlayerInputTimer[num11] = 0f;
					}
					if (InputManager.GetKeyState(playerIndex, 5) == ButtonState.Pressed)
					{
						SwitchScreen(1);
						m_PlayerInputTimer[num11] = 0f;
						m_ButtonArray[m_ModeIdx[(int)(GameContext.GameMode - 1)]].SetState(ButtonIcon.STATE.NORMAL);
						m_BackSound.Play();
					}
				}
				if (InputManager.GetKeyState(playerIndex, 1) == ButtonState.Pressed)
				{
					if (m_ButtonArray[m_nLmapBtIdx].GetState() == ButtonIcon.STATE.NORMAL)
					{
						if (m_nLevelNum > 0)
						{
							m_nLevelNum--;
						}
						else
						{
							m_nLevelNum = GameContext.SELECTABLE_LEVEL.Length - 1;
						}
						CheckLevelAvailableInGameMode(-1);
						m_SelectSound.Play();
						m_ButtonArray[m_nLmapBtIdx].SetState(ButtonIcon.STATE.PUSHED);
					}
					m_PlayerInputTimer[num11] = 0f;
					return true;
				}
				m_ButtonArray[m_nLmapBtIdx].SetState(ButtonIcon.STATE.NORMAL);
				if (InputManager.GetKeyState(playerIndex, 3) == ButtonState.Pressed)
				{
					if (m_ButtonArray[m_nRmapBtIdx].GetState() == ButtonIcon.STATE.NORMAL)
					{
						if (m_nLevelNum + 1 < GameContext.SELECTABLE_LEVEL.Length)
						{
							m_nLevelNum++;
						}
						else
						{
							m_nLevelNum = 0;
						}
						CheckLevelAvailableInGameMode(1);
						m_SelectSound.Play();
						m_ButtonArray[m_nRmapBtIdx].SetState(ButtonIcon.STATE.PUSHED);
					}
					m_PlayerInputTimer[num11] = 0f;
					return true;
				}
				m_ButtonArray[m_nRmapBtIdx].SetState(ButtonIcon.STATE.NORMAL);
			}
			m_ButtonArray[m_nLmapBtIdx].Unfocus();
			m_ButtonArray[m_nRmapBtIdx].Unfocus();
			break;
		}
		case 3:
		{
			if (GameContext.GameMode == GAME_MODE.JAM_BALL)
			{
				m_OptionList[2].SetLimit(0);
				m_OptionList[2].bLocked = true;
				m_OptionList[3].bLocked = true;
			}
			else
			{
				int num2 = 0;
				for (int i = 0; i < GameContext.Pinfo.Length; i++)
				{
					if (GameContext.Pinfo[i].Controller == PlayerController.PLAYER)
					{
						num2++;
					}
				}
				if (num2 <= 1)
				{
					m_OptionList[2].Minimum = 1;
				}
				m_OptionList[2].bLocked = false;
			}
			for (int j = 0; j < InputManager.GamerIndex.Length; j++)
			{
				PlayerIndex playerIndex = (PlayerIndex)InputManager.GamerIndex[j];
				if (playerIndex != m_FirstPlayerIndex)
				{
					continue;
				}
				num = GetGamerIdxFromPlayerIndex(playerIndex);
				if (InputManager.GamerIndex[j] == -1 || !(m_PlayerInputTimer[j] > InputManager.INPUT_LATENCY) || m_Gamer[num].Pms == PlayerMenuState.CHARACTER_JOIN)
				{
					continue;
				}
				if (InputManager.GetKeyState(playerIndex, 5) == ButtonState.Pressed)
				{
					SwitchScreen(2);
					m_PlayerInputTimer[j] = 0f;
					m_BackSound.Play();
				}
				if (InputManager.GetKeyState(playerIndex, 3) == ButtonState.Pressed)
				{
					m_OptionList[m_CurrentOption].IncrementeValue();
					m_PlayerInputTimer[j] = 0f;
					m_SelectSound.Play();
				}
				if (InputManager.GetKeyState(playerIndex, 1) == ButtonState.Pressed)
				{
					m_OptionList[m_CurrentOption].DecrementValue();
					m_PlayerInputTimer[j] = 0f;
					m_SelectSound.Play();
				}
				if (InputManager.GetKeyState(playerIndex, 0) == ButtonState.Pressed)
				{
					m_CurrentOption--;
					if (m_CurrentOption < 0)
					{
						m_CurrentOption = m_OptionList.Count - 1;
					}
					m_SelectSound.Play();
					m_PlayerInputTimer[j] = 0f;
				}
				if (InputManager.GetKeyState(playerIndex, 2) == ButtonState.Pressed)
				{
					m_CurrentOption++;
					if (m_CurrentOption >= m_OptionList.Count)
					{
						m_CurrentOption = 0;
					}
					m_SelectSound.Play();
					m_PlayerInputTimer[j] = 0f;
				}
				if (InputManager.GetKeyState(playerIndex, 4) != ButtonState.Pressed)
				{
					continue;
				}
				GameContext.SelectedLevel = GameContext.SELECTABLE_LEVEL[m_nLevelNum];
				GameContext.PointLimit = m_OptionList[0].GetValue();
				GameContext.TimeLimit = m_OptionList[1].GetValue();
				GameContext.BotNumber = m_OptionList[2].GetValue();
				GameContext.DifficultyLevel = m_OptionList[3].GetValue();
				m_ValidSound.Play();
				if (GameContext.BotNumber > 0)
				{
					for (int k = 0; k < GameContext.BotNumber; k++)
					{
						bool flag = false;
						for (int l = 0; l < m_Gamer.Length; l++)
						{
							int num3 = l;
							if (GameContext.Pinfo[num3].Controller != PlayerController.NONE || flag)
							{
								continue;
							}
							for (int m = 0; m < m_DollIdx.Count; m++)
							{
								if (m_ButtonArray[m_DollIdx[m]].GetState() != ButtonIcon.STATE.PUSHED && m_ButtonArray[m_DollIdx[m]].m_bSelectable)
								{
									m_ButtonArray[m_DollIdx[m]].SetState(ButtonIcon.STATE.PUSHED);
									GameContext.Pinfo[num3].CharacterIdx = m;
									break;
								}
							}
							int index = m_Randomizer.Next(0, m_SelectedDoll.Count - 1);
							m_ButtonArray[m_SelectedDoll[index]].SetState(ButtonIcon.STATE.PUSHED);
							GameContext.Pinfo[num3].CharacterIdx = m_ButtonArray[m_SelectedDoll[index]].m_UserData;
							m_SelectedDoll.RemoveAt(index);
							if (GameContext.Pinfo[num3].CharacterIdx != -1)
							{
								GameContext.Pinfo[num3].Controller = PlayerController.PLAYER_BOT;
								GameContext.Pinfo[num3].Name = "Bot" + k;
								flag = true;
							}
						}
					}
				}
				switch (GameContext.GameMode)
				{
				case GAME_MODE.DEATHMATCH:
					VersusLoadingScreen.Load(base.ScreenManager, true, PlayerIndex.One, new DeathMatchScreen());
					break;
				case GAME_MODE.CAPTURE_THE_JAM:
					LoadingScreen.Load(base.ScreenManager, true, PlayerIndex.One, new CaptureTheFlag());
					break;
				case GAME_MODE.JAM_BALL:
					LoadingScreen.Load(base.ScreenManager, true, PlayerIndex.One, new JamBall());
					break;
				}
				m_PlayerInputTimer[j] = 0f;
			}
			break;
		}
		}
		return false;
	}

	public void UpdateScroll(float elapsed)
	{
		Vector2 currentScroll = m_CurrentScroll;
		int num = 0;
		m_ScrollTimer += elapsed;
		if (!(m_ScrollTimer > 0.6f))
		{
			return;
		}
		if (m_CurrentScroll.X < m_FinishScroll.X)
		{
			m_CurrentScroll.X += 5f;
		}
		else if (m_CurrentScroll.X > m_FinishScroll.X)
		{
			m_CurrentScroll.X -= 5f;
		}
		if (m_CurrentScroll.Y < m_FinishScroll.Y)
		{
			m_CurrentScroll.Y += 5f;
		}
		else if (m_CurrentScroll.Y > m_FinishScroll.Y)
		{
			m_CurrentScroll.Y -= 5f;
		}
		if (m_CurrentScroll.X >= m_FinishScroll.X - 5f && m_CurrentScroll.X <= m_FinishScroll.X + 5f)
		{
			m_CurrentScroll.X = m_FinishScroll.X;
			num++;
		}
		if (m_CurrentScroll.Y >= m_FinishScroll.Y - 5f && m_CurrentScroll.Y <= m_FinishScroll.Y + 5f)
		{
			m_CurrentScroll.Y = m_FinishScroll.Y;
			num++;
		}
		if (num > 1)
		{
			m_bScroll = false;
			if (m_ScrollCbk != null)
			{
				m_ScrollCbk();
				m_ScrollCbk = null;
			}
		}
		MoveScreen(m_CurrentScroll - currentScroll);
		m_ScrollTimer = 0f;
		if (GameContext.IsTrialMode())
		{
			m_nLevelNum = 9;
		}
	}

	private void MoveScreen(Vector2 MoveVect)
	{
		for (int i = 0; i < m_Entities.Count; i++)
		{
			ScenaricEntitie scenaricEntitie = m_Entities[i];
			if (scenaricEntitie.TypeId == SCENARIC.TYPE_LAYER || scenaricEntitie.TypeId == SCENARIC.TYPE_ANIM || scenaricEntitie.TypeId == SCENARIC.TYPE_PLAYER || scenaricEntitie.TypeId == SCENARIC.TYPE_PARTICLE)
			{
				scenaricEntitie.SetPosition(scenaricEntitie.GetPosition() - MoveVect);
			}
		}
		for (int j = 0; j < m_AvatarPos.Length; j++)
		{
			m_AvatarPos[j] -= MoveVect;
		}
		for (int k = 0; k < m_LightMgr.GetLightCount(); k++)
		{
			m_LightMgr.m_lights[k].Position -= MoveVect;
		}
		m_OptionStart -= MoveVect;
	}

	public void SwitchScreen(int screen)
	{
		if (m_bScroll)
		{
			return;
		}
		m_CurrentScreen = screen;
		if (m_CurrentScreen > 3)
		{
			m_CurrentScreen = 2;
		}
		else if (m_CurrentScreen < 0)
		{
			m_CurrentScreen = 0;
		}
		switch (m_CurrentScreen)
		{
		case 0:
			m_ScrollCbk = (OnScrollOver)Delegate.Combine(m_ScrollCbk, new OnScrollOver(SetDollFocus));
			break;
		case 1:
			m_ScrollCbk = (OnScrollOver)Delegate.Combine(m_ScrollCbk, new OnScrollOver(SetModeFocus));
			if (GameContext.IsTrialMode())
			{
				m_nLevelNum = 9;
			}
			break;
		case 2:
			m_ScrollCbk = (OnScrollOver)Delegate.Combine(m_ScrollCbk, new OnScrollOver(SetZeusFocus));
			break;
		case 3:
			m_ScrollCbk = (OnScrollOver)Delegate.Combine(m_ScrollCbk, new OnScrollOver(SetOptionFocus));
			break;
		}
		m_FinishScroll.X = Dummy[m_CurrentScreen].Position.X - 640f;
		m_FinishScroll.Y = Dummy[m_CurrentScreen].Position.Y - 360f;
		m_bScroll = true;
	}

	public void OnEvent(ButtonIcon bt, ButtonIcon.STATE state, PlayerIndex pIndex, int KeyPressed)
	{
		switch (state)
		{
		case ButtonIcon.STATE.PUSHED:
			switch (KeyPressed)
			{
			case 4:
				switch (bt.GetRealName())
				{
				case "Passion":
				case "Maladie":
				case "Vice":
				case "Folie":
				case "Misere":
				case "Famine":
				case "Mort":
				case "Guerre":
				case "Tromperie":
				case "Esperance":
				{
					int gamerIdxFromPlayerIndex3 = GetGamerIdxFromPlayerIndex(pIndex);
					if (m_Gamer[gamerIdxFromPlayerIndex3].Pms == PlayerMenuState.CHARACTER_JOIN)
					{
						break;
					}
					m_Gamer[gamerIdxFromPlayerIndex3].SlotIdx = GetAvailableSlot();
					if (m_Gamer[gamerIdxFromPlayerIndex3].SlotIdx == -1)
					{
						break;
					}
					int num2 = bt.m_UserData;
					if (num2 != 5 && num2 != 0 && num2 != 2 && num2 != 8 && num2 != 3 && num2 != 6 && num2 != 1 && num2 != 7 && num2 != 9 && num2 != 4)
					{
						bt.Unfocus();
						bt.m_bSelected = false;
						m_Gamer[gamerIdxFromPlayerIndex3].Pms = PlayerMenuState.CHARACTER_SELECT;
						m_Gamer[gamerIdxFromPlayerIndex3].SlotIdx = -1;
						bt.SetFocus(pIndex);
						break;
					}
					if (InputManager.GetKeyState(pIndex, 11) == ButtonState.Pressed && InputManager.GetKeyState(pIndex, 10) == ButtonState.Pressed && !GameContext.IsTrialMode())
					{
						num2 = 9;
					}
					m_ValidSound.Play();
					GameContext.Pinfo[gamerIdxFromPlayerIndex3].CharacterIdx = num2;
					GameContext.Pinfo[gamerIdxFromPlayerIndex3].Controller = PlayerController.PLAYER;
					GameContext.Pinfo[gamerIdxFromPlayerIndex3].pIndex = pIndex;
					GameContext.Pinfo[gamerIdxFromPlayerIndex3].Name = m_Gamer[gamerIdxFromPlayerIndex3].Name;
					m_Gamer[gamerIdxFromPlayerIndex3].Pms = PlayerMenuState.CHARACTER_CONFIRM;
					m_SelectedDoll.Remove(bt.m_nId);
					break;
				}
				}
				break;
			case 5:
				switch (m_CurrentScreen)
				{
				case 0:
				{
					int gamerIdxFromPlayerIndex2 = GetGamerIdxFromPlayerIndex(pIndex);
					if (m_Gamer[gamerIdxFromPlayerIndex2].Pms == PlayerMenuState.CHARACTER_CONFIRM)
					{
						GameContext.Pinfo[gamerIdxFromPlayerIndex2].Name = "";
						GameContext.Pinfo[gamerIdxFromPlayerIndex2].CharacterIdx = -1;
						m_Gamer[gamerIdxFromPlayerIndex2].Pms = PlayerMenuState.CHARACTER_SELECT;
						m_Gamer[gamerIdxFromPlayerIndex2].SlotIdx = -1;
						m_SelectedDoll.Add(bt.m_nId);
					}
					break;
				}
				case 2:
					bt.Unfocus();
					GameContext.SelectedLevel = null;
					GameContext.GameMode = GAME_MODE.NONE;
					SwitchScreen(1);
					break;
				case 1:
					break;
				}
				break;
			}
			break;
		case ButtonIcon.STATE.OVER:
			switch (bt.GetRealName())
			{
			case "Passion":
			case "Maladie":
			case "Vice":
			case "Folie":
			case "Misere":
			case "Famine":
			case "Mort":
			case "Guerre":
			case "Tromperie":
			case "Esperance":
			{
				int gamerIdxFromPlayerIndex = GetGamerIdxFromPlayerIndex(pIndex);
				ref Vector2 reference = ref m_ArrowPos[gamerIdxFromPlayerIndex];
				reference = new Vector2(bt.GetMiddle().X - (float)(m_PlayerArrow.Width / 2), 370f);
				ref Vector2 reference2 = ref m_PlayerPos[gamerIdxFromPlayerIndex];
				reference2 = new Vector2(bt.GetMiddle().X, 350f);
				ref Color reference3 = ref m_PlayerColor[gamerIdxFromPlayerIndex];
				reference3 = PlayerConfig.CHARACTER_COLOR[bt.m_UserData];
				int num = 0;
				for (int i = 0; i < m_DollIdx.Count; i++)
				{
					if (m_ButtonArray[m_DollIdx[i]] == bt)
					{
						num = i;
					}
				}
				m_PlayerCharName[gamerIdxFromPlayerIndex] = TextManager.GetText((TextID)(48 + num));
				break;
			}
			}
			break;
		}
	}

	public bool IsAvailableSlot(int slotNum)
	{
		for (int i = 0; i < m_Gamer.Length; i++)
		{
			if (m_Gamer[i].SlotIdx == slotNum)
			{
				return false;
			}
		}
		return true;
	}

	public int GetGamerIdxFromSlot(int slot)
	{
		for (int i = 0; i < m_Gamer.Length; i++)
		{
			if (m_Gamer[i].SlotIdx == slot)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetAvailableSlot()
	{
		for (int i = 0; i < 4; i++)
		{
			int num = i;
			for (int j = 0; j < m_Gamer.Length; j++)
			{
				if (m_Gamer[j].SlotIdx == i)
				{
					num = -1;
				}
			}
			if (num != -1)
			{
				return num;
			}
		}
		return -1;
	}

	public int FindButtonIdx(string name)
	{
		for (int i = 0; i < m_ButtonArray.Count; i++)
		{
			string realName = m_ButtonArray[i].GetRealName();
			if (realName == name)
			{
				return i;
			}
		}
		return -1;
	}

	public void SetModeFocus()
	{
		GameContext.GameMode = GAME_MODE.DEATHMATCH;
	}

	public void SetDollFocus()
	{
		m_ButtonArray[m_FootBtId].m_UsedColor = Color.White;
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			if (InputManager.GamerIndex[i] != -1 && m_Gamer[i].Pms != PlayerMenuState.CHARACTER_JOIN)
			{
				ref Vector2 reference = ref m_ArrowPos[i];
				reference = new Vector2(m_ButtonArray[m_DollIdx[i]].GetMiddle().X - (float)(m_PlayerArrow.Width / 2), 370f);
				ref Vector2 reference2 = ref m_PlayerPos[i];
				reference2 = new Vector2(m_ButtonArray[m_DollIdx[i]].GetMiddle().X, 350f);
				ref Color reference3 = ref m_PlayerColor[i];
				reference3 = PlayerConfig.CHARACTER_COLOR[m_ButtonArray[m_DollIdx[i]].m_UserData];
				m_PlayerCharName[i] = TextManager.GetText((TextID)(48 + i));
			}
		}
		for (int j = 0; j < m_DollIdx.Count; j++)
		{
			m_ButtonArray[m_DollIdx[j]].m_bDrawLock = true;
		}
	}

	public void SetZeusFocus()
	{
		m_OptionList.Clear();
		switch (GameContext.GameMode)
		{
		case GAME_MODE.DEATHMATCH:
		case GAME_MODE.CAPTURE_THE_JAM:
		case GAME_MODE.JAM_BALL:
			m_OptionList.Add(new Option(Option.OptionType.Score));
			m_OptionList.Add(new Option(Option.OptionType.Time));
			m_OptionList.Add(new Option(Option.OptionType.BotNumber));
			m_OptionList.Add(new Option(Option.OptionType.BotLevel));
			break;
		}
	}

	public void SetOptionFocus()
	{
		m_CurrentOption = 0;
		m_OptionList[2].bLocked = true;
		m_OptionList[3].bLocked = true;
		int num = GameContext.Pinfo.Length - 1;
		for (int i = 0; i < m_Gamer.Length; i++)
		{
			if (m_Gamer[i].SlotIdx == -1)
			{
				m_OptionList[2].bLocked = false;
				m_OptionList[3].bLocked = false;
				num--;
			}
		}
		m_OptionList[2].SetLimit(num);
		if (GameContext.GameMode != GAME_MODE.DEATHMATCH && GameContext.GameMode != GAME_MODE.CAPTURE_THE_JAM)
		{
			return;
		}
		int num2 = 0;
		for (int j = 0; j < GameContext.Pinfo.Length; j++)
		{
			if (GameContext.Pinfo[j].Controller == PlayerController.PLAYER)
			{
				num2++;
			}
		}
		m_OptionList[2].currentValueIdx = 4 - num2;
	}

	public void CheckLevelAvailableInGameMode(int step)
	{
		if (GameContext.GameMode != GAME_MODE.JAM_BALL)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < GameContext.BALL_LEVEL.Length; i++)
		{
			if (GameContext.BALL_LEVEL[i] == GameContext.SELECTABLE_LEVEL[m_nLevelNum])
			{
				flag = true;
			}
		}
		if (!flag)
		{
			m_nLevelNum += step;
			if (m_nLevelNum >= GameContext.SELECTABLE_LEVEL.Length)
			{
				m_nLevelNum = 0;
			}
			if (m_nLevelNum < 0)
			{
				m_nLevelNum = GameContext.SELECTABLE_LEVEL.Length - 1;
			}
			CheckLevelAvailableInGameMode(step);
		}
	}
}
