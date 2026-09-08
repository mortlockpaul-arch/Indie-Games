using System;
using System.Globalization;
using System.Xml;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;

namespace JamSouls;

public abstract class Player : ScenaricEntitie
{
	public enum AnimStates : byte
	{
		STAND,
		WALK,
		JUMP,
		FALL,
		DUCK,
		EXPLODE,
		SP_STAND,
		SP_WALK,
		SP_JUMP,
		SP_FALL,
		SP_DUCK,
		SP_DASH,
		HALF_EXPLODE,
		KICK,
		ANIMECOUNT
	}

	public enum KickType
	{
		KICK_UP,
		KICK_LOW,
		KICK_HIGH
	}

	private const int HUD_JAUGE_OFFSET_Y = 70;

	private const int HUD_JAUGE_OFFSET_X = 20;

	private const int HUD_TEX_OFFSET_X = -80;

	private const int HUD_TEX_OFFSET_Y = -25;

	private const int HUD_SCORE_OFFSET_X = 120;

	private const int HUD_JAUGE_X = 10;

	private const int HUD_JAUGE_Y = 8;

	private const int HUD_JAUGE_SIZE_X = 74;

	private const int HUD_JAUGE_SIZE_Y = 4;

	private const int WIN_SCORE_POS = 72;

	public const float MINIMUM_FALL_VELOCITY = 1f;

	public const float FRICTION_FORCES = 0f;

	public const float MOVE_SPEED_WALK_MINI = 4f;

	public const float MOVE_SPEED_RUN_MINI = 6f;

	public const float MOVE_SPEED_WALK = 25f;

	public const float MOVE_SPEED_RUN = 40f;

	public const float JUMP_IMPULSE = 30f;

	public const float MAX_JUMP_IMPULSE = 11f;

	public const float FRAG_DAMPING_VALUE = 1280f;

	private const float RESPAWN_TIMER_OFFSET = 400f;

	public const float RESPAWN_EFFECT_LIFE = 2400f;

	public const float SPAWN_TIME = 3000f;

	public const float SIZE_REFERENCE = 2275f;

	public const float MAX_JPOWER_TIME = 5000f;

	private const int SPLASH_SOUND = 3;

	protected const float KICK_LATENCY = 400f;

	protected const float KICK_TIME = 160f;

	public const int TIME_TO_BURN = 3000;

	public const int TIME_TO_MORPH = 10000;

	public const float SMOKE_TIME = 500f;

	public const int LIFE = 100;

	public AnimatedSprite[] m_PlayerSprite = new AnimatedSprite[14];

	public AnimStates m_CurrentAnim;

	public Vector2 m_AnimPos;

	public Sprite m_HudTexture;

	private Sprite m_Hudjauge;

	private Sprite m_HudjaugeLight;

	private Texture2D m_HudJjaugeColor;

	public Sprite m_PlayerArrowTex;

	public Rectangle m_HudSource;

	public AnimatedSprite m_BigAvatar;

	private AnimatedSprite m_SmokeRun;

	protected Body m_PlayerBody;

	public Fixture m_PlayerFixture;

	public float m_JumpImpulse = -30f;

	public Vector2 m_FragDamper = new Vector2(0f, -1280f);

	public Fixture m_CurrentPlatform;

	public float m_MaxJumpImpulse = 11f;

	public bool m_bControlHorizontalMove = true;

	public bool m_bDampingEnable;

	public bool m_WallOnTheLeft;

	public bool m_WallOnTheRIght;

	public MercuryParticle m_RespawnEmitter;

	public MercuryParticle m_GibEmitter;

	public MercuryParticle m_BleedingEmitter;

	public MercuryParticle m_BubbleEffect;

	public float m_RespawnEmitterLife;

	public Color m_PlayerColor = Color.White;

	public AudioClip m_JumpSound;

	private int m_currentSound;

	public AudioClip[] m_SplatchSound = new AudioClip[3];

	public AudioClip m_JamSoulSound;

	public PlayerIndex m_PlayerNum;

	public int m_nCharacterIdx;

	public int m_Width;

	public int m_Height;

	public int m_OffsetX;

	public int m_OffsetY;

	public int m_SpOffsetX;

	public int m_SpOffsetY;

	public float m_CurrentJumpImpulse;

	public bool m_bIsOnGround;

	public bool m_bIsDucked;

	public bool m_bLockJump;

	public bool m_lockInput;

	public Color m_Team;

	public bool m_bIsPlayerBot;

	public float m_Speed;

	public int m_Score;

	public int m_Frag;

	public int m_UsedPowerUp;

	public int m_nDeathCount;

	public int m_Tag;

	public SpawnPoint m_CurrentSpawn;

	public bool m_FixedSpawn;

	public int SpawnSlot;

	public bool m_bLeftRelease;

	public bool m_bRightRelease;

	public bool m_bJumpRelease;

	public GameState m_GameStateInstance;

	public bool m_bToggleName;

	public int m_life = 100;

	public float m_Scale = 1f;

	private Vector2 m_PlayerDeathPos;

	public float m_Rotation;

	public Vector2 m_Origin = Vector2.Zero;

	public PlayerConfig.SBIRE_DEF m_SbireDef;

	public bool m_bAllowPowerUp = true;

	public bool m_bSlowDown;

	public float m_KickTimer;

	public float m_AnimLatency;

	protected Vector2 m_KickFlip = new Vector2(0f, -500f);

	public KickType m_Kick;

	public float m_PokeTime;

	private SpriteEffects m_SmokeEffect;

	private float m_SmokeTime;

	private bool m_PlaySmokeAnim;

	private Vector2 m_ScorePos;

	private Vector2 m_HudJaugePos;

	public Vector2 m_HudTexPos;

	public bool m_bSpecialEnable;

	public SpecialCharacter m_SpecialCharacter;

	public int m_SoulNumber;

	public bool m_bUsePowerUp;

	public PowerUp m_CurrentPowerUp;

	public bool m_bIsBurning;

	public bool m_bIsMorphing;

	public bool m_bUnStompable;

	public float m_ConsumingTime;

	public float m_MorphingTime;

