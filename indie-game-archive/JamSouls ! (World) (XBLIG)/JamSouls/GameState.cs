using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace JamSouls;

public class GameState : GameScreen
{
	public enum GameAtlas
	{
		GAME,
		MENU_ICON,
		MAIN_MENU,
		COUNT
	}

	public const float PAUSE_TIMER = 600f;

	public const float POWERUP_LIFETIME = 20000f;

	public const int MIN_BONUS_SPAWN_TIME = 1000;

	public const int MAX_BONUS_SPAWN_TIME = 2000;

	public const float SPAWNER_TIME = 200f;

	public ContentManager content;

	public bool m_bIsPaused;

	public bool m_bGameOver;

	public float m_PauseTimer;

	protected Vector2 m_TimePos = new Vector2(620f, 60f);

	protected int m_ReadyTextSize;

	public World m_PhysicManager;

	public bool m_bDrawPath;

	public Matrix Projection;

	public Matrix View = Matrix.Identity;

	public MercurySpriteBatchRenderer m_Renderer;

	public List<ScenaricEntitie> m_Entities = new List<ScenaricEntitie>();

	public List<Player> m_Players = new List<Player>();

	public List<MercuryParticle> m_ParticleManager = new List<MercuryParticle>();

	public List<WaveFx> m_FxManager = new List<WaveFx>();

	public List<SpawnPoint> m_SpawnInfo = new List<SpawnPoint>();

	public LightManager m_LightMgr;

	public Level m_Level;

	public Flag m_RedFlag;

	public Flag m_BlueFlag;

	public List<Player> m_Ranking = new List<Player>();

	public Song m_BackgroundMusic;

	public AudioClip m_GrabBonusSfx;

	public AudioClip m_SpawnBonusSfx;

	public AudioClip m_GameEndSfx;

	public AnimatedSprite[] m_btSprite = new AnimatedSprite[GameContext.PAD_BUTTON_HUD.Length];

	public AnimatedSprite[] m_btSpriteSoft = new AnimatedSprite[GameContext.PAD_BUTTON_HUD.Length];

	public Sprite[] m_btTexture = new Sprite[GameContext.PAD_BUTTON_HUD_TEX.Length];

	public Sprite m_PauseTexture;

	public Texture2D m_BackGroundTex;

	public Sprite m_GetReadyTex;

	public Sprite[] m_ResultSprite = new Sprite[4];

	public Sprite[] m_ResultJamSprite = new Sprite[4];

	public Splash m_SplashHandler;

	private AudioClip m_PauseSound;

	protected AnimatedSprite m_BombExplodeAnim;

	protected AnimatedSprite m_SeedExplodeAnim;

	private bool m_IsBombExploding;

	private bool m_IsSeedExploding;

	public List<PowerUp> m_Bonus = new List<PowerUp>();

	public List<Vector2> PowerUpSpawnList = new List<Vector2>();

	public PowerUp m_CurrentBonus;

	public MercuryParticle m_BonusSpawnEffect;

	public MercuryParticle m_BonusOutline;

	public float m_BonusSpawnTime;

	public float m_BonusSpawnFxTime;

	public Random m_Randomizer;

	public BattleMode m_BattleMode;

	public float m_BonusLifeTime;

	public bool m_bSpawnBonus = true;

	public static Atlas[] m_GameAtlas = new Atlas[3];

	public static bool bAtlasInitialised = false;

	public SoulSpawner m_SoulSpawner;

	public bool m_bAllowSoulSpawn;

	public List<Vector2> m_SoulSpawnPoint = new List<Vector2>();

	public override void LoadContent()
	{
		m_Randomizer = new Random();
		for (int i = 0; i < GameContext.SELECTABLE_LEVEL.Length; i++)
		{
			if (GameContext.SelectedLevel == GameContext.SELECTABLE_LEVEL[i])
			{
				GameContext.CurrentMusic = i;
				break;
			}
		}
		if (content == null)
		{
			content = new ContentManager(base.ScreenManager.Game.Services, "Content");
		}
		m_LightMgr = new LightManager(base.ScreenManager, content);
		m_Renderer = new MercurySpriteBatchRenderer();
		m_Renderer.GraphicsDeviceService = JamSoulGame.graphics;
		m_Renderer.LoadContent(content);
		m_PauseSound = new AudioClip("Menu_Pause");
		m_GameEndSfx = new AudioClip("Game_End");
		base.LoadContent();
	}