	public MercuryParticle m_FireFx;

	public float m_WalkAnimationSpeed;

	public Random m_Randomizer;

	public void InitPlayer(GameState GameStateInstance, int CharIdx, PlayerIndex nIndex, string name, PlayerConfig.SBIRE_DEF sbireDef)
	{
		m_Randomizer = new Random(CharIdx);
		m_currentSound = m_Randomizer.Next(3);
		m_zOrder = GameContext.PLAYER_Z + (float)CharIdx / 100f;
		TypeId = SCENARIC.TYPE_PLAYER;
		m_SbireDef = sbireDef;
		m_PlayerNum = nIndex;
		m_GameStateInstance = GameStateInstance;
		Name = name;
		InitSprite(CharIdx);
		m_PlayerNum = nIndex;
		m_PlayerBody = m_GameStateInstance.m_PhysicManager.CreateBody();
		m_PlayerBody.BodyType = BodyType.Dynamic;
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.SetAsBox((float)(m_Width / 2) / 10f, (float)(m_Height / 2) / 10f);
		m_PlayerFixture = m_PlayerBody.CreateFixture(polygonShape);
		m_Tag = 3;
		m_PlayerFixture.UserData = this;
		m_PlayerFixture.CollisionCategories = CollisionCategory.Cat1;
		m_PlayerFixture.CollidesWith = CollisionCategory.All;
		m_PlayerFixture.Friction = 0f;
		Fixture playerFixture = m_PlayerFixture;
		playerFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(playerFixture.OnCollision, new CollisionEventHandler(OnCollision));
		Fixture playerFixture2 = m_PlayerFixture;
		playerFixture2.OnSeparation = (SeparationEventHandler)Delegate.Combine(playerFixture2.OnSeparation, new SeparationEventHandler(OnSeparation));
		m_PlayerBody.FixedRotation = true;
		m_PlayerBody.SleepingAllowed = false;
		m_PlayerBody.IsStatic = true;
		m_PlayerBody.Active = false;
		m_PlayerBody.Mass = 1f;
		m_CurrentPlatform = null;
		m_JumpImpulse = 31.384615f;
		m_MaxJumpImpulse = 11f * m_JumpImpulse;
		m_FragDamper = new Vector2(0f, -1339.0769f);
		if (m_SbireDef != PlayerConfig.SBIRE_DEF.NONE)
		{
			m_JumpImpulse /= 2.5f;
			m_MaxJumpImpulse /= 2.5f;
			m_FragDamper /= 2.5f;
		}
		m_CurrentJumpImpulse = 0f;
		m_bIsOnGround = false;
		m_bLockJump = false;
		m_SpriteEffect = SpriteEffects.None;
		m_bLeftRelease = true;
		m_bRightRelease = true;
		m_bJumpRelease = true;
		m_CurrentAnim = AnimStates.STAND;
		m_Team = Color.White;
		m_lockInput = false;
		m_FixedSpawn = false;
		switch (m_PlayerNum)
		{
		case PlayerIndex.One:
			SetPosition(Vector2.Zero);
			break;
		case PlayerIndex.Two:
			SetPosition(new Vector2(1280f, 0f));
			break;
		case PlayerIndex.Three:
			SetPosition(new Vector2(0f, 1280f));
			break;
		case PlayerIndex.Four:
			SetPosition(new Vector2(1280f, 720f));
			break;
		}
		m_Score = 0;
		m_Frag = 0;
		m_UsedPowerUp = 0;
		m_nDeathCount = 0;
		m_bToggleName = true;
	}

	public void InitFx()
	{
		m_RespawnEmitterLife = 0f;
		ParticleEffect particleEffect = new ParticleEffect();
		particleEffect = m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/SpawnBeam");
		m_RespawnEmitter = new MercuryParticle(m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "PlayerRespawnEmitter", m_zOrder, bUseBlending: true);
		m_RespawnEmitter.SetParticleColor(PlayerConfig.CHARACTER_COLOR[m_nCharacterIdx], new Vector3(0f, 0f, 0f));
		particleEffect = m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/PlayerGibs");
		m_GibEmitter = new MercuryParticle(m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "PlayerGibEmitter", m_zOrder, bUseBlending: false);
		m_GibEmitter.SetParticleColor(PlayerConfig.CHARACTER_COLOR[m_nCharacterIdx], new Vector3(0f, 0f, 0f));
		m_GibEmitter.m_bUseBlending = true;
		particleEffect = m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/Burning");
		m_FireFx = new MercuryParticle(m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "PlayerFireEmitter", m_zOrder, bUseBlending: true);
		m_FireFx.m_bUseBlending = true;
		particleEffect = m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/BulletGibs");
		m_BleedingEmitter = new MercuryParticle(m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "Bleed", m_zOrder, bUseBlending: true);
		m_BleedingEmitter.SetParticleColor(m_PlayerColor, Vector3.Zero);
		particleEffect = m_GameStateInstance.content.Load<ParticleEffect>("Fx/Particle/Bubble");
		m_BubbleEffect = new MercuryParticle(m_GameStateInstance, 0, 0, particleEffect.DeepCopy(), "Bubble", m_zOrder, bUseBlending: true);
		m_BubbleEffect.SetParticleColor(m_PlayerColor, Vector3.Zero);
		m_BleedingEmitter.SetAutoTrigger(bAutoTrigger: false);
		m_RespawnEmitter.SetAutoTrigger(bAutoTrigger: false);
		m_GibEmitter.SetAutoTrigger(bAutoTrigger: false);
		m_FireFx.SetAutoTrigger(bAutoTrigger: false);
		m_BubbleEffect.SetAutoTrigger(bAutoTrigger: false);
		m_GameStateInstance.AddParticle(m_BleedingEmitter);
		m_GameStateInstance.AddParticle(m_RespawnEmitter);
		m_GameStateInstance.AddParticle(m_GibEmitter);
		m_GameStateInstance.AddParticle(m_FireFx);
		m_GameStateInstance.AddParticle(m_BubbleEffect);
	}