	public void InitGameAsset()
	{
		if (!bAtlasInitialised)
		{
			JamSoulGame.audioManager = new AudioManager();
			m_GameAtlas[0] = new Atlas("Atlas/GameAtlas", content, base.ScreenManager.SpriteBatch);
			m_GameAtlas[2] = new Atlas("Atlas/MenuAtlas", content, base.ScreenManager.SpriteBatch);
			m_GameAtlas[1] = new Atlas("Atlas/IconAtlas", content, base.ScreenManager.SpriteBatch);
			bAtlasInitialised = true;
		}
	}

	public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
	{
		if (m_PauseTimer > 0f)
		{
			m_PauseTimer -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_PauseTimer <= 0f)
			{
				m_bIsPaused = false;
			}
		}
		for (int i = 0; i < m_FxManager.Count; i++)
		{
			m_FxManager[i].Update(gameTime);
		}
		AudioManager.Update();
		base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
	}

	public override void DrawFx()
	{
		for (int i = 0; i < m_FxManager.Count; i++)
		{
			m_FxManager[i].Draw();
		}
	}

	public override void UnloadContent()
	{
		MediaPlayer.Stop();
		base.UnloadContent();
	}

	public virtual void Initialize()
	{
		m_LightMgr = null;
		m_PhysicManager = null;
		m_Randomizer = new Random();
		SignedInGamer.SignedIn += SignedInGamer_SignedIn;
		SignedInGamer.SignedOut += SignedInGamer_SignedOut;
	}

	public override void PostDraw()
	{
		for (int i = 0; i < m_ParticleManager.Count; i++)
		{
			m_ParticleManager[i].DrawEffect();
		}
	}

	public LightManager GetLightManager()
	{
		return m_LightMgr;
	}

	public void ResumePause()
	{
		m_PauseTimer = 600f;
	}

	public ScenaricEntitie GetEntitieByName(string name)
	{
		for (int i = 0; i < m_Entities.Count; i++)
		{
			if (name == m_Entities[i].Name)
			{
				return m_Entities[i];
			}
		}
		return null;
	}

	public int GetGamerIdxFromPlayerIndex(PlayerIndex pIndex)
	{
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			if (InputManager.GamerIndex[i] == (int)pIndex)
			{
				return i;
			}
		}
		return -1;
	}

	public void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
	{
		for (int i = 0; i < InputManager.GamerIndex.Length; i++)
		{
			if (InputManager.GamerIndex[i] == (int)e.Gamer.PlayerIndex)
			{
				InputManager.GamerIndex[i] = -1;
			}
		}
	}

	public void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
	{
		for (int i = 0; i < InputManager.GamerIndex.Length && InputManager.GamerIndex[i] != (int)e.Gamer.PlayerIndex; i++)
		{
			if (InputManager.GamerIndex[i] == -1)
			{
				InputManager.GamerIndex[(int)e.Gamer.PlayerIndex] = (int)e.Gamer.PlayerIndex;
				break;
			}
		}
	}

	public void HandleCommonInput(Player p)
	{
		if (InputManager.GetKeyState(p.m_PlayerNum, 8) == ButtonState.Pressed && !m_bGameOver)
		{
			m_PauseSound.Play();
			base.ScreenManager.AddScreen(new PauseMenuScreen(this), p.m_PlayerNum);
			m_bIsPaused = true;
		}
	}

	public void UpdateHud(GameTime gameTime)
	{
		AnimatedSprite[] btSprite = m_btSprite;
		foreach (AnimatedSprite animatedSprite in btSprite)
		{
			animatedSprite.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		}
		AnimatedSprite[] btSpriteSoft = m_btSpriteSoft;
		foreach (AnimatedSprite animatedSprite2 in btSpriteSoft)
		{
			animatedSprite2.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		}
	}

	public void UpdateEntities(GameTime gameTime)
	{
		for (int i = 0; i < m_Entities.Count; i++)
		{
			m_Entities[i].Update(gameTime);
		}
		if (!m_bSpawnBonus)
		{
			return;
		}
		if (m_Bonus.Count > 0)
		{
			m_BonusSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_BonusSpawnTime <= 0f && m_Randomizer != null)
			{
				if (m_CurrentBonus == null)
				{
					int index = m_Randomizer.Next(0, m_Bonus.Count);
					PowerUp powerUp = m_Bonus[index];
					if (powerUp.IsAvailable() && PowerUpSpawnList.Count > 0)
					{
						m_BonusLifeTime = 20000f;
						m_CurrentBonus = powerUp;
						m_BonusSpawnFxTime = 200f;
						int index2 = m_Randomizer.Next(0, PowerUpSpawnList.Count - 1);
						powerUp.SpawnBonus(PowerUpSpawnList[index2], GameContext.POWERUP_Z);
						PowerUpSpawnList.RemoveAt(index2);
						m_SpawnBonusSfx.Play();
					}
				}
				m_BonusSpawnTime = m_Randomizer.Next(1000, 2000);
			}
			if (m_CurrentBonus != null)
			{
				m_CurrentBonus.Update(gameTime);
				if (m_BonusSpawnFxTime > 0f)
				{
					m_BonusSpawnFxTime -= gameTime.ElapsedGameTime.Milliseconds;
					m_BonusSpawnEffect.Update(gameTime);
					m_BonusSpawnEffect.Trigger(m_CurrentBonus.m_Position);
				}
				else
				{
					m_BonusOutline.Update(gameTime);
					m_BonusOutline.Trigger(m_CurrentBonus.m_Position);
				}
				for (int j = 0; j < m_Players.Count; j++)
				{
					if (m_CurrentBonus.IsGrabbedByPlayer(m_Players[j]))
					{
						m_GrabBonusSfx.Play();
						PowerUpSpawnList.Add(m_CurrentBonus.m_Position);
						m_CurrentBonus = null;
						return;
					}
				}
				if (m_BonusLifeTime <= 0f)
				{
					m_BonusSpawnEffect.Trigger(m_CurrentBonus.m_Position);
					PowerUpSpawnList.Add(m_CurrentBonus.m_Position);
					m_CurrentBonus = null;
				}
				m_BonusLifeTime -= gameTime.ElapsedGameTime.Milliseconds;
			}
		}
		if (m_BattleMode != null)
		{
			m_BattleMode.Update(gameTime);
		}
		if (m_IsBombExploding)
		{
			m_BombExplodeAnim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			if (m_BombExplodeAnim.m_CurrentLoop < 0)
			{
				m_IsBombExploding = false;
				m_BombExplodeAnim.m_TotalLoop = 1;
				m_BombExplodeAnim.m_CurrentFrame = 0;
				m_BombExplodeAnim.m_CurrentLoop = 0;
			}
		}
		if (m_IsSeedExploding)
		{
			m_SeedExplodeAnim.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			if (m_SeedExplodeAnim.m_CurrentLoop < 0)
			{
				m_IsSeedExploding = false;
				m_SeedExplodeAnim.m_TotalLoop = 1;
				m_SeedExplodeAnim.m_CurrentFrame = 0;
				m_SeedExplodeAnim.m_CurrentLoop = 0;
			}
		}
	}

	public void DrawEntities()
	{
		for (int i = 0; i < m_Entities.Count; i++)
		{
			m_Entities[i].Draw();
		}
		if (m_CurrentBonus != null)
		{
			m_CurrentBonus.DrawBonus();
		}
		if (m_BattleMode != null)
		{
			m_BattleMode.Draw();
		}
		if (m_IsBombExploding)
		{
			m_BombExplodeAnim.DrawFixed(SpriteEffects.None, Color.White, GameContext.POWERUP_Z);
		}
		if (m_IsSeedExploding)
		{
			m_SeedExplodeAnim.DrawFixed(SpriteEffects.None, Color.White, GameContext.POWERUP_Z);
		}
	}

	public void AddEntity(ScenaricEntitie entity)
	{
		if (entity.TypeId == SCENARIC.TYPE_FLAG)
		{
			Flag flag = (Flag)entity;
			if (flag.FlagColor == Color.Red)
			{
				m_RedFlag = flag;
			}
			else
			{
				m_BlueFlag = flag;
			}
		}
		m_Entities.Add(entity);
	}

	public void AddLayerWave(string path, int x, int y, string shaderName, SpriteEffects spe, float zOrder, Color color)
	{
		WaveFx item = new WaveFx(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(path), x, y, content.Load<Effect>(shaderName));
		m_FxManager.Add(item);
	}

	public void AddLayerFadeFx(string path, int x, int y, string name, SpriteEffects spe, float zOrder, Color color, int seed)
	{
		LayerFadeFx layerFadeFx = new LayerFadeFx(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(path), x, y, name, seed);
		layerFadeFx.SetZ(zOrder);
		layerFadeFx.SetSpriteEffect(spe);
		layerFadeFx.SetTextureColor(color);
		m_Entities.Add(layerFadeFx);
	}

	public virtual void AddLayer(string path, int x, int y, string name, SpriteEffects spe, float zOrder, Color color)
	{
		BackgroundLayer backgroundLayer = new BackgroundLayer(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(path), x, y, name);
		backgroundLayer.SetZ(zOrder);
		backgroundLayer.SetSpriteEffect(spe);
		backgroundLayer.SetTextureColor(color);
		m_Entities.Add(backgroundLayer);
	}

	public TriggerTrap AddTrigger(string path1, string path2, int x, int y, int width, int height, int framecount, float speed, string name, SpriteEffects spe, float zOrder)
	{
		TriggerTrap triggerTrap = new TriggerTrap(this, content.Load<Texture2D>(path1), content.Load<Texture2D>(path2), x, y, name);
		triggerTrap.SetZ(zOrder);
		triggerTrap.SetSpriteEffect(spe);
		m_Entities.Add(triggerTrap);
		return triggerTrap;
	}

	public AnimatedTrap AddTrap(string path, int x, int y, int width, int height, int framecount, float speed, string name, SpriteEffects spe, float zOrder)
	{
		AnimatedSprite trapAnim = new AnimatedSprite(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(path), framecount, width, height, speed, 1);
		AnimatedTrap animatedTrap = new AnimatedTrap(this, trapAnim, x, y, name);
		animatedTrap.SetZ(zOrder);
		animatedTrap.SetSpriteEffect(spe);
		m_Entities.Add(animatedTrap);
		return animatedTrap;
	}

	public void AddAnim(string path, int x, int y, int width, int height, int framecount, float speed, string name, SpriteEffects spe, float zOrder, Color color, float startoffset)
	{
		BackgroundAnim backgroundAnim = new BackgroundAnim(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(path), framecount, x, y, width, height, speed, 0, name, startoffset);
		backgroundAnim.SetZ(zOrder);
		backgroundAnim.SetSpriteEffect(spe);
		backgroundAnim.SetTextureColor(color);
		m_Entities.Add(backgroundAnim);
	}

	public SpawnPoint GetSpawnPoint(ref int SpawnSlot, Color team)
	{
		SpawnSlot = m_Randomizer.Next(0, m_SpawnInfo.Count - 1);
		if (GameContext.GameMode == GAME_MODE.CAPTURE_THE_JAM || GameContext.GameMode == GAME_MODE.JAM_BALL)
		{
			bool flag = m_SpawnInfo[SpawnSlot].bIsFree && m_SpawnInfo[SpawnSlot].Team == team;
			while (!flag)
			{
				SpawnSlot++;
				if (SpawnSlot > m_SpawnInfo.Count - 1)
				{
					SpawnSlot = 0;
				}
				flag = m_SpawnInfo[SpawnSlot].bIsFree && m_SpawnInfo[SpawnSlot].Team == team;
			}
		}
		else
		{
			while (!m_SpawnInfo[SpawnSlot].bIsFree)
			{
				SpawnSlot++;
				if (SpawnSlot >= m_SpawnInfo.Count - 1)
				{
					SpawnSlot = 0;
				}
			}
		}
		return m_SpawnInfo[SpawnSlot];
	}

	public void AddParticle(MercuryParticle Mpe)
	{
		m_Entities.Add(Mpe);
		m_ParticleManager.Add(Mpe);
	}

	public void RemoveParticle(MercuryParticle Mpe)
	{
		m_Entities.Remove(Mpe);
		m_ParticleManager.Remove(Mpe);
	}

	public void InitHud(bool initPlayerButton)
	{
		m_btSprite[0] = LoadAnimatedSpriteFromXml("Hud/bt/bulle.xml", GameAtlas.GAME, GameContext.PAD_BUTTON_HUD[0]);
		m_btSpriteSoft[0] = LoadAnimatedSpriteFromXml("Hud/bt/bulle.xml", GameAtlas.GAME, GameContext.PAD_BUTTON_HUD_SOFT[0]);
		m_btTexture[0] = LoadSprite(GameContext.PAD_BUTTON_HUD_TEX[0], GameAtlas.GAME);
		m_GetReadyTex = LoadSprite("HUD_Ready", GameAtlas.GAME);
		m_ReadyTextSize = (int)base.ScreenManager.GoBoomBig.MeasureString(TextManager.GetText(TextID.GET_READY)).X;
		if (!initPlayerButton)
		{
			return;
		}
		for (int i = 1; i < GameContext.PAD_BUTTON_HUD.Length; i++)
		{
			Sprite sprite = m_GameAtlas[0].FindInAtlas(GameContext.PAD_BUTTON_HUD[i]);
			m_btSprite[i] = new AnimatedSprite(base.ScreenManager.SpriteBatch, m_GameAtlas[0].GetTexture(), m_btSprite[0].m_TotalFrames, m_btSprite[0].m_FrameWidth, m_btSprite[0].m_FrameHeight, m_btSprite[0].m_Speed, 0, sprite.rect.X, sprite.rect.Y);
			sprite = m_GameAtlas[0].FindInAtlas(GameContext.PAD_BUTTON_HUD_SOFT[i]);
			m_btSpriteSoft[i] = new AnimatedSprite(base.ScreenManager.SpriteBatch, m_GameAtlas[0].GetTexture(), m_btSpriteSoft[0].m_TotalFrames, m_btSpriteSoft[0].m_FrameWidth, m_btSpriteSoft[0].m_FrameHeight, m_btSpriteSoft[0].m_Speed, 0, sprite.rect.X, sprite.rect.Y);
			m_btTexture[i] = LoadSprite(GameContext.PAD_BUTTON_HUD_TEX[i], GameAtlas.GAME);
		}
		if ((object)GetType() != typeof(MultiPlayerMenuScreen))
		{
			m_PauseTexture = LoadSprite("Pause", GameAtlas.GAME);
			for (int j = 0; j < 4; j++)
			{
				m_ResultSprite[j] = LoadSprite("Rank" + (j + 1), GameAtlas.GAME);
				m_ResultJamSprite[j] = LoadSprite("Rankjam" + (j + 1), GameAtlas.GAME);
			}
			m_BackGroundTex = base.ScreenManager.CreateRectangle(1, 1, Color.White);
		}
	}

	public void InitRanking()
	{
		foreach (Player player in m_Players)
		{
			m_Ranking.Add(player);
		}
		m_Ranking.Sort(delegate(Player p1, Player p2)
		{
			int num = p1.m_Score.CompareTo(p2.m_Score);
			if (num != 0)
			{
				return num;
			}
			num = p2.m_nDeathCount.CompareTo(p1.m_nDeathCount);
			return (num != 0) ? num : p1.m_UsedPowerUp.CompareTo(p2.m_UsedPowerUp);
		});
		m_Ranking.Reverse();
	}

	public void InitPowerUp()
	{
		m_CurrentBonus = null;
		m_BonusSpawnTime = m_Randomizer.Next(1000, 2000);
		m_BombExplodeAnim = LoadAnimatedSpriteFromXml("PowerUp/Bomb/PowerUp_BombExplosion.xml", GameAtlas.GAME, "PowerUp_BombExplosion");
		m_BombExplodeAnim.m_bInfiniteLoop = false;
		m_BombExplodeAnim.m_TotalLoop = 1;
		m_SeedExplodeAnim = LoadAnimatedSpriteFromXml("PowerUp/Seed/PowerUp_SeedExplosion.xml", GameAtlas.GAME, "PowerUp_SeedExplosion");
		m_SeedExplodeAnim.m_bInfiniteLoop = false;
		m_SeedExplodeAnim.m_TotalLoop = 1;
		if (SaveHandler.GetSaveData().BonusFrequency[0] == 0)
		{
			m_Bonus.Add(new Skull(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new Skull(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[3] == 0)
		{
			m_Bonus.Add(new Bomb(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new Bomb(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[1] == 0)
		{
			m_Bonus.Add(new Fly(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new Fly(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[5] == 0)
		{
			m_Bonus.Add(new Seed(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[6] == 0)
		{
			m_Bonus.Add(new Heart(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[2] == 0)
		{
			m_Bonus.Add(new Soldier(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new Soldier(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[4] == 0)
		{
			m_Bonus.Add(new Cloud(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[8] == 0)
		{
			m_Bonus.Add(new FireProut(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new FireProut(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[7] == 0)
		{
			m_Bonus.Add(new Vile(this, base.ScreenManager.SpriteBatch));
		}
		if (SaveHandler.GetSaveData().BonusFrequency[9] == 0)
		{
			m_Bonus.Add(new BlackSugar(this, base.ScreenManager.SpriteBatch));
			m_Bonus.Add(new BlackSugar(this, base.ScreenManager.SpriteBatch));
		}
		ParticleEffect pe = content.Load<ParticleEffect>("Fx/Particle/BonusSpawner");
		m_BonusSpawnEffect = new MercuryParticle(this, 0, 0, pe, "BonusSpawnerFx", 0f, bUseBlending: true);
		m_BonusSpawnEffect.SetAutoTrigger(bAutoTrigger: false);
		pe = content.Load<ParticleEffect>("Fx/Particle/PowerUpOutline");
		m_BonusOutline = new MercuryParticle(this, 0, 0, pe, "PowerUpOutline", 0f, bUseBlending: true);
		m_BonusOutline.SetAutoTrigger(bAutoTrigger: false);
		m_GrabBonusSfx = new AudioClip("PowerUp_Grabbed");
		m_SpawnBonusSfx = new AudioClip("PowerUp_Spawn");
		AddParticle(m_BonusSpawnEffect);
		AddParticle(m_BonusOutline);
	}

	public virtual void StopGame()
	{
	}

	public virtual void ResetSouls()
	{
		m_SoulSpawner.ResetSouls();
	}

	public virtual void Destroy()
	{
		m_Entities.Clear();
		m_Players.Clear();
		m_ParticleManager.Clear();
		if (m_PhysicManager != null)
		{
			m_PhysicManager = null;
		}
		if (m_Renderer != null)
		{
			m_Renderer.Dispose();
		}
	}

	public void ExplodeBomb(Vector2 ExplosionPos, Color color, float scale)
	{
		ExplosionPos.X -= (float)(m_BombExplodeAnim.GetFrameWidth() / 2) * scale;
		ExplosionPos.Y -= (float)(m_BombExplodeAnim.GetFrameHeight() / 2) * scale;
		m_BombExplodeAnim.SetPosition(ExplosionPos);
		m_IsBombExploding = true;
	}

	public void ExplodeSeed(Vector2 ExplosionPos, Color color)
	{
		ExplosionPos.X -= m_SeedExplodeAnim.GetFrameWidth() / 2;
		ExplosionPos.Y -= m_SeedExplodeAnim.GetFrameHeight() / 2;
		m_SeedExplodeAnim.SetPosition(ExplosionPos);
		m_IsSeedExploding = true;
	}

	public AnimatedSprite LoadAnimatedSpriteFromXml(string XmlPath, string AssetPath)
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
				return new AnimatedSprite(base.ScreenManager.SpriteBatch, content.Load<Texture2D>(AssetPath), frameCount, width, height, speed, 0);
			}
		}
		return null;
	}

	public AnimatedSprite LoadAnimatedSpriteFromXml(string XmlPath, GameAtlas AtlasID, string TextureName)
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
				Sprite sprite = m_GameAtlas[(int)AtlasID].FindInAtlas(TextureName);
				return new AnimatedSprite(base.ScreenManager.SpriteBatch, m_GameAtlas[(int)AtlasID].GetTexture(), frameCount, width, height, speed, 0, sprite.rect.X, sprite.rect.Y);
			}
		}
		return null;
	}

	public Atlas Getatlas(int id)
	{
		return m_GameAtlas[id];
	}
}