	protected void ChangeFixture(Fixture fix)
	{
		m_PlayerFixture = fix;
		m_PlayerFixture.UserData = this;
		m_PlayerFixture.CollisionCategories = CollisionCategory.Cat1;
		m_PlayerFixture.CollidesWith = CollisionCategory.All;
		m_PlayerFixture.Friction = 0f;
		Fixture playerFixture = m_PlayerFixture;
		playerFixture.OnCollision = (CollisionEventHandler)Delegate.Combine(playerFixture.OnCollision, new CollisionEventHandler(OnCollision));
		Fixture playerFixture2 = m_PlayerFixture;
		playerFixture2.OnSeparation = (SeparationEventHandler)Delegate.Combine(playerFixture2.OnSeparation, new SeparationEventHandler(OnSeparation));
	}

	public void Morph(float scale)
	{
		if (!m_bSpecialEnable && !m_bIsMorphing)
		{
			m_bIsMorphing = true;
			m_MorphingTime = 10000f;
			PolygonShape polygonShape = new PolygonShape();
			polygonShape.SetAsBox((float)GetWidth() / scale / 2f / 10f, (float)GetHeight() / scale / 2f / 10f);
			m_Scale = 1f / scale;
			m_PlayerBody.DestroyFixture(m_PlayerFixture);
			m_PlayerBody.CreateFixture(polygonShape);
			ChangeFixture(m_PlayerBody.FixtureList[0]);
			m_PlayerBody.ResetDynamics();
			m_JumpImpulse = 71400f / scale / 2275f;
			m_MaxJumpImpulse = 11f * m_JumpImpulse / scale;
			m_FragDamper = new Vector2(0f, -3046400f / scale / 2275f);
			if (m_CurrentPowerUp != null)
			{
				m_CurrentPowerUp.StopBonus();
			}
			SetWalkSpeed(4f, m_WalkAnimationSpeed);
		}
	}

	public void DeMorph()
	{
		m_bIsMorphing = false;
		PolygonShape polygonShape = new PolygonShape();
		polygonShape.SetAsBox((float)(GetWidth() / 2) / 10f, (float)(GetHeight() / 2) / 10f);
		m_Scale = 1f;
		m_PlayerBody.DestroyFixture(m_PlayerFixture);
		m_PlayerBody.CreateFixture(polygonShape);
		ChangeFixture(m_PlayerBody.FixtureList[0]);
		m_JumpImpulse = 31.384615f;
		m_MaxJumpImpulse = 11f * m_JumpImpulse;
		m_FragDamper = new Vector2(0f, -1339.0769f);
	}

	public void InitSprite(int nCharactersIdx)
	{
		m_nCharacterIdx = nCharactersIdx;
		string text = PlayerConfig.CHARACTER_NAME[m_nCharacterIdx];
		if (m_SbireDef == PlayerConfig.SBIRE_DEF.NONE)
		{
			switch ((CHARACTERDEF)m_nCharacterIdx)
			{
			case CHARACTERDEF.VICE:
				m_SpecialCharacter = new Vice(this);
				break;
			case CHARACTERDEF.MORT:
				m_SpecialCharacter = new Mort(this);
				break;
			case CHARACTERDEF.FAMINE:
				m_SpecialCharacter = new Famine(this);
				break;
			case CHARACTERDEF.MALADIE:
				m_SpecialCharacter = new Maladie(this);
				break;
			case CHARACTERDEF.GUERRE:
				m_SpecialCharacter = new Guerre(this);
				break;
			case CHARACTERDEF.ESPERANCE:
				m_SpecialCharacter = new Esperance(this);
				break;
			case CHARACTERDEF.MISERE:
				m_SpecialCharacter = new Misere(this);
				break;
			case CHARACTERDEF.TROMPERIE:
				m_SpecialCharacter = new Tromperie(this);
				break;
			case CHARACTERDEF.PASSION:
				m_SpecialCharacter = new Passion(this);
				break;
			case CHARACTERDEF.FOLIE:
				m_SpecialCharacter = new Folie(this);
				break;
			default:
				m_SpecialCharacter = null;
				break;
			}
			m_HudTexture = m_GameStateInstance.LoadSprite("HUD_" + PlayerConfig.CHARACTER_NAME[m_nCharacterIdx], GameState.GameAtlas.GAME);
			m_Hudjauge = m_GameStateInstance.LoadSprite("HUD_jauge", GameState.GameAtlas.GAME);
			m_HudjaugeLight = m_GameStateInstance.LoadSprite("HUD_jauge_light", GameState.GameAtlas.GAME);
			m_HudJjaugeColor = new Texture2D(m_GameStateInstance.ScreenManager.GraphicsDevice, m_Hudjauge.Width, m_Hudjauge.Height, mipMap: false, SurfaceFormat.Color);
			Color[] array = new Color[m_Hudjauge.Width * m_Hudjauge.Height];
			for (int i = 0; i < array.Length; i++)
			{
				ref Color reference = ref array[i];
				reference = Color.White;
			}
			m_HudJjaugeColor.SetData(array);
			if (m_SbireDef == PlayerConfig.SBIRE_DEF.NONE)
			{
				m_ScorePos.X = PlayerConfig.SCORE_POSITION[(int)m_PlayerNum].X + 120f;
			}
			m_ScorePos.Y = GameContext.TileSafeTop;
			m_HudTexPos = m_ScorePos;
			m_HudTexPos.X += -80f;
			m_HudTexPos.Y += -25f;
			m_HudJaugePos = m_HudTexPos;
			m_HudJaugePos.X += 20f;
			m_HudJaugePos.Y += 70f;
			m_HudSource = new Rectangle(0, 0, m_HudTexture.Width, m_HudTexture.Height);
			m_BigAvatar = m_GameStateInstance.LoadAnimatedSpriteFromXml("BattleScreen/BattleChar.xml", GameState.GameAtlas.GAME, PlayerConfig.CHARACTER_NAME[m_nCharacterIdx] + "Battle");
			m_SmokeRun = m_GameStateInstance.LoadAnimatedSpriteFromXml("Fx/SpriteFx/RunSmoke.xml", GameState.GameAtlas.GAME, "RunSmoke");
			m_SmokeRun.m_bInfiniteLoop = false;
			m_SmokeRun.m_TotalLoop = 1;
			m_SmokeRun.SetLock(locked: true);
		}
		m_bSpecialEnable = false;
		m_bUsePowerUp = false;
		m_CurrentPowerUp = null;
		m_bIsBurning = false;
		m_bIsMorphing = false;
		m_ConsumingTime = 0f;
		m_Speed = 25f;
		m_Width = 0;
		m_Height = 0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		float num4 = 0f;
		int num5 = 0;
		SpawnSlot = -1;
		string text2;
		string inputUri;
		if (m_SbireDef != PlayerConfig.SBIRE_DEF.NONE)
		{
			text = m_SbireDef.ToString();
			text2 = "Char/Sbire/" + text + "/";
			inputUri = m_GameStateInstance.content.RootDirectory + "\\Char\\Sbire\\" + text + "\\" + text + "Info.xml";
		}
		else
		{
			text2 = "Char/Main/" + text + "/";
			inputUri = m_GameStateInstance.content.RootDirectory + "\\Char\\Main\\" + text + "\\" + text + "Info.xml";
		}
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.IgnoreWhitespace = true;
		xmlReaderSettings.IgnoreComments = true;
		XmlReader xmlReader = XmlReader.Create(inputUri, xmlReaderSettings);
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.AttributeCount == 0)
			{
				continue;
			}
			if (xmlReader.Name == "Collision")
			{
				m_Width = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
				m_Height = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
				m_OffsetX = int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
				m_OffsetY = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
				m_SpOffsetX = int.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture);
				m_SpOffsetY = int.Parse(xmlReader.GetAttribute(5), CultureInfo.InvariantCulture);
				continue;
			}
			num3 = int.Parse(xmlReader.GetAttribute(0), CultureInfo.InvariantCulture);
			num = int.Parse(xmlReader.GetAttribute(1), CultureInfo.InvariantCulture);
			num2 = int.Parse(xmlReader.GetAttribute(2), CultureInfo.InvariantCulture);
			num4 = int.Parse(xmlReader.GetAttribute(3), CultureInfo.InvariantCulture);
			num5 = int.Parse(xmlReader.GetAttribute(4), CultureInfo.InvariantCulture);
			AnimatedSprite animatedSprite = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + xmlReader.Name), num3, num, num2, num4, num5);
			switch (xmlReader.Name)
			{
			case "Stand":
				m_PlayerSprite[0] = animatedSprite;
				break;
			case "Walk":
				m_PlayerSprite[1] = animatedSprite;
				m_WalkAnimationSpeed = animatedSprite.m_Speed;
				if (GameContext.GameMode == GAME_MODE.JAM_BALL)
				{
					m_PlayerSprite[13] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "Shoot"), 1, num, num2, 160f, 0);
				}
				break;
			case "Jump":
				m_PlayerSprite[2] = animatedSprite;
				m_PlayerSprite[4] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "Jump"), 1, num, num2, num4, num5);
				break;
			case "Fall":
				m_PlayerSprite[3] = animatedSprite;
				break;
			case "Explode":
				m_PlayerSprite[5] = animatedSprite;
				if (m_SbireDef != PlayerConfig.SBIRE_DEF.NONE)
				{
					m_PlayerSprite[12] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "Explode"), num3 - 4, num, num2, num4, num5);
				}
				else
				{
					m_PlayerSprite[12] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "Explode"), num3 - 1, num, num2, num4, num5);
				}
				break;
			case "SpecialStand":
				m_PlayerSprite[6] = animatedSprite;
				break;
			case "SpecialWalk":
				m_PlayerSprite[7] = animatedSprite;
				break;
			case "SpecialJump":
				m_PlayerSprite[8] = animatedSprite;
				if (m_nCharacterIdx == 5)
				{
					m_PlayerSprite[10] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "SpecialDusk"), 4, num, num2, num4, 0);
				}
				else if (m_nCharacterIdx == 7)
				{
					m_PlayerSprite[10] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "SpecialDusk"), 4, num, num2, num4, 0);
				}
				else if (m_nCharacterIdx == 7 || m_nCharacterIdx == 0)
				{
					m_PlayerSprite[10] = m_PlayerSprite[6];
				}
				else
				{
					m_PlayerSprite[10] = new AnimatedSprite(m_GameStateInstance.ScreenManager.SpriteBatch, m_GameStateInstance.content.Load<Texture2D>(text2 + text + "SpecialJump"), 1, num, num2, num4, num5);
				}
				break;
			case "SpecialFall":
				m_PlayerSprite[9] = animatedSprite;
				break;
			case "SpecialDash":
				m_PlayerSprite[11] = animatedSprite;
				break;
			}
		}
		if (m_nCharacterIdx == 8 || m_nCharacterIdx == 1 || m_nCharacterIdx == 0)
		{
			m_PlayerSprite[7] = m_PlayerSprite[6];
			m_PlayerSprite[8] = m_PlayerSprite[6];
			m_PlayerSprite[9] = m_PlayerSprite[6];
			m_PlayerSprite[10] = m_PlayerSprite[6];
		}
		else if (m_nCharacterIdx == 3)
		{
			m_PlayerSprite[6] = m_PlayerSprite[7];
			m_PlayerSprite[9] = m_PlayerSprite[7];
			m_PlayerSprite[10] = m_PlayerSprite[6];
		}
		for (int j = 0; j < 14; j++)
		{
			if (m_PlayerSprite[j] == null)
			{
				m_PlayerSprite[j] = m_PlayerSprite[0];
			}
		}
		m_PlayerArrowTex = m_GameStateInstance.LoadSprite("PlayerArrow", GameState.GameAtlas.GAME);
		m_JumpSound = new AudioClip("Char_Jump");
		for (int k = 0; k < 3; k++)
		{
			m_SplatchSound[k] = new AudioClip("Char_Splatch" + (k + 1));
		}
		m_JamSoulSound = new AudioClip("JamSoul_Use");
	}

	public void SpawnPlayer()
	{
		SpawnSlot = 1;
		m_RespawnEmitterLife = 3000f;
		if (!m_FixedSpawn)
		{
			m_CurrentSpawn = m_GameStateInstance.GetSpawnPoint(ref SpawnSlot, m_Team);
			m_CurrentSpawn.bIsFree = false;
		}
		m_PlayerDeathPos = GetPosition();
		m_Tag = 3;
		m_CurrentJumpImpulse = 0f;
		m_PlayerBody.ResetDynamics();
		m_bIsOnGround = false;
		m_bLockJump = false;
		m_SpriteEffect = (SpriteEffects)m_CurrentSpawn.Flip;
		m_PlayerBody.Active = false;
		m_PlayerBody.IsStatic = true;
		m_bVisible = false;
		m_bIsDucked = false;
	}

	protected bool OnCollision(Fixture Fix1, Fixture Fix2, Contact contact)
	{
		Vector2 localNormal = contact.Manifold.LocalNormal;
		localNormal.Normalize();
		if (Fix2.UserData == null)
		{
			if (Fix2.CollisionCategories == CollisionCategory.Cat8)
			{
				if (!m_bSpecialEnable && (m_Tag == 0 || m_Tag == 2))
				{
					if (GameContext.GameMode != GAME_MODE.STORYMATCH)
					{
						DecreaseScore(1);
					}
					m_Frag--;
					m_Tag = 1;
				}
			}
			else
			{
				if (Fix2.CollisionCategories == CollisionCategory.Cat10 && !m_bIsBurning)
				{
					Burn();
				}
				if (localNormal.Y == 1f && Fix2.Body.Position.Y > m_PlayerBody.Position.Y)
				{
					localNormal.Y = -1f;
				}
				if (localNormal.Y == -1f && m_PlayerBody.LinearVelocity.Y > 1f)
				{
					m_bIsOnGround = true;
					m_CurrentJumpImpulse = 0f;
					m_CurrentPlatform = Fix2;
					if (Fix2.CollisionCategories == CollisionCategory.Cat11 && !m_bSpecialEnable)
					{
						m_bSlowDown = true;
					}
					else if (Fix2.CollisionCategories == CollisionCategory.Cat12)
					{
						m_PlayerBody.ApplyLinearImpulse(ref m_FragDamper);
					}
					else
					{
						m_bSlowDown = false;
					}
				}
				else
				{
					if (Fix2.CollisionCategories == CollisionCategory.Cat3)
					{
						return false;
					}
					if (localNormal.X == 1f)
					{
						m_WallOnTheLeft = true;
						m_WallOnTheRIght = false;
					}
					else if (localNormal.X == -1f)
					{
						m_WallOnTheLeft = false;
						m_WallOnTheRIght = true;
					}
				}
			}
		}
		else if ((object)Fix2.UserData.GetType() == typeof(PlayerHuman) || (object)Fix2.UserData.GetType() == typeof(PlayerBot))
		{
			Player player = (Player)Fix2.UserData;
			if (m_Tag != 1)
			{
				if (localNormal.Y == 1f || localNormal.Y == -1f)
				{
					if (Fix2.Body.Position.Y > m_PlayerBody.Position.Y)
					{
						if (player.m_CurrentPowerUp != null && (object)player.m_CurrentPowerUp.GetType() == typeof(Heart))
						{
							player.m_CurrentPowerUp.BONUS_DURATION = Heart.HEART_DIE_TIME;
						}
						else if (!player.m_bSpecialEnable)
						{
							if (!player.m_bUnStompable && (player.GetTeam() != m_Team || GameContext.GameMode == GAME_MODE.DEATHMATCH))
							{
								player.m_Tag = 1;
								IncreaseScore(1);
								m_GameStateInstance.m_SplashHandler.SpawnSplash(player.GetPosition(), player.m_PlayerColor, callOnce: true);
							}
							else
							{
								player.SetAnimation(AnimStates.HALF_EXPLODE, bForcePlay: true);
							}
						}
						m_bLockJump = true;
						m_PlayerBody.LinearVelocity = new Vector2(m_PlayerBody.LinearVelocity.X, 0f);
						if (player.m_bIsMorphing)
						{
							Vector2 impulse = m_FragDamper / 2f;
							m_PlayerBody.ApplyLinearImpulse(ref impulse);
						}
						else
						{
							m_PlayerBody.ApplyLinearImpulse(ref m_FragDamper);
						}
						SetAnimation(AnimStates.JUMP);
					}
				}
				else if (m_bIsBurning && !player.m_bSpecialEnable)
				{
					player.m_Tag = 2;
				}
			}
		}
		return true;
	}

	protected void OnSeparation(Fixture self, Fixture other)
	{
		if (other.UserData == null && m_CurrentPlatform != null && m_CurrentPlatform == other)
		{
			m_bIsOnGround = false;
			if (other.CollisionCategories == CollisionCategory.Cat11)
			{
				m_bSlowDown = false;
			}
		}
	}

	public void SetTeam(Color TeamColor)
	{
		m_Team = TeamColor;
		m_PlayerColor = PlayerConfig.CHARACTER_COLOR[m_nCharacterIdx];
	}

	public Color GetTeam()
	{
		return m_Team;
	}

	public void SetName(string name)
	{
		Name = name;
	}

	public string GetName()
	{
		return Name;
	}

	public override void SetPosition(Vector2 NewPos)
	{
		m_PlayerBody.Position = NewPos / 10f;
	}

	public void SetBodyPosition(Vector2 NewPos)
	{
		m_PlayerBody.Position = NewPos;
	}

	public override Vector2 GetPosition()
	{
		return m_PlayerBody.Position * 10f;
	}

	public Vector2 GetBodyPosition()
	{
		return m_PlayerBody.Position;
	}

	public override Vector2 GetTopLeftPosition()
	{
		Vector2 position = GetPosition();
		position.X -= m_Width / 2;
		position.Y -= m_Height / 2;
		return position;
	}

	public Vector2 GetHeadPlot()
	{
		Vector2 position = GetPosition();
		position.Y -= (int)((float)m_PlayerSprite[(uint)m_CurrentAnim].GetFrameHeight() / 2f);
		return position;
	}

	public int GetWidth()
	{
		return m_Width;
	}

	public int GetHeight()
	{
		return m_Height;
	}

	public override Vector2 GetBottomRightPosition()
	{
		Vector2 position = GetPosition();
		position.X += m_Width / 2;
		position.Y += m_Height / 2;
		return position;
	}

	public override Vector2 GetBottomLeftPosition()
	{
		Vector2 position = GetPosition();
		position.X -= m_Width / 2;
		position.Y += m_Height / 2;
		return position;
	}

	public Vector2 GetBottomPosition()
	{
		Vector2 position = GetPosition();
		position.Y += m_Height / 2;
		return position;
	}

	public Vector2 GetTopPosition()
	{
		Vector2 position = GetPosition();
		position.Y -= m_Height / 2;
		return position;
	}

	public Body GetBody()
	{
		return m_PlayerBody;
	}

	public Fixture GetFixture()
	{
		return m_PlayerFixture;
	}

	public Vector2 GetOrigin()
	{
		return new Vector2(m_Width / 2, m_Height / 2);
	}

	public void SetWalkSpeed(float NewSpeed, float AnimSpeed)
	{
		m_Speed = NewSpeed;
		m_PlayerSprite[1].m_Speed = AnimSpeed;
	}

	public void SetCurrentPowerUp(PowerUp powerup)
	{
		if (m_CurrentPowerUp != null)
		{
			m_CurrentPowerUp.StopBonus();
		}
		m_CurrentPowerUp = powerup;
		m_CurrentPowerUp.InitBonus();
		m_bUsePowerUp = true;
	}

	public bool IsPoked()
	{
		return m_PokeTime > 0f;
	}

	public void Poke(Vector2 impulse, float time)
	{
		if (!m_bSpecialEnable && m_Tag != 1)
		{
			m_PokeTime = time;
			m_bControlHorizontalMove = false;
			m_PlayerBody.ResetDynamics();
			if (m_bIsMorphing)
			{
				impulse /= 4f;
			}
			m_PlayerBody.ApplyLinearImpulse(ref impulse);
		}
	}

	public override void Update(GameTime gameTime)
	{
		if (!ManageDeath() && m_bVisible && !m_GameStateInstance.m_bIsPaused)
		{
			if (!m_lockInput)
			{
				ManageInput();
			}
			if ((GameContext.GameMode == GAME_MODE.DEATHMATCH || GameContext.GameMode == GAME_MODE.STORYMATCH) && m_SoulNumber == 6)
			{
				InitSpecial();
				m_GameStateInstance.ResetSouls();
			}
		}
		if (m_PokeTime > 0f)
		{
			m_PokeTime -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_PokeTime <= 0f)
			{
				m_bControlHorizontalMove = true;
			}
		}
		if (m_bSlowDown)
		{
			m_Speed = 4f;
		}
		if (m_PlaySmokeAnim)
		{
			m_SmokeRun.UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
			m_PlaySmokeAnim = m_SmokeRun.IsLocked();
			m_SmokeTime = 500f;
		}
		if (m_SmokeTime > 0f)
		{
			m_SmokeTime -= gameTime.ElapsedGameTime.Milliseconds;
		}
		if (m_bControlHorizontalMove)
		{
			if (!m_bLeftRelease)
			{
				if (m_CurrentAnim != AnimStates.WALK)
				{
					m_SmokeTime = 0f;
				}
				if (m_SmokeTime <= 0f && !m_PlaySmokeAnim && m_bIsOnGround && m_Speed == 40f)
				{
					m_SmokeRun.SetPosition(GetPosition());
					m_SmokeRun.Reset();
					m_SmokeRun.SetLock(locked: true);
					m_SmokeEffect = m_SpriteEffect;
					m_PlaySmokeAnim = true;
				}
				m_PlayerBody.LinearVelocity = new Vector2(0f - m_Speed, m_PlayerBody.LinearVelocity.Y);
				m_SpriteEffect = SpriteEffects.FlipHorizontally;
			}
			else if (!m_bRightRelease)
			{
				if (m_CurrentAnim != AnimStates.WALK)
				{
					m_SmokeTime = 0f;
				}
				if (m_SmokeTime <= 0f && !m_PlaySmokeAnim && m_bIsOnGround && m_Speed == 40f)
				{
					Vector2 position = GetPosition();
					position.X -= m_Width;
					m_SmokeRun.SetPosition(position);
					m_SmokeRun.Reset();
					m_SmokeRun.SetLock(locked: true);
					m_SmokeEffect = m_SpriteEffect;
					m_PlaySmokeAnim = true;
				}
				m_PlayerBody.LinearVelocity = new Vector2(m_Speed, m_PlayerBody.LinearVelocity.Y);
				m_SpriteEffect = SpriteEffects.None;
			}
			else
			{
				m_PlayerBody.LinearVelocity = new Vector2(0f, m_PlayerBody.LinearVelocity.Y);
			}
		}
		if (!m_bIsOnGround && m_PlayerBody.LinearVelocity.Y > 1f)
		{
			if (m_CurrentAnim != AnimStates.KICK || m_PlayerSprite[(uint)m_CurrentAnim].m_CurrentFrame == 0)
			{
				SetAnimation(AnimStates.FALL);
			}
			m_bLockJump = true;
		}
		if (m_bLockJump && m_bJumpRelease)
		{
			if (m_bDampingEnable)
			{
				m_bLockJump = false;
				m_bIsOnGround = true;
			}
			else
			{
				m_bLockJump = !m_bIsOnGround;
			}
		}
		if (m_bUsePowerUp)
		{
			m_CurrentPowerUp.Update(gameTime);
		}
		else if (m_bSpecialEnable)
		{
			m_SpecialCharacter.Update(gameTime);
		}
		m_PlayerSprite[(uint)m_CurrentAnim].UpdateFrame(gameTime.ElapsedGameTime.Milliseconds);
		UpdateFx(gameTime);
	}

	private Vector2 IntermediatePoint(Vector2 p0, Vector2 p1, float t)
	{
		Vector2 vector = p1 - p0;
		float num = vector.Length();
		if (num == 0f)
		{
			return p0;
		}
		Vector2 vector2 = vector / num;
		return p0 + vector2 * (num * t);
	}

	private void UpdateFx(GameTime gameTime)
	{
		if (m_RespawnEmitterLife > 0f)
		{
			if (m_RespawnEmitterLife >= 400f)
			{
				Vector2 tangent = IntermediatePoint(m_PlayerDeathPos, m_CurrentSpawn.Position, (2400f - m_RespawnEmitterLife) / 2000f);
				Vector2 tangent2 = IntermediatePoint(m_PlayerDeathPos, m_CurrentSpawn.Position, (2400f - m_RespawnEmitterLife) / 2000f);
				SetPosition(Vector2.Hermite(m_PlayerDeathPos, tangent, m_CurrentSpawn.Position, tangent2, (2400f - m_RespawnEmitterLife) / 2000f));
			}
			m_RespawnEmitterLife -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			m_RespawnEmitter.Trigger(GetPosition());
			if (m_RespawnEmitterLife <= 0f && m_Tag == 3)
			{
				Respawn();
			}
		}
		if (m_bIsBurning)
		{
			m_ConsumingTime -= gameTime.ElapsedGameTime.Milliseconds;
			m_FireFx.Update(gameTime);
			m_FireFx.Trigger(GetPosition());
			if (m_ConsumingTime < 0f)
			{
				if (m_CurrentPowerUp != null && (object)m_CurrentPowerUp.GetType() == typeof(Heart))
				{
					if (m_CurrentPowerUp.BONUS_DURATION > Heart.HEART_DIE_TIME)
					{
						m_CurrentPowerUp.BONUS_DURATION = Heart.HEART_DIE_TIME;
					}
					m_bIsBurning = false;
					m_Tag = 0;
				}
				else
				{
					DecreaseScore(1);
					m_Tag = 1;
					m_bIsBurning = false;
				}
			}
		}
		if (m_bIsMorphing)
		{
			m_MorphingTime -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_MorphingTime <= 0f)
			{
				m_bIsMorphing = false;
				DeMorph();
			}
		}
	}

	public void SetLockInput(bool block)
	{
		m_lockInput = block;
	}

	public virtual void ManageInput()
	{
	}

	public override void Draw()
	{
		if (m_bVisible)
		{
			m_AnimPos = GetPosition();
			if (m_bSpecialEnable)
			{
				m_SpecialCharacter.Draw();
				m_AnimPos.X -= m_PlayerSprite[(uint)m_CurrentAnim].GetFrameWidth() / 2 + m_SpOffsetX;
				m_AnimPos.Y -= m_PlayerSprite[(uint)m_CurrentAnim].GetFrameHeight() / 2 + m_SpOffsetY;
			}
			else
			{
				m_AnimPos.X -= m_PlayerSprite[(uint)m_CurrentAnim].GetFrameWidth() / 2 + m_OffsetX;
				m_AnimPos.Y -= m_PlayerSprite[(uint)m_CurrentAnim].GetFrameHeight() / 2 + m_OffsetY;
			}
			if (!m_bIsMorphing)
			{
				m_PlayerSprite[(uint)m_CurrentAnim].Draw(ref m_AnimPos, m_Rotation, m_Origin, m_SpriteEffect, Color.White, m_zOrder);
			}
			else
			{
				m_PlayerSprite[(uint)m_CurrentAnim].Draw(ref m_AnimPos, m_SpriteEffect, Color.White, m_Scale, m_zOrder);
			}
			if (m_CurrentPowerUp != null)
			{
				m_CurrentPowerUp.DrawBonus();
			}
			if (m_PlaySmokeAnim && !m_bSpecialEnable)
			{
				m_SmokeRun.DrawFixed(m_SmokeEffect, Color.White, m_zOrder);
			}
		}
	}

	public void DrawHud()
	{
		if (m_SbireDef != PlayerConfig.SBIRE_DEF.NONE)
		{
			return;
		}
		if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
		{
			m_HudTexture.Draw(m_HudTexPos, Color.White);
			m_GameStateInstance.ScreenManager.DrawTextOutline(m_GameStateInstance.ScreenManager.GoBoomBig, m_Score.ToString(), Color.White, m_PlayerColor, 1f, m_ScorePos, ScreenManager.TextOrigin.top_center);
			if (m_bSpecialEnable)
			{
				m_HudjaugeLight.Draw(m_HudJaugePos, Color.White);
				Rectangle destinationRectangle = new Rectangle((int)(m_HudJaugePos.X + 10f), (int)(m_HudJaugePos.Y + 8f), 74, 4);
				m_GameStateInstance.ScreenManager.SpriteBatch.Draw(m_HudJjaugeColor, destinationRectangle, m_PlayerColor);
			}
			else
			{
				m_Hudjauge.Draw(m_HudJaugePos, Color.White);
				Rectangle destinationRectangle2 = new Rectangle((int)(m_HudJaugePos.X + 10f), (int)(m_HudJaugePos.Y + 8f), m_SoulNumber * 74 / 6, 4);
				m_GameStateInstance.ScreenManager.SpriteBatch.Draw(m_HudJjaugeColor, destinationRectangle2, m_PlayerColor);
			}
		}
		if (m_RespawnEmitterLife > 0f || !m_bToggleName)
		{
			Vector2 topLeftPosition = GetTopLeftPosition();
			topLeftPosition.Y -= m_Height - 5;
			Vector2 position = new Vector2(topLeftPosition.X + (float)(m_PlayerArrowTex.Width / 2), topLeftPosition.Y);
			position.Y -= 30f;
			Color color = ((GameContext.GameMode != GAME_MODE.CAPTURE_THE_JAM && GameContext.GameMode != GAME_MODE.JAM_BALL) ? m_PlayerColor : m_Team);
			m_PlayerArrowTex.Draw(topLeftPosition, color);
			m_GameStateInstance.ScreenManager.DrawTextOutline(m_GameStateInstance.ScreenManager.GoBoom, Name, Color.Black, color, 1f, position, ScreenManager.TextOrigin.top_center);
		}
	}

	public void InitSpecial()
	{
		if (m_SpecialCharacter != null && ((object)m_SpecialCharacter.GetType() != typeof(Tromperie) || m_bIsOnGround))
		{
			m_SoulNumber = 0;
			if (m_CurrentPowerUp != null)
			{
				m_CurrentPowerUp.StopBonus();
			}
			if (m_bIsBurning)
			{
				m_bIsBurning = false;
				m_ConsumingTime = 0f;
			}
			if (m_bIsMorphing)
			{
				m_bIsMorphing = false;
				m_MorphingTime = 0f;
				DeMorph();
			}
			SetWalkSpeed(25f, m_WalkAnimationSpeed);
			m_SpecialCharacter.InitSpecial();
			m_JamSoulSound.Play();
			m_bSpecialEnable = true;
			m_GameStateInstance.m_bAllowSoulSpawn = false;
		}
	}

	public void IncreaseScore(int num)
	{
		if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
		{
			m_Score += num;
		}
		m_Frag += num;
	}

	public void DecreaseScore(int num)
	{
		if (GameContext.GameMode == GAME_MODE.DEATHMATCH)
		{
			m_Score -= num;
		}
	}

	public virtual bool ManageDeath()
	{
		if (m_Tag == 1)
		{
			if (m_bSpecialEnable)
			{
				m_Tag = 0;
				return false;
			}
			if (m_CurrentAnim == AnimStates.EXPLODE && m_PlayerSprite[(uint)m_CurrentAnim].IsAnimFinished())
			{
				GetPosition();
				m_PlayerDeathPos = GetPosition();
				if (!m_FixedSpawn)
				{
					m_CurrentSpawn = m_GameStateInstance.GetSpawnPoint(ref SpawnSlot, m_Team);
					m_CurrentSpawn.bIsFree = false;
				}
				m_zOrder = GameContext.PLAYER_Z;
				m_SpriteEffect = (SpriteEffects)m_CurrentSpawn.Flip;
				m_Tag = 3;
				m_RespawnEmitterLife = 2400f;
				m_life = 100;
				m_bIsBurning = false;
				m_ConsumingTime = 0f;
				if (m_bIsMorphing)
				{
					DeMorph();
				}
				m_bIsMorphing = false;
				m_MorphingTime = 0f;
				m_nDeathCount++;
				m_bVisible = false;
			}
			else if (m_CurrentAnim != AnimStates.EXPLODE)
			{
				m_SplatchSound[m_currentSound].Play();
				m_currentSound++;
				if (m_currentSound >= 3)
				{
					m_currentSound = 0;
				}
				SetAnimation(AnimStates.EXPLODE);
				m_CurrentJumpImpulse = 0f;
				m_PlayerBody.LinearVelocity = Vector2.Zero;
				m_bIsOnGround = false;
				m_bLockJump = false;
				m_PlayerBody.IsStatic = true;
				m_PlayerBody.Active = false;
				if (m_CurrentPowerUp != null)
				{
					if ((object)m_CurrentPowerUp.GetType() == typeof(Bomb))
					{
						Bomb bomb = (Bomb)m_CurrentPowerUp;
						bomb.Explode();
					}
					else
					{
						m_CurrentPowerUp.StopBonus();
					}
				}
				m_GibEmitter.Trigger(GetPosition());
			}
			return true;
		}
		if (m_Tag == 2)
		{
			Burn();
			m_Tag = 0;
			return false;
		}
		return false;
	}

	public void Respawn()
	{
		m_Tag = 0;
		m_bControlHorizontalMove = true;
		m_PokeTime = 0f;
		m_CurrentJumpImpulse = 0f;
		m_CurrentPlatform = null;
		m_bIsOnGround = false;
		m_bLockJump = false;
		m_PlayerBody.Active = true;
		m_PlayerBody.IsStatic = false;
		SetAnimation(AnimStates.STAND);
		SetWalkSpeed(25f, m_WalkAnimationSpeed);
		m_bVisible = true;
		m_CurrentSpawn.bIsFree = true;
	}

	public void Burn()
	{
		if (!m_bIsBurning && !m_bSpecialEnable && m_Tag != 1 && m_Tag != 3)
		{
			m_bIsBurning = true;
			m_ConsumingTime = 3000f;
		}
	}

	public void ProcessJump()
	{
		if (m_bIsDucked)
		{
			Vector2 impulse = new Vector2(0f, 0f - m_JumpImpulse);
			m_PlayerBody.ApplyLinearImpulse(ref impulse);
		}
		m_CurrentJumpImpulse += m_JumpImpulse;
		if (m_CurrentJumpImpulse < m_MaxJumpImpulse)
		{
			SetAnimation(AnimStates.JUMP);
			Vector2 impulse2 = new Vector2(0f, 0f - m_CurrentJumpImpulse);
			m_PlayerBody.ApplyLinearImpulse(ref impulse2);
		}
		if (m_bJumpRelease && m_bIsOnGround && !PlayerConfig.JumpSound.IsPlaying())
		{
			PlayerConfig.JumpSound.Play();
		}
	}

	public void SetAnimation(AnimStates Astate)
	{
		if (!m_PlayerSprite[(uint)m_CurrentAnim].IsLocked() && m_CurrentAnim != Astate)
		{
			if (m_bSpecialEnable)
			{
				m_CurrentAnim = Astate + 6;
			}
			else
			{
				m_CurrentAnim = Astate;
			}
			m_PlayerSprite[(uint)Astate].Reset();
		}
	}

	public void SetAnimation(AnimStates Astate, bool bForcePlay)
	{
		if (!m_PlayerSprite[(uint)m_CurrentAnim].IsLocked() && m_CurrentAnim != Astate)
		{
			if (m_bSpecialEnable)
			{
				m_CurrentAnim = Astate + 6;
			}
			else
			{
				m_CurrentAnim = Astate;
			}
			m_PlayerSprite[(uint)Astate].Reset();
			m_PlayerSprite[(uint)Astate].SetLock(bForcePlay);
		}
	}
}
